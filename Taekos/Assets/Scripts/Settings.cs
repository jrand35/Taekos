using UnityEngine;
using System.Collections;

//Need to use static?
public class Settings : MonoBehaviour {
    private static int numberOfLives;

    public static int NumberOfLives
    {
        get
        {
            return numberOfLives;
        }
        set
        {
            numberOfLives = value;
        }
    }
}
