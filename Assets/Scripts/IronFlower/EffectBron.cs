using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
[RequireComponent(typeof(IronFlowerEffect))]
public class EffectBron : MonoBehaviour
{
    public IronFlowerEffect ironEffect;
    public float temperature=0;
    public Material material;
    [Header("修正参数")]
    public float modifyNum;
    [Header("降温速度")]
    public float coolSpeed;
    private void Awake()
    {
        ironEffect = GetComponent<IronFlowerEffect>();
    }
    public void Update()
    {
        float redDeep = 255 - (temperature / 1000) * 255;
        material.color = new Color(255, redDeep, redDeep);
        temperature -= coolSpeed * Time.deltaTime;
    }
    public void OnInFire(float temperatureNum)
    {
        temperature += temperatureNum * 0.2f * Time.deltaTime;
    }
    public float GetIronFlowerMass(float input)
    {
        float mappedValue = 0f;
        if (input <= 800f)
        {
            mappedValue = Mathf.SmoothStep(0f, 1f, (input - 1f) / (800f - 1f));
        }
        else
        {
            mappedValue = Mathf.SmoothStep(1f, 1f, (input - 800f) / (1000f - 800f));
        }
        return mappedValue* modifyNum;
    }
    public float GetIronFlowerSpeed(float input)
    {
        return Mathf.Lerp(4, 5, input);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Stick" )
        {
            Quaternion rotation = Quaternion.LookRotation((other.transform.position - transform.position).normalized);
            //float speed = modifyNum * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            ironEffect.IronFlowerEffectGenerator(transform.position, rotation, GetIronFlowerSpeed(other.gameObject.GetComponent<Rigidbody>().velocity.magnitude), GetIronFlowerMass(temperature));
        }
        else
        {
            print("too slow");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stick" /*&& other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1*/)
        {
            Quaternion rotation = Quaternion.LookRotation((collision.transform.position - transform.position).normalized);
            //float speed = modifyNum * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            ironEffect.IronFlowerEffectGenerator(transform.position, rotation, 5, 0.8f);
        }
        else
        {
            print("too slow");
        }
    }
}
