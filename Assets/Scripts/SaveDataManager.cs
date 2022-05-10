using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;

public class SaveDataManager : MonoBehaviour {

    public static SaveDataManager instance;
    List<ObjectMergeMgr> objectList = new List<ObjectMergeMgr>();

    [SerializeField]
    GameObject p_obj;

    public bool isSaveMode, isLoadMode;

    const String s_data = "an_data";
    const String s_level = "an_level";
    const String s_moneyvalue = "an_MoneyValue";
    const String s_moneylank = "an_MoneyLank";
    const String s_count = "an_count";
    const String s_maxorder = "an_maxOrder";
    const String s_dropcount = "an_dropCount";
    const String s_waittimer = "an_waitTimer";
    const String s_lasttime = "an_lastTime";

    void Awake() {
        instance = this;   
        if(isLoadMode) {
            Observable.NextFrame().Subscribe(_ =>  Load());
        } 
        /*
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => Reset());
        */
    }

    private void OnApplicationQuit() {
        if(isSaveMode) Save();
    }
    
    private void OnApplicationPause (bool pauseStatus){
        if(pauseStatus){
            UnityEngine.Application.Quit();
        }
    }

    public void AddObjectData(ObjectMergeMgr mgr) {
        objectList.Add(mgr);
    }

    public void RemoveObjectData(ObjectMergeMgr mgr) {
        objectList.Remove(mgr);
        objectList = objectList.Where(x => x != null).ToList();
    }

    void Save() {
        var saveData = objectList
            .Select(x => new ObjectData(x.order, x.transform.position, x.transform.rotation))
            .ToList();
        //saveData = new List<ObjectData>();
        string serialized = Serialize(saveData);
        Debug.Log(serialized);
        PlayerPrefs.SetString(s_data, serialized);
        PlayerPrefs.SetInt(s_level, ObjectMaker.instance.level);
        PlayerPrefs.SetFloat(s_moneyvalue, MoneyMgr.instance.GetTotalMoney().value);
        PlayerPrefs.SetInt(s_moneylank, MoneyMgr.instance.GetTotalMoney().lank);
        PlayerPrefs.SetInt(s_count, CapsuleMaker.instance.count);
        PlayerPrefs.SetInt(s_maxorder, UI_NewItem.instance.maxObjOrder);
        PlayerPrefs.SetInt(s_dropcount, ObjectMaker.instance.dropCount);
        Pause();
    }

    void Pause() {
        PlayerPrefs.SetInt(s_waittimer, ObjectMaker.instance.waitTimer);
        PlayerPrefs.SetString(s_lasttime, System.DateTime.Now.ToString());
    }

    void Load() {
        string data = PlayerPrefs.GetString(s_data);
        if(data != "") {
            var saveData = Deserialize(data);
            foreach(var objData in saveData) {
                var mgr = Instantiate(p_obj, objData.position, objData.rotation)
                    .GetComponent<ObjectMergeMgr>();
                mgr.order = objData.order;
                MoneyMgr.instance.ModifyEarning(MoneyMgr.instance.CalcObjectEarning(objData.order), true);
            }
        }
        ObjectMaker.instance.level = PlayerPrefs.GetInt(s_level, 0);
        MoneyMgr.instance.SetMoney(PlayerPrefs.GetFloat(s_moneyvalue, 0), PlayerPrefs.GetInt(s_moneylank, 0));
        CapsuleMaker.instance.count = PlayerPrefs.GetInt(s_count, 0);
        UI_NewItem.instance.maxObjOrder = PlayerPrefs.GetInt(s_maxorder, 0);
        ObjectMaker.instance.dropCount = PlayerPrefs.GetInt(s_dropcount, 0);

        Restart();
    }


    void Restart() {
        ObjectMaker.instance.waitTimer = PlayerPrefs.GetInt(s_waittimer, 0);
        var lastTime = PlayerPrefs.GetString(s_lasttime);
        if(lastTime != "") {
            System.TimeSpan delta = System.DateTime.Now - System.DateTime.Parse(lastTime);
            int sleepTime = (int)delta.TotalSeconds;
            MoneyMgr.instance.AddSleepTime(sleepTime);
            ObjectMaker.instance.waitTimer -= sleepTime;
        }
    }

    static string Serialize(List<ObjectData> objects){
        var serializedList = objects.Select(x => JsonUtility.ToJson(x));
        return String.Join(";", serializedList);
    }

    static List<ObjectData> Deserialize(string json){
        var serializedList = json.Split(new char[]{ ';' }).ToList();
        return serializedList.Select(x => JsonUtility.FromJson<ObjectData>(x)).ToList();
    }

    [Serializable]
    public class ObjectData {
        public int order;
        public Vector3 position;
        public Quaternion rotation;

        public ObjectData(int order, Vector3 position, Quaternion rotation){
            this.order = order;
            this.position = position;
            this.rotation = rotation;
        }
    }

    void Reset() {
        Debug.Log("reset");
        ObjectMaker.instance.level = 0;
        MoneyMgr.instance.SetMoney(0, 0);
        CapsuleMaker.instance.count = 0;
        UI_NewItem.instance.maxObjOrder = -1;
        ObjectMaker.instance.waitTimer = 0;
        ObjectMaker.instance.dropCount = 0;
        objectList.ForEach(x => Destroy(x.gameObject));
        objectList = new List<ObjectMergeMgr>();
        Save();
        SceneManager.LoadScene("main");
    }
}
