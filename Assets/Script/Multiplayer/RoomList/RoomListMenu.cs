using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _content;
    [SerializeField] RoomListInfo _roomListInfo;

    private List<RoomListInfo> _roomListingInfos = new List<RoomListInfo>();

    private void Awake() {
        
    }

    void GetAllCurrentRoom(){
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        base.OnRoomListUpdate(roomList);

        Debug.Log("Room Updated");

        for(int i = 0; i < roomList.Count; i++){

            if(roomList[i].RemovedFromList){
                int index = _roomListingInfos.FindIndex(x => x.info.Name == roomList[i].Name);
                if(index != -1){
                    Destroy(_roomListingInfos[index].gameObject);
                    _roomListingInfos.RemoveAt(index);
                }
            }else{
                int index = _roomListingInfos.FindIndex(x => x.info.Name == roomList[i].Name);
                if(index == -1){
                    RoomListInfo roomInfo = Instantiate(_roomListInfo,_content);

                    if(roomInfo != null){
                        roomInfo.SetRoomInfo(roomList[i]);
                        _roomListingInfos.Add(roomInfo);
                    }
                }else{
                    _roomListingInfos[index].SetRoomInfo(roomList[i]);
                }
            }
        }
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        _roomListingInfos.Clear();
    }
}
