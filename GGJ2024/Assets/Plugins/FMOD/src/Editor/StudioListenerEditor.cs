﻿using FMOD;
using UnityEditor;

namespace FMODUnity
{
    [CustomEditor(typeof(StudioListener))]
    [CanEditMultipleObjects]
    public class StudioListenerEditor : Editor
    {
        public SerializedProperty attenuationObject;

        private void OnEnable()
        {
            attenuationObject = serializedObject.FindProperty("attenuationObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            var index = ((StudioListener)serializedObject.targetObject).ListenerNumber;
            EditorGUILayout.IntSlider("Listener Index", index, 0, CONSTANTS.MAX_LISTENERS - 1);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(attenuationObject);
            serializedObject.ApplyModifiedProperties();
        }
    }
}