using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCharacterScript : MonoBehaviour
{
    public Image hatSprite;
    // Start is called before the first frame update
    void Start()
    {
        hatSprite.enabled = true;
        GetComponent<Image>().sprite = PlayerPrefManager.GetCurrentSkinSprite();
        GetComponent<Image>().color = PlayerPrefManager.GetCurrentColor();
        hatSprite.sprite = PlayerPrefManager.GetCurrentHatSprite();
        if (hatSprite.sprite == null) {
            hatSprite.enabled = false;
        }
    }
}
