using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicPlayer;
    public AudioSource soundEffectsPlayer;

    public AudioClip achievementSound;
    public AudioClip deathSound;
    public AudioClip extraJumpSound;
    public AudioClip gravityChangeSound;
    public AudioClip jumpSound;
    public AudioClip selectionSound;

    void Awake()
    {
        // checks if music data is saved, and if so is saved for current song
        if (GameData.musicData.initialized && GameData.musicData.name == musicPlayer.clip.name)
        {
            // sets music time to saved time
            musicPlayer.time = GameData.musicData.time;
        }

        // sets volume of music player
        musicPlayer.volume = GameData.musicVolume;

        // sets volume of sound effects player
        soundEffectsPlayer.volume = GameData.soundEffectsVolume;

        musicPlayer.Play();
    }

    void Update()
    {
        // updates music data
        GameData.musicData = new MusicData(musicPlayer.clip.name, musicPlayer.time, true);
    }

    public void PlayAchievementSound()
    {
        soundEffectsPlayer.PlayOneShot(achievementSound);
    }

    public void PlayDeathSound()
    {
        soundEffectsPlayer.PlayOneShot(deathSound);
    }

    public void PlayExtraJumpSound()
    {
        soundEffectsPlayer.PlayOneShot(extraJumpSound);
    }

    public void PlayGravityChangeSound()
    {
        soundEffectsPlayer.PlayOneShot(gravityChangeSound);
    }

    public void PlayJumpSound()
    {
        soundEffectsPlayer.PlayOneShot(jumpSound);
    }

    public void PlaySelectionSound()
    {
        soundEffectsPlayer.PlayOneShot(selectionSound);
    }
}

public class MusicData
{
    public string name = "";
    public float time = -1;
    public bool initialized = false;

    public MusicData(string nameInput = "", float timeInput = -1, bool initilaizedInput = false)
    {
        name = nameInput;
        time = timeInput;
        initialized = initilaizedInput;
    }
}
