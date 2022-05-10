using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System;

public class UI_ItemInfo : MonoBehaviour
{
    public static UI_ItemInfo instance;
    public UI_LevelSelect levelSelect;
    public UI_ItemScrollBar scrollBar;

    public GameObject window;
    public Image newItemImage;

    public float imageSize;
    public Text objName;
    public Text objNum;
    public Text objMoney;
    public Text objInterval;

    public Text objInfo;

    int nowOrder;
    public bool isOpen;

    void Start() {
        instance = this;
    }

    public void Open(int order) {
        if(order > UI_NewItem.instance.maxObjOrder) return;
        isOpen = true;
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_push, 0);
        window.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic);
        UpdateInfo(order);
        Observable.Timer(System.TimeSpan.FromSeconds(0.25f))
            .Subscribe(_ => {
                ItemAppear(order);
            });
        levelSelect.SetLevelDirectly(Mathf.Max(9, order));
        scrollBar.SetPos(Mathf.Max(9, order));
        var per = UI_NewItem.instance.per;
        var mat = GetComponent<Image>().material;
        mat.SetVector("_Center", 
            new Vector4(800 * per, 300 * per, 0, 0));
        mat.SetFloat("_Dist", 500 * per);

    }

    public void OnNext(int dir) {
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_push, 0);
        int nextOrder = nowOrder + dir;
        if(nextOrder < 0 || nextOrder > UI_NewItem.instance.maxObjOrder) return;
        UpdateInfo(nextOrder);
        ItemAppear(nextOrder);
        levelSelect.SetLevelDirectly(Mathf.Max(9, nextOrder));
        scrollBar.SetPos(Mathf.Max(9, nextOrder));
    }

    void UpdateInfo(int order) {
        Debug.Log(order);
        objNum.text = (order + 1).ToString();
        objName.text = ObjectMaker.instance.objectNameList[order];
        objMoney.text = ObjectMergeMgr.calcActualEarning(order).ToString();
        objInterval.text = String.Format("{0:#.#} sec", ObjectMergeMgr.calcEarnIntarval(order));
        objInfo.text = ObjectMaker.instance.objectInfoList[order];
        nowOrder = order;
    }

    void ItemAppear(int order) {
        newItemImage.sprite = ObjectMaker.instance.objectList[order];
        var spriteSize = newItemImage.sprite.bounds.size;
        var per = imageSize / Mathf.Max(spriteSize.x, spriteSize.y);
        var rect = newItemImage.GetComponent<RectTransform>();
        rect.sizeDelta = spriteSize * per;
        newItemImage.transform.localScale = Vector3.zero;
        newItemImage.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        


    }
    
    public void OnClose() {
        isOpen = false;
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_item_list_close, 0);
        ObjectMaker.instance.isMainWindow = true;        
        window.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutCubic)
            .OnComplete(() => newItemImage.transform.localScale *= 0);
        Debug.Log("close");
    }   
}
