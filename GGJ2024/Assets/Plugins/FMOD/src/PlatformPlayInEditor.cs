using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "fmodstudioL";
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "fmodstudioL";
    }
}
#endif

namespace FMODUnity
{
    public class PlatformPlayInEditor : Platform
    {
        private static readonly List<CodecChannelCount> staticCodecChannels = new()
        {
            new() { format = CodecType.FADPCM, channels = 0 },
            new() { format = CodecType.Vorbis, channels = 256 }
        };

        public PlatformPlayInEditor()
        {
            Identifier = "playInEditor";
        }

        internal override string DisplayName => "Editor";

        internal override bool IsIntrinsic => true;
#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes => null;
#endif

        internal override List<CodecChannelCount> DefaultCodecChannels => staticCodecChannels;

        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.OSXEditor, this);
            settings.DeclareRuntimePlatform(RuntimePlatform.WindowsEditor, this);
            settings.DeclareRuntimePlatform(RuntimePlatform.LinuxEditor, this);
        }

        internal override string GetBankFolder()
        {
            // Use original asset location because streaming asset folder will contain platform specific banks
            var globalSettings = Settings.Instance;

            var bankFolder = globalSettings.SourceBankPath;
            if (globalSettings.HasPlatforms)
                bankFolder = RuntimeUtils.GetCommonPlatformPath(Path.Combine(bankFolder, BuildDirectory));

            return bankFolder;
        }

#if UNITY_EDITOR
        internal override string GetPluginPath(string pluginName)
        {
            var platformsFolder = Path.GetFullPath($"{RuntimeUtils.PluginBasePath}/platforms");

#if UNITY_EDITOR_WIN && UNITY_EDITOR_64
            return string.Format("{0}/win/lib/x86_64/{1}.dll", platformsFolder, pluginName);
#elif UNITY_EDITOR_WIN
            return string.Format("{0}/win/lib/x86/{1}.dll", platformsFolder, pluginName);
#elif UNITY_EDITOR_OSX
            var pluginPath = string.Format("{0}/mac/lib/{1}.bundle", platformsFolder, pluginName);
            if (Directory.Exists(pluginPath))
                return pluginPath;
            return string.Format("{0}/mac/lib/{1}.dylib", platformsFolder, pluginName);
#elif UNITY_EDITOR_LINUX && UNITY_EDITOR_64
            return string.Format("{0}/linux/lib/x86_64/lib{1}.so", platformsFolder, pluginName);
#elif UNITY_EDITOR_LINUX
            return string.Format("{0}/linux/lib/x86/lib{1}.so", platformsFolder, pluginName);
#endif
        }
#endif

        internal override void LoadStaticPlugins(FMOD.System coreSystem, Action<RESULT, string> reportResult)
        {
            // Ignore static plugins when playing in the editor
        }

        internal override void InitializeProperties()
        {
            base.InitializeProperties();

            PropertyAccessors.LiveUpdate.Set(this, TriStateBool.Enabled);
            PropertyAccessors.Overlay.Set(this, TriStateBool.Enabled);
            PropertyAccessors.SampleRate.Set(this, 48000);
            PropertyAccessors.RealChannelCount.Set(this, 256);
            PropertyAccessors.VirtualChannelCount.Set(this, 1024);
        }
#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield break;
        }

        internal override Legacy.Platform LegacyIdentifier => Legacy.Platform.PlayInEditor;

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return null;
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants,
            string suffix)
        {
            yield break;
        }
#endif
    }
}