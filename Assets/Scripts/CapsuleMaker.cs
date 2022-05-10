using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class CapsuleMaker : MonoBehaviour {
    
    public List<Sprite> images_close, images_open;
    public Vector3 spawnPos;
    public GameObject prefab_capsule;
    public static CapsuleMaker instance;

    public Image ui_count;
    public List<Sprite> sp_count;
    public int maxNum;

    public int count = 0;
    bool isMaking = false;

    Vector3 initialPos;

    [Range(0, 1)]
    public float feverPr;

    void Start() {
        instance = this;
        Observable.Timer(System.TimeSpan.FromMilliseconds(10))
            .Subscribe(_ => {
                ui_count.sprite = sp_count[count];
            });
        //CreateManyCapsule();
        //StartCoroutine(PlayManySound());
        initialPos = ui_count.transform.localPosition;
    }

    public void AddCount() {
        count++;
        if(!isMaking) ui_count.sprite = sp_count[count];
        if(count == 5) {
            int n;
            if(Random.value <= feverPr) {
                CreateManyCapsule();
                StartCoroutine(PlayManySound());
                n = 10;
            }
            else {
                CreateCapsule(spawnPos);
                SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_appear_capsule, 0);
                n = 1;
            }
            count = 0;
            isMaking = true;
            ui_count.transform.DOLocalJump(initialPos, 50, n, 0.5f)
                .OnComplete(() => {
                    ui_count.sprite = sp_count[count];
                    isMaking = false;
                });
        }
        else {
            SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_trash, 0);
        }
    }

    IEnumerator PlayManySound() {
        for(int i = 0; i < 5; i ++) {
            SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_appear_capsule, 0);
            yield return new WaitForSeconds(0.15f);
        }
    }


    void CreateCapsule(Vector3 pos) {
        var obj = Instantiate(prefab_capsule, pos, Quaternion.identity).GetComponent<Capsule>();
        int index = Random.Range(0, images_close.Count);
        obj.Init(images_close[index], images_open[index], obj.Action_SimpleItem);
    }

    void CreateManyCapsule() {
        var num = Mathf.Min(maxNum, ObjectMaker.instance.maxNum - ObjectMergeMgr.num);
        for(int i = 0; i < maxNum; i++) {
            Vector3 pos = spawnPos + (i / 5) * Vector3.up + (i % 5 - 2) * Vector3.right; 
            CreateCapsule(pos);
        }
    }

}
