using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform content;
    [SerializeField] RoomListInfo roomListInfo;

    List<RoomListInfo> roomListingInfos = new List<RoomListInfo>();
    CreateAndJoinRoom createAndJoinRoom;

    private void Awake() {
        createAndJoinRoom = FindObjectOfType<CreateAndJoinRoom>();
    }

    private void Update() {
        if(string.IsNullOrEmpty(createAndJoinRoom.playerName.text)){
            for(int i = 0; i < roomListingInfos.Count; i++){
                roomListingInfos[i].GetComponent<Button>().interactable = false;
            }
        }
        else{
            for(int i = 0; i < roomListingInfos.Count; i++){
                roomListingInfos[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        base.OnRoomListUpdate(roomList);

        Debug.Log("Room Updated");

        for(int i = 0; i < roomList.Count; i++){

            if(roomList[i].RemovedFromList){
                int index = roomListingInfos.FindIndex(x => x.info.Name == roomList[i].Name);
                if(index != -1){
                    Destroy(roomListingInfos[index].gameObject);
                    roomListingInfos.RemoveAt(index);
                }
            }else{
                int index = roomListingInfos.FindIndex(x => x.info.Name == roomList[i].Name);
                if(index == -1){
                    RoomListInfo roomInfo = Instantiate(roomListInfo,content);

                    if(roomInfo != null){
                        roomInfo.SetRoomInfo(roomList[i]);
                        roomListingInfos.Add(roomInfo);
                    }
                }else{
                    roomListingInfos[index].SetRoomInfo(roomList[i]);
                }
            }
        }
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        roomListingInfos.Clear();
    }
}
