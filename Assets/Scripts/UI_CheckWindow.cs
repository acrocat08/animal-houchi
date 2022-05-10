using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UI_CheckWindow : MonoBehaviour {

    public static UI_CheckWindow instance;
    public GameObject UI_background, UI_window;
    public Button button_yes, button_no;

    public Text ui_question, ui_value, ui_lank;

    void Awake() {
        instance = this;
        UI_background.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        UI_background.transform.localScale = Vector3.zero;
        UI_window.transform.localScale = Vector3.zero;
    }

    public void Open(bool canSelect, string question, Money money) {
        ObjectMaker.instance.isMainWindow = false;
        var img = UI_background.GetComponent<Image>();
        img.transform.localScale = Vector3.one;
        DOTween.ToAlpha(
            () => img.color,
            color => img.color = color,
            0.5f,
            0.5f);
        UI_window.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        button_yes.enabled = canSelect;
        button_yes.GetComponent<Image>().color
            = canSelect ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
        ui_question.text = question;
        ui_value.text = money.GetValueString();
        ui_lank.text = money.GetLankString();
    }

    public void Close() {
        ObjectMaker.instance.isMainWindow = true;
        var img = UI_background.GetComponent<Image>();
        
        DOTween.ToAlpha(
            () => img.color,
            color => img.color = color,
            0f,
            0.5f)
        .OnComplete(() => img.transform.localScale = Vector3.zero);
        UI_window.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuart);
    }

    public void OnYes(UnityAction func) {
        button_yes.onClick.RemoveAllListeners();
        button_yes.onClick.AddListener(func);
    }

    public void OnNo(UnityAction func) {
        button_no.onClick.RemoveAllListeners();
        button_no.onClick.AddListener(func);
    }


}
