using System.Collections.Generic;
using FMOD;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
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
    public class PlatformLinux : Platform
    {
        private static readonly List<CodecChannelCount> staticCodecChannels = new()
        {
            new() { format = CodecType.FADPCM, channels = 0 },
            new() { format = CodecType.Vorbis, channels = 32 }
        };

        static PlatformLinux()
        {
            Settings.AddPlatformTemplate<PlatformLinux>("b7716510a1f36934c87976f3a81dbf3d");
        }

        internal override string DisplayName => "Linux";

        internal override List<CodecChannelCount> DefaultCodecChannels => staticCodecChannels;

        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.LinuxPlayer, this);
        }

        internal override string GetPluginPath(string pluginName)
        {
            return string.Format("{0}/lib{1}.so", GetPluginBasePath(), pluginName);
        }

#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield return BuildTarget.StandaloneLinux64;
        }

        internal override Legacy.Platform LegacyIdentifier => Legacy.Platform.Linux;

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return new BinaryAssetFolderInfo("linux", "Plugins");
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants,
            string suffix)
        {
            yield return new FileRecord(string.Format("x86_64/libfmodstudio{0}.so", suffix));
        }

        protected override IEnumerable<FileRecord> GetOptionalBinaryFiles(BuildTarget buildTarget, bool allVariants)
        {
            if (allVariants)
            {
                yield return new FileRecord("x86_64/libfmod.so");
                yield return new FileRecord("x86_64/libfmodL.so");
            }

            yield return new FileRecord("x86_64/libgvraudio.so");
            yield return new FileRecord("x86_64/libresonanceaudio.so");
        }

        protected override IEnumerable<string> GetObsoleteFiles()
        {
            yield return "platforms/linux/lib/x86/libfmodstudio.so";
            yield return "platforms/linux/lib/x86/libfmodstudioL.so";
        }
#endif

#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes => sValidOutputTypes;

        private static readonly OutputType[] sValidOutputTypes =
        {
            new() { displayName = "Pulse Audio", outputType = OUTPUTTYPE.PULSEAUDIO },
            new() { displayName = "Advanced Linux Sound Architecture", outputType = OUTPUTTYPE.ALSA }
        };
#endif
    }
}