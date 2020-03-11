using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    PhotonView photonView;

    [Header ("Round Logic")]

    public Transform[] spawnPoints;
    public int requiredRoundsToWinMatch = 3;
    public float roundTime = 60.0f;
    public float intermissionTime = 15.0f;
    public bool isInLobby;
    

    [Header ("Small Health Pickups")]

    public HealthPickup[] smallHealthPickups;
    public float smallHealthPickupRespawnTime;

    [Header("Large Health Pickups")]

    public HealthPickup[] largeHealthPickups;
    public float timeBeforeLargePickupsSpawn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Activate all small health pickups and set their index number so they can be respawned
        if (smallHealthPickups != null)
        {
            for (int i = 0; i < smallHealthPickups.Length; i++)
            {
                smallHealthPickups[i].gameObject.SetActive(true);
                smallHealthPickups[i].pickupIndex = i;
            }
        }

        // Deactivate all large pickups on start and start timer for when they should be activated
        if (largeHealthPickups != null)
        {
            for (int i = 0; i < largeHealthPickups.Length; i++)
            {
                largeHealthPickups[i].gameObject.SetActive(true);
            }
            StartCoroutine("SpawnLargeHealthPickups");
        }
    }

    // public function to call rpc function to tell all clients that the health pickup should be deactivated on pickup
    public void OnHealthPickup (int pickupIndex)
    {
        photonView.RPC("RPC_OnHealthPickup", RpcTarget.All,pickupIndex);
    }

    [PunRPC]
    void RPC_OnHealthPickup(int pickupIndex)
    {
        smallHealthPickups[pickupIndex].gameObject.SetActive(false);
        StartCoroutine("DelaySpawnHealthPickup", pickupIndex);
    }

    IEnumerator DelaySpawnHealthPickup(int pickupIndex)
    {
        yield return new WaitForSeconds(smallHealthPickupRespawnTime);
        smallHealthPickups[pickupIndex].gameObject.SetActive(true);
    }

    IEnumerator SpawnLargeHealthPickups ()
    {

        yield return new WaitForSeconds(timeBeforeLargePickupsSpawn);

        if (largeHealthPickups != null)
        {
            for (int i = 0; i < largeHealthPickups.Length; i++)
            {
                largeHealthPickups[i].gameObject.SetActive(true);
            }
        }
    }
}
