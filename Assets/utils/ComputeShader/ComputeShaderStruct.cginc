#ifndef COMPUTE_SHADER_STRUCT
#define COMPUTE_SHADER_STRUCT

struct Particle
{
    float3 position;
    float scale;
    float2 life; //(age lifeTime)

    float3 velocity;
};

//用于有形状的粒子，例如正方形等
struct SpecialParticle
{
    float3 position;
    float scale;
    float2 life; //(age lifeTime)
    
    float3 velocity;
    
    float4 rotation;
    float4 angularVelocity;
};

struct Particle_Color
{
    float3 position;
    float scale;
    float2 life; //(age lifeTime)

    float3 velocity;

    float4 color;
};

//用于有形状的粒子，例如正方形等
struct SpecialParticle_Color
{
    
    float3 position;
    float scale;
    float2 life; //(age lifeTime)

    float3 velocity;
    
    float4 rotation;
    float4 angularVelocity;
    
    float4 color;
};

#endif