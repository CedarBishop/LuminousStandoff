using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniShield : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public TriShield triShield;

    public void BlockedProjectile()
    {
        triShield.ResetMiniShield(index);
    }
}
