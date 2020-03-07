using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HealthPickup : MonoBehaviour
{
    public int replenishAmount; 
    [HideInInspector] public int pickupIndex;


    private void OnTriggerEnter(Collider other)
    {
        print("Triggered something");
        if (other.TryGetComponent<PlayerCombat>(out PlayerCombat player ))
        {
            print("Triggered player");
            player.ReplenishHealth(replenishAmount);
            LevelManager.instance.StartHealthRespawnTimer(0);
            Destroy(gameObject);
        }
    }
}
