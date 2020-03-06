using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{

    public Sprite[] sprites;
    public Sprite sprite;
    int skillNumber;
    bool isPassive;
    public Text text;

   
    public void InitialiseButton (bool IsPassive, int SkillNumber)
    {
        isPassive = IsPassive;
        skillNumber = SkillNumber;
        if (isPassive)
        {
            PassiveSkills[] passive = SkillSelectionHolder.instance.GetPassiveSkills();
            text.text = passive[skillNumber].ToString();
        }
        else
        {
            ActiveSkills[] action = SkillSelectionHolder.instance.GetActiveSkills();
            text.text = action[skillNumber].ToString();
        }
    }

    void Start()
    {
        UIManager.DestroySkillButtons += () => Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UIManager.DestroySkillButtons -= () => Destroy(gameObject);

    }

    public void ChooseSkill ()
    {
        UIManager.instance.SkillSelectButton(isPassive,skillNumber);
    }

    
}
