using UnityEditor;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    [CustomEditor(typeof(SkyboxControllerSimple))]
    public class SkyboxControllerSimpleEditor : Editor
    {
        // Skybox
        private SerializedProperty _skyboxMaterial;
        // Sky
        private SerializedProperty _skyTopColor;
        private SerializedProperty _skyBottomColor;
        // Stars
        private SerializedProperty _starsTint;
        private SerializedProperty _starsExtinction;
        private SerializedProperty _starsTwinklingSpeed;
        // Clouds
        private SerializedProperty _cloudsHeight;
        private SerializedProperty _cloudsOffset;
        private SerializedProperty _cloudsRotationSpeed;
        // General
        private SerializedProperty _exposure;
        private SerializedProperty _adjustFogColor;

        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CustomGUILayout();
            serializedObject.ApplyModifiedProperties();
        }

        //---------------------------------------------------------------------
        // Protected
        //---------------------------------------------------------------------

        protected void OnEnable()
        {
            // Skybox
            _skyboxMaterial = serializedObject.FindProperty("SkyboxMaterial");
            // Sky
            _skyTopColor = serializedObject.FindProperty("_topColor");
            _skyBottomColor = serializedObject.FindProperty("_bottomColor");
            // Stars
            _starsTint = serializedObject.FindProperty("_starsTint");
            _starsExtinction = serializedObject.FindProperty("_starsExtinction");
            _starsTwinklingSpeed = serializedObject.FindProperty("_starsTwinklingSpeed");
            // Clouds
            _cloudsHeight = serializedObject.FindProperty("_cloudsHeight");
            _cloudsOffset = serializedObject.FindProperty("_cloudsOffset");
            _cloudsRotationSpeed = serializedObject.FindProperty("_cloudsRotationSpeed");
            // General
            _exposure = serializedObject.FindProperty("_exposure");
            _adjustFogColor = serializedObject.FindProperty("_adjustFogColor");
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private void CustomGUILayout()
        {
            // Skybox
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_skyboxMaterial);
            EditorGUILayout.Space();

            // Sky
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Sky", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_skyTopColor);
            EditorGUILayout.PropertyField(_skyBottomColor);
            EditorGUILayout.Space();

            // Stars
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Stars", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_starsTint);
            EditorGUILayout.PropertyField(_starsExtinction);
            EditorGUILayout.PropertyField(_starsTwinklingSpeed);
            EditorGUILayout.Space();

            // Clouds
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Clouds", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_cloudsHeight);
            EditorGUILayout.PropertyField(_cloudsOffset);
            EditorGUILayout.PropertyField(_cloudsRotationSpeed);
            EditorGUILayout.Space();

            // General
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_exposure);
            EditorGUILayout.PropertyField(_adjustFogColor);
            EditorGUILayout.Space();
        }
    }
}