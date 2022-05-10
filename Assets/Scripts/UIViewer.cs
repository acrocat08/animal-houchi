using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UIViewer : MonoBehaviour
{
    bool isActive;
    Vector3 prevMousePos;

    // Start is called before the first frame update
    void Start() {
        var click = this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(1));   
        isActive = true;
        var dClick = click
            .Select(_ => false)
            .Take(1)
            .Do(_ => prevMousePos = Input.mousePosition)
            .Concat(click.Select(_ => true)
                .Where(_ => (Input.mousePosition - prevMousePos).magnitude < 10)
                .Take(System.TimeSpan.FromSeconds(0.2f)).Take(1))
            .RepeatSafe(); 
        dClick.Subscribe(b => {
            if(b) ChangeState();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2)) {
            ChangeState();
        }
    }

    void ChangeState() {
        transform.localScale = isActive ? Vector3.zero : Vector3.one;
        isActive = !isActive;
    }
}
