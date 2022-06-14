using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class DisconnectFromServer : MonoBehaviourPunCallbacks
{
    TestMultiplayerSave testMultiplayerSave;

    private void Start() {
        testMultiplayerSave = GetComponent<TestMultiplayerSave>();
        
        if(PhotonNetwork.IsMasterClient && base.photonView.IsMine){
            FindObjectOfType<OwnerControllpanel>().disconnectFromServer = this;
        }
    }

    [PunRPC]
    void RPC_Disconnect(){
        Debug.Log(5);
        PhotonNetwork.LeaveRoom();
    }

    public Action myAction;
    public Action<string> anotherAction;

    public void OnClick_Disconnect(){
        if(PhotonNetwork.IsMasterClient){
            Debug.Log(1);
            base.photonView.RPC("RPC_Disconnect",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            StartCoroutine(PrepareDisconnect());
            return;
        }

        Debug.Log(6);
        PhotonNetwork.LeaveRoom();
    }

    public IEnumerator PrepareDisconnect(){
        Debug.Log(2);
        while(PhotonNetwork.CurrentRoom.PlayerCount > 1){
            yield return null;
        }
        
        Debug.Log(3);
        testMultiplayerSave.SaveOnExit(PhotonNetwork.NickName);
        
        while(!testMultiplayerSave.isSaveDone){
            yield return null;
        }

        Debug.Log(4);
        PhotonNetwork.LeaveRoom();
    }

    public void OnClick_CloseServer(){
        if(PhotonNetwork.IsMasterClient){
            base.photonView.RPC("RPC_Disconnect",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public void OnClick_OpenServer(){
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        if(PhotonNetwork.IsMasterClient && base.photonView.IsMine){
            Debug.Log(this.name);
            testMultiplayerSave.SaveOnExit(otherPlayer.NickName);
        }
    }
}
