using UnityEngine;

public class CameraDepthTextureMode : MonoBehaviour
{
    [SerializeField] private DepthTextureMode depthTextureMode;

    private void Start()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        Camera.main.depthTextureMode = depthTextureMode;
    }
}
