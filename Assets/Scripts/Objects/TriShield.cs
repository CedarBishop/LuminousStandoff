using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TriShield : MonoBehaviour
{
    PhotonView photonView;
    public MiniShield[] miniShields;
    public float rotationSpeeds;
    public float shieldRespawnTime;
    
    private AbilitiesManager abilitiesManager;
    bool hasTriShieldAbiliity;

    private void OnEnable()
    {
        abilitiesManager = GetComponentInParent<AbilitiesManager>();

        hasTriShieldAbiliity = (abilitiesManager.passiveSkills == PassiveSkills.TriShield) ? true : false;
        if (miniShields != null)
        {
            for (int i = 0; i < miniShields.Length; i++)
            {
                miniShields[i].isMine = true;
            }
        }
        photonView.RPC("RPC_ActivateAll", RpcTarget.All,hasTriShieldAbiliity);
    }


    [PunRPC]
    void RPC_ActivateAll (bool active)
    {
        if (miniShields != null)
        {
            for (int i = 0; i < miniShields.Length; i++)
            {
                miniShields[i].triShield = this;
                miniShields[i].index = i;
                miniShields[i].gameObject.SetActive(active);
            }
        }                                                                                                                                                                                                    
    }

    [PunRPC]
    void RPC_ActivateOne (bool active, int index)
    {
        miniShields[index].gameObject.SetActive(active);
    }

    void FixedUpdate()
    {
         transform.Rotate(0, rotationSpeeds * Time.fixedDeltaTime, 0);
    }

    public void ResetMiniShield (int index)
    {
        photonView.RPC("RPC_ActivateOne",RpcTarget.All,false,index);

        StartCoroutine("CoResetMiniShield", index);
    }

    IEnumerator CoResetMiniShield (int index)
    {
        yield return new WaitForSeconds(shieldRespawnTime);
        photonView.RPC("RPC_ActivateOne", RpcTarget.All, true, index);
    }
}
