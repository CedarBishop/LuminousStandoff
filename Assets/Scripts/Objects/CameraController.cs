using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
	public Vector3 offsetFromPlayer;
	private AvatarSetup avatar;
	private AvatarSetup[] avatarSetups;

	private void Start()
	{
		avatarSetups = FindObjectsOfType<AvatarSetup>();
		if (avatarSetups != null)
		{
			for (int i = 0; i < avatarSetups.Length; i++)
			{
				if (avatarSetups[i].GetComponent<PhotonView>().IsMine)
				{
					avatar = avatarSetups[i];
				}
			}
		}
	}

	void Update()
	{
		if (avatar != null)
		{
			transform.position = new Vector3(
				avatar.transform.position.x + offsetFromPlayer.x,
				offsetFromPlayer.y,
				avatar.transform.position.z + offsetFromPlayer.z
			);
		}
	}
}
