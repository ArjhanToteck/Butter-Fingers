using UnityEngine;

public class FakeKey : MonoBehaviour
{
    public GameObject fakeKeyText;

    public float timeSpent = 0;

    bool playerInTrigger = false;

    void Update()
    {
        // counts time player spends in trigger
        if (playerInTrigger) timeSpent += Time.deltaTime;

        // checks if at least 7 seconds were spent in the trigger
        if (timeSpent >= 7)
        {
            // tells player key is fake
            fakeKeyText.SetActive(true);

            Achievement yourLifeIsALie = GameData.achievements[GameData.GetAchievement("Your Life is a Lie")];

            // checks if the achievement was unlocked
            if (!yourLifeIsALie.unlocked)
            {
                // unlocks achievement
                FindObjectOfType<AchievementsManager>().EarnAchievement(ref yourLifeIsALie);
            }

            // destroys script
            Destroy(GetComponent<FakeKey>());
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // checks if player is colliding with trigger
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // checks if player is colliding with trigger
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
