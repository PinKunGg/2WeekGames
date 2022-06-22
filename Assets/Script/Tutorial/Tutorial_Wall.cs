using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Wall : MonoBehaviour
{
    Tutorial_Control tutorial_Control;
    void Start()
    {
        tutorial_Control = GetComponent<Tutorial_Control>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            tutorial_Control.CompleteStage(0);
            Destroy(this.gameObject);
        }
    }
}
