#ifndef MATH_FUNCTION
#define MATH_FUNCTION

#include "MathCommonNumbers.cginc"

//======================================================
//Vector

float3 UPWARD_UNIT_VECTOR = float3(0.0f, 1.0f, 0.0f);
float3 FORWARD_UNIT_VECTOR = float3(1.0f, 0.0f, 0.0f);

float3 SafeNormalize(float3 v, float3 fallback)
{
    float modulus = dot(v, v);//向量的模
    //假如向量的模为0，则返回一个安全值
    return modulus > EPSILON ? v / sqrt(modulus) : fallback;
}

float3 SafeNormalize(float3 v)
{
    return SafeNormalize(v, UPWARD_UNIT_VECTOR);
}

//project a vector to another vector
float3 projectToVector(float3 v, float3 onto)
{
    float3 onto_unit = normalize(onto);//投向向量方向的单位向量
    return dot(v, onto) * onto_unit;
}

//project a vector to a plane
float3 projectToPlane(float3 v, float3 n)
{
    //向量减去它自身在法线上自身的投影即为其在平面上的投影
    return v - projectToVector(v, n);
}

//find a orthogonal vector
float3 findOthogonal(float3 v)
{
    if (v == UPWARD_UNIT_VECTOR) return FORWARD_UNIT_VECTOR;
    return normalize(cross(v, UPWARD_UNIT_VECTOR));
}

//spherical linear interpolation 几何球面线性插值
//向量被视为球面上的方向而不是空间中的点。返回的向量的方向通过角度进行插值
float3 slerp(float3 a, float3 b, float t)
{
    float cosOmega = dot(normalize(a), normalize(b));
    //当 Ω(omega) → 0 时即 cosOmege → 1 时，这个方程就退化为线性插值方程
    if (cosOmega > EPSILON_ONE_MINUS) return lerp(a, b, t);

    float omege = acos(clamp(cosOmega, -1.0f, 1.0f));
    return (sin((1.0-t) * omege) * a + sin((t * omege) * b)) / sin(omege);
}

//======================================================



//======================================================
//Quaternion

float4 UNIT_QUATERNION = float4(0.0, 0.0, 0.0, 1.0);

//returns the conjugate quaternion
返回共轭四元数
float4 conjugate(float4 q) {
    return float4(-q.xyz, q.w);
}

//returns the rotated quaternion of corresponding multiple of angle
//返回角度的对应倍数的旋转四元数
float4 angleMultiplier(float4 q, float factor) {
    float length = length(q.xyz);
    if (length < EPSILON) return UNIT_QUATERNION;

    float theta = atan2(q.w, length) * factor;//atan2 = atan(q.w / length)
    return float4(sin(theta) * length, cos(theta));
}

//derive the ratational quaternion from the axis and angle of rotation
//根据旋转轴和旋转角得出旋转四元数
float4 rotationalQuaternion(float3 axis, float angle) {
    float theta = angle * 0.5;
    return float4(sin(theta) * normalize(axis), cos(theta));
}

//======================================================



//======================================================
//Color

//convert rgb color to hsv color.
//the range of hsv = (0~360, 0~1, 0~1)
//the range of rgb = (0~1, 0~1, 0~1)
float3 rgb2hsv(float3 rgb)
{
    float maxRGB = max(max(rgb.r, rgb.g), rgb.b);
    float minRGB = min(min(rgb.r, rgb.g), rgb.b);
    float delta = maxRGB - minRGB;

    //明度v即为最大的rgb值
    float brightness = maxRGB;
    //饱和度s反映了最大RGB值与最小RGB值差值和最大RGB值之间的比例，比例越大颜色越饱和
    float saturation = maxRGB == 0 ? 0 : delta / maxRGB;
    //色相h即所处的光谱颜色的位置，当maxRGB为0即delta为0时，默认hue为0(假如maxRGB为0，则在第一个判断语句就可以检测出来)
    //通过主色调外两颜色分量之差除以delta(即未比例化的"饱和度")反映当前颜色在两个非主色调之间的相对位置
    //而+0、+2、+4是为了将主色调映射到正确的区间去
    float hue;
    
    if (maxRGB == rgb.r)        hue = maxRGB =! 0 ? 60 * ((rgb.g - rgb.b) / delta + 0) : 0;
    else if(maxRGB == rgb.g)    hue = 60 * ((rgb.b - rgb.r) / delta + 2);
    else                        hue = 60 * ((rgb.r - rgb.g) / delta + 4);
    
    hue += step(0, hue) * 360;//如果hue为负则映射到正值上去

    return float3(hue, saturation, brightness);
}

//convert hsv color to rgb color.
//the range of hsv = (0~360, 0~1, 0~1)
//the range of rgb = (0~1, 0~1, 0~1)
float3 hsv2rgb(float3 hsv)
{
    float H = hsv.x, S = hsv.y, V = hsv.z;
    //色相所在的区域，取值范围为[0, 5]之间的整数
    int h_area = (int)floor(H / 60);
    //色相在两个非主色调之间的偏移量，取值范围[0, 1)
    //f实际上为 |G或B或R - B或R或G| / (maxRGB - minRGB) ，为一个范围为[0, 1)的色相偏移量
    float f = H / 60 - h_area;
    
    //p为RGB中的最小值（hsv中S = (maxRGB - minRGB) / maxRGB，而V = maxRGB）
    float p = V * (1 - S);
    //q为可能的非主色调RGB颜色值（f * S = |G或B或R - B或R或G|，即色相偏移值）
    float q = V * (1 - f * S);
    //t为另一可能的非主色调RGB颜色值（(1 - f) * S = |B或R或G - G或B或R|，即色相偏移值）
    float t = V * (1 - (1 - f) * S);//（当色相偏移量为0时，1 - (1 - f) * S = 1 - V。即t = p

    switch (h_area)
    {
        case 0: return float3(V, t, p);
        case 1: return float3(q, V, p);
        case 2: return float3(p, V, t);
        case 3: return float3(p, q, V);
        case 4: return float3(t, p, V);
        case 5: return float3(V, p, q);
        default: return float3(0, 0, 0);
    }
}

//======================================================

#endif
