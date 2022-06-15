using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

public class DisconnectFromServer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    TestMultiplayerSave testMultiplayerSave;

    public bool _isSaveOnExit;
    public bool _isSaveByPass;

    private void Start() {
        testMultiplayerSave = GetComponent<TestMultiplayerSave>();
        
        if(PhotonNetwork.IsMasterClient && base.photonView.IsMine){
            FindObjectOfType<OwnerControllpanel>().disconnectFromServer = this;
        }
    }

    public override void OnEnable(){
        base.OnEnable();

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable(){
        base.OnDisable();

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent){
        if (photonEvent.Code == 99) //Same player name deteced
        {
            //base.photonView.RPC("RPC_IsSaveOnExit", RpcTarget.MasterClient, false);
            object[] reciveData = (object[])photonEvent.CustomData;

            Debug.LogFormat("ID = {0}", PhotonNetwork.LocalPlayer.ActorNumber);

            for (int i = 0; i < reciveData.Length; i++){
                Debug.LogFormat("reciveData ID = {0}", (int)reciveData[i]);

                if (PhotonNetwork.LocalPlayer.ActorNumber == (int)reciveData[i]){
                    RPC_Disconnect();
                }
            }
        }
        else if(photonEvent.Code == 1) //Client leave server
        {
            if (PhotonNetwork.IsMasterClient){
                _isSaveOnExit = true;
            }
        }
    }

    [PunRPC]
    void RPC_IsSaveOnExit(bool value){
        _isSaveOnExit = value;
    }

    [PunRPC]
    void RPC_Disconnect(){
        Debug.Log(5);

        PhotonNetwork.LeaveRoom();
    }

    public Action myAction;
    public Action<string> anotherAction;

    public void OnClick_Disconnect(){
        base.photonView.RPC("RPC_IsSaveOnExit", RpcTarget.MasterClient, true);

        if (PhotonNetwork.IsMasterClient){
            Debug.Log(1);
            _isSaveByPass = true;
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
        if (_isSaveOnExit){
            testMultiplayerSave.SaveOnExit(PhotonNetwork.NickName);

            while (!testMultiplayerSave.isSaveDone){
                yield return null;
            }
        }

        _isSaveOnExit = false;
        Debug.Log(4);
        PhotonNetwork.LeaveRoom();
    }

    public void OnClick_CloseServer(){
        if(PhotonNetwork.IsMasterClient){
            base.photonView.RPC("RPC_IsSaveOnExit", RpcTarget.MasterClient, true);
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

            if(_isSaveByPass){
                testMultiplayerSave.SaveOnExit(otherPlayer.NickName);
            }
            else{
                if (_isSaveOnExit){
                    Debug.LogError("Save");
                    testMultiplayerSave.SaveOnExit(otherPlayer.NickName);
                    _isSaveOnExit = false;
                }else{
                    Debug.LogError("Not-Save");
                }
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient){
        base.OnMasterClientSwitched(newMasterClient);


    }
}
