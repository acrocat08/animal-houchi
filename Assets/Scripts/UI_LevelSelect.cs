using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UI_LevelSelect : MonoBehaviour {

    LinkedList<GameObject> images = new LinkedList<GameObject>();

    public float posY;

    public float radius;

    public float degreeInterval;

    public float maxScale;
    public float minScale;

    public float imageSize;

    int nowLevel;

    public GameObject window;

    bool isOpen = false;

    public Button button_openList;

    public UI_ItemScrollBar scrollBar;

    bool isMoving;

    public GameObject levelSelect;

    void Start() {
        isMoving = false;
        nowLevel = ObjectMaker.instance.level;
        for(int i = 0; i < transform.childCount; i++) {
            images.AddLast(transform.GetChild(i).gameObject);
            transform.GetChild(i).position = new Vector2(Screen.width / 2, posY);
        }
        window.transform.localScale = Vector3.zero;
        SetImage();

        Observable.Timer(System.TimeSpan.FromMilliseconds(10))
            .Subscribe(_ => {
                if(GetMaxOrder() < 9) button_openList.interactable = false;
            });
            var mat = levelSelect.GetComponent<Image>().material;
        var per = UI_NewItem.instance.per;
        mat.SetVector("_Center", 
            new Vector4(800 * per, 1600 * per, 0, 0));
        mat.SetFloat("_RadiusMin", 1100 * per);
        mat.SetFloat("_RadiusMax", 1300 * per);
        mat.SetFloat("_SubRadiusMin", 925 * per);
        mat.SetFloat("_SubRadiusMax", 950 * per);
    }

    public void Open() {
        if(isMoving) return;
        if(GetMaxOrder() < 9) return;
        isMoving = true;
        ObjectMaker.instance.isMainWindow = false;
        nowLevel = ObjectMaker.instance.level <= 9 ? 9 : ObjectMaker.instance.level;
        ObjectStopper.instance.StopAll();
        foreach(var i in images) i.SetActive(true);
        var barImage = scrollBar.gameObject.GetComponent<Image>();
        window.transform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {
                SetImage();
                MoveImage();
                scrollBar.transform.localScale = Vector3.one;
                scrollBar.SetPos(nowLevel);
                isOpen = true;
            });
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_item_list_open, 0);
    }

    public void Close() {
        if(isMoving) return;
        isMoving = true;
        ObjectMaker.instance.isMainWindow = true;
        ObjectStopper.instance.ReleaseAll();
        window.transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {
                isOpen = false;
                foreach(var i in images) i.SetActive(false);
                scrollBar.transform.localScale = Vector3.zero;
                isMoving = false;
            });
        foreach(var image in images) {
            image.transform.position = new Vector2(Screen.width / 2, posY);
            image.transform.localScale = Vector3.zero;
        }
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_item_list_close, 0);
    }


    void SetImage() {
        int i = 0;
        var objList = ObjectMaker.instance.objectList;
        var level = ObjectMaker.instance.level;
        foreach(var image in images) {
            int index = nowLevel + 2 - i;
            var renderer = image.GetComponent<Image>();
            if(index >= 0 && index < objList.Count) {
                renderer.sprite = objList[index];
                var isSide = i == 0 || i == 13;
                var isOut = i <= 1 || i >= 12;
                var isInLevel = index <= level && index >= level - 9;
                Color nextColor = Color.white;
                if(isOut || isSide) {
                    nextColor = new Color(1, 1, 1, 0);
                    image.transform.GetChild(0).localScale = Vector3.zero;
                }
                else if(!isInLevel) {
                    nextColor = new Color(1, 1, 1, 0.3f);
                    image.GetComponent<Shadow>().enabled = false;
                    image.transform.GetChild(0).localScale = Vector3.one;
                    image.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
                }
                else {
                    nextColor = new Color(1, 1, 1, 1);
                    image.GetComponent<Shadow>().enabled = true;
                    image.transform.GetChild(0).localScale = Vector3.one;
                    image.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 1);
                }
                DOTween.To(
                    () => renderer.color,          
                    color => renderer.color = color,  
                    nextColor,                    
                    0.25f                                 
                );
                SetSize(renderer);
            }
            else {
                renderer.color = new Color(1, 1, 1, 0);
            }
            image.GetComponentInChildren<Text>().text = (index + 1).ToString();
            i++;
        }
    }

    void SetSize(Image image) {
        var spriteSize = image.sprite.bounds.size;
        var per = imageSize / Mathf.Max(spriteSize.x, spriteSize.y);
        var rect = image.GetComponent<RectTransform>();
        rect.sizeDelta = spriteSize * per;
    }

    public void ChangeLevel(bool isForward) {
        int dir = isForward ? 1 : -1;
        if(isForward && nowLevel >= GetMaxOrder()) return;
        if(!isForward && nowLevel == 9) return;
        if(!isForward) {
            var tmp = images.First;
            images.RemoveFirst();
            images.AddLast(tmp);
        }
        else {
            var tmp = images.Last;
            images.RemoveLast();
            images.AddFirst(tmp);
        }
        nowLevel += dir;
        SetImage();
        MoveImage();
        scrollBar.SetPos(nowLevel);
    }

    void MoveImage() {
        int i = 0;
        foreach(var image in images) {
            image.transform.DOMove(CalcImagePos(i), 0.25f);
            float scale = Mathf.Lerp(maxScale, minScale, (Mathf.Abs(6.5f - i) - 0.5f) / 4);
            image.transform.DOScale(new Vector3(scale, scale, scale), 0.25f);
            i++;
        }
        Observable.Timer(System.TimeSpan.FromSeconds(0.3f))
            .Subscribe(_ => isMoving = false);
    }

    Vector2 CalcImagePos(int index) {
        Vector2 vec = Vector2.up * radius;
        float degree = degreeInterval * (index - 6.5f);
        Vector2 tiltedVec = Quaternion.Euler(0, 0, degree) * vec;
        var per = UI_NewItem.instance.per;
        return new Vector2(Screen.width / 2, posY * per) + tiltedVec * per;
    }

    int GetMaxOrder() {
        return Mathf.Max(ObjectMaker.instance.level, UI_NewItem.instance.maxObjOrder);
    }

    public void SetLevelDirectly(int level) {
        nowLevel = level;
        SetImage();
    }


}
