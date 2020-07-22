using Borodar.FarlandSkies.CloudyCrownPro.Helpers;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    public class SceneManager : Singleton<SceneManager>
    {
        public float TimeOfDay;
        [Space(10)]
        public bool PauseTime;
        public float TimeMultiplier = 1f;

        private SkyboxDayNightCycle _skyboxDayNightCycle;

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Start()
        {
            _skyboxDayNightCycle = SkyboxDayNightCycle.Instance;
        }

        protected void Update()
        {
            if (!PauseTime)
            {
                TimeOfDay = (TimeOfDay + 3f * Time.deltaTime * TimeMultiplier) % 100f;
            }

            if (_skyboxDayNightCycle != null)
            {
                _skyboxDayNightCycle.TimeOfDay = TimeOfDay;
            }
        }
    }
}