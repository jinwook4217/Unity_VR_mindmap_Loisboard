using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class PointerExitSubMenu : MonoBehaviour, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData ped)
        {
            if (!IsCube(ped.pointerCurrentRaycast.gameObject))
            {
                transform.parent.GetComponentInChildren<Cube>().HideSubContainer();
            }
        }

        private bool IsCube(GameObject currentHover)
        {
            if (!currentHover)
            {
                return false;
            }
            else
            {
                if (!currentHover.CompareTag("Cube"))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
