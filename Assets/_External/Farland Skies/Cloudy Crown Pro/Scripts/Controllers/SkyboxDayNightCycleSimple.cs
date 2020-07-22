#pragma warning disable 649
using System.Diagnostics.CodeAnalysis;
using Borodar.FarlandSkies.CloudyCrownPro.DotParams;
using Borodar.FarlandSkies.CloudyCrownPro.Helpers;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    [ExecuteInEditMode]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    public class SkyboxDayNightCycleSimple : Singleton<SkyboxDayNightCycleSimple>
    {
        // Sky

        [SerializeField]
        [Tooltip("List of sky colors, based on time of day. Each list item contains “time” filed that should be specified in percents (0 - 100)")]
        private SkyParamsList _skyParamsList = new SkyParamsList();

        // Stars

        [SerializeField]
        [Tooltip("Allows you to manage stars tint color over time. Each list item contains “time” filed that should be specified in percents (0 - 100)")]
        private StarsParamsList _starsParamsList = new StarsParamsList();

        // Private

        private SkyboxControllerSimple _skyboxController;

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------

        private float _timeOfDay;

        /// <summary>
        /// Time of day, in percents (0-100).</summary>
        public float TimeOfDay
        {
            get { return _timeOfDay; }
            set { _timeOfDay = value % 100f; }
        }

        public SkyParam CurrentSkyParam { get; private set; }
        public StarsParam CurrentStarsParam { get; private set; }

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Awake()
        {
            // DOT params
            _skyParamsList.Init();
            _starsParamsList.Init();
        }

        public void Start()
        {
            _skyboxController = SkyboxControllerSimple.Instance;
            CurrentSkyParam = _skyParamsList.GetParamPerTime(TimeOfDay);
            CurrentStarsParam = _starsParamsList.GetParamPerTime(TimeOfDay);
        }

        public void Update()
        {
            // Sky colors
            CurrentSkyParam = _skyParamsList.GetParamPerTime(TimeOfDay);

            _skyboxController.TopColor = CurrentSkyParam.TopColor;
            _skyboxController.BottomColor = CurrentSkyParam.BottomColor;

            // Stars colors
            CurrentStarsParam = _starsParamsList.GetParamPerTime(TimeOfDay);
            _skyboxController.StarsTint = CurrentStarsParam.TintColor;
        }

        protected void OnValidate()
        {
            // Dot params
            _skyParamsList.Update();
            _starsParamsList.Update();
        }
    }
}