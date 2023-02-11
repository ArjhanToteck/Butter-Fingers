using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gamePausedPanel;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;

    public bool isSceneLevel = false;

    void Awake()
    {
        Debug.Log(GameData.levelsAttempted.All(value => value));

        // checks if game data was loaded or not
        if(!GameData.dataLoaded)
        {
            // loads game data
            GameData.LoadGameData();
        }

        // checks if scene is level
        isSceneLevel = SceneManager.GetActiveScene().name.StartsWith("Level");

        // resets gravity and time
        Physics2D.gravity = Vector2.down * 9.81f;
        Time.timeScale = 1;
    }

    void Start()
    {
        Achievement openTheGame = GameData.achievements[GameData.GetAchievement("Open The Game")];
        Achievement sleepDeprivation = GameData.achievements[GameData.GetAchievement("Sleep Deprivation")];
        Achievement achiever = GameData.achievements[GameData.GetAchievement("Achiever")];

        // open the game achievement

        // checks if the achievement was unlocked
        if (!openTheGame.unlocked)
        {
            // unlocks achievement
            FindObjectOfType<AchievementsManager>().EarnAchievement(ref openTheGame);
        }

        // achiever achievement

        if (SceneManager.GetActiveScene().name == "Achievements" && !achiever.unlocked)
        {
            // unlocks achievement
            FindObjectOfType<AchievementsManager>().EarnAchievement(ref achiever);
        }

        // sleep deprivation achievement

        // checks time is before 5 AM and if the achievement was unlocked
        if (DateTime.Now.Hour < 5 && DateTime.Now.ToString("tt") != "PM" && !sleepDeprivation.unlocked)
        {
            FindObjectOfType<AchievementsManager>().EarnAchievement(ref sleepDeprivation);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isSceneLevel)
        {
            Pause();
        }

        GameData.timePlayed += Time.unscaledDeltaTime;

        // commitment achievement
        Achievement commitment = GameData.achievements[GameData.GetAchievement("Commitment")];

        if (GameData.timePlayed / 1000 / 60 / 60 > 5 && !commitment.unlocked)
        {
            FindObjectOfType<AchievementsManager>().EarnAchievement(ref commitment);
        }
    }

    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
    }

    public void LevelFailed()
    {
        levelFailedPanel.SetActive(true);
    }    

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenAchievements()
    {
        SceneManager.LoadScene("Achievements");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OpenLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void OpenLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void OpenLevel4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Pause()
    {
        StartCoroutine(WaitAndPause());

        IEnumerator WaitAndPause()
        {
            // pauses game
            if (Time.timeScale != 0)
            {
                // sets timescale
                Time.timeScale = 0;

                // animates panel
                gamePausedPanel.SetActive(true);
                gamePausedPanel.GetComponent<Animator>().SetBool("FadeIn", true);
                gamePausedPanel.GetComponent<Animator>().SetBool("FadeOut", false);
            }
            // unpauses game
            else
            {
                // animates panel
                gamePausedPanel.GetComponent<Animator>().SetBool("FadeOut", true);
                gamePausedPanel.GetComponent<Animator>().SetBool("FadeIn", false);

                // waits until panel is hidden
                while (gamePausedPanel.GetComponent<Image>().color.a > 0)
                {
                    yield return null;
                }

                // sets timescale
                Time.timeScale = 1;
            }
        }
    }  

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnApplicationQuit()
    {
        // saves data before app closes
        GameData.SaveGameData();
    }
}
