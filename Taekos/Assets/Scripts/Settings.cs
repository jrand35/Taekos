using UnityEngine;
using System.Collections;

//Need to use static?
public class Settings : MonoBehaviour {
    private static int numberOfLives;
    private static int numberOfContinues;

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

    public static int NumberOfContinues
    {
        get
        {
            return numberOfContinues;
        }
        set
        {
            numberOfContinues = value;
        }
    }
}
