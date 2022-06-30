using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager_Local : MonoBehaviour
{
    private void OnEnable() {
        FindObjectOfType<PlayerManager_Multiplayer>().AddPlayer(this.gameObject);
    }
}
