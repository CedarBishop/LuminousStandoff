using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInitiate : MonoBehaviour
{
    public delegate void ButtonClick();
    public static ButtonClick OnAbilityClick;

    public void OnButtonClick()
    {
        OnAbilityClick();
    }
}
