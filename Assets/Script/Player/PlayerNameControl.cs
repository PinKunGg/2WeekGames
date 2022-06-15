using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameControl : MonoBehaviour
{
    public static PlayerNameControl playerNameControl;
    private void Awake()
    {
        playerNameControl = GetComponent<PlayerNameControl>();
    }

    public List<string> NameOtherPlayer;
    public List<GameObject> HealthBar_Obj;
    public int CountGetName = 0;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddOtherNamePlayer(string NameValue) 
    {
        Debug.Log("name : " + NameValue);
        NameOtherPlayer.Add(NameValue);
    }

    public void RemoveOtherNamePlayer(string NameValue) 
    {
        for (int x = 0; x < NameOtherPlayer.Count; x++) 
        {
            if (NameOtherPlayer[x] == NameValue) 
            {
                NameOtherPlayer.RemoveAt(x);
                CountGetName = 1;
                break;
            }
        }
    }

    public string GetOtherNamePlayer() 
    {
        return NameOtherPlayer[CountGetName++];
    }

    public void AddHealthBar(GameObject ObjValue) 
    {
        HealthBar_Obj.Add(ObjValue);
    }

    public void RemoveHealthBar_ByName(string NameValue) 
    {
        for (int x = 0; x < NameOtherPlayer.Count; x++)
        {
            if (NameOtherPlayer[x] == NameValue)
            {
                Destroy(HealthBar_Obj[x-1]);
                HealthBar_Obj.RemoveAt(x-1);
                NameOtherPlayer.RemoveAt(x);
                CountGetName--;
                break;
            }
        }
    }
}
