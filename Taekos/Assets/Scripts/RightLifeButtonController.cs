﻿using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class RightLifeButtonController : MonoBehaviour
{

    private Button myButton;
    private Image image;
    private bool darkened;
    public delegate void RightLifeButtonEventHandler(int addScore, bool playSound);
    public static event RightLifeButtonEventHandler rightLifeButtonClicked;
    // Use this for initialization
    void Start()
    {
        myButton = GetComponent<Button>();
        image = GetComponent<Image>();
        darkened = false;
        myButton.onClick.AddListener(() =>
        {
            rightLifeButtonClicked(1, true);
            EnableButton();
        });
    }

    // Update is called once per frame
    void Update()
    {
        EnableButton();
    }

    void EnableButton()
    {
        if ((enabled || !darkened) && Settings.NumberOfLives == 9)
        {
            myButton.enabled = false;
            darkened = true;
            image.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else if ((!enabled || darkened) && Settings.NumberOfLives != 9)
        {
            myButton.enabled = true;
            darkened = false;
            image.color = new Color(1f, 1f, 1f, 1f);
        }
       // Debug.Log("Enabled:" + myButton.enabled + " Darkened: " + darkened);
    }
}