using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TestScript : MonoBehaviour
{
    public ARSession _ARSession;

    public ARTrackedImageManager _ARTrackedImageManager;

    public IReferenceImageLibrary referenceImageLibrary;

    private void Awake()
    {
        referenceImageLibrary = _ARTrackedImageManager.referenceLibrary;
    }

    public void OPD()
    {
        _ARTrackedImageManager.enabled = true;
        _ARTrackedImageManager.referenceLibrary = referenceImageLibrary;
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