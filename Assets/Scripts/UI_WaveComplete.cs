using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_WaveComplete : MonoBehaviour  {

    public int wave;
    public List<UI_BackItem> backItems;

    float imageSize = 200;


    public Text waveText;
    public Image emblemItem;
    public Text emblemText;

    public Transform emblemTransform;
    void Awake() {
        wave = Random.Range(1, 11);
        backItems.ForEach(b => b.wave = wave);
        waveText.text = wave.ToString();
        emblemText.text = (wave * 100).ToString();
    }

    void Start() {
        emblemItem.sprite = ObjectMaker.instance.objectList[wave * 100 - 1];
        var spriteSize = emblemItem.sprite.bounds.size;
        var per = imageSize / Mathf.Max(spriteSize.x, spriteSize.y);
        var rect = emblemItem.GetComponent<RectTransform>();
        rect.sizeDelta = spriteSize * per;

        float prevPosY = emblemTransform.localPosition.y;
        emblemTransform.localPosition += Vector3.up * 100;
        
        var group = emblemTransform.GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.DOFade(1, 1);
        emblemTransform.DOLocalMoveY(prevPosY, 1).SetEase(Ease.OutBounce);
    }

}
