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

    public HealthPickup[] healthPickups;
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
        photonView = GetComponent<PhotonView>();
        if (healthPickups != null)
        {
            for (int i = 0; i < healthPickups.Length; i++)
            {
                healthPickups[i].gameObject.SetActive(true);
                healthPickups[i].pickupIndex = i;
            }
        }        
    }

    public void OnHealthPickup (int pickupIndex)
    {
        photonView.RPC("RPC_OnHealthPickup", RpcTarget.All,pickupIndex);
    }

    [PunRPC]
    void RPC_OnHealthPickup(int pickupIndex)
    {
        healthPickups[pickupIndex].gameObject.SetActive(false);
        StartCoroutine("DelaySpawnHealthPickup", pickupIndex);
    }

    IEnumerator DelaySpawnHealthPickup(int pickupIndex)
    {
        yield return new WaitForSeconds(healthPickupRespawnTime);
        healthPickups[pickupIndex].gameObject.SetActive(true);
    }
}
