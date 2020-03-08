using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HealthPickup : MonoBehaviour
{
    public int replenishAmount; 
    [HideInInspector] public int pickupIndex;
    PlayerCombat player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject.GetComponentInParent<PlayerCombat>();
            
            player.ReplenishHealth(replenishAmount);
            LevelManager.instance.OnHealthPickup(pickupIndex);
                        
           
        }
    }
}
