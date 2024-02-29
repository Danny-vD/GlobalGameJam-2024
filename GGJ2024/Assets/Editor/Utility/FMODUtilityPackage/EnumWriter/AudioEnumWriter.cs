using System;
using System.IO;
using System.Linq;
using System.Text;
using FMODUnity;
using FMODUtilityPackage.Enums;
using FMODUtilityPackage.Utility;
using UnityEditor.Compilation;
using UnityEngine;
using Utility.EditorPackage;

namespace Utility.FMODUtilityPackage.EnumWriter
{
	/// <summary>
	///     Used to write all the event paths to a file in the resources folder to be able to retrieve the eventpaths without
	///     having to put an instantiated AudioManager in the scene
	/// </summary>
	public static class AudioEnumWriter
    {
        private const string scriptsFolder = "1. Scripts";
        private const string subFolder = "/zExternalPackages";

        private static readonly string typePath = @$"{Application.dataPath}/{scriptsFolder}{subFolder}/FMODUtilityPackage/Enums/";

        public static void WriteFmodEventsToEnum()
        {
            var editorEventRefs = EventManager.Events;
            var enumValues = editorEventRefs.Select(eventref => eventref.Path).ToArray();

            var eventPaths = new string[enumValues.Length];
            Array.Copy(enumValues, eventPaths, enumValues.Length);

            WriteToResourcesUtil.WriteToResources(eventPaths, "EventPaths.txt", "FMODUtilityPackage/");

            enumValues = EventPathToEnumValueUtil.ConvertEventPathToEnumValuesString(enumValues);

            WriteToFile(typePath, nameof(AudioEventType), enumValues, eventPaths);
        }

        public static void WriteToFile(string path, string typeName, string[] values, string[] documentation)
        {
            WriteEnumValues(values, documentation, path, typeName);

            CompilationPipeline.RequestScriptCompilation();
        }

        private static void WriteEnumValues(string[] values, string[] documentation, string path, string typeName)
        {
            var fullPath = $"{path}{typeName}.cs";

            var content = File.ReadAllText(fullPath);

            var enumDeclaration = $"enum {typeName}";

            var startIndex = content.IndexOf(enumDeclaration, StringComparison.Ordinal); // Find the enum declaration

            if (startIndex == -1)
            {
                Debug.LogError("No valid enum declaration found in file");
                return;
            }

            startIndex = content.IndexOf('{', startIndex); // Find the opening brace of the enum
            var closeIndex = content.IndexOf('}', startIndex); // Find the closing brace of the enum

            var afterEnum = string.Empty;

            if (closeIndex != -1) afterEnum = content.Substring(closeIndex);

            var beforeEnum = content.Substring(0, startIndex);
            var builder = new StringBuilder(beforeEnum);
            builder.AppendLine("{");

            for (var i = 0; i < values.Length; i++)
            {
                builder.AppendLine("\t\t/// <FMODEventPath>");
                builder.AppendLine(
                    $"\t\t/// {documentation[i].Replace("&", "&amp;").Replace("'", "&apos;")}"); // XML does not allow special characters directly
                builder.AppendLine("\t\t/// </FMODEventPath>");
                builder.AppendLine($"\t\t{values[i]},");

                if (i != values.Length - 1) builder.AppendLine();
            }

            builder.Append("\t");

            builder.Append(afterEnum);

            File.WriteAllText(fullPath, builder.ToString());
        }
    }
}