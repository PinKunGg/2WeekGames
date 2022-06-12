using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class player_move : MonoBehaviour
{
    PhotonView photonView;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }

    private void Update() {
        if(!photonView.IsMine){
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        this.transform.Translate(new Vector3(h * 2f * Time.deltaTime,v * 2f * Time.deltaTime,0));
    }
}
