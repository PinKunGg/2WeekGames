using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Tutorial_Control : MonoBehaviour
{
    public static Tutorial_Control tutorial_Control;
    private void Awake()
    {
        tutorial_Control = this;
    }

    public bool IsTutorial = false;
    public bool IsLobby = false;
    public bool[] Stage;
    public GameObject[] Stage_Obj;
    public GameObject HP_Boss;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Check());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Check() 
    {
        int count = 0;
        bool check = true;
        while (check) 
        {
            if (Stage[count]) 
            {
                Stage_Obj[count].SetActive(false);
                count++;
            }
            yield return new WaitForSeconds(0.5f);
            if (count == Stage.Length) 
            {
                check = false;
            }
        }
    }
    public void CompleteStage(int value) 
    {
        Stage[value] = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PhotonNetwork.LeaveRoom();
        }
    }
}
