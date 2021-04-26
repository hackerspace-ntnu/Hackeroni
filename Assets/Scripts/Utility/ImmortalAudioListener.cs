using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalAudioListener : MonoBehaviour
{
    private static GameObject singleton;
    void Awake()
    {
        if (singleton== null) {
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<AudioListener>();
            singleton = gameObject;
        }
    }
}
