using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Arachne_SpawnSpider : MonoBehaviour
{
    public Transform[] spawnPos;
    public GameObject SpiderMinion;
    ArachneAI arachneAI;

    private void Awake() {
        arachneAI = GetComponent<ArachneAI>();
    }

    public void SpawnSpiderMinion(){
        if(!PhotonNetwork.IsMasterClient){return;}
        
        for(int i = 0; i < spawnPos.Length; i++){
            GameObject spiderTemp = PhotonNetwork.Instantiate(SpiderMinion.name,spawnPos[i].position,spawnPos[i].rotation);
            spiderTemp.GetComponent<Monster_Movement>().monsterStat = arachneAI.monsterStat;
        }
    }
}
