using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene loading;
    Canvas canvas;

    private void Awake() {
        loading = this;

        canvas = GetComponent<Canvas>();
    }

    private void Start() {
        CloseLoading();
    }

    public void OpenLoading(){
        canvas.enabled = true;
    }
    public void CloseLoading(){
        canvas.enabled = false;
    }
}
