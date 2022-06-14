using UnityEngine;

/*
    PlayerPrefs

    PlayerPrefs.SetString(filename,"0"); <- Create or Set Value to PlayerPrefs name <filename> value = "0"
    PlayerPrefs.GetString(filename); <- Get value from PlayerPrefs name <filename> *If it exist*
        
    Ex.
    string Coin = PlayerPrefs.GetString(filename);

    if(!PlayerPrefs.HasKey(filename)) <- Check if it have PlayerPrefs <filename> or not if not Create new
    {
        PlayerPrefs.SetString(filename,"0");
    }
*/

public class PlayerPrefSaveSystem : MonoBehaviour
{
    public static PlayerPrefSaveSystem playerPrefsSaveLoad;

    private void Awake()
    {
        playerPrefsSaveLoad = this;
    }

    #region PlayerPrefs String
    public void SaveStringPlayerPrefs(string filename, string value)
    {
        Debug.Log("<color=#FF69B4>PlayerPrefs '" + filename + "' Saved</color>");
        PlayerPrefs.SetString(filename, value);
    }
    public string LoadStringPlayerPrefs(string filename, string defvalue)
    {
        if (!PlayerPrefs.HasKey(filename))
        {
            Debug.LogError("Can't find PlayerPrefs '" + filename + "' Default value = '" + defvalue + "'");
            PlayerPrefs.SetString(filename, defvalue);
            return defvalue;
        }
        else
        {
            Debug.Log("<color=cyan>PlayerPrefs '" + filename + "' Load = '" + PlayerPrefs.GetString(filename) + "'</color>");
            return PlayerPrefs.GetString(filename);
        }
    }
    public void ResetStringData(string filename, string value)
    {
        Debug.LogError("Reset PlayerPrefs '" + filename + "' value to = " + value +"' ");
        PlayerPrefs.SetString(filename, value);
    }
    #endregion

    #region PlayerPrefs Int
    public void SaveIntPlayerPrefs(string filename, int value)
    {
        Debug.Log("<color=#FF69B4>PlayerPrefs '" + filename + "' Saved</color>");
        PlayerPrefs.SetInt(filename, value);
    }
    public int LoadIntPlayerPrefs(string filename, int defvalue)
    {
        if (!PlayerPrefs.HasKey(filename))
        {
            Debug.LogError("Can't find PlayerPrefs '" + filename + "' Default value = '" + defvalue + "'");
            PlayerPrefs.SetInt(filename, defvalue);
            return defvalue;
        }
        else
        {
            Debug.Log("<color=cyan>PlayerPrefs '" + filename + "' Load = '" + PlayerPrefs.GetInt(filename) + "'</color>");
            return PlayerPrefs.GetInt(filename);
        }
    }
    public void ResetIntData(string filename, int value)
    {
        Debug.LogError("Reset PlayerPrefs '" + filename + "' value to = " + value +"' ");
        PlayerPrefs.SetInt(filename, value);
    }
    #endregion

    #region PlayerPrefs Float
    public void SaveFloatPlayerPrefs(string filename, float value)
    {
        Debug.Log("<color=#FF69B4>PlayerPrefs '" + filename + "' Saved</color>");
        PlayerPrefs.SetFloat(filename, value);
    }
    public float LoadFloatPlayerPrefs(string filename, float defvalue)
    {
        if (!PlayerPrefs.HasKey(filename))
        {
            Debug.LogError("Can't find PlayerPrefs '" + filename + "' Default value = '" + defvalue + "'");
            PlayerPrefs.SetFloat(filename, defvalue);
            return defvalue;
        }
        else
        {
            Debug.Log("<color=cyan>PlayerPrefs '" + filename + "' Load = '" + PlayerPrefs.GetFloat(filename) + "'</color>");
            return PlayerPrefs.GetFloat(filename);
        }
    }
    public void ResetFloatData(string filename, float value)
    {
        Debug.LogError("Reset PlayerPrefs '" + filename + "' value to = " + value +"' ");
        PlayerPrefs.SetFloat(filename, value);
    }
    #endregion
}