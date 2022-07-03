using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Linq;

public class ObjectMaker : MonoBehaviour {

    [SerializeField]
    GameObject p_object;

    public List<Sprite> objectList;
    public List<string> objectNameList;
    [Multiline(2)]
    public List<string> objectInfoList;

    public Vector3 spawnPos;
    public int maxNum;

    public int level = 0;

    Money necessaryMoneyForLevelUp;

    public float makeIntervalSec;

    public bool canMake = false;

    public int waitTimerInterval;
    public int dropNum;
    public int waitTimer;
    public int dropCount;
    bool isFinishedTimer = false;
    bool canLevelUp = false;

    public bool isMainWindow = true;

    public bool isPlaying;
    public GameObject stopButton;


    public Text ui_level;
    public GameObject ui_timer;
    public Button button_levelUp;

    public static ObjectMaker instance;

    public BlackHole hole;

    public bool isMainMode;

    void Awake() {
        isPlaying = true;
        instance = this;
        //level = Random.Range(9, objectList.Count);
        if(!isMainMode) return;
        StartCoroutine(WaitTimerCoroutine());
        //if (Application.platform == RuntimePlatform.WindowsPlayer ||
            //Application.platform == RuntimePlatform.OSXPlayer ||
            //Application.platform == RuntimePlatform.LinuxPlayer ) {
            Screen.SetResolution(1600, 900, false);
        //}
    }

    void Start() {
        if(!isMainMode) return;
        Observable.NextFrame().Subscribe(_ =>  {
            necessaryMoneyForLevelUp = MoneyMgr.instance.GetNecessaryMoneyForLevelUp(level);
            ui_level.text = (level + 1).ToString();
            if(level == objectList.Count - 1) button_levelUp.transform.localScale = Vector3.zero;
            SoundPlayer.instance.PlayBackGroundMusic(BackGroundMusicType.bgm_main);
        });
 
        this.UpdateAsObservable()
            .Where(_ => MoneyMgr.instance.GetTotalMoney().IsMoreThan(necessaryMoneyForLevelUp))
            .Where(_ => !canLevelUp)
            .Subscribe(_ => {
                canLevelUp = true;
                StartCoroutine(LevelUpButtonCoroutine());
            });

    }

    void Update() {
        //MakeObjectFromKey();
    }

    void MakeObjectFromKey() {
        if(Input.anyKeyDown) {
            string keyStr = Input.inputString;
            int offset = int.Parse(keyStr);
            if(offset == 0) offset = 10;
            int order = Mathf.Min(Mathf.Max(level + offset - 10, 0), objectList.Count);
            MakeObject(order, spawnPos);
        }
    }

    IEnumerator WaitTimerCoroutine() {
        yield return new WaitForSeconds(0.01f);
        var timerText = ui_timer.GetComponentInChildren<Text>();
        timerText.text 
            = string.Format("{0:00} : {1:00}", waitTimer / 60, waitTimer % 60);
        ui_timer.transform.localScale = Vector3.one * 0.8f;
        while(true) {
            waitTimer--;
            if(waitTimer <= 0) {
                StartCoroutine(DropObjectCoroutine());
                ui_timer.transform.localScale = Vector3.zero;
                break;
            }
            else {
                timerText.text 
                    = string.Format("{0:00} : {1:00}", waitTimer / 60, waitTimer % 60);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DropObjectCoroutine() {
        float interval = makeIntervalSec * 0.5f;
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_ready, 1);
        stopButton.SetActive(true);
        hole.ChangeState(true);
        
        while(dropCount < dropNum) {
            yield return null;
            if(!canMake) continue;
            if(isPlaying) interval += Time.deltaTime;
            if(interval >= makeIntervalSec) {
                interval -= makeIntervalSec;
                MakeRandomObjectOnSky();
                dropCount++;
            }
        }
        waitTimer = waitTimerInterval;
        dropCount = 0;
        stopButton.SetActive(false);
        hole.ChangeState(false);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(WaitTimerCoroutine());
    }

    IEnumerator LevelUpButtonCoroutine() {
        button_levelUp.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        float startTime = Time.time;
        var originalPos = button_levelUp.transform.localPosition;
        while(true) {
            yield return null;
            if(!canLevelUp) {
                button_levelUp.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                button_levelUp.transform.localPosition = originalPos;
                yield break;
            }
            button_levelUp.transform.localPosition = originalPos 
                + Vector3.up * Mathf.Sin((Time.time - startTime) * 5) * 15f;
        }
    }

    public GameObject MakeObject(int order, Vector3 pos) {
        var obj = Instantiate(p_object, pos, Quaternion.identity);
        var mgr =  obj.GetComponent<ObjectMergeMgr>();
        mgr.Appear();
        mgr.order = order;
        return obj;
    }  

    void MakeRandomObjectOnSky() {
        if(ObjectMergeMgr.num >= maxNum) return;
        int order = Mathf.Max(level - 9, 0) + GetRondomObjectOrder();
        //Vector3 offset = Random.value * 5 *  count * Vector3.right;
        //count *= -1;
        MakeObject(order, spawnPos);
        Observable.Timer(System.TimeSpan.FromSeconds(1.5f))
            .Subscribe(_ => {
                MoneyMgr.instance.ModifyEarning(MoneyMgr.instance.CalcObjectEarning(order), true);
            });
    }

    public void OnLevelUpSelected() {
        var window = UI_CheckWindow.instance; 
        window.Open(canLevelUp, $"「{objectNameList[level + 1]}」\nを仕入れますか？", necessaryMoneyForLevelUp, objectList[level + 1]);
        window.OnYes(() => {
            MoneyMgr.instance.UseMoney(necessaryMoneyForLevelUp);
            LevelUp();
            window.Close();
        });
        window.OnNo(window.Close);
    }

    void LevelUp() {
        level = Mathf.Min(level + 1, objectList.Count - 1);
        ui_level.text = (level + 1).ToString();
        necessaryMoneyForLevelUp = MoneyMgr.instance.GetNecessaryMoneyForLevelUp(level);
        SoundPlayer.instance.PlaySoundEffect(SoundEffectType.se_levelup, 1);
        canLevelUp = false;
        if(level == objectList.Count - 1) button_levelUp.transform.localScale = Vector3.zero;
    }    

    int GetRondomObjectOrder() {
        if(level == 0) return 0;
        float valueX = Random.value;
        float valueY = Random.value;
        float gaussianValue = Mathf.Sqrt(-2.0f * Mathf.Log(valueX)) * Mathf.Cos(2.0f * Mathf.PI * valueY);
        
        int x = Mathf.Min(level, 9);
        for(int i = 0; i <= x; i++) {
            float border = (4f / (x + 1)) * (i  + 1) - 2f;
            if(gaussianValue < border) return i;
        }    
        return x;
    }




}
