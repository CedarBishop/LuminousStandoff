using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SkillSelectionHolder : MonoBehaviour
{
    public static SkillSelectionHolder instance = null;

    private PhotonView photonView;

    private List<PassiveSkills> allPassiveSkills = new List<PassiveSkills>() { PassiveSkills.BouncyBullet, PassiveSkills.HelperBullet, PassiveSkills.SlowdownBullet, PassiveSkills.SpeedUp, PassiveSkills.TriShield};
    private List<ActiveSkills> allActiveAbilities = new List<ActiveSkills>() {ActiveSkills.DropMine,ActiveSkills.Rewind,ActiveSkills.Shotgun,ActiveSkills.Stealth,ActiveSkills.TempShield };


    private List<PassiveSkills> thisMatchPassiveSkills = new List<PassiveSkills>();
    private List<ActiveSkills> thisMatchActiveSkills = new List<ActiveSkills>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 3; i++)
            {
                int randNum = Random.Range(0, allPassiveSkills.Count);
                thisMatchPassiveSkills.Add(allPassiveSkills[randNum]);
                allPassiveSkills.RemoveAt(randNum);
            }
            for (int i = 0; i < 3; i++)
            {
                int randNum = Random.Range(0, allActiveAbilities.Count);
                thisMatchActiveSkills.Add(allActiveAbilities[randNum]);
                allActiveAbilities.RemoveAt(randNum);
            }
            PrintRemainingSkills();

            //photonView.RPC("RPC_InitRandomSkills", RpcTarget.OthersBuffered, thisMatchPassiveSkills,thisMatchActiveSkills);
        }
       
    }

    //void RPC_InitRandomSkills(List<PassiveSkills> passiveSkills, List<ActiveSkills> activeSkills)
    //{
    //    thisMatchPassiveSkills = passiveSkills;
    //    thisMatchActiveSkills = activeSkills;
    //}

    public void RemovePassiveSkill (int index)
    {
        thisMatchPassiveSkills.RemoveAt(index);
        PrintRemainingSkills();
       // photonView.RPC("RPC_RemovePassiveSkills",RpcTarget.All, index);
    }

    //[PunRPC]
    //void RPC_RemovePassiveSkills(int index)
    //{
    //    thisMatchPassiveSkills.RemoveAt(index);
    //    PrintRemainingSkills();
    //}

    public void RemoveActiveSkill (int index)
    {
        thisMatchActiveSkills.RemoveAt(index);
        PrintRemainingSkills();
        //photonView.RPC("RPC_RemoveActiveSkills", RpcTarget.All, index);
    }

    //[PunRPC]
    //void RPC_RemoveActiveSkills(int index)
    //{
    //    thisMatchActiveSkills.RemoveAt(index);
    //    PrintRemainingSkills();
    //}

    public PassiveSkills[] GetPassiveSkills()
    {
        PassiveSkills[] skills = new PassiveSkills[thisMatchPassiveSkills.Count];
        for (int i = 0; i < thisMatchPassiveSkills.Count; i++)
        {
            skills[i] = thisMatchPassiveSkills[i];
        }
        return skills;
    }

    public ActiveSkills[] GetActiveSkills()
    {
        ActiveSkills[] skills = new ActiveSkills[thisMatchActiveSkills.Count];
        for (int i = 0; i < thisMatchActiveSkills.Count; i++)
        {
            skills[i] = thisMatchActiveSkills[i];
        }
        return skills;
    }

    void PrintRemainingSkills()
    {
        for (int i = 0; i < thisMatchPassiveSkills.Count; i++)
        {
            print(thisMatchPassiveSkills[i]);
        }
        for (int i = 0; i < thisMatchActiveSkills.Count; i++)
        {
            print(thisMatchActiveSkills[i]);
        }
    }


}
