using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
   public void OnClickCharacterPick(int character)
    {
        if (PlayerInfo.playerInfo != null)
        {
            PlayerInfo.playerInfo.selectedCharacter = character;
            PlayerPrefs.SetInt(PlayerInfo.playerInfo.selectedCharacterKey,character);
        }
    }
}
