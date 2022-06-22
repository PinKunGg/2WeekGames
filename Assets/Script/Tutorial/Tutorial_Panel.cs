using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Panel : MonoBehaviour
{
    public GameObject Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player) { Player = GameObject.FindGameObjectWithTag("Player"); }
        this.transform.LookAt(Player.transform);
    }
}
