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

    private void Update() {
        sc.pos = this.transform.position;
    }

    public void ReciveSaveData(string data){
        Debug.LogWarningFormat("[Client] Recive Save Data");
        tutorial_Control = Tutorial_Control.tutorial_Control;
        if (tutorial_Control.IsLobby) { return; }
        JsonUtility.FromJsonOverwrite(data,sc);
        this.transform.position = sc.pos;
    }
}

[System.Serializable]
public class SaveClass_PlayerData
{
    public Vector3 pos;
}
