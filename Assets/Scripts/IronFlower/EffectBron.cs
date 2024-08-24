using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
[RequireComponent(typeof(IronFlowerEffect))]
public class EffectBron : MonoBehaviour
{
    public IronFlowerEffect ironEffect;
    public float modifyNum;
    private void Awake()
    {
        ironEffect = GetComponent<IronFlowerEffect>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Stick" /*&& other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1*/)
        {
            Quaternion rotation = Quaternion.LookRotation((other.transform.position - transform.position).normalized);
            //float speed = modifyNum * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            ironEffect.IronFlowerEffectGenerator(transform.position, rotation, 5, 0.8f);
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
