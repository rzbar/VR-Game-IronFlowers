using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallParticlesGenerator : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float SmallParticleGenerationProbability = 0.3f; //对小粒子的生成进行控制
    
    public ParticleSystem part; //生成主粒子的粒子系统
    public GameObject SmallSparks; //需要生成的小粒子系统
    
    public List<ParticleCollisionEvent> collisionEvents;    //存储碰撞事件

    //初始化
    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        //获取事件数量，并将事件存储入事件数组
        part.GetCollisionEvents(other, collisionEvents);
        int count = part.GetSafeCollisionEventSize();
        print(count);

        for (int i = 0; i < count; i++)
        {
            //为防止过多粒子造成性能问题，对小粒子的生成进行控制
            if (Random.value < SmallParticleGenerationProbability) return;

            //创建小粒子系统
            Vector3 pos = collisionEvents[i].intersection;
            print(pos);
            GameObject smallSpark = Instantiate(SmallSparks, pos, new Quaternion());
            Destroy(smallSpark, 2f);
        }
    }
}
