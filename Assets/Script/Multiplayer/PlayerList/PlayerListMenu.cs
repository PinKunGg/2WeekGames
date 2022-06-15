using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _content;
    [SerializeField] PlayerListInfo _playerListInfo;

    private List<PlayerListInfo> _playerListingInfos = new List<PlayerListInfo>();

    public const byte SamePlayerNameCode = 99;

    public override void OnEnable(){
        base.OnEnable();

        GetAllCurrentPlayerInRoom();
    }

    void GetAllCurrentPlayerInRoom(){
        if(!PhotonNetwork.IsConnected){return;}
        
        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerList(playerInfo.Value);
        }
    }

    void AddPlayerList(Player player){
        PlayerListInfo playerInfo = Instantiate(_playerListInfo,_content);

        if(playerInfo != null){
            playerInfo.SetPlayerInfo(player);
            _playerListingInfos.Add(playerInfo);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);

        if(PhotonNetwork.IsMasterClient && base.photonView.IsMine){
            List<string> playersName = new List<string>();

            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                playersName.Add(playerInfo.Value.NickName);
            }

            for (int i = 0; i < playersName.Count - 1; i++)
            {
                if (playersName[i] == newPlayer.NickName)
                {
                    Debug.Log("Kick " + newPlayer.NickName);

                    object[] datas = new object[] { newPlayer.ActorNumber };

                    PhotonNetwork.RaiseEvent(SamePlayerNameCode, datas, RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
            }
        }
        
        Debug.LogFormat("Player '{0}' join the game",newPlayer.NickName);
        AddPlayerList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.LogFormat("Player '{0}' is leave the game",otherPlayer.NickName);

        int index = _playerListingInfos.FindIndex(x => x.info.NickName == otherPlayer.NickName);
        if(index != -1){
            Destroy(_playerListingInfos[index].gameObject);
            _playerListingInfos.RemoveAt(index);
        }
    }
}
