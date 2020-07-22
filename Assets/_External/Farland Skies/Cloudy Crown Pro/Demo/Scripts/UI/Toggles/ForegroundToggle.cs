using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    public class ForegroundToggle : MonoBehaviour
    {
        [SerializeField]
        protected GameObject ForegroundObject;

        public void OnValueChanged(bool value)
        {
            ForegroundObject.SetActive(value);
        }
    }
}