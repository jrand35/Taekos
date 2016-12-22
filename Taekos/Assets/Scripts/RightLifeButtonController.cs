﻿using UnityEngine.UI;
using UnityEngine;
using System.Collections;

/// <summary>
/// The script attached to the button for increasing the number of lives in the options menu
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class RightLifeButtonController : MonoBehaviour
{

    private Button myButton;
    private Image image;
    private bool darkened;
    public delegate void RightLifeButtonEventHandler(int addScore, bool playSound);
    public static event RightLifeButtonEventHandler rightLifeButtonClicked;         ///< Event fired when the button is clicked
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
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && myButton.enabled)
        {
            rightLifeButtonClicked(1, true);
            EnableButton();
        }
    }

    /// <summary>
    /// Enable or disable the button
    /// </summary>
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
    }
}