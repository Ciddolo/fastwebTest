using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TestScript : MonoBehaviour
{
	public GameObject _ArSessionPrefab;

	public GameObject _MarkerPrefab;

	public GameObject _ARSessionGO;

	public ARTrackedImageManager _ARTrackedImageManager;

	public XRReferenceImageLibrary _Library;

	public IReferenceImageLibrary ReferenceImageLibrary;

	public List<GameObject> _Markers;

	public ARTrackedImagesChangedEventArgs _Info;

	public int _Int = 0;

	public bool _IsResetting;
	public bool _IsCreatingARTrackingImageManager;

	private void Awake()
	{
		CreateARTrackedImageManager();

		ReferenceImageLibrary = _ARTrackedImageManager.referenceLibrary;

		_Markers = new List<GameObject>();
	}

	private void Update()
	{
		if (!_ARTrackedImageManager) return;
	}

	public void CreateARTrackedImageManager()
	{
		if (_ARTrackedImageManager) return;

		if (_IsCreatingARTrackingImageManager) return;

		StartCoroutine(CreatingARTrackedImageManager());
	}

	public IEnumerator CreatingARTrackedImageManager()
	{
		_IsCreatingARTrackingImageManager = true;

		_ARTrackedImageManager = gameObject.AddComponent<ARTrackedImageManager>();

		while (!_ARTrackedImageManager) yield return new WaitForEndOfFrame();

		_ARTrackedImageManager.enabled = false;
		_ARTrackedImageManager.referenceLibrary = _Library;
		_ARTrackedImageManager.trackedImagePrefab = _MarkerPrefab;
		_ARTrackedImageManager.requestedMaxNumberOfMovingImages = 1;

		_ARTrackedImageManager.trackedImagesChanged += (info) => { PrefabManager(info); };

		_IsCreatingARTrackingImageManager = false;
	}

	public void OPD()
	{
		if (!_ARTrackedImageManager) return;

		if (_IsCreatingARTrackingImageManager) return;

		if (_IsResetting) return;

		_ARTrackedImageManager.referenceLibrary = ReferenceImageLibrary;

		_ARTrackedImageManager.enabled = true;
	}

	public void OPU()
	{
		if (!_ARTrackedImageManager) return;

		if (_IsCreatingARTrackingImageManager) return;

		if (_IsResetting) return;

		_ARTrackedImageManager.enabled = false;
	}

	public void ARReset()
	{
		if (_IsResetting) return;

		StartCoroutine(ARResetting());
	}

	public IEnumerator ARResetting()
	{
		_IsResetting = true;

		for (int i = _Markers.Count - 1; i >= 0; i--)
		{
			DestroyImmediate(_Markers[i]);

			while (_Markers[i]) yield return new WaitForEndOfFrame();
		}

		_Markers.Clear();

		Destroy(_ARTrackedImageManager);

		while (_ARTrackedImageManager) yield return new WaitForEndOfFrame();

		DestroyImmediate(_ARSessionGO);

		while (_ARSessionGO) yield return new WaitForEndOfFrame();

		_ARSessionGO = Instantiate(_ArSessionPrefab);

		while (!_ARSessionGO) yield return new WaitForEndOfFrame();

		CreateARTrackedImageManager();

		while (!_ARTrackedImageManager) yield return new WaitForEndOfFrame();

		_IsResetting = false;
	}

	private void PrefabManager(ARTrackedImagesChangedEventArgs info)
	{
		_Info = info;

		for (int i = 0; i < _Info.added.Count; i++)
		{
			Debug.Log("ADDED " + _Info.added[i].referenceImage.name);
			Debug.Log(_Info.added[i].referenceImage.ToString());
		}

		for (int i = 0; i < _Info.updated.Count; i++)
		{
			Debug.Log("UPDATED " + _Info.updated[i].referenceImage.name);
		}

		for (int i = 0; i < _Info.removed.Count; i++)
		{
			Debug.Log("REMOVED " + _Info.removed[i].referenceImage.name);
		}
	}
}