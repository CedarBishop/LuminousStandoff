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

    public Sprite[] passiveSprites;
    public Sprite[] activeSprites;

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

            int[] passiveSkillNums = new int[thisMatchPassiveSkills.Count];
            int[] activeSkillNums = new int[thisMatchActiveSkills.Count];
            for (int i = 0; i < thisMatchPassiveSkills.Count; i++)
            {
                passiveSkillNums[i] = (int)thisMatchPassiveSkills[i];
            }
            for (int i = 0; i < thisMatchActiveSkills.Count; i++)
            {
                activeSkillNums[i] = (int)thisMatchActiveSkills[i];
            }

            photonView.RPC("RPC_InitRandomSkills", RpcTarget.OthersBuffered, passiveSkillNums,activeSkillNums);
        }
       
    }

    [PunRPC]
    void RPC_InitRandomSkills(int[] passiveSkillNums, int[] activeSkillNums)
    {
        for (int i = 0; i < passiveSkillNums.Length; i++)
        {
            thisMatchPassiveSkills.Add((PassiveSkills)passiveSkillNums[i]);
        }
        for (int i = 0; i < activeSkillNums.Length; i++)
        {
            thisMatchActiveSkills.Add((ActiveSkills)activeSkillNums[i]);

        }
       
        PrintRemainingSkills();
    }

    public void RemovePassiveSkill (int index)
    {
        //thisMatchPassiveSkills.RemoveAt(index);
        //PrintRemainingSkills();
        photonView.RPC("RPC_RemovePassiveSkills",RpcTarget.All, index);
    }

    [PunRPC]
    void RPC_RemovePassiveSkills(int index)
    {
        thisMatchPassiveSkills.RemoveAt(index);
        //PrintRemainingSkills();
    }

    public void RemoveActiveSkill (int index)
    {
        //thisMatchActiveSkills.RemoveAt(index);
        //PrintRemainingSkills();
        photonView.RPC("RPC_RemoveActiveSkills", RpcTarget.All, index);
    }

    [PunRPC]
    void RPC_RemoveActiveSkills(int index)
    {
        thisMatchActiveSkills.RemoveAt(index);
        //PrintRemainingSkills();
    }

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

    public int GetChosenPassiveSkillSprite (PassiveSkills passive)
    {
        switch (passive)
        {
            case PassiveSkills.None:
                return 0;
            case PassiveSkills.BouncyBullet:
                return 0;
            case PassiveSkills.HelperBullet:
                return 1;
            case PassiveSkills.SlowdownBullet:
                return 2;
            case PassiveSkills.SpeedUp:
                return 3;
            case PassiveSkills.TriShield:
                return 4;
            default:
                return 0;
        }
    }

    public int GetChosenActiveSkillSprite(ActiveSkills active)
    {
        switch (active)
        {
            case ActiveSkills.None:
                return 0;
            case ActiveSkills.DropMine:
                return 0;
            case ActiveSkills.Rewind:
                return 1;
            case ActiveSkills.Shotgun:
                return 2;
            case ActiveSkills.Stealth:
                return 3;
            case ActiveSkills.TempShield:
                return 4;
            default:
                return 0;

        }
    }


}
