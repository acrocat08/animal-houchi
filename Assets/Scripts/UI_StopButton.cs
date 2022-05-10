using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StopButton : MonoBehaviour {

    public Sprite sprite_playing, sprite_stopping;
    public Image image;

    public BlackHole hole;

    public void ChangeState() {
        ObjectMaker.instance.isPlaying = !ObjectMaker.instance.isPlaying;
        image.sprite = ObjectMaker.instance.isPlaying ? sprite_playing : sprite_stopping;
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_blackhole, 0, 1);
        hole.SetActive();
    }  

}
