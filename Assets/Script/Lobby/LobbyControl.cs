using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyControl : MonoBehaviour
{
    public int PlayerCount = 0;
    public List<GameObject> AllPlayerObj;
    public GameObject[] Pos;
    PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addPlayer(GameObject player) 
    {
        AllPlayerObj.Add(player);
        setPlayerPosition();
    }

    public void setPlayerPosition() 
    {
        Debug.Log("WTF : "+PlayerCount);
        AllPlayerObj[PlayerCount].transform.position = Pos[PlayerCount].transform.position;
        AllPlayerObj[PlayerCount].transform.rotation = Pos[PlayerCount].transform.rotation;
        PlayerCount++;
    }
}
