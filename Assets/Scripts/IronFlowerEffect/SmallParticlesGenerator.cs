using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallParticlesGenerator : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float OnPergolaParticleGenerationProbability = 0.3f; //对小粒子在花架上的生成进行控制
    [Range(0.0f, 1.0f)]
    public float OtherParticleGenerationProbability = 0.2f; //对小粒子在地板等其他对象上的生成进行控制

    public GameObject pergola; //指定花架，因为花架上的小粒子生成概率更高
    
    public GameObject SmallSparks; //需要生成的小粒子系统

    public List<ParticleCollisionEvent> collisionEvents;    //存储碰撞事件
    private Transform ironFlower; //将生成的小粒子效果放在ironFlower父物体下

    //对设置进行检查和初始化
    private void Start()
    {
        if (pergola == null) Debug.LogWarning("SmallParticlesGenerator.cs: 没有设置pergola花架");
        if (SmallSparks == null) Debug.LogWarning("SmallParticlesGenerator.cs: 没有设置需要生成SmallSparks小粒子系统");
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    //在粒子碰撞时生成小粒子
    private void OnParticleCollision(GameObject other)
    {
        //获取事件数量，并将事件存储入事件数组
        ParticleSystem part = gameObject.transform.GetComponent<ParticleSystem>();
        part.GetCollisionEvents(other, collisionEvents);
        int count = part.GetSafeCollisionEventSize();
        
        //如果没有初始化父物体，则进行初始化
        if (ironFlower == null) ironFlower = GameObject.Find("IronFlower(Clone)").transform;

        for (int i = 0; i < count; i++)
        {
            //为防止过多粒子造成性能问题，对小粒子的生成进行控制
            if (other == pergola)
            {
                if (Random.value > OnPergolaParticleGenerationProbability) return;
            }
            else
            {
                if (Random.value > OtherParticleGenerationProbability) return;
            }

            //创建小粒子系统
            Vector3 pos = collisionEvents[i].intersection;
            GameObject smallSpark = Instantiate(SmallSparks, pos, new Quaternion());
            smallSpark.transform.SetParent(ironFlower);
            Destroy(smallSpark, 2f);
        }
    }
}
