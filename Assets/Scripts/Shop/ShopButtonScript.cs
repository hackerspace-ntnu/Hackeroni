using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonScript : MonoBehaviour
{
    public string ButtonCode;
    private ShopEngineScript ShopEngine;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        ShopEngine = GameObject.Find("ShopEngine").GetComponent<ShopEngineScript>();
    }

    void OnClick()
    {
        ShopEngine.ShopButtonListener(ButtonCode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
