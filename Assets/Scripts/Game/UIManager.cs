using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class UIManager : MonoBehaviour
{
	public static UIManager instance = null;

	public static System.Action roundDrawEvent;

	PhotonView photonView;

	public UIGroup uIGroupPrefab;
	public LayoutGroup layoutGroup;
	private Player[] players;
	private List<UIGroup> uIGroups = new List<UIGroup>();
	public Text winText;
	int roundNumber = 1;
	public FixedJoystick leftJoystick;
	public FixedJoystick rightJoystick;
	public Text roundTimerText;
	public float startingRoundTime;
	float roundTimer;
	bool roundIsUnderway;
	bool isRoundIntermission;

	public bool isInLobby;

	public GameObject skillSelectionParent;
	public GameObject passiveSkillLayout;
	public GameObject activeSkillLayout;
	public GameObject skillButtonPrefab;

	bool hasSelectedPassive;
	bool hasSelectedAction;

	public static System.Action DestroySkillButtons;

	// Make Script Singleton
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WEBGL
		leftJoystick.gameObject.SetActive(true);
        rightJoystick.gameObject.SetActive(true);
#elif UNITY_EDITOR || UNITY_STANDALONE
		leftJoystick.gameObject.SetActive(false);
		rightJoystick.gameObject.SetActive(false);
#endif
	}

	private IEnumerator Start()
	{
		skillSelectionParent.SetActive(false);
		winText.text = "";
		yield return new WaitForSeconds(0.24f);
		players = PhotonNetwork.PlayerList;
		roundNumber = 1;

		// Insatiate the UI Group for each player and initialize with room number
		for (int i = 0; i < players.Length; i++)
		{
			UIGroup uI = Instantiate(uIGroupPrefab, layoutGroup.transform);
			uIGroups.Add(uI);
			uI.SetPlayerNumber(i + 1);

			if (i == 0)
			{
				uI.transform.SetAsFirstSibling();
			}
		}

		photonView = GetComponent<PhotonView>();
		StartRoundTimer();
	}

	// Function that is called from player combat to
	public void HealthUpdate(int health, int playerNumber)
	{
		uIGroups[playerNumber - 1].SetHealth(health);
		if (health <= 0)
		{
			DisplayWinText(playerNumber);
		}
	}

	public void DisplayWinText(int playerNumber)
	{
		string displayText = "";
		if (playerNumber == 2)
		{
			displayText = "Player One Wins The Round ";
			if (uIGroups[0].IncrementRoundWins())
			{
				displayText = "Player One Wins";
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (num == 1)
					{
						EarnPassion(true);
					}
					else if (num == 2)
					{
						EarnPassion(false);
					}
				}
				StartCoroutine("CoEndMatch");
			}
			else
			{
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (num == 2)
					{
						//SpawnSkillSelectionButtons();
					}
				}

				Intermission();
			}
		}
		else
		{
			displayText = "Player Two Wins The Round ";
			if (uIGroups[1].IncrementRoundWins())
			{
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (num == 1)
					{
						EarnPassion(false);
					}
					else if (num == 2)
					{
						EarnPassion(true);
					}
				}
				displayText = "Player Two Wins";
				StartCoroutine("CoEndMatch");
			}
			else
			{
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (num == 1)
					{
						//SpawnSkillSelectionButtons();
					}
				}

				Intermission();
			}
		}

		print(displayText);

		winText.text = displayText;
	}

	public void ClearWinText()
	{
		roundNumber++;
		winText.text = "";
	}

	IEnumerator CoEndMatch()
	{
		yield return new WaitForSeconds(3);
		PhotonRoom.photonRoom.EndMatch();
	}

	public void PlayerForfeited(int playerNumber)
	{
		string displayText = "";
		if (playerNumber == 1)
		{
			displayText = "Player One Forfeited \nPlayer Two Wins";
		}
		else if (playerNumber == 2)
		{
			displayText = "Player Two Forfeited \nPlayer One Wins";
		}
		EarnPassion(true);
		StartCoroutine("CoEndMatch");
		winText.text = displayText;
	}

	private void FixedUpdate()
	{
		if (isInLobby)
		{
			roundTimerText.text = "";
			return;
		}
		if (roundIsUnderway)
		{
			if (roundTimer <= 0)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					roundIsUnderway = false;
					isRoundIntermission = false;
					photonView.RPC("RPC_RoundDraw", RpcTarget.All);
					print("Stopped timer");
				}
			}
			else
			{
				roundTimer -= Time.fixedDeltaTime;
				roundTimerText.text = roundTimer.ToString("F1");
			}
		}
		else if (isRoundIntermission)
		{
			if (roundTimer <= 0)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					isRoundIntermission = false;
					RPC_StartNewRound();
					photonView.RPC("RPC_StartNewRound", RpcTarget.Others);
				}
			}
			else
			{
				roundTimer -= Time.fixedDeltaTime;
				roundTimerText.text = roundTimer.ToString("F1");
			}
		}
	}

	public void StartRoundTimer()
	{
		roundTimer = startingRoundTime;
		roundTimerText.text = roundTimer.ToString("F1");
		roundIsUnderway = true;
		isRoundIntermission = false;
	}

	[PunRPC]
	void RPC_StartNewRound()
	{
		StartRoundTimer();
		ClearWinText();


		if (DestroySkillButtons != null)
		{
			DestroySkillButtons();
		}

		
		AvatarSetup[] avatarSetups = FindObjectsOfType<AvatarSetup>();
		for (int i = 0; i < avatarSetups.Length; i++)
		{
			if (avatarSetups[i].GetComponent<PhotonView>().IsMine)
			{
				avatarSetups[i].StartNewRound();
			}
		}
	}

	[PunRPC]
	void RPC_RoundDraw()
	{
		print("RPC_RoundDraw");
		if (uIGroups.Count >= 2)
		{
			// Increment both players and check if have caused a tie break
			uIGroups[0].IncrementRoundWins();
			uIGroups[1].IncrementRoundWins();

			if (uIGroups[0].roundWins >= LevelManager.instance.requiredRoundsToWinMatch &&
			    uIGroups[1].roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
			{
				// tie break, go to sudden death
				roundTimerText.text = "";
			}

			else if (uIGroups[0].roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
			{
				// Player 1 wins match
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (num == 1)
					{
						EarnPassion(true);
					}
					else if (num == 2)
					{
						EarnPassion(false);
					}
				}
				winText.text = "Player One Wins";
				StartCoroutine("CoEndMatch");
			}
			else if (uIGroups[1].roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
			{
				// Player Two wins match
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (num == 1)
					{
						EarnPassion(false);
					}
					else if (num == 2)
					{
						EarnPassion(true);
					}
				}
				winText.text = "Player Two Wins";
				StartCoroutine("CoEndMatch");
			}
			else
			{
				// go to next round

				Intermission();
			}
		}
		else
		{
			Intermission();
		}
	}

	void Intermission()
	{

		roundTimer = 3;
		isRoundIntermission = true;
		roundIsUnderway = false;
		roundTimerText.text = "";
		roundTimerText.text = "Intermission";

		AvatarSetup[] avatarSetups = FindObjectsOfType<AvatarSetup>();
		if (avatarSetups == null)
		{
			return;
		}

		if (avatarSetups.Length == 1)
		{
			avatarSetups[0].DisableControls();
		}
		else
		{
			for (int i = 0; i < avatarSetups.Length; i++)
			{
				if (avatarSetups[i].GetComponent<PhotonView>().IsMine)
				{
					avatarSetups[i].DisableControls();
				}
			}
		}
	}


	public void SkillSelectButton(bool isPassive, int skillNumber)
	{
		//assign skill to player
		if (isPassive)
		{
			SkillSelectionHolder.instance.RemovePassiveSkill(skillNumber);
			hasSelectedPassive = true;
			activeSkillLayout.SetActive(true);
		}
		else
		{
			SkillSelectionHolder.instance.RemoveActiveSkill(skillNumber);
			hasSelectedAction = true;
			activeSkillLayout.SetActive(false);
		}
		if (DestroySkillButtons != null)
		{
			DestroySkillButtons();
		}

		skillSelectionParent.SetActive(false);
	}


	void SpawnSkillSelectionButtons()
	{
		skillSelectionParent.SetActive(true);
		print("Reached skill selection button");
		if (hasSelectedPassive == false)
		{
			print("Reached spawn passive");
			PassiveSkills[] passiveSkills = SkillSelectionHolder.instance.GetPassiveSkills();
			for (int i = 0; i < passiveSkills.Length; i++)
			{
				Button button = Instantiate(skillButtonPrefab, passiveSkillLayout.transform).GetComponent<Button>();
				button.GetComponent<SkillButton>().InitialiseButton(true, i);
			}
		}

		if (hasSelectedAction == false)
		{
			ActiveSkills[] activeSkills = SkillSelectionHolder.instance.GetActiveSkills();

			for (int i = 0; i < activeSkills.Length; i++)
			{
				Button button = Instantiate(skillButtonPrefab, activeSkillLayout.transform).GetComponent<Button>();
				button.GetComponent<SkillButton>().InitialiseButton(false,i);
			}
		}
	}

	void EarnPassion (bool wonMatch)
	{
		if (wonMatch)
		{
			PlayerInfo.playerInfo.passionEarnedThisMatch = 10;
		}
		else
		{
			PlayerInfo.playerInfo.passionEarnedThisMatch = 5;
		}
	}
}
