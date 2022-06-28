using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerData : MonoBehaviourPunCallbacks
{
    public Tutorial_Control tutorial_Control;
    public SaveClass_PlayerData sc;

    public void ReciveSaveData(string data){
        Debug.LogWarningFormat("[Client] Recive Save Data");
        tutorial_Control = Tutorial_Control.tutorial_Control;
        if (tutorial_Control.IsLobby) { return; }
        JsonUtility.FromJsonOverwrite(data,sc);
    }
}

[System.Serializable]
public class SaveClass_PlayerData
{

}
