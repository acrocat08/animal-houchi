using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

    bool isActive;
    float speed;

    public void Start() {
        isActive = true;
        speed = 0;
    }

    void Update() {
        transform.Rotate(-Vector3.forward * speed);
    }

    public void SetActive() {
        isActive = !isActive;
        ChangeState(isActive);
    }

    public void ChangeState(bool state){
        speed = state ? 1 : 0;
    }

}
