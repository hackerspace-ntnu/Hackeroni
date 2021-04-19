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
    public AudioSource musicSource;
    public bool pauseMusicWhenPaused = false;
    void Start()
    {
        var volume = PlayerPrefs.GetFloat("settings/musicVolume/" + SceneManager.GetActiveScene().name, 0.5f);
        if (musicSource == null && SimpleBootstrap.theImmortalMusicSource != null) {
            musicSource = SimpleBootstrap.theImmortalMusicSource;
        }
        SettingsPanel.SetActive(false);
        musicVolumeSlider.value = volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (isToggled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var rect = SettingsPanel.GetComponent<RectTransform>();
                if (!RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, null))
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
            Time.timeScale = 0;
            if (pauseMusicWhenPaused) {
                musicSource.Pause();
            }
        }
        else
        {
            Time.timeScale = 1;
            PlayerPrefs.SetFloat("settings/musicVolume/" + SceneManager.GetActiveScene().name, musicSource.volume);
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
}
