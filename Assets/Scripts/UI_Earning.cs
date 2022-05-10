using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_Earning : MonoBehaviour {

    public float endPosY;
    public Text comboText;
    public int combo;

    void Start() {
        var per = UI_NewItem.instance.per;
        GetComponent<Text>().fontSize = (int)(GetComponent<Text>().fontSize * per);
        comboText.fontSize = (int)(comboText.fontSize * per);
        endPosY *= per;
        var icon = transform.GetChild(0).GetComponent<Image>();
        icon.transform.localScale = Vector3.one * (GetComponent<Text>().fontSize / 100f);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(transform.position + new Vector3(0, endPosY, 0), 0.5f)
            .SetEase(Ease.OutCirc));
        Text text = GetComponent<Text>();
        seq.Append(DOTween.ToAlpha(
        () => icon.color,
        color => icon.color = color, 0f, 0.5f));
        seq.Join(DOTween.ToAlpha(
        () => text.color,
        color => text.color = color, 0f, 0.5f)
        .OnComplete(() => Destroy(gameObject)));
        if(combo > 1) {
            comboText.text = combo + " combo !!";
            seq.Join(DOTween.ToAlpha(
            () => comboText.color,
            color => comboText.color = color, 0f, 0.5f));
        }
        else {
            comboText.text = "";
        }
    }


}
