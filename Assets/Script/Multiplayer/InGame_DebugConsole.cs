using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class InGame_DebugConsole : MonoBehaviour
{
    public string Tag;
    private void Awake() {

        GameObject[] obj = GameObject.FindGameObjectsWithTag(Tag);

        if(obj.Length > 1){
            this.gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    
    // Adjust via the Inspector
    public int maxLines = 8;
    private Queue<string> queue = new Queue<string>();
    private string currentText = "";

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(logString);

        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append(st).Append("\n");
        }

        currentText = builder.ToString();
    }

    void OnGUI()
    {
        GUI.Label(
           new Rect(
               5,                   // x, left offset
               Screen.height - 150, // y, bottom offset
               300f,                // width
               150f                 // height
           ),      
           currentText,             // the display text
           GUI.skin.textArea        // use a multi-line text area
        );
    }
}
