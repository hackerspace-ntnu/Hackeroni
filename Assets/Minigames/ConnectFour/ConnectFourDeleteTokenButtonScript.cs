using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectFourDeleteTokenButtonScript : MonoBehaviour
{
    public int x;
    public int y;
    private ConnectFourEngine Engine;

    // Start is called before the first frame update
    void Start()
    {
        Engine = GameObject.Find("Engine").GetComponent<ConnectFourEngine>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Engine.PlayerChooseToDestroyOnClick(x, y, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
