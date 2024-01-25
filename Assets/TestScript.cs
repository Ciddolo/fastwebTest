using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

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

	public ARCustomDebug _Debug;

	public ARTrackedImagesChangedEventArgs _Info;

	public void Init()
	{
		SetState(ARCustomStates.Starting);

		_ARTrackedImageManager.trackedImagesChanged += (info) => { PrefabManager(info); };

		_Debug.Init(this);

		SetState(ARCustomStates.Waiting);
	}

	private void PrefabManager(ARTrackedImagesChangedEventArgs info)
	{
		_Info = info;
	}

	public void SessionSwitch()
	{
		_ARSession.enabled = !_ARSession.enabled;
	}

	public void StartTracking()
	{
		if (_ARState != ARCustomStates.Waiting) return;

		if (_ARTrackedImageManager.trackables.count > 0) return;

		_ARTrackedImageManager.enabled = true;

		SetState(ARCustomStates.Tracking);
	}

	public void StopTracking()
	{
		if (_ARState != ARCustomStates.Tracking) return;

		_ARTrackedImageManager.enabled = false;

		SetState(ARCustomStates.Waiting);
	}

	public IEnumerator ResetSession()
	{
		if (_ARState == ARCustomStates.Resetting) yield break;

		if (_ARTrackedImageManager.trackables.count <= 0) yield break;

		SetState(ARCustomStates.Resetting);

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

		_Debug._Markers.Clear();

		SetState(ARCustomStates.Waiting);
	}

	public void SetState(ARCustomStates newState)
	{
		_ARState = newState;
	}
}

[Serializable]
public class ARCustomDebug
{
	[Header("References")]
	public TextMeshProUGUI _DebugText;

	[Header("Data")]
	public List<GameObject> _Markers;

	public bool _PrintARSession;
	public bool _PrintARState;
	public bool _PrintLibrary;
	public bool _PrintListAdded;
	public bool _PrintListUpdated;
	public bool _PrintListRemoved;
	public bool _PrintTrackablesCount;

	private ARCustom arCustom;

	public void Init(ARCustom currentCustom)
	{
		_Markers = new List<GameObject>();

		arCustom = currentCustom;
	}

	public void DebugData()
	{
		_DebugText.text = "";

		if (_PrintARSession)
		{
			_DebugText.text += string.Format("AR Session: {0}\n\n", arCustom._ARSession.enabled);
		}

		if (_PrintARState)
		{
			_DebugText.text += string.Format("AR State: {0}\n\n", arCustom._ARState);
		}

		if (_PrintLibrary)
		{
			_DebugText.text += string.Format("Tracked Manager: {0} {1}\n\n",arCustom._ARTrackedImageManager.enabled,  arCustom._ARTrackedImageManager.referenceLibrary);
		}

		if (arCustom._Info != null)
		{
			if (_PrintListAdded && arCustom._Info.added != null)
			{
				for (int i = 0; i < arCustom._Info.added.Count; i++)
				{
					_DebugText.text += string.Format("ADDED {0}\n\n", arCustom._Info.added[i].referenceImage.name, arCustom._Info.added[i].referenceImage.ToString());
				}
			}

			if (_PrintListUpdated && arCustom._Info.updated != null)
			{
				for (int i = 0; i < arCustom._Info.updated.Count; i++)
				{
					_DebugText.text += string.Format("UPDATED {0}\n\n", arCustom._Info.updated[i].referenceImage.name, arCustom._Info.updated[i].referenceImage.ToString());
				}
			}

			if (_PrintListRemoved && arCustom._Info.removed != null)
			{
				for (int i = 0; i < arCustom._Info.removed.Count; i++)
				{
					_DebugText.text += string.Format("REMOVED {0}\n\n", arCustom._Info.removed[i].referenceImage.name, arCustom._Info.removed[i].referenceImage.ToString());
				}
			}
		}

		if (_PrintTrackablesCount)
		{
			_DebugText.text += string.Format("Trackables Count: {0}\n\n", arCustom._ARTrackedImageManager.trackables.count.ToString());
		}

		_DebugText.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, _DebugText.preferredHeight + 50.0f);
	}
}

public class TestScript : MonoBehaviour
{
	public ARCustom _ARCustom;

	private void Awake()
	{
		_ARCustom.Init();
	}

	private void Update()
	{
		_ARCustom._Debug.DebugData();
	}

	public void ButtonARSessionSwitch()
	{
		_ARCustom.SessionSwitch();
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