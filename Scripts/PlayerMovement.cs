using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D playerRigidbody;
    public SpriteRenderer playerRenderer;
    public Camera mainCamera;
    public PlayerAim playerAim;
    public GameObject butterStain;
    public GameObject explosion;
    public GameObject keyUI;

    public float power = 4f;
    public float rotation = 50f;

    public Vector2 minimumPower;
    public Vector2 maximumPower;

    public Vector2 velocityCap;

    bool keyFound = false;
    bool levelDone = false;

    Vector2 force;
    Vector3 startPoint;
    Vector3 endPoint;
    public Vector2 downDirection = Vector3.down;
    bool dragging = false;

    int jumpsLeft = 0;

    List<Vector2> butterPoints = new List<Vector2>();

    void Update()
    {
        // prevents player from being controlled while paused
        if (Time.timeScale == 0) return;

        // enforces x velocity cap
        if (playerRigidbody.velocity.x > velocityCap.x)
        {
            playerRigidbody.velocity = new Vector2(velocityCap.x, playerRigidbody.velocity.y);
        }
        else if(playerRigidbody.velocity.x < -velocityCap.x)
        {
            playerRigidbody.velocity = new Vector2(-velocityCap.x, playerRigidbody.velocity.y);
        }

        // enforces y velocity cap
        if (playerRigidbody.velocity.y > velocityCap.y)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, velocityCap.y);
        }
        else if (playerRigidbody.velocity.y < -velocityCap.y)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -velocityCap.y);
        }

        // resets jumpsLeft
        if (IsPlayerGrounded()) jumpsLeft = 0;

        // makes sure there are enough jumps left to move again
        if (jumpsLeft > 0 || IsPlayerGrounded() && !levelDone)
        {
            // gets mouse drag
            if (Input.GetMouseButton(0))
            {
                // sets dragging to true
                dragging = true;

                // sets start point
                startPoint = transform.position;
                startPoint.z = 0;

                // draws arrow to point at current point
                Vector3 currentPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                currentPoint.z = 0;
                playerAim.RenderArrow(transform.position, currentPoint);
            }
            // this means the mouse is not down but dragging is false, so the player has let go
            else if (dragging == true)
            {
                FindObjectOfType<AudioManager>().PlayJumpSound();
                // decreases jumps left if needed
                if (!IsPlayerGrounded()) jumpsLeft--;

                // resets dragging to false to avoid an infinite loop
                dragging = false;

                // erases arrow
                playerAim.EraseArrow();

                // gets endpoint
                endPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                endPoint.z = 0;

                // calculates force to go in direction of arrow
                force = new Vector2(Mathf.Clamp(endPoint.x - startPoint.x, minimumPower.x, maximumPower.x), Mathf.Clamp(endPoint.y - startPoint.y, minimumPower.y, maximumPower.y));

                // moves player in direction of arrow
                playerRigidbody.AddForce(force * power, ForceMode2D.Impulse);

                // rotates player with strength of rotation depending on size of arrow
                playerRigidbody.angularVelocity -= (force.x + force.y) / 2 * rotation;
            }
        }
        else
        {
            // erases arrow
            playerAim.EraseArrow();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // checks if collided with spikes
        if(collision.gameObject.CompareTag("Spikes") && !levelDone)
        {
            Die();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // gets contact points
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // draws butter stain on contact points
            ButterStain(contact.point);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // checks if bread was hit and level is not finished
        if ((collider.gameObject.CompareTag("Bread") && !collider.gameObject.GetComponent<Bread>().locked && !levelDone) || (collider.gameObject.CompareTag("Bread") && collider.gameObject.GetComponent<Bread>().locked && keyFound))
        {
            // unlocks bread
            collider.gameObject.GetComponent<Bread>().locked = false;

            // hides key in UI
            keyUI.SetActive(false);
            keyFound = false;

            // stains bread with butter
            ButterStain(new Vector2(collider.gameObject.transform.position.x, collider.gameObject.transform.position.y), 8f);

            // some good toast achievement
            Achievement someGoodToast = GameData.achievements[GameData.GetAchievement("Some Good Toast")];
            FindObjectOfType<AchievementsManager>().IncrementAchievement(ref someGoodToast);

            // ends level
            FinishLevel();
        }
        // checks if extra jump was hit and not grounded
        else if(collider.gameObject.CompareTag("ExtraJump") && !IsPlayerGrounded())
        {
            FindObjectOfType<AudioManager>().PlayExtraJumpSound();
            jumpsLeft++;
        }
        // checks key was hit
        else if (collider.gameObject.CompareTag("Key"))
        {
            keyFound = true;
            // removes key
            Destroy(collider.gameObject);

            // shows key in UI
            keyUI.SetActive(true);
        }
        else if(collider.gameObject.CompareTag("GravityChange"))
        {
            FindObjectOfType<AudioManager>().PlayGravityChangeSound();
            Physics2D.gravity = (Quaternion.AngleAxis(collider.transform.rotation.eulerAngles.z, Vector3.forward)) * (Vector3.down * 9.81f);
            downDirection = Quaternion.AngleAxis(collider.transform.rotation.eulerAngles.z, Vector3.forward) * Vector3.down;
        }
    }

    bool IsPlayerGrounded()
    {
        // raycasts to look for floor
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, downDirection, playerRenderer.bounds.size.x > playerRenderer.bounds.size.y ? playerRenderer.bounds.size.x : playerRenderer.bounds.size.y);

        // goes through each raycast hit
        foreach(RaycastHit2D hit in hits)
        {
            // checks if raycast hit floor
            if(!!hit && hit.collider.gameObject.CompareTag("Floor"))
            {
                return true;
            }
        }

        // will only reach this point if not grounded
        return false;
    }

    void ButterStain(Vector2 position, float sizeInput = -1)
    {
        // goes through each point in butterPoints
        foreach(Vector2 point in butterPoints)
        {
            // checks if a stain was already placed nearby
            if(Vector2.Distance(point, position) < 0.35)
            {
                // exits function to prevent drawing too much butter
                return;
            }
        }

        // adds position to list of stains
        butterPoints.Add(position);
            
        // instantiates butter stain
        GameObject stain = Instantiate(butterStain);

        // places stain in position
        stain.transform.position = position;

        // rotates stain randomly
        stain.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 365));

        // sizes stain randomly
        float size = sizeInput != -1 ? sizeInput : Random.Range(0.75f, 1.5f);
        stain.transform.localScale = new Vector3(size, size, stain.transform.localScale.z);
    }

    void Die()
    {
        FindObjectOfType<AudioManager>().PlayDeathSound();

        // erases arrow
        playerAim.EraseArrow();

        // shows and starts explostion particle system
        explosion.SetActive(true);
        levelDone = true;

        // sad moment achievement
        Achievement sadMoment = GameData.achievements[GameData.GetAchievement("Sad Moment")];

        // sad moment achievement

        // checks if the achievement was unlocked
        if (!sadMoment.unlocked)
        {
            // unlocks achievement
            FindObjectOfType<AchievementsManager>().EarnAchievement(ref sadMoment);
        }

        // very sad moment achievement
        Achievement verySadMoment = GameData.achievements[GameData.GetAchievement("Very Sad Moment")];
        FindObjectOfType<AchievementsManager>().IncrementAchievement(ref verySadMoment);

        // bruh moment achievement
        Achievement bruhMoment = GameData.achievements[GameData.GetAchievement("Bruh Moment")];

        if(Time.timeSinceLevelLoad <= 3f && !bruhMoment.unlocked)
        {
            FindObjectOfType<AchievementsManager>().EarnAchievement(ref bruhMoment);
        }

        // levels attempted
        Achievement youTried = GameData.achievements[GameData.GetAchievement("You Tried")];

        if (!GameData.levelsAttempted[int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value) - 1])
        {
            GameData.levelsAttempted[int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value) - 1] = true;
            FindObjectOfType<AchievementsManager>().IncrementAchievement(ref youTried);
        }

        // ends level
        gameManager.LevelFailed();
    }

    void FinishLevel()
    {
        // erases arrow
        playerAim.EraseArrow();

        // levels attempted
        Achievement youTried = GameData.achievements[GameData.GetAchievement("You Tried")];

        if (!GameData.levelsAttempted[int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value) - 1])
        {
            GameData.levelsAttempted[int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value) - 1] = true;
            FindObjectOfType<AchievementsManager>().IncrementAchievement(ref youTried);
        }

        // level was completed
        Achievement epicGamer = GameData.achievements[GameData.GetAchievement("Epic Gamer")];

        if (!GameData.levelsCompleted[int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value) - 1])
        {
            GameData.levelsCompleted[int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value) - 1] = true;
            FindObjectOfType<AchievementsManager>().IncrementAchievement(ref epicGamer);
        }

        // ends level
        gameManager.LevelComplete();
    }
}
