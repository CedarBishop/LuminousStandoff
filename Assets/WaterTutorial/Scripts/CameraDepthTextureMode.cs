using UnityEngine;

public class CameraDepthTextureMode : MonoBehaviour
{
	[SerializeField] private DepthTextureMode depthTextureMode;

	private void Start()
	{
		if (Camera.main != null)
		{
			SetCameraDepthTextureMode();
		}
	}

	private void SetCameraDepthTextureMode()
	{
		Camera.main.depthTextureMode = depthTextureMode;
	}
}
