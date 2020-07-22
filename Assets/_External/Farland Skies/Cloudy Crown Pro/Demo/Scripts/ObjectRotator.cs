using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
    public class ObjectRotator : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 EulerAngles;

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Update()
        {
            transform.Rotate(EulerAngles * Time.deltaTime);
        }
    }
}
