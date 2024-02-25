﻿#if UNITY_ADDRESSABLES_EXIST
    // The Addressables package depends on the ScriptableBuildPipeline package
    #define UNITY_SCRIPTABLEBUILDPIPELINE_EXIST
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using FMOD;
using FMOD.Studio;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using ADVANCEDSETTINGS = FMOD.Studio.ADVANCEDSETTINGS;
using Debug = FMOD.Debug;
using GUID = FMOD.GUID;
using INITFLAGS = FMOD.Studio.INITFLAGS;
using Object = UnityEngine.Object;
#if UNITY_SCRIPTABLEBUILDPIPELINE_EXIST
using UnityEditor.Build.Pipeline;
#endif

namespace FMODUnity
{
    public class EditorUtils : MonoBehaviour
    {
        public const string BuildFolder = "Build";

        private const int StudioScriptPort = 3663;

        private static FMOD.Studio.System system;
        private static SPEAKERMODE speakerMode;
        private static string encryptionKey;

        private static readonly List<Bank> loadedPreviewBanks = new();
        private static EventDescription previewEventDesc;
        private static EventInstance previewEventInstance;
        private static NetworkStream networkStream;
        private static Socket socket;
        private static IAsyncResult socketConnection;

        private static readonly List<EventInstance> previewEventInstances = new();

        public static FMOD.Studio.System System
        {
            get
            {
                if (!system.isValid()) CreateSystem();
                return system;
            }
        }

        public static bool PreviewBanksLoaded => loadedPreviewBanks.Count > 0;

        private static NetworkStream ScriptStream
        {
            get
            {
                if (networkStream == null)
                    try
                    {
                        if (socket == null)
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        if (!socket.Connected)
                        {
                            socketConnection = socket.BeginConnect("127.0.0.1", StudioScriptPort, null, null);
                            socketConnection.AsyncWaitHandle.WaitOne();
                            socket.EndConnect(socketConnection);
                            socketConnection = null;
                        }

                        networkStream = new NetworkStream(socket);

                        var headerBytes = new byte[128];
                        var read = ScriptStream.Read(headerBytes, 0, 128);
                        var header = Encoding.UTF8.GetString(headerBytes, 0, read - 1);
                        if (header.StartsWith("log():"))
                            RuntimeUtils.DebugLog("FMOD Studio: Script Client returned " + header.Substring(6));
                    }
                    catch (Exception e)
                    {
                        RuntimeUtils.DebugLog(
                            "FMOD Studio: Script Client failed to connect - Check FMOD Studio is running");

                        socketConnection = null;
                        socket = null;
                        networkStream = null;

                        throw e;
                    }

                return networkStream;
            }
        }

        private static void Update()
        {
            // Update the editor system
            if (system.isValid())
            {
                CheckResult(system.update());

                if (speakerMode != Settings.Instance.PlayInEditorPlatform.SpeakerMode) RecreateSystem();

                if (encryptionKey != Settings.Instance.EncryptionKey) RecreateSystem();
            }

            for (var i = 0; i < previewEventInstances.Count; i++)
            {
                var instance = previewEventInstances[i];
                if (instance.isValid())
                {
                    PLAYBACK_STATE state;
                    instance.getPlaybackState(out state);
                    if (state == PLAYBACK_STATE.STOPPED)
                    {
                        PreviewStop(instance);
                        i--;
                    }
                }
            }
        }

        public static void CheckResult(RESULT result)
        {
            if (result != RESULT.OK)
                RuntimeUtils.DebugLogError(string.Format("FMOD Studio: Encountered Error: {0} {1}", result,
                    Error.String(result)));
        }

        public static void ValidateSource(out bool valid, out string reason)
        {
            valid = true;
            reason = "";
            var settings = Settings.Instance;
            if (settings.HasSourceProject)
            {
                if (string.IsNullOrEmpty(settings.SourceProjectPath))
                {
                    valid = false;
                    reason = "The FMOD Studio project path must be set to an .fspro file.";
                    return;
                }

                if (!File.Exists(settings.SourceProjectPath))
                {
                    valid = false;
                    reason = string.Format("The FMOD Studio project path '{0}' does not exist.",
                        settings.SourceProjectPath);
                    return;
                }

                var projectPath = settings.SourceProjectPath;
                var projectFolder = Path.GetDirectoryName(projectPath);
                var buildFolder = RuntimeUtils.GetCommonPlatformPath(Path.Combine(projectFolder, BuildFolder));
                if (!Directory.Exists(buildFolder) ||
                    Directory.GetDirectories(buildFolder).Length == 0 ||
                    Directory.GetFiles(Directory.GetDirectories(buildFolder)[0], "*.bank", SearchOption.AllDirectories)
                        .Length == 0
                   )
                {
                    valid = false;
                    reason = string.Format(
                        "The FMOD Studio project '{0}' does not contain any built banks. Please build your project in FMOD Studio.",
                        settings.SourceProjectPath);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(settings.SourceBankPath))
                {
                    valid = false;
                    reason = "The build path has not been set.";
                    return;
                }

                if (!Directory.Exists(settings.SourceBankPath))
                {
                    valid = false;
                    reason = string.Format("The build path '{0}' does not exist.", settings.SourceBankPath);
                    return;
                }

                if (settings.HasPlatforms)
                {
                    if (Directory.GetDirectories(settings.SourceBankPath).Length == 0)
                    {
                        valid = false;
                        reason = string.Format(
                            "Build path '{0}' does not contain any platform sub-directories. Please check that the build path is correct.",
                            settings.SourceBankPath);
                    }
                }
                else
                {
                    if (Directory.GetFiles(settings.SourceBankPath, "*.strings.bank").Length == 0)
                    {
                        valid = false;
                        reason = string.Format("Build path '{0}' does not contain any built banks.",
                            settings.SourceBankPath);
                    }
                }
            }
        }

        public static string[] GetBankPlatforms()
        {
            var buildFolder = Settings.Instance.SourceBankPath;
            try
            {
                if (Directory.GetFiles(buildFolder, "*.bank").Length == 0)
                {
                    var buildDirectories = Directory.GetDirectories(buildFolder);
                    var buildNames = new string[buildDirectories.Length];
                    for (var i = 0; i < buildDirectories.Length; i++)
                        buildNames[i] = Path.GetFileNameWithoutExtension(buildDirectories[i]);
                    return buildNames;
                }
            }
            catch
            {
            }

            return new string[0];
        }

        public static string VersionString(uint version)
        {
            var major = (version & 0x00FF0000) >> 16;
            var minor = (version & 0x0000FF00) >> 8;
            var patch = version & 0x000000FF;

            return string.Format("{0:X1}.{1:X2}.{2:X2}", major, minor, patch);
        }

        public static string DurationString(float seconds)
        {
            var minutes = seconds / 60;
            var hours = minutes / 60;

            if (hours >= 1)
                return Pluralize(Mathf.FloorToInt(hours), "hour", "hours");
            if (minutes >= 1)
                return Pluralize(Mathf.FloorToInt(minutes), "minute", "minutes");
            if (seconds >= 1)
                return Pluralize(Mathf.FloorToInt(seconds), "second", "seconds");
            return "a moment";
        }

        public static string SeriesString(string separator, string finalSeparator, IEnumerable<string> elements)
        {
            if (!elements.Any())
                return string.Empty;
            if (!elements.Skip(1).Any())
                return elements.First();
            return string.Join(separator, elements.Take(elements.Count() - 1)) + finalSeparator + elements.Last();
        }

        public static string Pluralize(int count, string singular, string plural)
        {
            return string.Format("{0} {1}", count, count == 1 ? singular : plural);
        }

        public static Texture2D LoadImage(string filename)
        {
            var texture = EditorGUIUtility.Load($"{RuntimeUtils.PluginBasePath}/images/{filename}") as Texture2D;

            if (texture == null)
                texture = EditorGUIUtility.Load($"Assets/Editor Default Resources/FMOD/{filename}") as Texture2D;

            return texture;
        }

        public static string GameObjectPath(Component component, GameObject root = null)
        {
            var transform = component.transform;

            var objectPath = new StringBuilder();

            while (transform != null && transform.gameObject != root)
            {
                if (objectPath.Length > 0) objectPath.Insert(0, "/");

                objectPath.Insert(0, transform.name);

                transform = transform.parent;
            }

            return objectPath.ToString();
        }

        public static bool HasAttribute<T>(MemberInfo member)
            where T : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(member, typeof(Attribute), true);

            return attributes.Any(a => typeof(T).IsAssignableFrom(a.GetType()));
        }

        public static bool AssetExists(string path)
        {
            var fullPath = $"{Environment.CurrentDirectory}/{path}";

            // We check that the file or directory exists as well because recently deleted assets remain in the database
            return !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path))
                   && (File.Exists(fullPath) || Directory.Exists(fullPath));
        }

        public static void EnsureFolderExists(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                var parentFolder = GetParentFolder(folderPath);

                EnsureFolderExists(parentFolder);

                var folderName = Path.GetFileName(folderPath);

                AssetDatabase.CreateFolder(parentFolder, folderName);
            }
        }

        // Path.GetDirectoryName replaces '/' with '\\' in some scripting runtime versions,
        // so we have to roll our own.
        public static string GetParentFolder(string assetPath)
        {
            var endIndex = assetPath.LastIndexOf('/');

            return endIndex > 0 ? assetPath.Substring(0, endIndex) : string.Empty;
        }

        public static void DrawLegacyEvent(SerializedProperty property, string migrationTarget)
        {
            // Display the legacy event field if it is not empty
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                EditorGUILayout.PropertyField(property, new GUIContent("Legacy Event"));

                using (new EditorGUI.IndentLevelScope())
                {
                    var content = new GUIContent(
                        string.Format("Will be migrated to <b>{0}</b>", migrationTarget),
                        EditorGUIUtility.IconContent("console.infoicon.sml").image);
                    var style = new GUIStyle(GUI.skin.label) { richText = true };

                    var rect = EditorGUILayout.GetControlRect(false, style.CalcSize(content).y);
                    rect = EditorGUI.IndentedRect(rect);

                    GUI.Label(rect, content, style);
                }
            }
        }

        // Gets a control rect, draws a help button at the end of the line,
        // and returns a rect describing the remaining space.
        public static Rect DrawHelpButtonLayout(Func<PopupWindowContent> createContent)
        {
            var helpSize = GetHelpButtonSize();

            var rect = EditorGUILayout.GetControlRect(true, helpSize.y);

            return DrawHelpButton(rect, createContent);
        }

        public static Rect DrawHelpButton(Rect rect, Func<PopupWindowContent> createContent)
        {
            GUIContent content;
            GUIStyle style;
            GetHelpButtonData(out content, out style);

            var helpSize = style.CalcSize(content);

            var helpRect = rect;
            helpRect.xMin = helpRect.xMax - helpSize.x;

            if (GUI.Button(helpRect, content, style)) PopupWindow.Show(helpRect, createContent());

            var remainderRect = rect;
            remainderRect.xMax = helpRect.xMin;

            return remainderRect;
        }

        public static float DrawParameterValueLayout(float value, EditorParamRef paramRef)
        {
            if (paramRef.Type == ParameterType.Labeled)
                return EditorGUILayout.Popup((int)value, paramRef.Labels);
            if (paramRef.Type == ParameterType.Discrete)
                return EditorGUILayout.IntSlider((int)value, (int)paramRef.Min, (int)paramRef.Max);
            return EditorGUILayout.Slider(value, paramRef.Min, paramRef.Max);
        }

        public static Vector2 GetHelpButtonSize()
        {
            GUIContent content;
            GUIStyle style;
            GetHelpButtonData(out content, out style);

            return style.CalcSize(content);
        }

        private static void GetHelpButtonData(out GUIContent content, out GUIStyle style)
        {
            content = EditorGUIUtility.IconContent("_Help");
            style = GUI.skin.label;
        }

#if !FMOD_STORE_UPLOAD
        [InitializeOnLoadMethod]
#endif
        private static void Startup()
        {
            EditorApplication.update += Update;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
            EditorApplication.pauseStateChanged += HandleOnPausedModeChanged;

            if (Application.isBatchMode)
                BuildStatusWatcher.Startup();
            else
                EditorApplication.update += CallStartupMethodsWhenReady;
        }

        private static void HandleBeforeAssemblyReload()
        {
            DestroySystem();
        }

        private static void HandleOnPausedModeChanged(PauseState state)
        {
            if (RuntimeManager.IsInitialized && RuntimeManager.HaveMasterBanksLoaded)
            {
                RuntimeManager.GetBus("bus:/").setPaused(EditorApplication.isPaused);
                RuntimeManager.StudioSystem.update();
            }
        }

        private static void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            // Entering Play Mode will cause scripts to reload, losing all state
            // This is the last chance to clean up FMOD and avoid a leak.
            if (state == PlayModeStateChange.ExitingEditMode) DestroySystem();
        }

        private static void CallStartupMethodsWhenReady()
        {
            if (EditorApplication.isUpdating)
                // Some startup code accesses Settings.Instance; this can obliterate settings if
                // the asset database is being updated, so wait until the update is finished.
                return;

            EditorApplication.update -= CallStartupMethodsWhenReady;

            // Explicitly initialize Settings so that both it and EditorSettings will work.
            Settings.Initialize();
            Settings.EditorSettings.CheckActiveBuildTarget();

            CheckBaseFolderGUID();
            CheckMacLibraries();

            Legacy.CleanTemporaryChanges();
            CleanObsoleteFiles();

#if UNITY_TIMELINE_EXIST
            // Register timeline event receivers.
            FMODEventPlayableBehavior.Enter += (sender, args) =>
            {
                var behavior = sender as FMODEventPlayableBehavior;
                if (!string.IsNullOrEmpty(behavior.EventReference.Path))
                {
                    LoadPreviewBanks();
                    var eventRef = EventManager.EventFromPath(behavior.EventReference.Path);
                    var paramValues = new Dictionary<string, float>();
                    foreach (var param in eventRef.Parameters) paramValues.Add(param.Name, param.Default);
                    foreach (var param in behavior.Parameters) paramValues[param.Name] = param.Value;

                    args.eventInstance = PreviewEvent(eventRef, paramValues, behavior.CurrentVolume,
                        behavior.ClipStartTime);
                }
            };

            FMODEventPlayableBehavior.Exit += (sender, args) =>
            {
                var behavior = sender as FMODEventPlayableBehavior;
                if (behavior.StopType != STOP_MODE.None)
                {
                    var stopType = behavior.StopType == STOP_MODE.Immediate
                        ? FMOD.Studio.STOP_MODE.IMMEDIATE
                        : FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
                    PreviewStop(args.eventInstance, stopType);
                }
            };

            FMODEventPlayableBehavior.GraphStop += (sender, args) => { PreviewStop(args.eventInstance); };

            FMODEventPlayable.OnCreatePlayable += (sender, args) =>
            {
                var playable = sender as FMODEventPlayable;
                if (playable.Parameters.Length > 0 || playable.Template.ParameterLinks.Count > 0)
                {
                    LoadPreviewBanks();
                    EventDescription eventDescription;
                    system.getEventByID(playable.EventReference.Guid, out eventDescription);
                    playable.LinkParameters(eventDescription);
                }
            };
#endif

            BuildStatusWatcher.Startup();
            BankRefresher.Startup();
            BoltIntegration.Startup();
            EventManager.Startup();
            SetupWizardWindow.Startup();
        }

        private static void RecreateSystem()
        {
            StopAllPreviews();
            DestroySystem();
            CreateSystem();
        }

        private static void DestroySystem()
        {
            if (system.isValid())
            {
                RuntimeUtils.DebugLog("FMOD Studio: Destroying editor system instance");
                UnloadPreviewBanks();
                system.release();
                system.clearHandle();
            }
        }

        private static void CreateSystem()
        {
            RuntimeUtils.DebugLog("FMOD Studio: Creating editor system instance");
            RuntimeUtils.EnforceLibraryOrder();

            var result = Debug.Initialize(DEBUG_FLAGS.LOG, DEBUG_MODE.FILE, null, "fmod_editor.log");
            if (result != RESULT.OK)
                RuntimeUtils.DebugLogWarning(
                    "FMOD Studio: Cannot open fmod_editor.log. Logging will be disabled for importing and previewing");

            CheckResult(FMOD.Studio.System.create(out system));

            FMOD.System lowlevel;
            CheckResult(system.getCoreSystem(out lowlevel));

            // Use play-in-editor speaker mode for event browser preview and metering
            speakerMode = Settings.Instance.PlayInEditorPlatform.SpeakerMode;
            CheckResult(lowlevel.setSoftwareFormat(0, speakerMode, 0));

            encryptionKey = Settings.Instance.EncryptionKey;
            if (!string.IsNullOrEmpty(encryptionKey))
            {
                var studioAdvancedSettings = new ADVANCEDSETTINGS();
                CheckResult(system.setAdvancedSettings(studioAdvancedSettings, encryptionKey));
            }

            CheckResult(system.initialize(256, INITFLAGS.ALLOW_MISSING_PLUGINS | INITFLAGS.SYNCHRONOUS_UPDATE,
                FMOD.INITFLAGS.NORMAL, IntPtr.Zero));

            ChannelGroup master;
            CheckResult(lowlevel.getMasterChannelGroup(out master));
            DSP masterHead;
            CheckResult(master.getDSP(CHANNELCONTROL_DSP_INDEX.HEAD, out masterHead));
            CheckResult(masterHead.setMeteringEnabled(false, true));
        }

        public static void UpdateParamsOnEmitter(SerializedObject serializedObject, string path)
        {
            if (string.IsNullOrEmpty(path) || EventManager.EventFromPath(path) == null) return;

            var eventRef = EventManager.EventFromPath(path);
            serializedObject.ApplyModifiedProperties();
            if (serializedObject.isEditingMultipleObjects)
                foreach (var obj in serializedObject.targetObjects)
                    UpdateParamsOnEmitter(obj, eventRef);
            else
                UpdateParamsOnEmitter(serializedObject.targetObject, eventRef);
            serializedObject.Update();
        }

        private static void UpdateParamsOnEmitter(Object obj, EditorEventRef eventRef)
        {
            var emitter = obj as StudioEventEmitter;
            if (emitter == null)
                // Custom game object
                return;

            for (var i = 0; i < emitter.Params.Length; i++)
                if (!eventRef.LocalParameters.Exists(x => x.Name == emitter.Params[i].Name))
                {
                    var end = emitter.Params.Length - 1;
                    emitter.Params[i] = emitter.Params[end];
                    Array.Resize(ref emitter.Params, end);
                    i--;
                }

            emitter.OverrideAttenuation = false;
            emitter.OverrideMinDistance = eventRef.MinDistance;
            emitter.OverrideMaxDistance = eventRef.MaxDistance;
        }

        [MenuItem("FMOD/Help/Getting Started", priority = 2)]
        private static void OnlineGettingStarted()
        {
            OpenOnlineDocumentation("unity", "user-guide");
        }

        [MenuItem("FMOD/Help/Integration Manual", priority = 3)]
        public static void OnlineManual()
        {
            OpenOnlineDocumentation("unity");
        }

        [MenuItem("FMOD/Help/API Manual", priority = 4)]
        private static void OnlineAPIDocs()
        {
            OpenOnlineDocumentation("api");
        }

        [MenuItem("FMOD/Help/Support Forum", priority = 16)]
        private static void OnlineQA()
        {
            Application.OpenURL("https://qa.fmod.com/");
        }

        [MenuItem("FMOD/Help/Revision History", priority = 5)]
        private static void OnlineRevisions()
        {
            OpenOnlineDocumentation("api", "welcome-revision-history");
        }

        public static void OpenOnlineDocumentation(string section, string page = null, string anchor = null)
        {
            const string Prefix = "https://fmod.com/docs/";
            var version = string.Format("{0:X}.{1:X}", VERSION.number >> 16, (VERSION.number >> 8) & 0xFF);
            string url;

            if (!string.IsNullOrEmpty(page))
            {
                if (!string.IsNullOrEmpty(anchor))
                    url = string.Format("{0}/{1}/{2}/{3}.html#{4}", Prefix, version, section, page, anchor);
                else
                    url = string.Format("{0}/{1}/{2}/{3}.html", Prefix, version, section, page);
            }
            else
            {
                url = string.Format("{0}/{1}/{2}", Prefix, version, section);
            }

            Application.OpenURL(url);
        }

        [MenuItem("FMOD/About Integration", priority = 7)]
        public static void About()
        {
            FMOD.System lowlevel;
            CheckResult(System.getCoreSystem(out lowlevel));

            uint version;
            CheckResult(lowlevel.getVersion(out version));

            var text = string.Format(
                "Version: {0}\n\nCopyright \u00A9 Firelight Technologies Pty, Ltd. 2014-2023 \n\n" +
                "See LICENSE.TXT for additional license information.",
                VersionString(version));

            EditorUtility.DisplayDialog("FMOD Studio Unity Integration", text, "OK");
        }

        public static void LoadPreviewBanks()
        {
            if (PreviewBanksLoaded) return;

            foreach (var bank in EventManager.Banks)
            {
                Bank previewBank;
                var result = System.loadBankFile(bank.Path, LOAD_BANK_FLAGS.NORMAL, out previewBank);
                if (result != RESULT
                        .ERR_EVENT_ALREADY_LOADED) // ignore error when a bank is already loaded, e.g. localized banks.
                    CheckResult(result);
                loadedPreviewBanks.Add(previewBank);
            }
        }

        public static void UnloadPreviewBanks()
        {
            if (!PreviewBanksLoaded) return;

            loadedPreviewBanks.ForEach(x =>
            {
                x.unload();
                x.clearHandle();
            });
            loadedPreviewBanks.Clear();
        }

        public static EventInstance PreviewEvent(EditorEventRef eventRef, Dictionary<string, float> previewParamValues,
            float volume = 1, float startTime = 0.0f)
        {
            EventDescription eventDescription;
            EventInstance eventInstance;

            CheckResult(System.getEventByID(eventRef.Guid, out eventDescription));
            CheckResult(eventDescription.createInstance(out eventInstance));

            foreach (var param in eventRef.Parameters)
            {
                PARAMETER_DESCRIPTION paramDesc;
                if (param.IsGlobal)
                    CheckResult(System.getParameterDescriptionByName(param.Name, out paramDesc));
                else
                    CheckResult(eventDescription.getParameterDescriptionByName(param.Name, out paramDesc));

                var value = previewParamValues.ContainsKey(param.Name) ? previewParamValues[param.Name] : param.Default;
                param.ID = paramDesc.id;

                if (param.IsGlobal)
                    CheckResult(System.setParameterByID(param.ID, value));
                else
                    CheckResult(eventInstance.setParameterByID(param.ID, value));
            }

            CheckResult(eventInstance.setVolume(volume));
            CheckResult(eventInstance.setTimelinePosition((int)(startTime * 1000.0f)));
            CheckResult(eventInstance.start());

            previewEventInstances.Add(eventInstance);

            return eventInstance;
        }

        public static void PreviewPause(EventInstance eventInstance)
        {
            if (eventInstance.isValid() && previewEventInstances.Contains(eventInstance))
            {
                bool paused;
                CheckResult(eventInstance.getPaused(out paused));
                CheckResult(eventInstance.setPaused(!paused));
            }
        }

        public static void PreviewStop(EventInstance eventInstance,
            FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
        {
            if (previewEventInstances.Contains(eventInstance))
            {
                previewEventInstances.Remove(eventInstance);
                if (eventInstance.isValid())
                {
                    eventInstance.stop(stopMode);
                    eventInstance.release();
                    eventInstance.clearHandle();
                }
            }
        }

        public static void StopAllPreviews()
        {
            foreach (var eventInstance in previewEventInstances) PreviewStop(eventInstance);
        }

        public static float[] GetMetering()
        {
            FMOD.System lowlevel;
            CheckResult(System.getCoreSystem(out lowlevel));
            ChannelGroup master;
            CheckResult(lowlevel.getMasterChannelGroup(out master));
            DSP masterHead;
            CheckResult(master.getDSP(CHANNELCONTROL_DSP_INDEX.HEAD, out masterHead));

            DSP_METERING_INFO outputMetering;
            CheckResult(masterHead.getMeteringInfo(IntPtr.Zero, out outputMetering));

            SPEAKERMODE mode;
            int rate, raw;
            lowlevel.getSoftwareFormat(out rate, out mode, out raw);
            int channels;
            lowlevel.getSpeakerModeChannels(mode, out channels);

            var data = new float[channels];
            if (outputMetering.numchannels > 0) Array.Copy(outputMetering.rmslevel, data, channels);
            return data;
        }

        private static void AsyncConnectCallback(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);
            }
            catch (Exception)
            {
            }
            finally
            {
                socketConnection = null;
            }
        }

        public static bool IsConnectedToStudio()
        {
            try
            {
                if (socket != null && socket.Connected)
                    if (SendScriptCommand("true"))
                        return true;

                if (socketConnection == null)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketConnection = socket.BeginConnect("127.0.0.1", StudioScriptPort, AsyncConnectCallback, null);
                }

                return false;
            }
            catch (Exception e)
            {
                RuntimeUtils.DebugLogException(e);
                return false;
            }
        }

        public static bool SendScriptCommand(string command)
        {
            var commandBytes = Encoding.UTF8.GetBytes(command);
            try
            {
                ScriptStream.Write(commandBytes, 0, commandBytes.Length);
                var commandReturnBytes = new byte[128];
                var read = ScriptStream.Read(commandReturnBytes, 0, 128);
                var result = Encoding.UTF8.GetString(commandReturnBytes, 0, read - 1);
                return result.Contains("true");
            }
            catch (Exception)
            {
                if (networkStream != null)
                {
                    networkStream.Close();
                    networkStream = null;
                }

                return false;
            }
        }


        public static string GetScriptOutput(string command)
        {
            var commandBytes = Encoding.UTF8.GetBytes(command);
            try
            {
                ScriptStream.Write(commandBytes, 0, commandBytes.Length);
                var commandReturnBytes = new byte[2048];
                var read = ScriptStream.Read(commandReturnBytes, 0, commandReturnBytes.Length);
                var result = Encoding.UTF8.GetString(commandReturnBytes, 0, read - 1);
                if (result.StartsWith("out():")) return result.Substring(6).Trim();
                return null;
            }
            catch (Exception)
            {
                networkStream.Close();
                networkStream = null;
                return null;
            }
        }

        private static string GetMasterBank()
        {
            GetScriptOutput("masterBankFolder = studio.project.workspace.masterBankFolder;");
            var bankCountString = GetScriptOutput("masterBankFolder.items.length;");
            var bankCount = int.Parse(bankCountString);
            for (var i = 0; i < bankCount; i++)
            {
                var isMaster =
                    GetScriptOutput(string.Format("masterBankFolder.items[{1}].isOfExactType(\"MasterBank\");", i));
                if (isMaster == "true")
                {
                    var guid = GetScriptOutput(string.Format("masterBankFolder.items[{1}].id;", i));
                    return guid;
                }
            }

            return "";
        }

        private static bool CheckForNameConflict(string folderGuid, string eventName)
        {
            const string checkForNameConflictFunc =
                @"function(folderGuid, eventName) {
                    var nameConflict = false;
                    studio.project.lookup(folderGuid).items.forEach(function(val) {
                        nameConflict |= val.name == eventName;
                    });
                    return nameConflict;
                }";

            var conflictBool = GetScriptOutput(string.Format("({0})(\"{1}\", \"{2}\")", checkForNameConflictFunc,
                folderGuid, eventName));
            return conflictBool == "1";
        }

        public static string CreateStudioEvent(string eventPath, string eventName)
        {
            var folders = eventPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var folderGuid = GetScriptOutput("studio.project.workspace.masterEventFolder.id;");

            const string getFolderGuidFunc =
                @"function(parentGuid, folderName) {
                    folderGuid = """";
                    studio.project.lookup(parentGuid).items.forEach(function(val) {
                        folderGuid = val.isOfType(""EventFolder"") && val.name == folderName ? val.id : folderGuid;
                    });
                    if (folderGuid == """")
                    {
                        var newFolder = studio.project.create(""EventFolder"");
                        newFolder.name = folderName;
                        newFolder.folder = studio.project.lookup(parentGuid);
                        folderGuid = newFolder.id;
                    }
                    return folderGuid;
                }";

            for (var i = 0; i < folders.Length; i++)
            {
                var parentGuid = folderGuid;
                folderGuid = GetScriptOutput(string.Format("({0})(\"{1}\", \"{2}\")", getFolderGuidFunc, parentGuid,
                    folders[i]));
            }

            if (CheckForNameConflict(folderGuid, eventName))
            {
                EditorUtility.DisplayDialog("Name Conflict",
                    string.Format("The event {0} already exists under {1}", eventName, eventPath), "OK");
                return null;
            }

            const string createEventFunc =
                @"function(eventName, folderGuid) {
                    event = studio.project.create(""Event"");
                    event.note = ""Placeholder created via Unity"";
                    event.name = eventName;
                    event.folder = studio.project.lookup(folderGuid);

                    track = studio.project.create(""GroupTrack"");
                    track.mixerGroup.output = event.mixer.masterBus;
                    track.mixerGroup.name = ""Audio 1"";
                    event.relationships.groupTracks.add(track);

                    tag = studio.project.create(""Tag"");
                    tag.name = ""placeholder"";
                    tag.folder = studio.project.workspace.masterTagFolder;
                    event.relationships.tags.add(tag);

                    return event.id;
                }";

            var eventGuid =
                GetScriptOutput(string.Format("({0})(\"{1}\", \"{2}\")", createEventFunc, eventName, folderGuid));
            return eventGuid;
        }

        // The FMOD base folder needs to have a known GUID so that we can find it
        // if it has been moved, and so that platform specific integration packages can
        // be installed correctly.
        //
        // However, old FMOD packages didn't specify a GUID for the base folder, meaning Unity
        // would generate a new one. If this is the case, we need to patch the metadata with
        // the correct GUID.
        private static void CheckBaseFolderGUID()
        {
            if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(RuntimeUtils.BaseFolderGUID)))
            {
                var folderPath = RuntimeUtils.PluginBasePathDefault;

                if (!Directory.Exists(folderPath))
                {
                    UnityEngine.Debug.LogErrorFormat("FMOD: Couldn't find base folder by GUID ({0}) or path ({1})",
                        RuntimeUtils.BaseFolderGUID, RuntimeUtils.PluginBasePathDefault);
                    return;
                }

                const string DialogTitle = "Update FMOD Folder Metadata";

                var update = EditorUtility.DisplayDialog(DialogTitle,
                    $"The metadata for the {folderPath} folder needs to be updated"
                    + " so that FMOD can locate required files.\n\n"
                    + "After this change you may move the FMOD folder to any location within your project.",
                    "Update Metadata", "Ignore");

                while (update)
                {
                    var error = ReplaceMetaFileGUID(folderPath, RuntimeUtils.BaseFolderGUID);

                    if (error == null) return;

                    update = EditorUtility.DisplayDialog(DialogTitle,
                        $"Error updating metadata for {folderPath}:\n\n{error}\n\nDo you want to try again?",
                        "Try Again", "Ignore");
                }
            }
        }

        private static string ReplaceMetaFileGUID(string assetPath, string newGUID)
        {
            try
            {
                var filePath = $"{assetPath}.meta";

                if (!AssetDatabase.MakeEditable(filePath)) return $"Failed to open {filePath} for editing";

                var lines = File.ReadAllLines(filePath);

                const string GuidPrefix = "guid:";
                var guidReplaced = false;

                using (var stream = File.CreateText(filePath))
                {
                    foreach (var line in lines)
                        if (!guidReplaced && line.StartsWith(GuidPrefix))
                        {
                            guidReplaced = true;
                            stream.WriteLine($"{GuidPrefix} {newGUID}");
                        }
                        else
                        {
                            stream.WriteLine(line);
                        }
                }

                if (!guidReplaced) return $"Couldn't find a line starting with '{GuidPrefix}' in {filePath}";

                UnityEngine.Debug.LogFormat("FMOD: Updated the GUID for {0} to {1}", assetPath, newGUID);

                AssetDatabase.ImportAsset(assetPath);

                return null;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarningFormat("FMOD: Failed to update the GUID for {0}: {1}", assetPath,
                    e.Message);

                return e.Message;
            }
        }

        private static void CheckMacLibraries()
        {
            var platformMac = EditorSettings.Instance.GetPlatform(BuildTarget.StandaloneOSX);

            var allLibraries = platformMac.GetBuildTargets()
                .SelectMany(t => platformMac.GetBinaryAssetPaths(t, Platform.BinaryType.All))
                .Distinct();

            var librariesToRepair = allLibraries.Where(path =>
                {
                    var infoPlistPath = $"{path}/Contents/Info.plist";

                    if (File.Exists(infoPlistPath))
                    {
                        var contents = File.ReadAllText(infoPlistPath);

                        return contents.Contains("\r\n");
                    }

                    return false;
                })
                .ToList();

            if (!librariesToRepair.Any()) return;

            librariesToRepair.Sort();

            const string DialogTitle = "Repair FMOD Libraries";

            var repair = EditorUtility.DisplayDialog(DialogTitle,
                "The following FMOD libraries contain incorrect line endings, and need to be repaired:\n\n" +
                $"{string.Join("\n", librariesToRepair)}\n\n" +
                "Do you want to repair them now?", "Repair", "Ignore");

            while (repair)
                try
                {
                    RepairMacLibraries(librariesToRepair);
                    repair = false;
                }
                catch (Exception e)
                {
                    repair = EditorUtility.DisplayDialog(DialogTitle,
                        $"Error repairing FMOD libraries:\n\n{e.Message}\n\nDo you want to try again?",
                        "Try Again", "Ignore");
                }
        }

        private static void RepairMacLibraries(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var infoPlistPath = $"{path}/Contents/Info.plist";

                if (!AssetDatabase.MakeEditable(infoPlistPath))
                    throw new Exception($"Failed to open {infoPlistPath} for editing");

                var contents = File.ReadAllText(infoPlistPath);
                contents = contents.Replace("\r\n", "\n");

                File.WriteAllText(infoPlistPath, contents);

                UnityEngine.Debug.LogFormat("FMOD: Replaced CRLF line endings with LF in {0}", infoPlistPath);
            }
        }

        private static void CleanObsoleteFiles()
        {
            if (Environment.GetCommandLineArgs().Any(a => a == "-exportPackage"))
                // Don't delete anything or it won't be included in the package
                return;
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                // Messing with the asset database while entering play mode causes a NullReferenceException
                return;

            var obsoleteFolder = $"{RuntimeUtils.PluginBasePath}/obsolete";

            if (AssetDatabase.IsValidFolder(obsoleteFolder))
            {
                EditorApplication.LockReloadAssemblies();

                var guids = AssetDatabase.FindAssets(string.Empty, new[] { obsoleteFolder });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (AssetDatabase.DeleteAsset(path))
                        RuntimeUtils.DebugLogFormat("FMOD: Removed obsolete file {0}", path);
                }

                if (AssetDatabase.MoveAssetToTrash(obsoleteFolder))
                    RuntimeUtils.DebugLogFormat("FMOD: Removed obsolete folder {0}", obsoleteFolder);
                AssetDatabase.Refresh();
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        public static string WritableAssetPath(string cacheAssetName)
        {
            if (RuntimeUtils.PluginBasePath.StartsWith("Assets/"))
                return $"{RuntimeUtils.PluginBasePath}/Cache/Editor/{cacheAssetName}.asset";
            return $"Assets/Plugins/FMOD/Cache/Editor/{cacheAssetName}.asset";
        }
    }

    public class StagingSystem
    {
        private const string AnyCPU = "AnyCPU";

        private static readonly LibInfo[] LibrariesToUpdate =
        {
            new()
            {
                cpu = "x86", os = "Windows", lib = "fmodstudioL.dll", platform = "win",
                buildTarget = BuildTarget.StandaloneWindows
            },
            new()
            {
                cpu = "x86_64", os = "Windows", lib = "fmodstudioL.dll", platform = "win",
                buildTarget = BuildTarget.StandaloneWindows64
            },
            new()
            {
                cpu = "x86_64", os = "Linux", lib = "libfmodstudioL.so", platform = "linux",
                buildTarget = BuildTarget.StandaloneLinux64
            },
            new()
            {
                cpu = AnyCPU, os = "OSX", lib = "fmodstudioL.bundle", platform = "mac",
                buildTarget = BuildTarget.StandaloneOSX
            }
        };

        public static readonly UpdateStep[] UpdateSteps =
        {
            UpdateStep.Create(
                Settings.SharedLibraryUpdateStages.DisableExistingLibraries,
                "Disable Existing Native Libraries",
                "Disable the existing FMOD native libraries so that Unity will not load them " +
                "at startup time.",
                () =>
                {
                    var importers =
                        LibrariesToUpdate.Select(GetPluginImporter).Where(p => p != null);

                    if (!importers.Any()) return string.Empty;

                    var paths = importers.Select(p => $"\n* {p.assetPath}");

                    return $"This will disable these native libraries:{string.Join(string.Empty, paths)}";
                },
                () =>
                {
                    foreach (var libInfo in LibrariesToUpdate)
                    {
                        var pluginImporter = GetPluginImporter(libInfo);
                        if (pluginImporter != null && pluginImporter.GetCompatibleWithEditor())
                        {
                            pluginImporter.SetCompatibleWithEditor(false);
                            pluginImporter.SetCompatibleWithAnyPlatform(false);
                            EditorUtility.SetDirty(pluginImporter);
                            pluginImporter.SaveAndReimport();
                        }
                    }

                    Settings.Instance.SharedLibraryUpdateStage = Settings.SharedLibraryUpdateStages.RestartUnity;
                    Settings.Instance.SharedLibraryTimeSinceStart = EditorApplication.timeSinceStartup;
                    EditorUtility.SetDirty(Settings.Instance);
                }
            ),

            UpdateStep.Create(
                Settings.SharedLibraryUpdateStages.RestartUnity,
                "Restart Unity",
                "Restart Unity so that it releases its lock on the existing FMOD native libraries.",
                () =>
                {
                    return "This will restart Unity. You will be prompted to save your work if you have unsaved " +
                           "scene modifications.";
                },
                () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorApplication.OpenProject(Environment.CurrentDirectory);
                }
            ),

            UpdateStep.Create(
                Settings.SharedLibraryUpdateStages.CopyNewLibraries,
                "Copy New Native Libraries",
                "Copy the new FMOD native libraries to the correct location and enable them.",
                () =>
                {
                    var actions = new List<string>();

                    foreach (var libInfo in LibrariesToUpdate)
                    {
                        var sourcePath = GetSourcePath(libInfo);
                        var targetPath = GetTargetPath(libInfo);

                        if (EditorUtils.AssetExists(sourcePath))
                        {
                            if (targetPath != null) actions.Add($"Delete {targetPath}");

                            targetPath = GetTargetPath(libInfo, Platform.FileLayout.Latest);

                            actions.Add($"Copy {sourcePath} to {targetPath}");
                        }

                        if (targetPath != null) actions.Add($"Enable {targetPath}");
                    }

                    actions.Add($"Remove {StagingFolder}");

                    return $"This will do the following:\n* {string.Join("\n* ", actions)}";
                },
                () =>
                {
                    var allCopiesSucceeded = true;

                    foreach (var libInfo in LibrariesToUpdate)
                    {
                        var sourcePath = GetSourcePath(libInfo);
                        var targetPath = GetTargetPath(libInfo);

                        if (EditorUtils.AssetExists(sourcePath))
                        {
                            if (targetPath != null)
                                if (!AssetDatabase.DeleteAsset(targetPath))
                                    RuntimeUtils.DebugLogError(string.Format("FMOD: Could not delete {0}", targetPath));

                            targetPath = GetTargetPath(libInfo, Platform.FileLayout.Latest);

                            EditorUtils.EnsureFolderExists(EditorUtils.GetParentFolder(targetPath));

                            if (!AssetDatabase.CopyAsset(sourcePath, targetPath))
                            {
                                RuntimeUtils.DebugLogError(string.Format("FMOD: Could not copy {0} to {1}", sourcePath,
                                    targetPath));
                                allCopiesSucceeded = false;
                            }
                        }

                        var pluginImporter = AssetImporter.GetAtPath(targetPath) as PluginImporter;

                        if (pluginImporter != null)
                        {
                            pluginImporter.ClearSettings();
                            pluginImporter.SetCompatibleWithEditor(true);
                            pluginImporter.SetCompatibleWithAnyPlatform(false);
                            pluginImporter.SetCompatibleWithPlatform(libInfo.buildTarget, true);
                            pluginImporter.SetEditorData("CPU", libInfo.cpu);
                            pluginImporter.SetEditorData("OS", libInfo.os);
                            EditorUtility.SetDirty(pluginImporter);
                            pluginImporter.SaveAndReimport();
                        }
                    }

                    if (allCopiesSucceeded)
                    {
                        if (AssetDatabase.MoveAssetToTrash(StagingFolder))
                            RuntimeUtils.DebugLogFormat("FMOD: Removed staging folder {0}", StagingFolder);
                        else
                            RuntimeUtils.DebugLogError(string.Format("FMOD: Could not remove staging folder {0}",
                                StagingFolder));
                    }

                    ResetUpdateStage();

                    // This is so that Unity finds the new libraries
                    EditorUtility.RequestScriptReload();
                }
            )
        };

        private static string PlatformsFolder => $"{RuntimeUtils.PluginBasePath}/platforms";
        private static string StagingFolder => $"{RuntimeUtils.PluginBasePath}/staging";

        public static bool SourceLibsExist
        {
            get
            {
                return LibrariesToUpdate.Any(info =>
                {
                    var sourcePath = GetSourcePath(info);

                    if (sourcePath != null)
                        return AssetImporter.GetAtPath(sourcePath) as PluginImporter != null;
                    return false;
                });
            }
        }

        private static string GetTargetPath(LibInfo libInfo)
        {
            foreach (var layout in Platform.OldFileLayouts)
            {
                var path = GetTargetPath(libInfo, layout);

                if (EditorUtils.AssetExists(path)) return path;
            }

            return null;
        }

        private static string GetTargetPath(LibInfo libInfo, Platform.FileLayout layout)
        {
            switch (layout)
            {
                case Platform.FileLayout.Release_1_10:
                    return $"Assets/Plugins/{CPUAndLibPath(libInfo)}";
                case Platform.FileLayout.Release_2_0:
                    return $"Assets/Plugins/FMOD/lib/{libInfo.platform}/{CPUAndLibPath(libInfo)}";
                case Platform.FileLayout.Release_2_1:
                case Platform.FileLayout.Release_2_2:
                    return $"{PlatformsFolder}/{libInfo.platform}/lib/{CPUAndLibPath(libInfo)}";
                default:
                    throw new ArgumentException("Unrecognised file layout: " + layout);
            }
        }

        private static string CPUAndLibPath(LibInfo libInfo)
        {
            return libInfo.cpu == AnyCPU ? libInfo.lib : $"{libInfo.cpu}/{libInfo.lib}";
        }

        private static string GetSourcePath(LibInfo libInfo)
        {
            return $"{StagingFolder}/{libInfo.platform}/lib/{CPUAndLibPath(libInfo)}";
        }

        private static PluginImporter GetPluginImporter(LibInfo libInfo)
        {
            var targetPath = GetTargetPath(libInfo);

            if (targetPath != null)
                return AssetImporter.GetAtPath(targetPath) as PluginImporter;
            return null;
        }

        private static UpdateStep FindUpdateStep(Settings.SharedLibraryUpdateStages stage)
        {
            return UpdateSteps.FirstOrDefault(s => s.Stage == stage);
        }

        private static void ResetUpdateStage()
        {
            if (Settings.Instance.SharedLibraryUpdateStage != Settings.SharedLibraryUpdateStages.Start ||
                Settings.Instance.SharedLibraryTimeSinceStart != 0)
            {
                Settings.Instance.SharedLibraryUpdateStage = Settings.SharedLibraryUpdateStages.Start;
                Settings.Instance.SharedLibraryTimeSinceStart = 0;
                EditorUtility.SetDirty(Settings.Instance);
            }
        }

        public static UpdateStep Startup()
        {
            if (!AssetDatabase.IsValidFolder(StagingFolder))
            {
                ResetUpdateStage();
                return null;
            }

            if (Settings.Instance.SharedLibraryUpdateStage == Settings.SharedLibraryUpdateStages.Start)
            {
                var targetLibsExist = LibrariesToUpdate.Any(info => GetPluginImporter(info) != null);

                if (targetLibsExist)
                {
                    Settings.Instance.SharedLibraryUpdateStage =
                        Settings.SharedLibraryUpdateStages.DisableExistingLibraries;
                    EditorUtility.SetDirty(Settings.Instance);
                }
                else
                {
                    FindUpdateStep(Settings.SharedLibraryUpdateStages.CopyNewLibraries).Execute();

                    ResetUpdateStage();
                    return null;
                }
            }

            if (Settings.Instance.SharedLibraryUpdateStage == Settings.SharedLibraryUpdateStages.RestartUnity
                && EditorApplication.timeSinceStartup < Settings.Instance.SharedLibraryTimeSinceStart)
            {
                // Unity has been restarted
                Settings.Instance.SharedLibraryUpdateStage = Settings.SharedLibraryUpdateStages.CopyNewLibraries;
                EditorUtility.SetDirty(Settings.Instance);
            }

            return GetNextUpdateStep();
        }

        public static UpdateStep GetNextUpdateStep()
        {
            var step = FindUpdateStep(Settings.Instance.SharedLibraryUpdateStage);

            if (step != null) step.CacheDetails();

            return step;
        }

        private struct LibInfo
        {
            public string cpu;
            public string os;
            public string lib;
            public string platform;
            public BuildTarget buildTarget;
        }

        public class UpdateStep
        {
            public string Description;
            public string Details;
            public Action Execute;

            private Func<string> GetDetails;
            public string Name;
            internal Settings.SharedLibraryUpdateStages Stage;

            public void CacheDetails()
            {
                Details = GetDetails();
            }

            internal static UpdateStep Create(Settings.SharedLibraryUpdateStages stage, string name, string description,
                Func<string> details, Action execute)
            {
                return new UpdateStep
                {
                    Stage = stage,
                    Name = name,
                    Description = description,
                    GetDetails = details,
                    Execute = execute
                };
            }
        }
    }

    public abstract class HelpContent : PopupWindowContent
    {
        private GUIContent icon;

        protected abstract void Prepare();
        protected abstract Vector2 GetContentSize();
        protected abstract void DrawContent();

        public override void OnOpen()
        {
            icon = EditorGUIUtility.IconContent("console.infoicon");

            Prepare();
        }

        public override Vector2 GetWindowSize()
        {
            var contentSize = GetContentSize();

            var iconSize = GUI.skin.label.CalcSize(icon);

            return new Vector2(contentSize.x + iconSize.x,
                Math.Max(contentSize.y, iconSize.y) + EditorGUIUtility.standardVerticalSpacing);
        }

        public override void OnGUI(Rect rect)
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(icon);
                }

                using (new GUILayout.VerticalScope())
                {
                    DrawContent();
                }
            }
        }
    }

    public class SimpleHelp : HelpContent
    {
        private GUIStyle style;
        private readonly GUIContent text;
        private readonly float textWidth;

        public SimpleHelp(string text, float textWidth = 300)
        {
            this.text = new GUIContent(text);
            this.textWidth = textWidth;
        }

        protected override void Prepare()
        {
            style = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft
            };
        }

        protected override Vector2 GetContentSize()
        {
            var textHeight = style.CalcHeight(text, textWidth) + style.margin.bottom;

            return new Vector2(textWidth, textHeight);
        }

        protected override void DrawContent()
        {
            GUILayout.Label(text, style);
        }
    }

    public class BuildStatusWatcher
    {
        public static Action OnBuildStarted;
        public static Action OnBuildEnded;

        private static bool buildInProgress;

        private static void SetBuildInProgress(bool inProgress)
        {
            if (inProgress != buildInProgress)
            {
                buildInProgress = inProgress;

                if (buildInProgress)
                {
                    EditorApplication.update += PollBuildStatus;

                    if (OnBuildStarted != null) OnBuildStarted();
                }
                else
                {
                    EditorApplication.update -= PollBuildStatus;

                    if (OnBuildEnded != null) OnBuildEnded();
                }
            }
        }

        private static void PollBuildStatus()
        {
            SetBuildInProgress(BuildPipeline.isBuildingPlayer);
        }

        public static void Startup()
        {
#if UNITY_SCRIPTABLEBUILDPIPELINE_EXIST
            BuildCallbacks callbacks = ContentPipeline.BuildCallbacks;

            callbacks.PostDependencyCallback += (parameters, dependencyData) => {
                SetBuildInProgress(true);
                return ReturnCode.Success;
            };

            callbacks.PostWritingCallback += (parameters, dependencyData, writeData, results) => {
                SetBuildInProgress(false);
                return ReturnCode.Success;
            };
#endif
        }

        private class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            public void OnPostprocessBuild(BuildReport report)
            {
                SetBuildInProgress(false);
            }

            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                SetBuildInProgress(true);
            }
        }
    }

    public static class SerializedPropertyExtensions
    {
        public static bool ArrayContains(this SerializedProperty array, Func<SerializedProperty, bool> predicate)
        {
            return FindArrayIndex(array, predicate) >= 0;
        }

        public static bool ArrayContains(this SerializedProperty array, string subPropertyName,
            Func<SerializedProperty, bool> predicate)
        {
            return FindArrayIndex(array, subPropertyName, predicate) >= 0;
        }

        public static int FindArrayIndex(this SerializedProperty array, Func<SerializedProperty, bool> predicate)
        {
            for (var i = 0; i < array.arraySize; ++i)
            {
                var current = array.GetArrayElementAtIndex(i);

                if (predicate(current)) return i;
            }

            return -1;
        }

        public static int FindArrayIndex(this SerializedProperty array, string subPropertyName,
            Func<SerializedProperty, bool> predicate)
        {
            for (var i = 0; i < array.arraySize; ++i)
            {
                var current = array.GetArrayElementAtIndex(i);
                var subProperty = current.FindPropertyRelative(subPropertyName);

                if (predicate(subProperty)) return i;
            }

            return -1;
        }

        public static void ArrayAdd(this SerializedProperty array, Action<SerializedProperty> initialize)
        {
            array.InsertArrayElementAtIndex(array.arraySize);
            initialize(array.GetArrayElementAtIndex(array.arraySize - 1));
        }

        public static void ArrayClear(this SerializedProperty array)
        {
            while (array.arraySize > 0) array.DeleteArrayElementAtIndex(array.arraySize - 1);
        }

        private static GUID GetGuid(this SerializedProperty property)
        {
            return new GUID
            {
                Data1 = property.FindPropertyRelative("Data1").intValue,
                Data2 = property.FindPropertyRelative("Data2").intValue,
                Data3 = property.FindPropertyRelative("Data3").intValue,
                Data4 = property.FindPropertyRelative("Data4").intValue
            };
        }

        public static void SetGuid(this SerializedProperty property, GUID guid)
        {
            property.FindPropertyRelative("Data1").intValue = guid.Data1;
            property.FindPropertyRelative("Data2").intValue = guid.Data2;
            property.FindPropertyRelative("Data3").intValue = guid.Data3;
            property.FindPropertyRelative("Data4").intValue = guid.Data4;
        }

        public static void SetEventReference(this SerializedProperty property, GUID guid, string path)
        {
            var guidProperty = property.FindPropertyRelative("Guid");
            guidProperty.SetGuid(guid);

            var pathProperty = property.FindPropertyRelative("Path");
            pathProperty.stringValue = path;
        }

        public static EventReference GetEventReference(this SerializedProperty property)
        {
            var pathProperty = property.FindPropertyRelative("Path");
            var guidProperty = property.FindPropertyRelative("Guid");

            return new EventReference
            {
                Path = pathProperty.stringValue,
                Guid = guidProperty.GetGuid()
            };
        }
    }

    public class NoIndentScope : IDisposable
    {
        private readonly int oldIndentLevel;

        public NoIndentScope()
        {
            oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = oldIndentLevel;
        }
    }

    public class NaturalComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return EditorUtility.NaturalCompare(a, b);
        }
    }
}