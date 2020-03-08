using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainParent;
    public GameObject settingsParent;
    public GameObject shopParent;

    public Text passionCountText;
    public Text goldCountText;

    private Currency currency;

    public Slider musicSlider;
    public Slider sfxSlider;


    private int characterNumber;

    public GameObject[] currentCharacterDisplayObjects;
    void Start()
    {
        SetMenuType(1);
        currency = GetComponent<Currency>();
        InitText();
        characterNumber = 0;
        ActivateCharacterDisplay();

    }

    public void Quit  ()
    {
        Application.Quit();
    }

    public void SetMenuType(int menuType)
    {
        switch (menuType)
        {
            case 1:
                mainParent.SetActive(true);
                settingsParent.SetActive(false);
                shopParent.SetActive(false);
                ActivateCharacterDisplay();
                break;
            case 2:
                mainParent.SetActive(false);
                settingsParent.SetActive(true);
                shopParent.SetActive(false);
                DeactivateCharacterDisplay();
                break;
            case 3:
                mainParent.SetActive(false);
                settingsParent.SetActive(false);
                shopParent.SetActive(true);
                DeactivateCharacterDisplay();
                break;
            default:
                break;
        }
    }

    public void UpdateCurrencyUI ()
    {
        passionCountText.text = currency.GetPassion().ToString();
        goldCountText.text = currency.GetGold().ToString();
    }

    public void SetMusicVolume()
    {
        SoundManager.instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSFXVolume()
    {
        SoundManager.instance.SetSFXVolume(sfxSlider.value);
    }

    private void Update()
    {
        TestCurrency();
    }

    public void TestCurrency()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            currency.EarnPassion(1);
            
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            currency.EarnGold(1);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {

        }

    }

    void InitText()
    {
        if (PlayerPrefs.HasKey("Passion"))
        {
            passionCountText.text = PlayerPrefs.GetInt("Passion", 0).ToString();

        }
        if (PlayerPrefs.HasKey("Gold"))
        {
            goldCountText.text = PlayerPrefs.GetInt("Gold",0).ToString();
        }
    }

    public void NextCharacter ()
    {
        characterNumber++;
        if (characterNumber > currentCharacterDisplayObjects.Length - 1)
        {
            characterNumber = 0;
        }
        ActivateCharacterDisplay();
        if (PlayerInfo.playerInfo != null)
        {
            PlayerInfo.playerInfo.selectedCharacter = characterNumber;
            PlayerPrefs.SetInt(PlayerInfo.playerInfo.selectedCharacterKey, characterNumber);

        }
    }


    void ActivateCharacterDisplay ()
    {
        if (currentCharacterDisplayObjects == null)
        {
            return;
        }
        for (int i = 0; i < currentCharacterDisplayObjects.Length; i++)
        {
            currentCharacterDisplayObjects[i].SetActive(false);
            if (i == characterNumber)
            {
                currentCharacterDisplayObjects[i].SetActive(true);
            }
        }
        
    }

    void DeactivateCharacterDisplay ()
    {
        if (currentCharacterDisplayObjects == null)
        {
            return;
        }
        for (int i = 0; i < currentCharacterDisplayObjects.Length; i++)
        {
            currentCharacterDisplayObjects[i].SetActive(false);
        }

    }

}
