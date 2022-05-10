using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public enum SoundEffectType {
    se_appear,
    se_appear_new,
    se_levelup,
    se_earn,
    se_delete,
    se_new_item,
    se_item_list_open,
    se_item_list_close,
    se_trash,
    se_ready,
    se_open_capsule,
    se_appear_capsule,
    se_push,
    se_blackhole
}

public enum BackGroundMusicType {
    bgm_main,
}


public class SoundPlayer : MonoBehaviour {

    public Sprite on, off;

    [SerializeField]
    SoundTable soundTable;

    [SerializeField]
    MusicTable musicTable;

    public static SoundPlayer instance;

    AudioSource[] sources;

    void Start() {
        instance = this;
        sources = GetComponents<AudioSource>();
    }

    public void PlaySoundEffect(SoundEffectType type, int channel, float vol = 0.85f) {
        var free = sources.Where(s => !s.isPlaying).FirstOrDefault();
        if(free == null) return;
        free.clip = soundTable.GetTable()[type];
        free.volume = vol;
        free.Play();
    }

    public void PlayBackGroundMusic(BackGroundMusicType type) {
        AudioSource source = sources.Last();
        source.clip = musicTable.GetTable()[type];
        source.Play();
    }

    [System.Serializable]
    public class SoundTable : Serialize.TableBase<SoundEffectType, AudioClip, SoundPair>{
    }


    [System.Serializable]
    public class SoundPair : Serialize.KeyAndValue<SoundEffectType, AudioClip>{
        public SoundPair (SoundEffectType key, AudioClip value) : base (key, value) {
        }
    }

    [System.Serializable]
    public class MusicTable : Serialize.TableBase<BackGroundMusicType, AudioClip, MusicPair>{
    }


    [System.Serializable]
    public class MusicPair : Serialize.KeyAndValue<BackGroundMusicType, AudioClip>{
        public MusicPair (BackGroundMusicType key, AudioClip value) : base (key, value) {
        }
    }

    public void ChangeMuteMode(Image image) {
        foreach(var i in sources) {
            i.mute = !i.mute;
        }
        image.sprite = sources[0].mute ? off : on;
    }



}


