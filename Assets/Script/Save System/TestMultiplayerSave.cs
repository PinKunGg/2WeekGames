using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class TestMultiplayerSave : MonoBehaviourPunCallbacks
{
    public TMP_Text pos_txt;
    public PlayerData sc;
    JsonSaveSystem js;
    SaveManager sm;

    public bool isSaveDone;

    public override void OnEnable() {
        base.OnEnable();

        js = JsonSaveSystem.js;
        sm = SaveManager.sm;
    }

    private void Update() {
        sc.pos = this.gameObject.transform.position;
        pos_txt.text = sc.pos.ToString();
    }

    [PunRPC]
    public void RPC_SaveNow(string playerName){
        isSaveDone = false;

        PlayerData playerSc = null;
        var photonView = FindObjectsOfType<PhotonView>();

        for(int i = 0; i < photonView.Length; i++){
            if(photonView[i].Owner.NickName == playerName){
                playerSc = photonView[i].GetComponent<TestMultiplayerSave>()?.sc;

                string data = JsonUtility.ToJson(playerSc, true);
                js.SaveJson(Application.persistentDataPath,playerName, data);
                Debug.Log("Save Data");

                break;
            }
        }

        isSaveDone = true;
    }

    [PunRPC]
    public void RPC_LoadNow(){

    }

    public void SaveOnExit(string playerName){
        // sc.pos = this.gameObject.transform.position;
        base.photonView.RPC("RPC_SaveNow",RpcTarget.MasterClient,playerName);
    }
}

[System.Serializable]
public class PlayerData{
    public int a;
    public string b;
    public Vector3 pos;
}
