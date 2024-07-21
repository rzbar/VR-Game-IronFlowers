using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IronFlowerEffect : MonoBehaviour
{
    public GameObject prefab; //铁花的预制体

    public int MinNumOfParticals = 1000; //最少主粒子数量

    public int MaxNumOfParticals = 5000; //最多主粒子数量

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            IronFlowerEffectTest();
        }
    }

    /// <summary>
    ///  <para>默认在坐标(0, 0.5, 0)无旋转生成一个测试用铁花效果</para>
    /// </summary>
    public void IronFlowerEffectTest()
    {
        IronFlowerEffectGenerator(new Vector3(0, 0.5f, 0), new Quaternion(), 15f, 0.8f);
    }

    /// <summary>
    ///  <para>根据输入的位置、旋转方向和一个抽象的铁水质量生成对应的铁花效果</para>
    /// </summary>
    /// <param name="pos">position</param>
    /// <param name="rotation">ratation</param>
    /// <param name="speed">the stroker's speed</param>
    /// <param name="mass">an abstract mass of iron water (between 0 and 1)</param>
    public void IronFlowerEffectGenerator(Vector3 pos, Quaternion rotation, float speed, float mass)
    {
        //对预制体和参数进行检查
        if (prefab == null)
        {
            Debug.LogWarning("IronFlowerEffect.cs: 预制体未设置");
            return;
        }
        if (speed <= 0 || mass < 0)
        {
            Debug.LogWarning("IronFlowerEffect.cs: 铁花speed速度或者mass质量值错误");
        }
        
        //预制体实例化，并进行参数设置
        GameObject ironFlower = Instantiate(prefab, pos, new Quaternion());
        ironFlower.transform.position = pos;
        ironFlower.transform.rotation = rotation;
        
        ParticleSystem particleSystem = ironFlower.transform.GetChild(1).GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psMain = particleSystem.main;
        ParticleSystem.EmissionModule psEmission = particleSystem.emission;
        
        psMain.startSpeed = new ParticleSystem.MinMaxCurve(speed - speed / (float)Math.E / 2, speed + speed / (float)Math.E / 2);
        psMain.maxParticles = (int)Mathf.Lerp(MinNumOfParticals, MaxNumOfParticals, mass);
        psEmission.rateOverTime = (int)Mathf.Lerp(MinNumOfParticals + MinNumOfParticals / 3, 
                                        MaxNumOfParticals + MaxNumOfParticals / 3, mass);
        
        Destroy(ironFlower, 5f);
    }
}
