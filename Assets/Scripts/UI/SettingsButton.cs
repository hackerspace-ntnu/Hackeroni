using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject ToggleButton;
    public Slider musicVolumeSlider;
    public Slider soundEffectsVolumeSlider;
    public AudioSource musicSource;
    public AudioSource[] soundEffectSources;
    private float[] initialVolumes;
    
    private float soundEffectVolume;
    private float nonPausedTimeScale = 1;
    public bool pauseMusicWhenPaused = false;
    void Start()
    {
        var volume = PlayerPrefs.GetFloat("settings/musicVolume/" + SceneManager.GetActiveScene().name, 0.5f);
        if (musicSource == null && SimpleBootstrap.theImmortalMusicSource != null) {
            musicSource = SimpleBootstrap.theImmortalMusicSource;
        }
        SettingsPanel.SetActive(false);
        musicVolumeSlider.value = volume;

        soundEffectVolume = PlayerPrefs.GetFloat("settings/soundEffectVolume/" + SceneManager.GetActiveScene().name, 0.5f);
        soundEffectsVolumeSlider.value = soundEffectVolume;
        initialVolumes = new float[soundEffectSources.Length];
        for (int i = 0; i < soundEffectSources.Length; i++) 
        {
            //Some extreme hacks ok
            var bsm = soundEffectSources[i].gameObject.GetComponent<ButtonSoundManager>();
            if (bsm != null && ButtonSoundManager.singletonSource != null)
            {
                soundEffectSources[i] = ButtonSoundManager.singletonSource;
                initialVolumes[i] = 1;
            } else {
                initialVolumes[i] = soundEffectSources[i].volume;
            }
            //Hack stops here
            
            soundEffectSources[i].volume *= 2 * soundEffectVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isToggled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var rect = SettingsPanel.GetComponent<RectTransform>();
                if (!(RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, null) || RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, Camera.main)))
                {
                    OnButtonPress();
                }
            }
        }
    }

    private bool isToggled = false;

    public void OnButtonPress()
    {
        isToggled = !isToggled;

        SettingsPanel.SetActive(isToggled);
        ToggleButton.GetComponent<Button>().enabled = (!isToggled);
        if (isToggled)
        {
            nonPausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            if (pauseMusicWhenPaused) {
                musicSource.Pause();
            }
        }
        else
        {
            Time.timeScale = nonPausedTimeScale;
            PlayerPrefs.SetFloat("settings/musicVolume/" + SceneManager.GetActiveScene().name, musicSource.volume);
            PlayerPrefs.SetFloat("settings/soundEffectVolume/" + SceneManager.GetActiveScene().name, soundEffectVolume);
            if (pauseMusicWhenPaused) {
                musicSource.Play();
            }
        }
    }
    
    void OnDisable() {
        Time.timeScale = 1;
    }

    public void OnScrolling()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolumeSlider.value;
        }
    }
    public void OnSoundEffectsScrolling()
    {
        if (initialVolumes == null) return;
        soundEffectVolume = soundEffectsVolumeSlider.value;
        for (int i = 0; i < soundEffectSources.Length; i++) 
        {
            soundEffectSources[i].volume = 2 * soundEffectVolume * initialVolumes[i];
        }
    }
}
