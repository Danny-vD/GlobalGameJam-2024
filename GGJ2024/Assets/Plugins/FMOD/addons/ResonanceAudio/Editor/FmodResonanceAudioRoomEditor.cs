// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEditor;
using UnityEngine;

namespace FMODUnityResonance
{
    /// A custom editor for properties on the FmodResonanceAudioRoom script. This appears in the
    /// Inspector window of a FmodResonanceAudioRoom object.
    [CustomEditor(typeof(FmodResonanceAudioRoom))]
    [CanEditMultipleObjects]
    public class FmodResonanceAudioRoomEditor : Editor
    {
        private SerializedProperty backWall;
        private SerializedProperty ceiling;
        private SerializedProperty floor;
        private SerializedProperty frontWall;
        private SerializedProperty leftWall;
        private SerializedProperty reflectivity;

        private readonly GUIContent reflectivityLabel = new("Reflectivity",
            "Adjusts what proportion of the direct sound is reflected back by each surface, after an " +
            "appropriate delay. Reverberation is unaffected by this setting.");

        private SerializedProperty reverbBrightness;

        private readonly GUIContent reverbBrightnessLabel = new("Brightness",
            "Adjusts the balance between high and low frequencies in the reverb.");

        private SerializedProperty reverbGainDb;

        private readonly GUIContent reverbGainLabel = new("Gain (dB)",
            "Applies a gain adjustment to the reverberation in the room. The default value will leave " +
            "reverb unaffected.");

        private readonly GUIContent reverbPropertiesLabel = new("Reverb Properties",
            "Parameters to adjust the reverb properties of the room.");

        private SerializedProperty reverbTime;

        private readonly GUIContent reverbTimeLabel = new("Time",
            "Adjusts the overall duration of the reverb by a positive scaling factor.");

        private SerializedProperty rightWall;
        private SerializedProperty size;
        private readonly GUIContent sizeLabel = new("Size", "Sets the room dimensions.");

        private readonly GUIContent surfaceMaterialLabel = new("Surface Material",
            "Surface material used to calculate the acoustic properties of the room.");

        private readonly GUIContent surfaceMaterialsLabel = new("Surface Materials",
            "Room surface materials to calculate the acoustic properties of the room.");

        private void OnEnable()
        {
            leftWall = serializedObject.FindProperty("LeftWall");
            rightWall = serializedObject.FindProperty("RightWall");
            floor = serializedObject.FindProperty("Floor");
            ceiling = serializedObject.FindProperty("Ceiling");
            backWall = serializedObject.FindProperty("BackWall");
            frontWall = serializedObject.FindProperty("FrontWall");
            reflectivity = serializedObject.FindProperty("Reflectivity");
            reverbGainDb = serializedObject.FindProperty("ReverbGainDb");
            reverbBrightness = serializedObject.FindProperty("ReverbBrightness");
            reverbTime = serializedObject.FindProperty("ReverbTime");
            size = serializedObject.FindProperty("Size");
        }

        /// @cond
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Add clickable script field, as would have been provided by DrawDefaultInspector()
            var script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField(surfaceMaterialsLabel);
            ++EditorGUI.indentLevel;
            DrawSurfaceMaterial(leftWall);
            DrawSurfaceMaterial(rightWall);
            DrawSurfaceMaterial(floor);
            DrawSurfaceMaterial(ceiling);
            DrawSurfaceMaterial(backWall);
            DrawSurfaceMaterial(frontWall);
            --EditorGUI.indentLevel;

            EditorGUILayout.Separator();

            EditorGUILayout.Slider(reflectivity, 0.0f, FmodResonanceAudio.MaxReflectivity, reflectivityLabel);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(reverbPropertiesLabel);
            ++EditorGUI.indentLevel;
            EditorGUILayout.Slider(reverbGainDb, FmodResonanceAudio.MinGainDb, FmodResonanceAudio.MaxGainDb,
                reverbGainLabel);
            EditorGUILayout.Slider(reverbBrightness, FmodResonanceAudio.MinReverbBrightness,
                FmodResonanceAudio.MaxReverbBrightness, reverbBrightnessLabel);
            EditorGUILayout.Slider(reverbTime, 0.0f, FmodResonanceAudio.MaxReverbTime, reverbTimeLabel);
            --EditorGUI.indentLevel;

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(size, sizeLabel);

            serializedObject.ApplyModifiedProperties();
        }

        /// @endcond
        private void DrawSurfaceMaterial(SerializedProperty surfaceMaterial)
        {
            surfaceMaterialLabel.text = surfaceMaterial.displayName;
            EditorGUILayout.PropertyField(surfaceMaterial, surfaceMaterialLabel);
        }
    }
}