using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScroller : MonoBehaviour
{
    public Transform cameraPos;

    void Start()
    {
        
    }

    void Update() {
        var vecRaw = Input.GetAxis("Mouse ScrollWheel");
        if(vecRaw == 0) return;
        var vec = vecRaw / Mathf.Abs(vecRaw);
        var afterPos = cameraPos.localPosition + vec * Vector3.up;
        afterPos.y = Mathf.Min(Mathf.Max(afterPos.y, 0), 10);
        cameraPos.localPosition = afterPos;
    }
}
