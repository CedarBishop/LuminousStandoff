using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;


public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	public static event Action roundDrawEvent;

	private PhotonView photonView;

	public PlayerStats statsPrefab;
	public LayoutGroup layoutGroup;
	private Player[] players;
	private List<PlayerStats> playerStats = new List<PlayerStats>();
	public Text winText;
	private int roundNumber = 1;
	public FixedJoystick leftJoystick;
	public FixedJoystick rightJoystick;
	public Button abilityButton;
	public Text roundTimerText;
	private float roundTimer;
	private bool roundIsUnderway;
	private bool isRoundIntermission;

	public GameObject skillSelectionParent;
	public GameObject passiveSkillLayout;
	public GameObject activeSkillLayout;
	public GameObject skillButtonPrefab;

	private bool hasSelectedPassive;
	private bool hasSelectedAction;

	public static event Action DestroySkillButtons;

	public static event Action OnActionButton;

	public static event Action<bool> OnDoubleDamage;

	[HideInInspector] public bool isDoubleDamage;

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
		abilityButton.gameObject.SetActive(false);
#elif UNITY_EDITOR || UNITY_STANDALONE
	
		 leftJoystick.gameObject.SetActive(false);
		 rightJoystick.gameObject.SetActive(false);
		 abilityButton.gameObject.SetActive(false);
#endif
	}

	private IEnumerator Start()
	{
		skillSelectionParent.SetActive(false);
		winText.text = "";
		yield return new WaitForSeconds(0.24f);
		players = PhotonNetwork.PlayerList;
		roundNumber = 1;

		// Instantiate the UI Group for each player and initialize with room number
		for (int i = 0; i < players.Length; i++)
		{
			PlayerStats stats = Instantiate(statsPrefab, layoutGroup.transform);
			playerStats.Add(stats);
			stats.SetPlayerNumber(i + 1);

			if (i == 0)
			{
				stats.transform.SetAsFirstSibling();
			}
		}

		photonView = GetComponent<PhotonView>();
		StartRoundTimer();
	}

	// Called when a player dies
	public void PlayerDied(int dyingPlayerNumber, int sendingPlayerNumber)
	{
		string displayText = "";
		//player two dies
		if (dyingPlayerNumber == 2)
		{
			//player one wins the match
			displayText = "Player One Wins The Round ";
			if (playerStats[0].IncrementRoundWins())
			{
				displayText = "Player One Wins";
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (dyingPlayerNumber != sendingPlayerNumber)
					{
						EarnPassion(true);
					}
					else
					{
						EarnPassion(false);
					}
				}
				StartCoroutine("CoEndMatch");
			}
			//player one wins the round but not the match
			else
			{
				if (dyingPlayerNumber == sendingPlayerNumber)
				{
					//SpawnSkillSelectionButtons();
				}

				//Start Intermission between rounds
				Intermission();
			}
		}
		//player one dies
		else
		{
			displayText = "Player Two Wins The Round ";
			//player two wins the match
			if (playerStats[1].IncrementRoundWins())
			{
				if (int.TryParse(PhotonNetwork.NickName, out int num))
				{
					if (dyingPlayerNumber != sendingPlayerNumber)
					{
						EarnPassion(true);
					}
					else
					{
						EarnPassion(false);
					}
				}
				displayText = "Player Two Wins";
				StartCoroutine("CoEndMatch");
			}
			//player two wins the round but not the match
			else
			{
				if (dyingPlayerNumber == sendingPlayerNumber)
				{
					//SpawnSkillSelectionButtons();
				}

				Intermission();
			}
		}

		print(displayText);
		//Display text of who won the round or match
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
		if (LevelManager.instance.isInLobby) // if in lobby dont use timer
		{
			roundTimerText.text = "";
			return;
		}
		if (roundIsUnderway)
		{
			if (roundTimer <= 20.0f)
			{
				if (isDoubleDamage == false)
				{
					isDoubleDamage = true;
					if (OnDoubleDamage != null)
					{
						OnDoubleDamage(true);
					}
				}
			}
			if (roundTimer <= 0)
			{
				roundIsUnderway = false;
				isRoundIntermission = false;
				RPC_RoundDraw();
				//if (PhotonNetwork.IsMasterClient)
				//{
				//	roundIsUnderway = false;
				//	isRoundIntermission = false;
				//	photonView.RPC("RPC_RoundDraw", RpcTarget.All);
				//	print("Stopped timer");
				//}
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
				isRoundIntermission = false;
				RPC_StartNewRound();

				//if (PhotonNetwork.IsMasterClient)
				//{
				//	isRoundIntermission = false;
				//	RPC_StartNewRound();
				//	photonView.RPC("RPC_StartNewRound", RpcTarget.Others);
				//}
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
		roundTimer = LevelManager.instance.roundTime;
		roundTimerText.text = roundTimer.ToString("F1");
		roundIsUnderway = true;
		isRoundIntermission = false;
	}

	[PunRPC]
	void RPC_StartNewRound()
	{
		StartRoundTimer();
		ClearWinText();

		if (OnDoubleDamage != null)
		{
			OnDoubleDamage(false);
		}

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
		if (playerStats.Count >= 2)
		{
			// Increment both players and check if have caused a tie break
			playerStats[0].IncrementRoundWins();
			playerStats[1].IncrementRoundWins();

			if (playerStats[0].roundWins >= LevelManager.instance.requiredRoundsToWinMatch &&
			    playerStats[1].roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
			{
				// tie break, go to sudden death
				roundTimerText.text = "";
			}

			else if (playerStats[0].roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
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
			else if (playerStats[1].roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
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

		roundTimer = LevelManager.instance.intermissionTime;
		isRoundIntermission = true;
		roundIsUnderway = false;
		roundTimerText.text = "";
		roundTimerText.text = "Intermission";

		AvatarSetup[] avatarSetups = FindObjectsOfType<AvatarSetup>();
		if (avatarSetups == null)
		{
			return;
		}

		for (int i = 0; i < avatarSetups.Length; i++)
		{
			if (avatarSetups[i].GetComponent<PhotonView>().IsMine)
			{
				avatarSetups[i].DisableControls();
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

	public void ActionButton ()
	{
		if (OnActionButton != null)
		{
			OnActionButton();
		}
	}
}
