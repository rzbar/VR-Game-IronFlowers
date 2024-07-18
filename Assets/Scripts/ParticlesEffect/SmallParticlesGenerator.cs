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
        part = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        
        // 清空之前的碰撞事件
        collisionEvents.Clear();
        //获取事件数量，并将事件存储入事件数组
        part.GetCollisionEvents(other,collisionEvents);
        int count = part.GetSafeCollisionEventSize();
        //print(count);

        for (int i = 0; i < count; i++)
        {
            //Vector3 vet= collisionEvents[i].velocity;  
            //print (vet);
            // 从碰撞事件中获取位置
            Vector3 pos = collisionEvents[i].intersection;

            // 验证位置是否正确
            if (pos != Vector3.zero)
            {
                // 为防止过多粒子造成性能问题，对小粒子的生成进行控制
                if (Random.value < SmallParticleGenerationProbability)
                {
                    // 创建小粒子系统
                    GameObject smallSpark = Instantiate(SmallSparks, pos, Quaternion.identity);
                    Destroy(smallSpark, 2f);
                    print(smallSpark.name);
                }
            }
            else
            {
                // 输出错误信息，调试位置为 Vector3.zero 的问题
                print(666);
            }
        }
    }
}
