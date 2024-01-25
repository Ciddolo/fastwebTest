using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public enum ARCustomStates
{
	Starting,
	Tracking,
	Waiting,
	Resetting
}

public enum ARGameObjects
{
	SessionGO,
	SessionPrefab,
	MarkerPrefab,
}

[Serializable]
public class ARCustom
{
	[Header("References")]
	public GameObject[] _ArGameObjects;

	public ARSession _ARSession;

	public ARTrackedImageManager _ARTrackedImageManager;

	public XRReferenceImageLibrary _Library;

	[Header("Data")]
	public ARCustomStates _ARState;

	public bool _PrintDebug;

	public List<GameObject> _Markers;

	private ARTrackedImagesChangedEventArgs _info;

	public void Init()
	{
		_ARState = ARCustomStates.Starting;

		_ARTrackedImageManager.trackedImagesChanged += (info) => { PrefabManager(info); };

		_Markers = new List<GameObject>();

		_ARState = ARCustomStates.Waiting;
	}

	private void PrefabManager(ARTrackedImagesChangedEventArgs info)
	{
		_info = info;

		if (!_PrintDebug) return;

		for (int i = 0; i < _info.added.Count; i++)
		{
			Debug.Log(string.Format("ADDED {0} {1}", _info.added[i].referenceImage.name, _info.added[i].referenceImage.ToString()));
		}

		for (int i = 0; i < _info.updated.Count; i++)
		{
			Debug.Log(string.Format("UPDATED {0} {1}", _info.updated[i].referenceImage.name, _info.updated[i].referenceImage.ToString()));
		}

		for (int i = 0; i < _info.removed.Count; i++)
		{
			Debug.Log(string.Format("REMOVED {0} {1}", _info.removed[i].referenceImage.name, _info.removed[i].referenceImage.ToString()));
		}

		Debug.Log(_ARTrackedImageManager.trackables);
	}

	public void StartTracking()
	{
		if (_ARState != ARCustomStates.Waiting) return;

		if (_ARTrackedImageManager.trackables.count > 0) return;

		_ARTrackedImageManager.enabled = true;

		_ARState = ARCustomStates.Tracking;
	}

	public void StopTracking()
	{
		if (_ARState != ARCustomStates.Tracking) return;

		_ARTrackedImageManager.enabled = false;

		_ARState = ARCustomStates.Waiting;
	}

	public IEnumerator ResetSession()
	{
		if (_ARState == ARCustomStates.Resetting) yield break;

		if (_Markers.Count <= 0) yield break;

		_ARState = ARCustomStates.Resetting;

		_ARTrackedImageManager.enabled = true;

		while (_ARTrackedImageManager.trackables.count > 0)
		{
			_ARSession.Reset();

			int resetCounter = 0;

			while (_ARTrackedImageManager.trackables.count > 0 && resetCounter < 120)
			{
				resetCounter++;

				yield return new WaitForEndOfFrame();
			}
		}

		_ARTrackedImageManager.enabled = false;

		_Markers.Clear();

		_ARState = ARCustomStates.Waiting;
	}
}

public class TestScript : MonoBehaviour
{
	public ARCustom _ARCustom;

	private void Awake()
	{
		_ARCustom.Init();
	}

	public void ButtonMarkerDown()
	{
		_ARCustom.StartTracking();
	}

	public void ButtonMarkerUp()
	{
		_ARCustom.StopTracking();
	}

	public void ARReset()
	{
		StartCoroutine(_ARCustom.ResetSession());
	}
}