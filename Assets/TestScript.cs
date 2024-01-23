using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class TestScript : MonoBehaviour
{
	public ARSession _ARSession;

	public ARTrackedImageManager _ARTrackedImageManager;

	public void OPD()
	{
		_ARTrackedImageManager.enabled = true;
	}

	public void OPU()
	{
		_ARTrackedImageManager.enabled = false;
	}

	public void ARReset()
	{
		_ARSession.Reset();
	}
}
