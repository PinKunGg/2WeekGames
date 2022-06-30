using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameAlert_Nortification : MonoBehaviour
{
    public GameObject AlertNotification;

    [Space]
    public TMP_Text Header;
    public TMP_Text Detail;

    [Space]
    public GameObject yes_but;
    public GameObject no_but;
    public GameObject close_but;

    private void Start() {
        AlertNotification.SetActive(false);
    }
    
    public void SetAlert(string header, string detail, bool isClose){
        Header.text = header;
        Detail.text = detail;

        yes_but.SetActive(!isClose);
        no_but.SetActive(!isClose);
        close_but.SetActive(isClose);

        AlertNotification.SetActive(true);
    }

    int actionInt;
    public IEnumerator WaiteForCallBack(System.Action<bool> callback){
        actionInt = 0;
        
        while(actionInt == 0){
            yield return null;
        }

        if(actionInt == 1){
            callback(true);
        }
        else{
            callback(false);
        }
    }

    public void OnClickCloseAlert(){
        AlertNotification.SetActive(false);
    }
    public void OnClickYes(){
        actionInt = 1;
        OnClickCloseAlert();
    }
    public void OnClickNo(){
        actionInt = 2;
        OnClickCloseAlert();
    }
}
