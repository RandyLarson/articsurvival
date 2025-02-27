using UnityEngine;

public class CameraHelper : MonoBehaviour
{
    public Vector3 StartingPosition;

    void Start()
    {
    }

    internal void ResetCameraToStartingPosition()
    {
        if (TryGetComponent(out Camera ourCam))
        {
            ourCam.transform.position = StartingPosition;
        }
    }

}
