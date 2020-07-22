using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCheck : MonoBehaviour {


	void Update () {
		if(Application.internetReachability == NetworkReachability.NotReachable)
        {
		StartCoroutine(TutorialManager.Instance.NetworkWarning());
		}
	}
}
