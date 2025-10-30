using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixers : MonoBehaviour {
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            loadVolume();
        }
        else
        {
            setMusicVolume();
        }
    }

    void Update()
    {
        setMusicVolume();
        setSfxVolume();
    }

    public void setMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    public void setSfxVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    void loadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        setMusicVolume();
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        setSfxVolume();
    }

}