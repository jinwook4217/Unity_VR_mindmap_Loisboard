using Borodar.FarlandSkies.CloudyCrownPro.Helpers;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    [ExecuteInEditMode]
    public class SkyboxControllerSimple : Singleton<SkyboxControllerSimple>
    {
        // Skybox
        public Material SkyboxMaterial;

        // Sky

        [SerializeField]
        [Tooltip("Color at the top pole of skybox sphere")]
        private Color _topColor = new Color(0.247f, 0.318f, 0.561f);

        [SerializeField]
        [Tooltip("Color at the bottom pole of skybox sphere")]
        private Color _bottomColor = new Color(0.773f, 0.455f, 0.682f);

        // Stars

        [SerializeField]
        private Color _starsTint = Color.gray;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("Reduction in stars apparent brightness closer to the horizon")]
        private float _starsExtinction = 2f;

        [SerializeField]
        [Range(0f, 25f)]
        [Tooltip("Variation in stars apparent brightness caused by the atmospheric turbulence")]
        private float _starsTwinklingSpeed = 4f;

        // Clouds

        [SerializeField]
        [Range(-0.75f, 0.75f)]
        [Tooltip("Height of the clouds relative to the horizon")]
        private float _cloudsHeight;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Distance between the cloud waves")]
        private float _cloudsOffset = 0.2f;

        [SerializeField]
        [Range(-50f, 50f)]
        [Tooltip("Rotation of the clouds around the positive y axis")]
        private float _cloudsRotationSpeed = 1f;

        // General

        [SerializeField]
        [Range(0, 10f)]
        [Tooltip("Adjusts the brightness of the skybox")]
        private float _exposure = 1f;

        [SerializeField]
        [Tooltip("Keep fog color in sync with the sky middle color automatically")]
        private bool _adjustFogColor;

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------

        // Sky

        public Color TopColor
        {
            get { return _topColor; }
            set
            {
                _topColor = value;
                SkyboxMaterial.SetColor("_TopColor", _topColor);
            }
        }

        public Color BottomColor
        {
            get { return _bottomColor; }
            set
            {
                _bottomColor = value;
                SkyboxMaterial.SetColor("_BottomColor", _bottomColor);
            }
        }

        // Stars

        public Color StarsTint
        {
            get { return _starsTint; }
            set
            {
                _starsTint = value;
                SkyboxMaterial.SetColor("_StarsTint", _starsTint);
            }
        }

        public float StarsExtinction
        {
            get { return _starsExtinction; }
            set
            {
                _starsExtinction = value;
                SkyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
            }
        }

        public float StarsTwinklingSpeed
        {
            get { return _starsTwinklingSpeed; }
            set
            {
                _starsTwinklingSpeed = value;
                SkyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
            }
        }

        // Clouds

        public float CloudsHeight
        {
            get { return _cloudsHeight; }
            set
            {
                _cloudsHeight = value;
                SkyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
            }
        }

        public float CloudsOffset
        {
            get { return _cloudsOffset; }
            set
            {
                _cloudsOffset = value;
                SkyboxMaterial.SetFloat("_CloudsOffset", _cloudsOffset);
            }
        }

        public float CloudsRotationSpeed
        {
            get { return _cloudsRotationSpeed; }
            set
            {
                _cloudsRotationSpeed = value;
                SkyboxMaterial.SetFloat("_CloudsRotationSpeed", _cloudsRotationSpeed);
            }
        }

        // General

        public float Exposure
        {
            get { return _exposure; }
            set
            {
                _exposure = value;
                SkyboxMaterial.SetFloat("_Exposure", _exposure);
            }
        }

        public bool AdjustFogColor
        {
            get { return _adjustFogColor; }
            set
            {
                _adjustFogColor = value;
                if (_adjustFogColor) RenderSettings.fogColor = BottomColor;
            }
        }

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Awake()
        {
            if (SkyboxMaterial != null)
            {
                RenderSettings.skybox = SkyboxMaterial;
                UpdateSkyboxProperties();
            }
            else
            {
                Debug.LogWarning("SkyboxController: Skybox material is not assigned.");
            }
        }

        protected void OnValidate()
        {
            UpdateSkyboxProperties();
        }

        protected void Update()
        {
            if (_adjustFogColor) RenderSettings.fogColor = _bottomColor;
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private void UpdateSkyboxProperties()
        {
            if (SkyboxMaterial == null) return;

            // Sky
            SkyboxMaterial.SetColor("_TopColor", _topColor);
            SkyboxMaterial.SetColor("_BottomColor", _bottomColor);
            // Stars
            SkyboxMaterial.SetColor("_StarsTint", _starsTint);
            SkyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
            SkyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
            // Clouds
            SkyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
            SkyboxMaterial.SetFloat("_CloudsOffset", _cloudsOffset);
            SkyboxMaterial.SetFloat("_CloudsRotationSpeed", _cloudsRotationSpeed);
            // General
            SkyboxMaterial.SetFloat("_Exposure", _exposure);
        }
    }
}
