using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DebugLog : MonoBehaviour
{
    public GameObject DebugScreen;
    public GameObject Content;
    public GameObject textObject;
    public ScrollRect scrollRect;

    StringBuilder stringBuilder = new StringBuilder();

    bool shouldUpdateText = true;
    Text text;


    void Start () {
        InstantiateText();
        //Application.logMessageReceived += HandleLog;
        Application.logMessageReceivedThreaded += HandleLog;
    }
    
    void OnDestroy () {
        //Application.logMessageReceived -= HandleLog;
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type){
        stringBuilder.Append("[");
        stringBuilder.Append(type);
        stringBuilder.Append("]");
        stringBuilder.AppendLine(logString);

        
        if (type == LogType.Exception)
        {
            stringBuilder.AppendLine(stackTrace);
        }
        
        shouldUpdateText = true;
    }


    public void ToggleDrawLog()
    {
        DebugScreen.SetActive(!DebugScreen.activeInHierarchy);
        shouldUpdateText = DebugScreen.activeInHierarchy;
    }

    void OnGUI()
    {
        if (shouldUpdateText)
        {
            if (stringBuilder.Length > 1000)
            {
                InstantiateText();
            }
            else{
                text.text = stringBuilder.ToString();
            }
            if (scrollRect.velocity.sqrMagnitude < 0.01f)
                scrollRect.normalizedPosition  = Vector2.zero;
            shouldUpdateText = false;
        }
    }

    void InstantiateText()
    {
        if (text != null)
            text.text = stringBuilder.ToString();
        text = Instantiate(textObject,Content.transform).GetComponent<Text>();
        stringBuilder.Clear();
    }
}