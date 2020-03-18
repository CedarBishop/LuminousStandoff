using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{

    Image image;
    int skillNumber;
    bool isPassive;

   
    public void InitialiseButton (bool IsPassive, int SkillNumber)
    {
        isPassive = IsPassive;
        skillNumber = SkillNumber;
        if (isPassive)
        {
            PassiveSkills[] passive = SkillSelectionHolder.instance.GetPassiveSkills();
            int num = SkillSelectionHolder.instance.GetChosenPassiveSkillSprite(passive[skillNumber]);
            image = GetComponent<Image>();
            image.sprite = SkillSelectionHolder.instance.passiveSprites[num];

        }
        else
        {
            ActiveSkills[] active = SkillSelectionHolder.instance.GetActiveSkills();
            int num = SkillSelectionHolder.instance.GetChosenActiveSkillSprite(active[skillNumber]);
            image = GetComponent<Image>();
            image.sprite = SkillSelectionHolder.instance.activeSprites[num];
        }
    }

    void Start()
    {
        GameManager.DestroySkillButtons += DestroySelf;
    }

    private void OnDestroy()
    {
        GameManager.DestroySkillButtons -= DestroySelf;

    }

    public void ChooseSkill ()
    {
        GameManager.instance.SkillSelectButton(isPassive,skillNumber);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }    
}
