﻿using System.Collections.Generic;
using FMOD;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "__Internal";
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "__Internal";
    }
}
#endif

namespace FMODUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class PlatformWebGL : Platform
    {
        static PlatformWebGL()
        {
            Settings.AddPlatformTemplate<PlatformWebGL>("46fbfdf3fc43db0458918377fd40293e");
        }

        internal override string DisplayName => "WebGL";

        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.WebGLPlayer, this);
        }

        internal override string GetPluginPath(string pluginName)
        {
#if UNITY_2021_2_OR_NEWER
            return string.Format("{0}/{1}.a", GetPluginBasePath(), pluginName);
#else
            return string.Format("{0}/{1}.bc", GetPluginBasePath(), pluginName);
#endif
        }

#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield return BuildTarget.WebGL;
        }

        internal override Legacy.Platform LegacyIdentifier => Legacy.Platform.WebGL;

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return new BinaryAssetFolderInfo("html5", "Plugins/WebGL");
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants,
            string suffix)
        {
#if UNITY_2021_2_OR_NEWER
            var useWASM = true;
#else
            bool useWASM = false;
#endif

            if (allVariants || useWASM) yield return new FileRecord(string.Format("2.0.19/libfmodstudio{0}.a", suffix));

            if (allVariants || !useWASM)
                yield return new FileRecord(string.Format("libfmodstudiounityplugin{0}.bc", suffix));
        }

        internal override bool IsFMODStaticallyLinked => true;
#endif
#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes => sValidOutputTypes;

        private static readonly OutputType[] sValidOutputTypes =
        {
            new() { displayName = "JavaScript webaudio output", outputType = OUTPUTTYPE.WEBAUDIO }
        };
#endif
    }
}