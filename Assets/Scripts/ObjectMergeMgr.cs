using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ObjectMergeMgr : MonoBehaviour {

    public int order;
    static int maxID = 0;
    int id;

    bool isMerging = false;

    public static int num;

    public static List<GameObject> all = new List<GameObject>();

    public GameObject earn_text;


    void Start() {
        all.Add(gameObject);
        id = maxID;
        GetComponent<SpriteRenderer>().sortingOrder = id;
        maxID++;
        num++;
        GetComponent<SpriteRenderer>().sprite = ObjectMaker.instance.objectList[order];
        gameObject.AddComponent<PolygonCollider2D>();
        SaveDataManager.instance.AddObjectData(this);
        if(order == 0) UI_NewItem.instance.OnFirstItem();
        StartCoroutine(EarnCoroutine());
    }

    void OnDestroy() {
        num--;
        all.Remove(gameObject);
    }

    public void Appear() {
        var endScale = transform.localScale;
        transform.localScale = new Vector3(0f, 0f, 0f);
        SetRigidBody(this, true);
        transform.DOScale(endScale, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                if(!isMerging) SetRigidBody(this, false);
            });
    }

    void OnCollisionEnter2D(Collision2D col) {
        ObjectMergeMgr mergeMgr = col.gameObject.GetComponent<ObjectMergeMgr>();
        if(mergeMgr == null) return;
        if(id >= mergeMgr.GetID()) return;
        if(order != mergeMgr.order) return;
        if(isMerging && mergeMgr.isMerging) return;
        if(order == ObjectMaker.instance.objectList.Count - 1) return;
        Merge(mergeMgr);
        
    }

    void Merge(ObjectMergeMgr other) {
        isMerging = true;
        other.isMerging = true;
        ObjectMergeMgr[] mgrList = {this, other};
        SaveDataManager.instance.RemoveObjectData(this);
        SaveDataManager.instance.RemoveObjectData(other);
        foreach(var mgr in mgrList){
            mgr.GetComponent<PolygonCollider2D>().enabled = false;
            mgr.GetComponent<ObjectDragMgr>().SetDragActive(false);
            SetRigidBody(mgr, true);
        }
        MoveToMerge(other.transform);
    }

    void MoveToMerge(Transform other) {
        Vector3 midPos = (transform.position + other.position) / 2;
        transform.DOMove(midPos, 0.5f).SetEase(Ease.OutCirc)
            .OnComplete(() => {
                var obj = ObjectMaker.instance.MakeObject(order + 1, midPos);
                UI_NewItem.instance.OnMerged(order + 1);
                var combo = ComboMgr.instance.AddCombo(gameObject, other.gameObject, obj);
                AddComboMoney(combo);
                FeedOut(transform);
            });
        other.DOMove(midPos, 0.5f).SetEase(Ease.OutCirc)
            .OnComplete(() => {
                FeedOut(other);
            });

    }

    void AddComboMoney(int combo) {
        var mgr = MoneyMgr.instance;
        var baseMoney = mgr.CalcObjectEarning(order + 1);
        int per = (int)(Mathf.Pow(10, (Mathf.Min(combo, 10) / 5.0f)) * 60);
        var addingMoney = new Money(baseMoney.value * per, baseMoney.lank).Normalize();
        MakeEarnText(addingMoney, 200, combo);

        Debug.Log("add " + addingMoney.ToString());
        mgr.AddTotalMoney(addingMoney);
    }

    void FeedOut(Transform ts) {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        var sp = ts.GetComponent<SpriteRenderer>();
        sp.sortingOrder = 10;
        DOTween.ToAlpha(
        () => sp.color,
        color => sp.color = color,
        0f,
        0.5f
        ).OnComplete(() => {
            Destroy(ts.gameObject);
        });
        
    }

    void SetRigidBody(ObjectMergeMgr mgr, bool isFreeze) {
        var rb = mgr.GetComponent<Rigidbody2D>();
        rb.isKinematic = isFreeze;
        rb.velocity = new Vector2(0f, 0f);
        rb.angularVelocity = 0f;
        rb.rotation = 0f;
    }

    public int GetID(){
        return id;
    }

    IEnumerator EarnCoroutine() {
        float interval = calcEarnIntarval(order);
        MoneyMgr moneyMgr = MoneyMgr.instance;
        Money actualEarning = calcActualEarning(order);
        yield return new WaitForSeconds(2f + Random.Range(0f, interval));
        while(true) {
            var baseScale = transform.localScale;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(baseScale * 1.1f, 0.1f)
                .OnComplete(() => {
                    moneyMgr.AddTotalMoney(actualEarning);
                    SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_earn, 0);
                }));
            seq.Append(transform.DOScale(baseScale, 0.1f));
            MakeEarnText(actualEarning, 50, 0);

            yield return new WaitForSeconds(interval);
        }

    }

    public static float calcEarnIntarval(int order) {
        float per = ((float)(10 - (order % 10)) / 2 + 5) / 10f;
        return 10f * per;
    }

    public static Money calcActualEarning(int order) {
        MoneyMgr moneyMgr = MoneyMgr.instance;
        Money secEarning = moneyMgr.CalcObjectEarning(order);
        return new Money(secEarning.value * calcEarnIntarval(order), secEarning.lank);
    }

    void MakeEarnText(Money money, float endPosY, int combo) {
        var obj = Instantiate(earn_text);
        obj.GetComponent<UI_Earning>().endPosY = endPosY;
        obj.GetComponent<UI_Earning>().combo = combo;
        obj.transform.parent = GameObject.Find("earnings").transform;
        obj.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        obj.GetComponent<Text>().text = money.ToString();
        obj.GetComponent<Text>().fontSize = GetTextSize(money);
    }

    int GetTextSize(Money money) {
        int baseLevel = Mathf.Max(0, ObjectMaker.instance.level - 9);
        var mgr = MoneyMgr.instance;
        var baseValue = calcActualEarning(baseLevel).Log();
        var delta = money.Log() - baseValue;
        Debug.Log("get : " + order + " : " + delta);
        return (int)(25 * delta + 50);
    }

}
