
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.SceneManagement;


public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoom photonRoom;
    PhotonView photonView;

    public bool isGameLoaded;
    public int currentScene;
    public int lobbyScene = 1;
    public int gameScene;

    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;
    int maxPlayersInRoom = 2;
    public int playersInGame;

    private void Awake()
    {
        if (photonRoom == null)
        {
            photonRoom = this;
        }
        else if (photonRoom != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == lobbyScene || currentScene == gameScene)
        {
            CreatePlayer();
        }

    }

    [PunRPC]
    void RPC_TellMasterToStartGame()
    {
        gameScene = Random.Range(2, 3);
        PhotonNetwork.LoadLevel(gameScene);
    }


    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(("PhotonPrefabs/PhotonNetworkPlayer") ,transform.position,Quaternion.identity, 0);
        PhotonNetwork.Instantiate(("PhotonPrefabs/Skill Selection Holder"), transform.position, Quaternion.identity, 0);

    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("Joined Room");
        if (PhotonNetwork.PlayerList.Length >= maxPlayersInRoom)
        {
            CreateRoom();
        }
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();



        StartGame();
    }


    void StartGame()
    {
        if (playersInRoom == maxPlayersInRoom)
        {
            photonView.RPC("RPC_TellMasterToStartGame", RpcTarget.MasterClient);
            return;
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        PhotonNetwork.LoadLevel(lobbyScene);


    }

    void CreateRoom()
    {
        print("Trying to create a new room");
        int randomRoomName = UnityEngine.Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2};
        PhotonNetwork.CreateRoom("Room " + randomRoomName, roomOptions);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UIManager.instance.PlayerForfeited(int.Parse(PhotonNetwork.NickName));
        print(otherPlayer.NickName + " has left the game");
        playersInRoom--;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameSetup.instance.DisconnectPlayer();
        }
    }

    public void EndMatch ()
    {
        photonView.RPC("RPC_EndMatch",RpcTarget.All);
    }

    [PunRPC]
    void RPC_EndMatch()
    {
        GameSetup.instance.DisconnectPlayer();
    }

}
