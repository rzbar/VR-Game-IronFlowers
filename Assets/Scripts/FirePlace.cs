using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FirePlace : MonoBehaviour
{
    public TMP_Text temperatureText;
    public TMP_Text warmText;
    private float temperatureNum;
    public float TemperatureNum
    {
        get
        {
            return temperatureNum;
        }
        set
        {
            temperatureNum = value;
            temperatureText.text = ((int)temperatureNum).ToString();
        }
    }
    public bool isBegin;
    private void OnTriggerStay(Collider other)
    {
        if (!isBegin) return;
        if (other.gameObject.GetComponent<EffectBron>() != null)
        {
            EffectBron item = other.gameObject.GetComponent<EffectBron>();
            item.OnInFire(temperatureNum);
        }
    }
    private void Update()
    {
        if (!isBegin) return;
        if (temperatureNum < 700) warmText.text = "ƫ��";
        else if (temperatureNum > 810) warmText.text = "ƫ��";
        else warmText.text = "����";
    }
    public void BeginFire()
    {
        isBegin = true;
    }
    public void CloseFire()
    {
        isBegin = false;
        warmText.text = "δ����";
    }
    public void Changetemperature(float value)
    {
        if (!isBegin) return;
        TemperatureNum = value*1000;
    }
}
