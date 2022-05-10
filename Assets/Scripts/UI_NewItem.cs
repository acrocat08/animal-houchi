using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class UI_NewItem : MonoBehaviour {
    
    public static UI_NewItem instance;

    [HideInInspector]
    public int maxObjOrder = -1;

    public GameObject window;
    public Image newItemImage;

    public float imageSize;
    public GameObject button;

    public Text objName;
    public Text objNum;

    public Button button_itemList;

    public float per;

    void Awake() {
        per = (float)Screen.width / 1600;
    }

    void Start() {
        instance = this;
        maxObjOrder = ObjectMaker.instance.level;
        var mat = GetComponent<Image>().material;
        mat.SetVector("_Center", 
            new Vector4(800 * per, 450 * per, 0, 0));
        mat.SetFloat("_Dist", 500 * per);
    }

    public void OnMerged(int order) {
        if(order > maxObjOrder) {
            Debug.Log("new");
            maxObjOrder = order;
            Observable.Timer(System.TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => ObjectStopper.instance.StopAll());
            Observable.Timer(System.TimeSpan.FromSeconds(1f))
                .Subscribe(_ => NewItem(order));
            SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_appear_new, 1);
            
        }
        else {
            SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_appear, 0, 0.7f);
        }

    }

    void NewItem(int order) {
        ObjectMaker.instance.isMainWindow = false;
        window.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
        newItemImage.sprite = ObjectMaker.instance.objectList[order];
        var spriteSize = newItemImage.sprite.bounds.size;
        var per = imageSize / Mathf.Max(spriteSize.x, spriteSize.y);
        var rect = newItemImage.GetComponent<RectTransform>();
        rect.sizeDelta = spriteSize * per;
        newItemImage.transform.localScale = Vector3.zero;
        Observable.Timer(System.TimeSpan.FromSeconds(0.75f))
            .Subscribe(_ => {
                newItemImage.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_new_item, 0);
                if(order == 9)button_itemList.interactable = true;
            });
        Observable.Timer(System.TimeSpan.FromSeconds(2f))
            .Subscribe(_ => button.transform.localScale = Vector3.one);
        
        objNum.text = (order + 1).ToString();
        objName.text = ObjectMaker.instance.objectNameList[order];

    }
    
    public void OnClose() {
        ObjectMaker.instance.isMainWindow = true;        
        window.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCubic)
            .OnComplete(() => ObjectStopper.instance.ReleaseAll());
        Debug.Log("close");
        button.transform.localScale = Vector3.zero;
    }   

    public void OnFirstItem() {
        if(maxObjOrder == -1) {
            Observable.Timer(System.TimeSpan.FromSeconds(1.75f))
                .Subscribe(_ => OnMerged(0));
        }
    }
}
