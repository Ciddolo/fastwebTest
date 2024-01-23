using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class TestMarker : MonoBehaviour
{
	public TestScript _TestScript;

	public ARTrackedImage _ARTrackedImage;

	public int _Int;

	private void Start()
	{
		GameObject GO = GameObject.FindGameObjectWithTag("Test");

		_TestScript = GO.GetComponent<TestScript>();

		_ARTrackedImage = GO.GetComponent<ARTrackedImage>();

		_TestScript._Markers.Add(gameObject);

		_Int = _TestScript._Int;

		_TestScript._Int++;
	}
}
