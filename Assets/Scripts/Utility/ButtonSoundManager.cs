using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundManager : MonoBehaviour
{
    public Button [] buttons;
    
    public static AudioSource singletonSource;
    // Start is called before the first frame update
    void Awake()
    {
        if (singletonSource == null) {
            var src = GetComponent<AudioSource>();
            if (src != null) {
                DontDestroyOnLoad(gameObject);
                singletonSource = src; 
            }
        }
    }
    void Start()
    {
        foreach (var button in buttons) {
            button.onClick.AddListener(OnClicked);
        } 
    }

    // Update is called once per frame
    public void OnClicked()
    {
        if (singletonSource == null) return;
        singletonSource.Play();  
    }
}
