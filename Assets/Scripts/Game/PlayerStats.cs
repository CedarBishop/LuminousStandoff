using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //public Text healthText;
    public Text playerNumberText;
    public Image roundWinImage;
    //public int health;
    public int playerNumber;
    public int roundWins;


    public void SetHealth(int newHealth)
    {
        //  health = newHealth;
       // healthText.text = health.ToString() + "%";
    }

    public void SetPlayerNumber (int num)
    {         
        playerNumber = num;
       // playerNumberText.text = "P" + playerNumber.ToString();
      //  playerNumberText.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f), 1.0f);
        roundWinImage.fillAmount = 0.0f;

    }

    public bool IncrementRoundWins()
    {
        roundWins++;
        if (roundWins >= LevelManager.instance.requiredRoundsToWinMatch)
        {
            // player won yay
            roundWinImage.fillAmount = 1;
            return true;
        }


        roundWinImage.fillAmount += 0.34f;
        return false;
        
    }

    
}
