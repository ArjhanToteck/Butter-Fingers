using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public bool musicVolume = true;

    void Start()
    {
        Debug.Log(GameData.soundEffectsVolume);
        GetComponent<Scrollbar>().value = musicVolume ? GameData.musicVolume : GameData.soundEffectsVolume;

        GetComponent<Scrollbar>().onValueChanged.AddListener((float val) => ChangeVolume(val));        
    }

    public void ChangeVolume(float value)
    {
        if (musicVolume)
        {
            GameData.musicVolume = value;
            FindObjectOfType<AudioManager>().musicPlayer.volume = value;
            GameData.SaveGameData();
        }
        else
        {
            GameData.soundEffectsVolume = value;
            FindObjectOfType<AudioManager>().soundEffectsPlayer.volume = value;
            GameData.SaveGameData();
        }
    }
}