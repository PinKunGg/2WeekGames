using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tests : MonoBehaviour
{
    public string Tag;

    private void Awake()
    {

        GameObject[] obj = GameObject.FindGameObjectsWithTag(Tag);

        if (obj.Length > 1)
        {
            this.gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
