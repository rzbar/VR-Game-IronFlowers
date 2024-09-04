using System;
using System.Collections;
using ComputeShaderStruct;
using UnityEngine;
using UnityEngine.Rendering;

namespace ParticleGenerator
{
    public class ParticleGenerator : MonoBehaviour
    {
        public ComputeShader computeShader; //ParticleResolver.compute
        public Material material;

        [Range(0.0f, 30.0f)] public float entityLifetime = 10.0f;
        
        public Vector3 entityPosition = new Vector3(0.0f, 0.0f, 0.0f); //粒子总体在此位置开始生成
        public Vector3 initialDir = Vector3.up; //粒子总体初始的速度方向
        public float maxBiasAngle = 30.0f; //速度方向的最大偏移角度(范围为0~180)
        public float posEffectValue = 0.6f; //位置对速度大小和速度方向的影响，范围0到1（例如：值越大，越靠外的粒子速度越小，速度角度偏移越大）

        public int particlesNum = 128000; //粒子数量
        public Vector2 scaleRange = new Vector2(0.01f, 0.03f); //粒子大小范围 (min, max)
        public Vector2 speedRange = new Vector2(5.0f, 6.0f); //粒子速度范围 (min, max)
        public Vector2 lifetimeRange = new Vector2(5.0f, 7.0f); //生命周期范围 (min, max)

        [Range(0.0f, 3.0f)] public float timeScale = 1.0f; //时间流速
        
        public Vector3 groundNormal = Vector3.up; //地面倾斜程度，等效角度范围(-90, 90)
        [Range(-10.0f, 10.0f)] public float groundHeight = 0.0f; //地面高度
        
        [Range(1.0f, 30.0f)] public float gravity = 9.8f; //重力
        [Range(0.0f, 1.0f)] public float restitution = 0.3f; //恢复系数
        [Range(0.0f, 1.0f)] public float friction = 0.7f; //摩擦系数

        private int _bufferNum = 3; //缓存数量，决定最多有多少个粒子系统同时存在，不建议超过3个
        private ComputeBuffer[] _particlesBuffers;
        private int[] _particlesNumBuffers; //保存每个缓存对应的粒子数量
        private ComputeBuffer[] _instanceArgsBuffers;

        private Mesh _mesh;
        private MaterialPropertyBlock _materialPropertyBlock;

        private int _initKernelId;
        private int _updateKernelId;

        private int _particlesBufferId;
        
        private int _entityPositionId;
        private int _initialDirId;
        private int _maxBiasAngleId;
        private int _posEffectValueId;
        
        private int _scaleRangeId;
        private int _speedRangeId;
        private int _lifetimeRangeId;
        
        private int _deltaTimeId;
        private int _physicalFactorId;
        private int _groundId;

        private void OnEnable()
        {
            //创建Mesh
            _mesh = MeshFactory.MeshFactory.SmoothShadedSphere(6, 7);
            //创建GPU实例化需要的材质代码块
            _materialPropertyBlock = new MaterialPropertyBlock();
            //初始化实例化缓存数组
            _instanceArgsBuffers = new ComputeBuffer[_bufferNum];
            
            //初始化ComputeShader内存缓冲区数组
            _particlesBuffers = new ComputeBuffer[_bufferNum];
            _particlesNumBuffers = new int[_bufferNum];
            
            //获取ComputeShader中内核id
            _initKernelId = computeShader.FindKernel("Init");
            _updateKernelId = computeShader.FindKernel("Update");
            //获取ComputeShader中各变量id
            _particlesBufferId = Shader.PropertyToID("particlesBuffer");
            _entityPositionId = Shader.PropertyToID("entityPosition");
            _initialDirId = Shader.PropertyToID("initialDir");
            _maxBiasAngleId = Shader.PropertyToID("maxBiasAngle");
            _posEffectValueId = Shader.PropertyToID("posEffectValue");
            _scaleRangeId = Shader.PropertyToID("scaleRange");
            _speedRangeId = Shader.PropertyToID("speedRange");
            _lifetimeRangeId = Shader.PropertyToID("lifetimeRange");
            _deltaTimeId = Shader.PropertyToID("deltaTime");
            _physicalFactorId = Shader.PropertyToID("physicalFactor");
            _groundId = Shader.PropertyToID("ground");
            
            //Material开启实例化
            material.enableInstancing = true;
        }

        private void Update()
        {
            //如果没有激活和启用的话，则不更新
            if (!isActiveAndEnabled)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < _bufferNum; i++)
                {
                    if (_particlesBuffers[i] == null && _instanceArgsBuffers[i] == null && _particlesNumBuffers[i] == 0)
                    {
                        InitParticles(i);
                        break;
                    }
                    if (i == _bufferNum - 1) print("粒子缓存区已达上限，请等待");
                }
            }
            
            for (int i = 0; i < _bufferNum; i++)
            {
                if (_particlesBuffers[i] != null && _instanceArgsBuffers[i] != null && _particlesNumBuffers[i] != 0)
                    UpdateParticles(i);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _bufferNum; i++)
            {
                DestroyParticles(i);
            }
        }

        private void InitParticles(int index)
        {
            //初始化粒子数量缓存(最好为64的倍数)
            _particlesNumBuffers[index] = (particlesNum / 64) * 64;
            //初始化Mesh的实例参数数组，并设置到内存缓冲区(用于DrawMeshInstancedIndirect函数(用于GPU实例化))
            uint[] instanceArgs = new uint[] { 0, 0, 0, 0, 0 };
            instanceArgs[0] = (uint)_mesh.GetIndexCount(0);
            instanceArgs[1] = (uint)_particlesNumBuffers[index];
            instanceArgs[2] = (uint)_mesh.GetIndexStart(0);
            instanceArgs[3] = (uint)_mesh.GetBaseVertex(0);
            _instanceArgsBuffers[index] = new ComputeBuffer(1, instanceArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _instanceArgsBuffers[index].SetData(instanceArgs);
            
            //初始化ComputeShader粒子数组缓冲
            _particlesBuffers[index] = new ComputeBuffer(_particlesNumBuffers[index], sizeof(float) * 9);
            
            //ComputeShader初始化参数
            computeShader.SetFloats(_entityPositionId, new float[] {entityPosition.x, entityPosition.y, entityPosition.z});
            computeShader.SetFloats(_initialDirId, new float[] {initialDir.x, initialDir.y, initialDir.z});
            computeShader.SetFloat(_maxBiasAngleId, maxBiasAngle);
            computeShader.SetFloat(_posEffectValueId, posEffectValue);
            computeShader.SetFloats(_scaleRangeId, new float[] {scaleRange.x, scaleRange.y});
            computeShader.SetFloats(_speedRangeId, new float[] {speedRange.x, speedRange.y});
            computeShader.SetFloats(_lifetimeRangeId, new float[] {lifetimeRange.x, lifetimeRange.y});
            
            //将粒子信息缓存区数据传递给Init内核
            computeShader.SetBuffer(_initKernelId, _particlesBufferId, _particlesBuffers[index]);
            
            //ComputeShader开始初始化
            computeShader.Dispatch(_initKernelId, _particlesNumBuffers[index] / 64, 1, 1);
            
            //一段时间后销毁粒子，释放内存
            StartCoroutine(ParticleGenerator.DelayedExecute(() => { DestroyParticles(index); }, entityLifetime));
        }

        private void UpdateParticles(int index)
        {
            //将粒子信息缓存区传递给Update内核
            computeShader.SetBuffer(_updateKernelId, _particlesBufferId, _particlesBuffers[index]);
            
            //设置时间增量
            computeShader.SetFloat(_deltaTimeId, timeScale * Time.deltaTime);
            //设置相关物理系数
            computeShader.SetFloats(_physicalFactorId, new float[] {gravity, restitution, friction});
            
            //将地板信息加载入ComputeShader
            computeShader.SetFloats(_groundId, new float[]{groundNormal.x, groundNormal.y, groundNormal.z, groundHeight});
            
            //分配线程，进行ComputeShader计算
            computeShader.Dispatch(_updateKernelId, _particlesNumBuffers[index] / 64, 1, 1);
            
            //给材质代码块设置缓存
            material.SetBuffer(_particlesBufferId, _particlesBuffers[index]);
            
            //绘制粒子
            Graphics.DrawMeshInstancedIndirect(_mesh, 0, material, new Bounds(entityPosition, 50.0f * Vector3.one),
                                                _instanceArgsBuffers[index], 0, _materialPropertyBlock, ShadowCastingMode.Off);
        }

        private void DestroyParticles(int index)
        {
            if (_particlesBuffers[index] != null)
            {
                _particlesBuffers[index].Dispose();
                _particlesBuffers[index] = null;

                _particlesNumBuffers[index] = 0;
            }

            if (_instanceArgsBuffers[index] != null)
            {
                _instanceArgsBuffers[index].Dispose();
                _instanceArgsBuffers[index] = null;
            }
        }

        public static IEnumerator DelayedExecute(Action action, float delayedSeconds)
        {
            yield return new WaitForSeconds(delayedSeconds);
            action();
        }
    }
}