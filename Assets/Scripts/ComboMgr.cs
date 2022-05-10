using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using UnityEngine.UI;

public class ComboMgr : MonoBehaviour {

    public static ComboMgr instance;
    int comboNum;
    GameObject comboObj;

    public GameObject ui_combo;
    public GameObject ui_pop;

    void Awake() {
        instance = this;
        comboNum = 1;
        comboObj = null;
    }

    void Start() {
        this.UpdateAsObservable()
            .Select(_ => comboObj)
            .DistinctUntilChanged()
            .Where(obj => obj != null)
            .Delay(System.TimeSpan.FromSeconds(3))
            .Subscribe(obj => ResetCombo(obj));
    }

    public int AddCombo(GameObject in_1, GameObject in_2, GameObject out_1) {
        if(comboObj == in_1 || comboObj == in_2) comboNum++;
        else comboNum = 1;
        comboObj = out_1;  
        Debug.Log("COMBO : " + comboNum);
        //SetCombo();
        return comboNum;
    }

    void ResetCombo(GameObject obj) {
        if(obj != comboObj) return;
        comboObj = null;
        ui_combo.transform.DOScale(Vector3.zero, 0.25f);
    }

    void SetCombo() {
        ui_combo.GetComponent<Text>().text = "Combo\n" + comboNum;
        if(comboNum == 1) {
            ui_combo.transform.DOScale(Vector3.zero, 0.25f);
        }
        else if(comboNum == 2) {
            ui_combo.transform.DOScale(Vector3.one, 0.25f);
        }
    }


}
