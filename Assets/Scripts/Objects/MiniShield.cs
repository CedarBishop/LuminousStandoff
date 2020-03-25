using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniShield : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public TriShield triShield;
    [HideInInspector] public bool isMine;


    public bool BlockedProjectile()
    {
        if (isMine == false)
        {
            triShield.ResetMiniShield(index);

            return true;
        }
        return false;
    }
}
