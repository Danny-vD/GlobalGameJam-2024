using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FMODUnity
{
    public class PlatformDefault : Platform
    {
        public const string ConstIdentifier = "default";

        public PlatformDefault()
        {
            Identifier = ConstIdentifier;
        }

        internal override string DisplayName => "Default";

        internal override bool IsIntrinsic => true;

        // null means no valid output types - don't display the field in the UI
#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes => null;
#endif
        internal override void DeclareRuntimePlatforms(Settings settings)
        {
        }

        internal override void InitializeProperties()
        {
            base.InitializeProperties();

            PropertyAccessors.Plugins.Set(this, new List<string>());
            PropertyAccessors.StaticPlugins.Set(this, new List<string>());
        }

        internal override void EnsurePropertiesAreValid()
        {
            base.EnsurePropertiesAreValid();

            if (StaticPlugins == null) PropertyAccessors.StaticPlugins.Set(this, new List<string>());
        }
#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield break;
        }

        internal override Legacy.Platform LegacyIdentifier => Legacy.Platform.Default;

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