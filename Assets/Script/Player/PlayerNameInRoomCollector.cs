using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameInRoomCollector : MonoBehaviour
{
    public List<string> AllPlayerName;
    public int CountPlayer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayerName(string value) 
    {
        AllPlayerName.Add(value);
        CountPlayer++;
    }
}
