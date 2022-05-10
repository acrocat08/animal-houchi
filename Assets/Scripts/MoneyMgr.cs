using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MoneyMgr : MonoBehaviour {

    public static MoneyMgr instance;
    public bool canEarn = true;

    Money totalMoney = new Money(0, 0);
    Money totalEarning = new Money(0, 0);

    public Text ui_moneyValue, ui_moneyLank, ui_earn;

    public int moneyPropToLevel;

    void Awake() {
        instance = this;
        Observable.Timer(System.TimeSpan.FromMilliseconds(10))
            .Subscribe(_ => UpdateText());
        //StartCoroutine(EarnMoneyCoroutine());
    }

    public Money CalcObjectEarning(int order) {
        float log = Mathf.Log10(2) * order;
        int lank = (int)Mathf.Floor(log) / 3;
        float value = Mathf.Pow(10, log % 1 + (int)Mathf.Floor(log) % 3);

        return new Money(value, lank);
    }

    IEnumerator EarnMoneyCoroutine() {
        float timer = 0;
        while(true) {
            yield return null;
            if(!canEarn) continue;
            timer += Time.deltaTime;
            if(timer >= 1f) {
                timer -= 1f;
                if(totalEarning.value != 0 || totalEarning.lank != 0)
                    totalMoney = totalMoney.AddMoney(totalEarning);
                UpdateText();
            }
        }
    }

    public void UseMoney(Money money) {
        Debug.Log(money.ToString());
        totalMoney = totalMoney.SubMoney(money);
        UpdateText();
    }

    public void ModifyEarning(Money money, bool isAdd) {
        if(isAdd) totalEarning = totalEarning.AddMoney(money);
        else totalEarning = totalEarning.SubMoney(money);
        UpdateText();
    }


    void UpdateText() {
        ui_moneyValue.text = totalMoney.GetValueString();
        ui_moneyLank.text = totalMoney.GetLankString();
        ui_earn.text = totalEarning.ToString() + " / sec";
    }

    public Money GetNecessaryMoneyForLevelUp(int level){
        if(level == 0) return new Money(60, 1);
        if(level == 1) return new Money(600, 1);
        if(level == 2) return new Money(6, 2);
        var earn = CalcObjectEarning(level);
        return new Money(earn.value * moneyPropToLevel, earn.lank).Normalize();
    }

    public Money GetTotalMoney() {
        return totalMoney;
    }

    public void AddTotalMoney(Money money) {
        totalMoney = totalMoney.AddMoney(money);
        UpdateText();
    }

    public void SetMoney(float value, int lank) {
        Debug.Log("set " + value + " " + lank);
        totalMoney = new Money(value, lank);
    }

    public void AddSleepTime(int sleepTime) {
        totalMoney = totalMoney.AddMoney(new Money(totalEarning.value * sleepTime, totalEarning.lank).Normalize());
        
    }

}

public struct Money {
    public float value;    //0~1000まで
    public int lank;       //1000で一つ上のlankに上がる（1000A = 1B）

    public Money(float value, int lank) {
        this.value = value;
        this.lank = lank;
        Normalize();
    }

    public Money AddMoney(Money money) {
        if(Mathf.Abs(this.lank - money.lank) >= 3) 
            return this.lank > money.lank ? this : money;
        float resultValue = 0;
        int resultLank = 0;
        float lowValue = 0;
        float highValue = 0;
        if(money.lank <= this.lank) {
            lowValue = Mathf.Round(money.value * 1000);
            highValue = Mathf.Round(this.value * Mathf.Pow(1000, this.lank - money.lank) * 1000);
            resultLank = money.lank - 1;
        }
        else {
            lowValue = Mathf.Round(this.value * 1000);
            highValue = Mathf.Round(money.value * Mathf.Pow(1000, money.lank - this.lank) * 1000);
            resultLank = this.lank - 1;
        }
        resultValue = highValue + lowValue;
        return new Money(resultValue, resultLank).Normalize();
    }

    public Money SubMoney(Money money) {
        if(money.value == 0 && money.lank == 0) return this;
        if(Mathf.Abs(this.lank - money.lank) >= 5) 
            return this.lank > money.lank ? this : money;
        float resultValue = 0;
        int resultLank = 0;
        float lowValue = 0;
        float highValue = 0;
        if(money.lank <= this.lank) {
            lowValue = Mathf.Round(money.value * 1000);
            highValue = Mathf.Round(this.value * Mathf.Pow(1000, this.lank - money.lank) * 1000);
            resultLank = money.lank - 1;
        }
        else {
            Debug.LogError("minus money");
        }
        resultValue = highValue - lowValue;
        Debug.Log(resultValue);
        return new Money(resultValue, resultLank).Normalize();
    }

    public float Log() {
        return Mathf.Log10(value) + lank * 3;
    }

    public bool IsMoreThan(Money money) {
        if(this.lank > money.lank) return true;
        else if(this.lank < money.lank) return false;
        else {
            return this.value > money.value;
        }
    }

    public Money Normalize() {
        while(value >= 1000) {
            value /= 1000;
            lank++;
        }
        while(value > 0 && value < 1 && lank > 0) {
            value *= 1000;
            lank--;
        }
        if(lank < 0) {
            value = 0;
            lank = 0;
        }
        if(lank == 0) value = Mathf.Round(value);
        return this;
    }

    public string GetValueString() {
        if(lank == 0) return ((int)value).ToString();
        else return string.Format("{0:0.000}", value);
    }
    public string GetLankString() {
        string lankString = "";
        if(lank == 0) {}
        else if(lank <= 26) lankString += (char)('A' + lank - 1);
        else {
            lankString += (char)('A' + ((lank - 1) / 26) - 1);
            lankString += (char)('A' + ((lank - 1) % 26));
        }
        return lankString;
    }

    public override string ToString() {
        return GetValueString() + " " + GetLankString();
    }
}
