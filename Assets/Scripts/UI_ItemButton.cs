using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemButton : MonoBehaviour {

    void Update() {
        if(Input.GetMouseButtonDown(0) && !UI_ItemInfo.instance.isOpen) {
            var diff = GetComponent<RectTransform>().position - Input.mousePosition;
            if(new Vector2(diff.x, diff.y).magnitude < 100 * UI_NewItem.instance.per) {
                var num = GetComponentInChildren<Text>().text;
                Debug.Log(num);
                UI_ItemInfo.instance.Open(int.Parse(num) - 1);
            }
        }
    }

}
