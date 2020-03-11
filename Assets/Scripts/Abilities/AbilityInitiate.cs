using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityInitiate : MonoBehaviour
{
    public delegate void ButtonClick();
    public static ButtonClick OnAbilityClick;

    private Button abilityButton;

    private void OnEnable()
    {
        abilityButton = gameObject.GetComponent<Button>();
        abilityButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        OnAbilityClick();
    }
}
