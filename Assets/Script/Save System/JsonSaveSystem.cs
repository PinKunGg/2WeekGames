using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class JsonSaveSystem : MonoBehaviour
{
    /**
    * ! TO USE SAVE FUNCTION
    
    * * string data = JsonUtility.ToJson(this, true); //? Save all variable in that script.

    * * string path; //? Path to save and load.
    * * #if UNITY_ANDROID
    * * path = "jar:file://" + Application.dataPath + "!/assets";
    * * #else
    * * path = Application.streamingAssetsPath;
    * * path = Application.persistentDataPath;
    * * #endif

    * * js.SaveJson(path,"<filename>", data); //? Call SaveJson in JsonSaveSystem to save.

    * ! TO USE LOAD FUNCTION

    * * JsonUtility.FromJsonOverwrite(js.LoadJson(path,"<filename>"),this); //? Load all variable in save file and assign to variable in that script with out checking.

    * * string loadData = js.LoadJson(path,"<filename>"); //? Load all variable in save file and assign to variable in that script with checking.
    * * if (string.IsNullOrEmpty(loadData)){
    * *     Debug.LogError("Fail to load <filename>");
    * *     return;
    * * }
    
    */


    #region Singleton
    public static JsonSaveSystem js;
    private void Awake()
    {
        js = this;
    }
    #endregion

    private void Start()
    {
        Debug.LogWarning(Application.persistentDataPath);
    }

    #region Save Data
    public void SaveJson(string path, string filename, string json) //Call this Method to save
    {
        SaveData(path, filename, json); //Send Read Data to SaveData
    }
    private void SaveData(string path, string filename, string json)
    {
        string DirecPath = Path.Combine(path, filename + ".json");
        Debug.LogWarning(DirecPath);

        if (Directory.Exists(path)) //Check if it have Directory
        {
            if (File.Exists(DirecPath)) //Check if it have File to save
            {
                Debug.Log("<color=lime>*--* Save '" + filename + "' Json *--*</color>");
                FileStream stream = new FileStream(DirecPath, FileMode.Create); //get file

                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json); //write data to save file
                writer.Close();
                stream.Close();

                SaveManager.AfterLoadAndSave();
            }
            else
            {
                Debug.LogError("*--* Save '" + filename + "' File Json Not Found *--*");
                Debug.Log("<color=orange>*--* Create New Save '" + filename + "' Json *--*</color>");
                FileStream stream = new FileStream(DirecPath, FileMode.Create); //create file
                stream.Close();
                SaveJson(path, filename, json); //repeat save method
            }
        }
        else
        {
            Debug.LogError("*--* Directory Not Found *--*");
            Debug.Log("<color=orange>*--* Create New Directory *--*</color>");
            Directory.CreateDirectory(path); //create Directory
            FileStream stream = new FileStream(DirecPath, FileMode.Create); //create file
            stream.Close();
            SaveJson(path, filename, json); //repeat save method
        }
    }
    #endregion

    #region Load Data
    public string LoadJson(string path, string filename) //Call this Method to load
    {
        string json = LoadData(path, filename); //Send filename to LoadData
        return json; //Return Read Data
    }
    private string LoadData(string path, string filename)
    {
        string DirecPath = Path.Combine(path, filename + ".json");
        Debug.LogWarning(DirecPath);

        if (Directory.Exists(path)) //Check if it have Directory
        {
            if (File.Exists(DirecPath)) //Check if it have File to save
            {
                Debug.Log("<color=yellow>*--* Load '" + filename + "' Json *--*</color>");

                StreamReader reader = new StreamReader(DirecPath);
                string json = reader.ReadToEnd();
                reader.Close();

                SaveManager.AfterLoadAndSave();
                
                return json;
            }
            else
            {
                Debug.LogError("*--* Save '" + filename + "' File Json Not Found *--*");
                Debug.Log("<color=orange>*--* Create New Save '" + filename + "' Json *--*</color>");
                FileStream stream = new FileStream(DirecPath, FileMode.Create); //create file
                stream.Close();
                SaveJson(path, filename, ""); //repeat save method

                return null;
            }
        }
        else
        {
            Debug.LogError("*--* Directory Not Found *--*");
            Debug.Log("<color=orange>*--* Create New Directory *--*</color>");
            Directory.CreateDirectory(path); //create Directory
            FileStream stream = new FileStream(DirecPath, FileMode.Create); //create file
            stream.Close();
            SaveJson(path, filename, ""); //repeat save method

            return null;
        }
    }
    #endregion

    #region Reset Data
    public void ResetSaveJson(string path, string filename) //Call this Method to Reset Save
    {
        ResetSaveData(path, filename, ""); //Send Read Data to SaveData
    }
    private void ResetSaveData(string path, string filename, string json)
    {
        string DirecPath = Path.Combine(path, filename + ".json");
        Debug.LogWarning(DirecPath);

        if (Directory.Exists(DirecPath))
        {
            if (File.Exists(path))
            {
                Debug.LogError("*--* Reset Save '" + filename + "' Json *--*");
                FileStream stream = new FileStream(path, FileMode.Create);

                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Close();
                stream.Close();
            }
            else
            {
                Debug.LogError("*--* Save '" + filename + "' File Json Not Found *--*");
            }
        }
        else
        {
            Debug.LogError("*--* Directory Not Found *--*");
            Debug.Log("<color=orange>*--* Create New Directory *--*</color>");
            Directory.CreateDirectory(path); //create Directory
            FileStream stream = new FileStream(DirecPath, FileMode.Create); //create file
            stream.Close();
        }
    }
    #endregion

}