#ifndef PHYSICS_COMPUTER
#define PHYSICS_COMPUTER

#include "Assets/utils/ComputeShader/MathFunction.cginc"

//------------------------------------------------------------------------------------------------------------
//======================================================
//Struct

//返回的碰撞计算结果
struct CollisionResult
{
    float3 pos;
    float3 velocity;
    float4 angularVelocity;
};

//======================================================
//------------------------------------------------------------------------------------------------------------
//======================================================
//Resolver

//pos -- 物体当前位置     normal -- 碰撞点法线     velocity -- 物体速度
//penetration -- 当前穿透情况，用于修正物体位置
//restitution -- 恢复系数，为1时为弹性碰撞 (0 ~ 1)
//friction -- 摩擦系数 (0 ~ 1)
inline CollisionResult collisionResolver(float3 pos, float3 normal, float3 velocity,
                                            float penetration, float restitution, float friction)
{
    float speedRateN = dot(velocity, normal); //法线方向上的速率
    float3 velocityN = normal * speedRateN; //法线方向上的速度
    float3 velocityT = velocity - velocityN; //切线方向上的速度
    float ratio = -speedRateN / length(velocity); //速度系数，用于计算摩擦（speedRateN应为负值，所以此处要取反）
    //最终速度改变量
    float3 velocityResolution = -(1.0 + restitution) * velocityN - friction * ratio * velocityT;

    CollisionResult result;
    result.pos = pos + penetration * normal;
    result.velocity = velocity + velocityResolution * step(EPSILON, penetration);

    return result;
}

//======================================================
//------------------------------------------------------------------------------------------------------------
//======================================================
//Model

inline CollisionResult sphereVSground(float3 pos, float radius, float3 groundNdir, float groundCenterHeight,
                                        float3 velocity, float restitution, float friction)
{
    //计算穿透量，用于修正球体位置
    float penetration = max( 0.0f, radius - (dot(pos, groundNdir) + groundCenterHeight) );
    return collisionResolver(pos, groundNdir, velocity, penetration, restitution, friction);
}

//======================================================
//------------------------------------------------------------------------------------------------------------

#endif