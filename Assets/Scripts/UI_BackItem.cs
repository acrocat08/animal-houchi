using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BackItem : MonoBehaviour {

    float speed = 3f;
    float imageSize = 150;
    public int wave;


    void Start() {
        ChangeImage();
    }

    void Update() {
        transform.localPosition += Vector3.left * speed;
        if(transform.localPosition.x <= -925) {
            ChangeImage();
            transform.localPosition += Vector3.right * 1800;
        }
    }

    void ChangeImage() {
        int index = Random.Range(0, 100);
        GetComponent<Image>().sprite = ObjectMaker.instance.objectList[(wave - 1) * 100 + index];
        var spriteSize = GetComponent<Image>().sprite.bounds.size;
        var per = imageSize / Mathf.Max(spriteSize.x, spriteSize.y);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = spriteSize * per;
    }

}
