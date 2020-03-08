using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerInfo : MonoBehaviourPunCallbacks
{
	public static PlayerInfo playerInfo = null;
	public int selectedCharacter;
	public GameObject[] allCharacters;

	[HideInInspector] public int passionEarnedThisMatch;

	[HideInInspector] public string selectedCharacterKey = "SelectedCharacter";

	private void Awake()
	{
		if (playerInfo == null)
		{
			playerInfo = this;
		}
		else if (playerInfo != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}


	void Start()
	{
		if (PlayerPrefs.HasKey(selectedCharacterKey))
		{
			selectedCharacter = PlayerPrefs.GetInt(selectedCharacterKey);
		}
		else
		{
			selectedCharacter = 0;
			PlayerPrefs.SetInt(selectedCharacterKey, selectedCharacter);
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		// Instantiate(Resources.Load("PhotonPrefabs/Room Controller"));
	}
}
