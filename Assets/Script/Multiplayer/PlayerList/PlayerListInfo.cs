using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListInfo : MonoBehaviour
{
    [SerializeField] TMP_Text _playerName;

    public Player info {get; private set;}

    public void SetPlayerInfo(Player playerInfo){
        info = playerInfo;
        _playerName.text = playerInfo.NickName;
    }
}
