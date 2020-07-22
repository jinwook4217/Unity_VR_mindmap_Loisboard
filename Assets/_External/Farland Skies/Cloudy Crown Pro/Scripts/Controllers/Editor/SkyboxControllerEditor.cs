using UnityEditor;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    [CustomEditor(typeof(SkyboxController))]
    public class SkyboxControllerEditor : Editor
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
        // Sun
        private SerializedProperty _sunLight;
        private SerializedProperty _sunTint;
        private SerializedProperty _sunSize;
        private SerializedProperty _sunFlare;
        private SerializedProperty _sunFlareBrightness;
        // Moon
        private SerializedProperty _moonLight;
        private SerializedProperty _moonTint;
        private SerializedProperty _moonSize;
        private SerializedProperty _moonFlare;
        private SerializedProperty _moonFlareBrightness;
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
            // Sun
            _sunLight = serializedObject.FindProperty("_sunLight");
            _sunTint = serializedObject.FindProperty("_sunTint");
            _sunSize = serializedObject.FindProperty("_sunSize");
            _sunFlare = serializedObject.FindProperty("_sunFlare");
            _sunFlareBrightness = serializedObject.FindProperty("_sunFlareBrightness");
            // Moon
            _moonLight = serializedObject.FindProperty("_moonLight");
            _moonTint = serializedObject.FindProperty("_moonTint");
            _moonSize = serializedObject.FindProperty("_moonSize");
            _moonFlare = serializedObject.FindProperty("_moonFlare");
            _moonFlareBrightness = serializedObject.FindProperty("_moonFlareBrightness");
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

            // Sun
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Sun", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_sunLight);
            EditorGUILayout.PropertyField(_sunTint);
            EditorGUILayout.PropertyField(_sunSize);
            EditorGUILayout.PropertyField(_sunFlare);
            EditorGUILayout.PropertyField(_sunFlareBrightness);
            EditorGUILayout.Space();

            // Moon
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Moon", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_moonLight);
            EditorGUILayout.PropertyField(_moonTint);
            EditorGUILayout.PropertyField(_moonSize);
            EditorGUILayout.PropertyField(_moonFlare);
            EditorGUILayout.PropertyField(_moonFlareBrightness);
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