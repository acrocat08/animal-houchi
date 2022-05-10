using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MousePointer : MonoBehaviour
{
    ObjectDragMgr now = null;


    // Update is called once per frame
    void Update() {
        int n = -1;
        if(Input.GetMouseButtonDown(0)) n = 0;
        if(Input.GetMouseButtonDown(1)) n = 1;
        if(Input.GetMouseButtonUp(0)) n = 2;

        if(n != -1 && n != 2) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.RaycastAll (ray.origin, ray.direction, Mathf.Infinity);
            if(ObjectMaker.instance.isMainWindow) {
                var obj = hit.ToList()
                    .Where(h => h.transform.tag == "Object")
                    .Select(h => h.transform.GetComponent<ObjectDragMgr>());
                foreach (var o in obj) {
                    o.OnPointerDown(n);
                    now = o;
                    return;
                }
                var cap = hit.ToList()
                    .Where(h => h.transform.tag == "Capsule")
                    .Select(h => h.transform.GetComponent<Capsule>());
                foreach (var c in cap) {
                    if(n == 0) c.OnPointerDown();
                    return;
                }
            }
            else {
                var ui = hit.ToList();              
                foreach (var u in ui) {
                    Debug.Log(u.transform.gameObject.name);
                }
            }

        }
        if(n == 2) {
            if(now != null) {
                now.OnPointerUp();
                now = null;
            }
        }
        
    }
}
