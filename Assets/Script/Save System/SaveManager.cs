using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour
{
    public static SaveManager sm;
    private void Awake() {
        sm = this;

        saveEvent = new UnityEvent();
        loadEvent = new UnityEvent();
        afterLoad_SaveEvent = new UnityEvent();

        resetEvent = new UnityEvent();
        deleteEvent = new UnityEvent();
    }

    public static UnityEvent saveEvent;
    public static UnityEvent loadEvent;
    public static UnityEvent afterLoad_SaveEvent;
    public static UnityEvent resetEvent;
    public static UnityEvent deleteEvent;

    private void OnApplicationQuit() {
        SaveNow();
    }

    private void Start() {
        LoadNow();
    }

    private void Update() {
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.S)){
            SaveNow();
        }
        else if(Input.GetKeyDown(KeyCode.D)){
            LoadNow();
        }
        #endif
    }
    public static void SaveNow(){
        saveEvent.Invoke();
    }
    public static void LoadNow(){
        loadEvent.Invoke();
    }
    public static void AfterLoadAndSave(){
        afterLoad_SaveEvent.Invoke();
    }

    public static void ResetNow(){
        resetEvent.Invoke();
    }
    static string[] path = new string[3];
    public static void DeleteNow(){
        path[0] = Application.persistentDataPath;

        if(Directory.Exists(path[0])){
            Directory.Delete(path[0],true);
            Directory.CreateDirectory(path[0]);
        }
    }
}
