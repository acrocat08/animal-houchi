using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UniRx;
using UniRx.Triggers;

public class ObjectDragMgr : MonoBehaviour {
    bool canMove = true;
    ReactiveProperty<bool> isDragging = new ReactiveProperty<bool>(false);
    Vector3 offset = new Vector3();


    void Start() {

        this.UpdateAsObservable()
            .Where(_ => isDragging.Value == true)
            .Where(_ => canMove == true)
            .Subscribe(_ => {
                AttachToMouse();
            });

        this.UpdateAsObservable()
            .Select(_ => transform.position)
            .Where(wPos => IsOutOfScreen(wPos))
            .Subscribe(_ => GetCapsuleCount());

    }

    bool IsOutOfScreen(Vector2 wPos) {
        if(Mathf.Abs(wPos.x) > 9f) return true;
        if(wPos.y < -5f) return true;
        return false;
    }

    public void OnPointerDown(int pos) {
        if(pos == 0) {
            isDragging.Value = true;
            offset = CalcOffset();
            //SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_grab, 0);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if(pos == 1) {
            if(GetComponent<ObjectMergeMgr>().order > ObjectMaker.instance.level) return;
            GetCapsuleCount();
        }
    }

    void GetCapsuleCount() {
        if(GetComponent<ObjectMergeMgr>().order > ObjectMaker.instance.level) {
            ObjectMaker.instance.MakeObject(GetComponent<ObjectMergeMgr>().order,
                ObjectMaker.instance.spawnPos + Vector3.up);
            SaveDataManager.instance.RemoveObjectData(GetComponent<ObjectMergeMgr>());
        }
        else {
            SaveDataManager.instance.RemoveObjectData(GetComponent<ObjectMergeMgr>());
            MoneyMgr.instance.ModifyEarning(MoneyMgr.instance.CalcObjectEarning(GetComponent<ObjectMergeMgr>().order), false);
            CapsuleMaker.instance.AddCount();
        }
        Destroy(gameObject);
     }

    public void OnPointerUp() {
        isDragging.Value = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    void AttachToMouse() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseWorldPos + offset;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
    }

    Vector3 CalcOffset() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return transform.position - mouseWorldPos;
    }

    public void SetDragActive(bool mode) {
        canMove = mode;
    }

}
