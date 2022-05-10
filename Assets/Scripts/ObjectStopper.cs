using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStopper : MonoBehaviour {

    public bool isStopped = false;

    public static ObjectStopper instance;

    void Start() {
        instance = this;
    }

    public void StopAll() {
        foreach(var obj in ObjectMergeMgr.all) {
            var rb = obj.GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            obj.GetComponent<ObjectDragMgr>().SetDragActive(false);
        }
        ObjectMaker.instance.canMake = false;
        isStopped = true;
    }

    public void ReleaseAll() {
        foreach(var obj in ObjectMergeMgr.all) {
            var rb = obj.GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.None;
            obj.GetComponent<ObjectDragMgr>().SetDragActive(true);
        }
        ObjectMaker.instance.canMake = true;
        isStopped = false;
    }
}
