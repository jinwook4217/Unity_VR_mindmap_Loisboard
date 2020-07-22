using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRModeToggle : MonoBehaviour {

public bool VRMode = true;
public static VRModeToggle Instance;
void Awake()
{
	if(Instance == null) Instance = this;
}
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			VRMode = !VRMode;
			GvrViewer.Instance.VRModeEnabled = VRMode;
			
		}
		
	}
}
