using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemScrollBar : MonoBehaviour, IDragHandler {

    public float radius;
    public float posY;
    public float length;

    public UI_LevelSelect levelSelect;

    public void OnDrag(PointerEventData e) {
        var center = new Vector3(Screen.width / 2, posY, 0);
        var delta = Input.mousePosition - center;
        var angle = Mathf.Atan2(delta.y, delta.x);
        angle = Mathf.Max(Mathf.Min(angle, Mathf.PI * length), Mathf.PI * (1 - length));
        var per = ((angle / Mathf.PI) - length) / (1 - 2 * length);
        var maxLevel = Mathf.Max(ObjectMaker.instance.level, UI_NewItem.instance.maxObjOrder);
        var level = 9 + (int)((maxLevel - 9) * per);
        var fittedAngle = GetAngleFromLevel(level);
        transform.position = center + new Vector3(Mathf.Cos(fittedAngle), Mathf.Sin(fittedAngle)) * radius;
        levelSelect.SetLevelDirectly(level);
        transform.rotation = Quaternion.Euler(0, 0, (fittedAngle / Mathf.PI) * 180 + 90);
    }

    float GetAngleFromLevel(int level) {
        var maxLevel = Mathf.Max(ObjectMaker.instance.level, UI_NewItem.instance.maxObjOrder);
        if(maxLevel == 9) return Mathf.PI * (1 - length);
        var per = (float)(level - 9) / (maxLevel - 9);
        return Mathf.PI * ((1 - 2 * length) * per + length);
    }

    public void SetPos(int level) {
        var center = new Vector3(Screen.width / 2, posY, 0);
        var angle = GetAngleFromLevel(level);
        transform.position = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        transform.rotation = Quaternion.Euler(0, 0, (angle / Mathf.PI) * 180 + 90);
    }

}
