using UnityEngine;

namespace ComputeShaderStruct
{
    public struct Particle
    {
        public Vector3 position;
        public float scale;
        public Vector2 life; //(age lifeTime)

        public Vector3 velocity;
    };

    //用于有形状的粒子，例如正方形等
    public struct SpecialParticle
    {
        public Vector3 position;
        public float scale;
        public Vector2 life; //(age lifeTime)
    
        public Vector3 velocity;
    
        public Vector4 rotation;
        public Vector4 angularVelocity;
    };

    public struct Particle_Color
    {
        public Vector3 position;
        public float scale;
        public Vector2 life; //(age lifeTime)

        public Vector3 velocity;

        public Color color;
    };

    //用于有形状的粒子，例如正方形等
    public struct SpecialParticle_Color
    {
    
        public Vector3 position;
        public float scale;
        public Vector2 life; //(age lifeTime)

        public Vector3 velocity;
    
        public Vector4 rotation;
        public Vector4 angularVelocity;
    
        public Color color;
    };
}