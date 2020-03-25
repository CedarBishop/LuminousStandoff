using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriShield : MonoBehaviour
{
    public MiniShield[] miniShields;
    public float rotationSpeeds;
    public float shieldRespawnTime;
    
    private AbilitiesManager abilitiesManager;
    public bool hasTriShieldAbiliity;

    private void OnEnable()
    {
        abilitiesManager = GetComponentInParent<AbilitiesManager>();

        if (miniShields != null)
        {
            for (int i = 0; i < miniShields.Length; i++)
            {
                miniShields[i].triShield = this;
                miniShields[i].index = i;
                miniShields[i].gameObject.SetActive(true);
            }
        }
       
        hasTriShieldAbiliity = (abilitiesManager.passiveSkills == PassiveSkills.TriShield) ? true : false;
        
        if (hasTriShieldAbiliity == false)
        {
            if (miniShields != null)
            {
                for (int i = 0; i < miniShields.Length; i++)
                {
                    miniShields[i].gameObject.SetActive(false);
                }
            }           
        }
    }

    void FixedUpdate()
    {
        if (hasTriShieldAbiliity)
        {
            transform.Rotate(0, rotationSpeeds * Time.fixedDeltaTime, 0);
        }
    }

    public void ResetMiniShield (int index)
    {
        miniShields[index].gameObject.SetActive(false);
        StartCoroutine("CoResetMiniShield", index);
    }

    IEnumerator CoResetMiniShield (int index)
    {
        yield return new WaitForSeconds(shieldRespawnTime);
        miniShields[index].gameObject.SetActive(true);
    }
}
