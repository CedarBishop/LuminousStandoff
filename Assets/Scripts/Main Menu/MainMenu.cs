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

    void Start()
    {
        SetMenuType(1);
        currency = GetComponent<Currency>();
        InitText();
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
                break;
            case 2:
                mainParent.SetActive(false);
                settingsParent.SetActive(true);
                shopParent.SetActive(false);
                break;
            case 3:
                mainParent.SetActive(false);
                settingsParent.SetActive(false);
                shopParent.SetActive(true);
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
}
