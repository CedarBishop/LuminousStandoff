using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    PhotonView photonView;

    public Transform[] spawnPoints;
    public int requiredRoundsToWinMatch = 5;

    public Transform[] healthPickupLocations;
    public float healthPickupRespawnTime;

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
        if (PhotonNetwork.IsMasterClient)
        {
            if (healthPickupLocations != null)
            {
                for (int i = 0; i < healthPickupLocations.Length; i++)
                {
                    RPC_SpawnHealthPickup(i);
                }
            }          
            
        }
    }

    public void StartHealthRespawnTimer (int pickupIndex)
    {
        StartCoroutine(DelaySpawnHealthPickup(pickupIndex));
    }

    IEnumerator DelaySpawnHealthPickup(int pickupIndex)
    {
        yield return new WaitForSeconds(healthPickupRespawnTime);
        photonView.RPC("RPC_SpawnHealthPickup", RpcTarget.MasterClient, pickupIndex);
    }

    [PunRPC]
    void RPC_SpawnHealthPickup (int pickupIndex)
    {
        GameObject pickup = PhotonNetwork.Instantiate("PhotonPrefabs/Health Pickup", healthPickupLocations[pickupIndex].position, Quaternion.identity);
       // pickup.GetComponent<HealthPickup>().pickupIndex = pickupIndex;
    }
}
