using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class TestMarker : MonoBehaviour
{
	public ARCustom _ARCustom;

	private void Start()
	{
		_ARCustom = GameObject.FindGameObjectWithTag("Test").GetComponent<TestScript>()._ARCustom;

		_ARCustom._Debug._Markers.Add(gameObject);
	}
}
