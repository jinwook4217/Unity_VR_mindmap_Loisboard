using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    public class TimeSlider : MonoBehaviour
    {
        private Slider _slider;
        private SceneManager _sceneManager;

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        protected void Start()
        {
            _sceneManager = SceneManager.Instance;
        }

        protected void Update()
        {
            _slider.value = _sceneManager.TimeOfDay;
        }

        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------

        public void OnValueChanged(float value)
        {
            _sceneManager.TimeOfDay = value;
        }
    }
}