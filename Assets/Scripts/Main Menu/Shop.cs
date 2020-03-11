using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Shop : MonoBehaviour
{
    Currency currency;

    public ShopItem[] shopItems;



    void Start()
    {
        currency = GetComponent<Currency>();
        InitShop();

    }


    public void BuyItem(int itemNumber)
    {
        if (shopItems[itemNumber].requiresPassion)
        {
            //price is in passion
            if (currency.SpendPassion(shopItems[itemNumber].itemPrice))
            {
                PlayerPrefs.SetInt(shopItems[itemNumber].itemKey, 1);
                shopItems[itemNumber].itemButton.interactable = false;
            }
        }
        else
        {
            // needs gold
            if (currency.SpendGold(shopItems[itemNumber].itemPrice))
            {
                PlayerPrefs.SetInt(shopItems[itemNumber].itemKey, 1);
                shopItems[itemNumber].itemButton.interactable = false;

            }
        }
    }

    public void InitShop ()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (PlayerPrefs.HasKey(shopItems[i].itemKey))
            {
                shopItems[i].itemButton.interactable = (PlayerPrefs.GetInt(shopItems[i].itemKey) == 1) ? false : true;
            }
            string str = (shopItems[i].requiresPassion) ? " Passion" : " Gold";
            shopItems[i].itemText.text = shopItems[i].itemKey + "\n" + shopItems[i].itemPrice.ToString() + str;
        }
    }
}

[System.Serializable]
public struct ShopItem
{
    public Button itemButton;
    public string itemKey;
    public int itemPrice;
    public bool requiresPassion;
    public Text itemText;
}

