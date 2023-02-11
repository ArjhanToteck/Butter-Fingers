using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class GameData
{
    public static bool dataLoaded = false;
    public static float musicVolume = 1f;
    public static float soundEffectsVolume = 1f;
    public static float timePlayed = 0f;

    public static bool[] levelsAttempted = { false, false, false, false };
    public static bool[] levelsCompleted = { false, false, false, false };

    public static Achievement[] achievements = {
        new Achievement("Achiever", "Open the achievements page"),
        new Achievement("Bruh Moment", "Die within 3 seconds of starting a level"),
        new Achievement("Open The Game", "Literally open the game"),
        new Achievement("Sad Moment", "Die for the first time"),
        new Achievement("Very Sad Moment", "Die for the 100th time", progressInput: 0, progressMaxInput: 100),
        new Achievement("You Tried", "Try each level at least once", progressInput: 0, progressMaxInput: 4),
        new Achievement("Epic Gamer", "Beat every level", progressInput: 0, progressMaxInput: 4),
        new Achievement("Overachiever", "Unlock all other achievements"),
        new Achievement("Commitment", "Play the game for 5 hours"),
        new Achievement("Some Good Toast", "Spread butter on 100 pieces of bread", true, progressInput: 0, progressMaxInput: 100),
        new Achievement("Sleep Deprivation", "Open the game before 5 AM", true),
        new Achievement("Your Life is a Lie", "Get tricked by the fake key in level 3", true)
    };

    [System.NonSerialized]
    public static MusicData musicData = new MusicData();

    [System.NonSerialized]
    public static List<Achievement> achievementsQueue = new List<Achievement>();

    public static int GetAchievement(string name)
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            if(achievements[i].name == name)
            {
                return i;
            }
        }

        return -1;
    }

    public static void LoadGameData()
    {
        // creates a binary formatter
        BinaryFormatter formatter = new BinaryFormatter();

        // gets path of save data
        string path = Application.persistentDataPath + "/SaveData";

        // check if file exists in path
        if (File.Exists(path))
        {
            // creates a file stream to read file
            FileStream stream = new FileStream(path, FileMode.Open);

            // reads file and closes
            GameDataSerializable data = formatter.Deserialize(stream) as GameDataSerializable;
            stream.Close();

            // copies save file data to local class
            dataLoaded = data.dataLoaded;
            musicVolume = data.musicVolume;
            soundEffectsVolume = data.soundEffectsVolume;
            levelsAttempted = data.levelsAttempted;
            levelsCompleted = data.levelsCompleted;
            achievements = data.achievements;
        }
        else
        {
            // saves file since it doesn't yet exist
            SaveGameData();
        }
    }

    public static void SaveGameData()
    {
        // creates a binary formatter
        BinaryFormatter formatter = new BinaryFormatter();

        // gets path of save data
        string path = Application.persistentDataPath + "/SaveData";

        // opens file stream to create save file
        FileStream stream = new FileStream(path, FileMode.Create);

        // saves file and closes
        formatter.Serialize(stream, new GameDataSerializable());
        stream.Close();
    }
}

[System.Serializable]
class GameDataSerializable
{
    public bool dataLoaded = true;
    public float musicVolume = GameData.musicVolume;
    public float soundEffectsVolume = GameData.soundEffectsVolume;

    public bool[] levelsAttempted = GameData.levelsAttempted;
    public bool[] levelsCompleted = GameData.levelsCompleted;

    public Achievement[] achievements = GameData.achievements;
}
