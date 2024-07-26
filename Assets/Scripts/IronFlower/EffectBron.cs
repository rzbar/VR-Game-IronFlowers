using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBron : MonoBehaviour
{
    private IronFlowerEffect ironEffect;
    public float modifyNum;
    private void Awake()
    {
        ironEffect = GetComponent<IronFlowerEffect>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag== "Stick"&& collision.rigidbody.velocity.magnitude>1)
        {
            Quaternion rotation = Quaternion.LookRotation((collision.transform.position - transform.position).normalized);
            float speed = modifyNum * collision.rigidbody.velocity.magnitude;
            ironEffect.IronFlowerEffectGenerator(transform.position, rotation, speed,0.8f);
        }
    }
}
