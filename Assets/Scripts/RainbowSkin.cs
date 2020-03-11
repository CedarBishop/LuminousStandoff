using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowSkin : MonoBehaviour
{
    private Color currentColor;
    private Color targetColor;
    Material material;
    public float colorLerpSpeed;
    float currentHue;
    void Start()
    {
        currentColor = Color.white;
        material = GetComponent<Renderer>().material;
        targetColor = new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),1.0f);

    }

    void Update()
    {
        //if (currentColor == targetColor)
        //{
        //    targetColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        //}
        //else
        //{
        //    currentColor = Color.Lerp(currentColor,targetColor,colorLerpSpeed * Time.deltaTime);
        //}


        currentHue += colorLerpSpeed * Time.deltaTime;
        if (currentHue >= 1.0f)
        {
            currentHue = 0.01f; 
        }
        currentColor = Color.HSVToRGB(currentHue,1.0f,1.0f);
        material.SetColor("_EdgeColor", currentColor);
    }
}
