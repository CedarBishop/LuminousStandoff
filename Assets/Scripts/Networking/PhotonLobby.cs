using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby photonLobby;
    public GameObject battleButton;
    public GameObject cancelButton;
    public PhotonRoom room;

    private void Awake()
    {
        if (photonLobby == null)
        {
            photonLobby = this;
        }
        else if (photonLobby != this)
        {
            Destroy(gameObject);
        }
        Instantiate(room);
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            battleButton.SetActive(true);
            cancelButton.SetActive(false);
        }
        else
        {
            battleButton.SetActive(false);
            cancelButton.SetActive(false);
            PhotonNetwork.ConnectUsingSettings();
        }
       
    }

    public override void OnConnectedToMaster ()
    {
        print("Player has connected to master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.SetActive(true);
    }

    public void OnBattleButtonClicked ()
    {
        PhotonNetwork.JoinRandomRoom();
        battleButton.SetActive(false);
        cancelButton.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("failed to join random room");
        CreateRoom();
    }

    void CreateRoom()
    {
        print("Trying to create a new room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room " + randomRoomName, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Failed to create a new room");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }   
}
