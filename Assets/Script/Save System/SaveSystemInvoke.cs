using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemInvoke : MonoBehaviour
{
    public void LoadNow()
    {
        SaveManager.LoadNow();
    }

    public void SaveNow()
    {
        SaveManager.SaveNow();
    }
}
