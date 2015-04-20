using UnityEngine;
using System.Collections;

//Need to use static?
public class Settings : MonoBehaviour {
    private static int numberOfLives = 2;
    private static int numberOfContinues = 2;
    private static int musicVolume = 100;
    private static int soundVolume = 100;

    public class Results
    {
        private static long score;
        private static int feathers;
        private static int time;

        public static long Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public static int Feathers
        {
            get
            {
                return feathers;
            }
            set
            {
                feathers = value;
            }
        }

        public static int Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }
    }

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

    public static int MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;
        }
    }

    public static int SoundVolume
    {
        get
        {
            return soundVolume;
        }
        set
        {
            soundVolume = value;
        }
    }
}
