using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Holds a reference to an InkFile object for every .ink file detected in the Assets folder.
/// Provides helper functions to easily obtain these files.
/// ScriptableSingleton doesn't reload when the backing file changes, which means if you pull changes via source control you need to make unity recompile before it'll load the change.
/// </summary>
namespace Ink.UnityIntegration
{
    // #if UNITY_2020_1_OR_NEWER
    // [FilePath("ProjectSettings/InkSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    // public class InkSettings : ScriptableSingleton<InkSettings> {
    // #else
    public class InkSettings : ScriptableObject
    {
        // #endif
        // #if !UNITY_2020_1_OR_NEWER
        public static bool created =>
            // If it's null, there's just no InkSettings asset in the project
            _instance != null;

        private static string absoluteSavePath =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", "ProjectSettings", "InkSettings.asset"));

        public static void SaveStatic(bool saveAsText)
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new[] { instance }, absoluteSavePath, saveAsText);
        }

        public void Save(bool saveAsText)
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new InkSettings[1] { this }, absoluteSavePath,
                saveAsText);
        }

        private static InkSettings _instance;

        public static InkSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    var objects = InternalEditorUtility.LoadSerializedFileAndForget(absoluteSavePath);
                    if (objects != null && objects.Length > 0)
                    {
                        instance = objects[0] as InkSettings;
                    }
                    else
                    {
                        instance = CreateInstance<InkSettings>();
                        instance.Save(true);
                    }
                    // Oh gosh Unity never unloads ScriptableObjects once created! This fixes it but is more of an expensive call than I like.
                    // I've commented this out in favour of a callback approach - see OnEnable. Left this for posterity in case we need to return to this. 
                    // foreach (var settings in Resources.FindObjectsOfTypeAll<InkSettings>()) {
                    // 	if(settings == instance) continue;
                    // 	DestroyImmediate(settings);
                    // }
                }

                return _instance;
            }
            private set
            {
                if (_instance == value) return;
                _instance = value;
            }
        }
        // #endif

        public class AssetSaver : AssetModificationProcessor
        {
            private static string[] OnWillSaveAssets(string[] paths)
            {
                instance.Save(true);
                return paths;
            }
        }


        public DefaultAsset templateFile;

        public string templateFilePath
        {
            get
            {
                if (templateFile == null) return "";
                return AssetDatabase.GetAssetPath(templateFile);
            }
        }


        public DefaultAsset defaultJsonAssetPath;

        [FormerlySerializedAs("compileAutomatically")]
        public bool compileAllFilesAutomatically = true;

        public List<DefaultAsset> includeFilesToCompileAsMasterFiles = new();
        public List<DefaultAsset> filesToCompileAutomatically = new();
        public bool delayInPlayMode = true;
        public bool handleJSONFilesAutomatically = true;

        public int compileTimeout = 30;

        public bool printInkLogsInConsoleOnCompile;

        public bool suppressStartupWindow;

#if UNITY_EDITOR && !UNITY_2018_1_OR_NEWER
		[MenuItem("Edit/Project Settings/Ink", false, 500)]
		public static void SelectFromProjectSettings() {
			Selection.activeObject = instance;
		}
#elif UNITY_EDITOR && UNITY_2018_1_OR_NEWER
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(instance);
        }
#endif

        public bool ShouldCompileInkFileAutomatically(InkFile inkFile)
        {
            return compileAllFilesAutomatically ||
                   (inkFile.isMaster && filesToCompileAutomatically.Contains(inkFile.inkAsset));
        }


        private void OnEnable()
        {
            // Oh gosh Unity never unloads ScriptableObjects once created! We destroy these objects before we recompile so there's only ever one in memory at once.
            AssemblyReloadEvents.beforeAssemblyReload += () => { DestroyImmediate(this); };
            // Validate the includeFilesToCompileAsMasterFiles list.
            for (var i = includeFilesToCompileAsMasterFiles.Count - 1; i >= 0; i--)
                if (includeFilesToCompileAsMasterFiles[i] == null)
                {
                    includeFilesToCompileAsMasterFiles.RemoveAt(i);
                    Debug.LogError("REMOVE " + includeFilesToCompileAsMasterFiles.Count);
                }

            // Validate the filesToCompileAutomatically list.
            for (var i = filesToCompileAutomatically.Count - 1; i >= 0; i--)
                if (filesToCompileAutomatically[i] == null)
                    filesToCompileAutomatically.RemoveAt(i);
            // Deletes the persistent version of this asset that we used to use prior to 0.9.71
            if (!Application.isPlaying && EditorUtility.IsPersistent(this))
            {
                var path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
#if !UNITY_2020_1_OR_NEWER
                    if(_instance == this) _instance = null;
#endif
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}