using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsManager : MonoBehaviour
{
    public GameObject achievementAlertTemplate;
    public GameObject achievementAlertsDisplay;

    public GameObject achievementTemplate;
    public GameObject achievementsListDisplay;
    public bool achievementsMenu = false;
    public float lockedAchievementColorDifference = 0.3f;

    void Start()
    {
        if(achievementsMenu)
        {
            DisplayAchievementsList();
        }

        // checks if queue has any achievements
        if(GameData.achievementsQueue.Count > 0)
        {
            // unlocks achievements in queue
            for(int i = 0; i < GameData.achievementsQueue.Count; i++)
            {
                Achievement achievement = GameData.achievementsQueue[i];
                EarnAchievement(ref achievement);
            }
        }
    }

    public void IncrementAchievement(ref Achievement achievement)
    {
        // inrements achievement progress
        achievement.progress++;

        // caps achievement progress
        if (achievement.progress > achievement.progressMax) achievement.progress = achievement.progressMax;

        // checks if achievement completed
        if (achievement.progress == achievement.progressMax) EarnAchievement(ref achievement);
    }

    public void EarnAchievement(ref Achievement achievement)
    {
        if(!GameData.achievementsQueue.Contains(achievement)) GameData.achievementsQueue.Add(achievement);

        // unlocks achievement
        achievement.unlocked = true;
        achievement.hidden = false;

        Achievement localAchievement = achievement;

        StartCoroutine(WaitAndEarnAchievement());

        IEnumerator WaitAndEarnAchievement()
        {
            // overachiever achievement
            Achievement overachiever = GameData.achievements[GameData.GetAchievement("Overachiever")];

            if (!overachiever.unlocked)
            {
                int unlockedCount = 0;

                foreach (Achievement achievement in GameData.achievements)
                {
                    if (achievement.unlocked) unlockedCount++;
                }

                if (unlockedCount + 1 == GameData.achievements.Length)
                {
                    FindObjectOfType<AchievementsManager>().EarnAchievement(ref overachiever);
                }
            }            

            // checks if other achievement is currently being displayed
            while (achievementAlertsDisplay.transform.childCount >= 1)
            {
                yield return null;
            }

            // removes achievement from queue
            GameData.achievementsQueue.Remove(localAchievement);

            // plays achievement sound effect
            FindObjectOfType<AudioManager>().PlayAchievementSound();

            // creates achievement alert
            GameObject achievementAlert = Instantiate(achievementAlertTemplate);
            achievementAlert.transform.SetParent(achievementAlertsDisplay.transform, false);

            // sets name and description
            achievementAlert.transform.Find("Name").GetComponent<Text>().text = localAchievement.name;
            achievementAlert.transform.Find("Description").GetComponent<Text>().text = localAchievement.description;

            // waits until animation is finished and alert is hidden
            while (achievementAlert.transform.Find("Image").GetComponent<Image>().color.a != 0)
            {
                yield return null;
            }

            // destroys alert
            Destroy(achievementAlert);
        }
    }

    public void DisplayAchievementsList()
    {
        List<string> unlockedAchievements = new List<string>();
        List<string> lockedAchievements = new List<string>();
        List<string> hiddenAchievements = new List<string>();
        List<string> achievements;

        // orders achievements
        foreach (Achievement achievement in GameData.achievements)
        {
            // checks if achievement is unlocked
            if (achievement.unlocked)
            {
                // adds unlocked achievements to unlocked list
                unlockedAchievements.Add(achievement.name);
            }
            else if(achievement.hidden)
            {
                hiddenAchievements.Add(achievement.name);
            }
            else
            {
                // adds locked achievements to locked list
                lockedAchievements.Add(achievement.name);
            }
        }

        // alphabetizes lists
        unlockedAchievements.Sort();
        lockedAchievements.Sort();

        // puts lists together
        achievements = unlockedAchievements.Concat(lockedAchievements).ToList();
        achievements = achievements.Concat(hiddenAchievements).ToList();

        foreach (string achievementName in achievements)
        {
            Achievement achievement = GameData.achievements[GameData.GetAchievement(achievementName)];

            // creates achievement object
            GameObject achievementObject = Instantiate(achievementTemplate);

            // sets parent
            achievementObject.transform.SetParent(achievementsListDisplay.transform, false);

            // checks if hidden
            if(achievement.hidden)
            {
                achievementObject.transform.Find("Name").GetComponent<Text>().text = "???";
                achievementObject.transform.Find("Description").GetComponent<Text>().text = "???";
            }
            else
            {
                achievementObject.transform.Find("Name").GetComponent<Text>().text = achievement.name;
                achievementObject.transform.Find("Description").GetComponent<Text>().text = achievement.description;

                // checks if achievement has progress
                if (achievement.progress != -1)
                {
                    achievementObject.transform.Find("Description").GetComponent<Text>().text += $" ({achievement.progress}/{achievement.progressMax})";
                }
            }

            // checks if locked
            if (!achievement.unlocked)
            {
                // changes color of locked achievements
                Color currentColor = achievementObject.transform.Find("Image").GetComponent<Image>().color;
                achievementObject.transform.Find("Image").GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a - lockedAchievementColorDifference);

                // changes name of locked achievement
                if (!achievement.hidden) achievementObject.transform.Find("Name").GetComponent<Text>().text += " (Locked)";
            }

            // positions achievement
            achievementObject.transform.localPosition = new Vector3(0, -(achievementsListDisplay.transform.childCount * (Screen.height / 4f)), 0);
        }

        achievementsListDisplay.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(achievementsListDisplay.transform.parent.GetComponent<RectTransform>().sizeDelta.x, 200 * achievements.Count);
    }

}

[System.Serializable]
public class Achievement
{
    public string name;
    public string description;
    public bool hidden;
    public bool unlocked;
    public int progress;
    public int progressMax;

    public Achievement(string nameInput, string descriptionInput, bool hiddenInput = false, bool unlockedInput = false, int progressInput = -1, int progressMaxInput = -1)
    {
        name = nameInput;
        description = descriptionInput;
        hidden = hiddenInput;
        unlocked = unlockedInput;
        progress = progressInput;
        progressMax = progressMaxInput;
    }
}