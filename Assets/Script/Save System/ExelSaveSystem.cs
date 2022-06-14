using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExelSaveSystem : MonoBehaviour
{
    #region Singleton
    public static ExelSaveSystem exelSaveLoad;

    private void Awake()
    {
        exelSaveLoad = this;
    }
    #endregion

    string filePath;
    string line;
    string[] row = new string[5];

    string[] allQustion;
    string[] eachQuestion;
    int eachQuestionCount = 0;

    public LoadContainer[] loadcontent = new LoadContainer[1];

    public string[][] LoadCsv(string FileName)
    {
        Debug.Log("<color=yellow>*--* Load '" + FileName + "' *--*</color>");

#if UNITY_ANDROID
        filePath = "jar:file://" + Application.dataPath + "!/assets" + "/Question/" + FileName;
#else
        filePath = Application.streamingAssetsPath + "/Question/" + FileName;
#endif

        WWW reader = new WWW(filePath);
        while (!reader.isDone)
        {
        }

        string data = reader.text;

        allQustion = data.Split(new char[] {'\n'});
        loadcontent = new LoadContainer[allQustion.Length - 2];

        for (int i = 1; i < allQustion.Length - 1; i++)
        {
            eachQuestion = allQustion[i].Split(',');

            loadcontent[eachQuestionCount] = new LoadContainer(eachQuestion.Length);

            for (int j = 0; j < eachQuestion.Length; j++)
            {
                loadcontent[eachQuestionCount].loadData[j] = eachQuestion[j];
            }

            eachQuestionCount++;
        }

        string[][] returnData = new string[loadcontent.Length][];

        for (int i = 0; i < returnData.Length; i++)
        {
            returnData[i] = new string[loadcontent[i].loadData.Length];

            for (int j = 0; j < loadcontent[i].loadData.Length; j++)
            {
                returnData[i][j] = loadcontent[i].loadData[j];
            }
        }

        return returnData;
    }
}

[System.Serializable]
public class LoadContainer
{
    public string[] loadData;
    public LoadContainer(int size)
    {
        loadData = new string[size];
    }
}