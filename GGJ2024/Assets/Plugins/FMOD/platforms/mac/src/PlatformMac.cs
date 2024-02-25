using System.Collections.Generic;
using System.IO;
using FMOD;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "fmodstudio" + dllSuffix;
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "fmodstudio" + dllSuffix;
    }
}
#endif

namespace FMODUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class PlatformMac : Platform
    {
        private static readonly List<CodecChannelCount> staticCodecChannels = new()
        {
            new() { format = CodecType.FADPCM, channels = 0 },
            new() { format = CodecType.Vorbis, channels = 32 }
        };

        static PlatformMac()
        {
            Settings.AddPlatformTemplate<PlatformMac>("52eb9df5db46521439908db3a29a1bbb");
        }

        internal override string DisplayName => "macOS";

        internal override List<CodecChannelCount> DefaultCodecChannels => staticCodecChannels;

        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.OSXPlayer, this);
        }

        internal override string GetPluginPath(string pluginName)
        {
            var pluginPath = string.Format("{0}/{1}.bundle", GetPluginBasePath(), pluginName);
            if (Directory.Exists(pluginPath))
                return pluginPath;
            return string.Format("{0}/{1}.dylib", GetPluginBasePath(), pluginName);
        }

#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield return BuildTarget.StandaloneOSX;
        }

        internal override Legacy.Platform LegacyIdentifier => Legacy.Platform.Mac;

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return new BinaryAssetFolderInfo("mac", "Plugins");
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants,
            string suffix)
        {
            yield return new FileRecord(string.Format("fmodstudio{0}.bundle", suffix));
        }

        protected override IEnumerable<FileRecord> GetOptionalBinaryFiles(BuildTarget buildTarget, bool allVariants)
        {
            yield return new FileRecord("gvraudio.bundle");
            yield return new FileRecord("resonanceaudio.bundle");
        }

        internal override bool SupportsAdditionalCPP(BuildTarget target)
        {
            return false;
        }
#endif
#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes => sValidOutputTypes;

        private static readonly OutputType[] sValidOutputTypes =
        {
            new() { displayName = "Core Audio", outputType = OUTPUTTYPE.COREAUDIO }
        };
#endif
    }
}