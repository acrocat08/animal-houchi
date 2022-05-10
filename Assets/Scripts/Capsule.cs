using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using UniRx;


public class Capsule : MonoBehaviour{
    Sprite image_close, image_open;
    Action action;
    
    void Start() {
        
    }

    public void Init(Sprite image_close, Sprite image_open, Action action) {
        this.image_close = image_close;
        this.image_open = image_open;
        this.action = action; 
        GetComponent<SpriteRenderer>().sprite = image_close;
    }

    public void OnPointerDown() {
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_open_capsule, 0);
        Observable.Timer(System.TimeSpan.FromSeconds(0.25f)).Subscribe(_ => action());
        var sp = GetComponent<SpriteRenderer>();
        sp.sprite = image_open;
        transform.rotation = Quaternion.Euler(0, 0, 90) * transform.localRotation;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CircleCollider2D>().enabled = false;
        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        seq.Append(DOTween.ToAlpha(() => sp.color, color => sp.color = color, 0f, 0.5f));
        seq.OnComplete(() => {
            Destroy(gameObject);
        });

    }

    public void Action_SimpleItem() {
        var order = Mathf.Max(0, GetRandomOrder() + ObjectMaker.instance.level - 4);
        ObjectMaker.instance.MakeObject(order, transform.position);
        MoneyMgr.instance.ModifyEarning(MoneyMgr.instance.CalcObjectEarning(order), true);
    }

    int GetRandomOrder() {
        int value = UnityEngine.Random.Range(0, 100);
        if(value < 35) return 0;
        if(value < 65) return 1;
        if(value < 85) return 2;
        if(value < 95) return 3;
        return 4;
    }
}
