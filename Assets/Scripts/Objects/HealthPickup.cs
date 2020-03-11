using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HealthPickup : MonoBehaviour
{
    [Range(0.0f,1.0f)]public float replenishAmountPercentage; 
    [HideInInspector] public int pickupIndex;
    PlayerCombat player;
    public bool respawns;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject.GetComponentInParent<PlayerCombat>();
            
            player.ReplenishHealth(replenishAmountPercentage);
            
            if (respawns)
            {
                LevelManager.instance.OnHealthPickup(pickupIndex);
            }                                 
        }
    }
}
