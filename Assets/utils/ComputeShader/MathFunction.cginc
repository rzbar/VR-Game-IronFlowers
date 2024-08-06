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

//find an orthogonal vector
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
    return (sin((1.0f-t) * omege) * a + sin((t * omege) * b)) / sin(omege);
}

//======================================================



//======================================================
//Quaternion

float4 UNIT_QUATERNION = float4(0.0f, 0.0f, 0.0f, 1.0f);

//returns the conjugate quaternion
//返回共轭四元数
float4 conjugateQuaternion(float4 quaternion)
{
    return float4(-quaternion.xyz, quaternion.w);
}   

//derive the rotated vector by the rotational quaternion
//得出通过旋转四元数旋转后的向量
float3 rotateVector(float3 vec, float4 quaternion)
{
    //设四元数为 a + bi + cj + dk ，向量为 xi + yj + zk ，则旋转后向量为：
    //[(a^2 + b^2 - c^2 - d^2)x + (   2bc    -    2ad   )y + (   2ac    +    2bd   )z]i +
    //[(   2ad    +    2bc   )x + (a^2 + c^2 - b^2 - d^2)y + (   2cd    -    2ab   )z]j +
    //[(   2bd    -    2ac   )x + (   2ab    +    2cd   )y + (a^2 + d^2 - b^2 - c^2)z]k     (算了我半个小时...)
    //下面计算中的四元素在上式来看，格式为(b, c, d, a)，因为游戏引擎中四元素一般以(vector, real number)的形式出现
    return
        dot(quaternion.xyz, vec) * quaternion.xyz// = ( (b^2)x + (bc)y + (bd)z, (bc)x + (c^2)y + (cd)z, (bd)x + (cd)y + (b^2)z )
        + quaternion.w * quaternion.w * vec// = ( (a^2)x, (a^2)y, (a^2)z )
        + 2.0f * quaternion.w * cross(quaternion.xyz, vec)// = ( (-2ad)y + (2ac)z, (2ad)x + (-2ab)z, (-2ac)x + (2ab)y )
        - cross(cross(quaternion.xyz, vec), quaternion.xyz);// = - ( (c^2 + d^2)x + (-bc)y + (-bd)z, (-bc)x + (d^2 + b^2)y + (-cd)z, (-bd)x + (-cd)y + (c^2 + b^2)z )
}

//returns the rotated quaternion of corresponding multiple of angle
//返回角度的对应倍数的旋转四元数
float4 angleMultiplier(float4 quaternion, float factor)
{
    float length = length(quaternion.xyz);
    if (length < EPSILON) return UNIT_QUATERNION;

    float theta = atan2(quaternion.w, length) * factor;//atan2 = atan(q.w / length)
    return float4(sin(theta) * quaternion.xyz / length, cos(theta));
}

//derive the rotational quaternion from the axis and angle of rotation
//根据旋转轴和旋转角得出旋转四元数
float4 getRotationalQuaternion(float3 axis, float angle)
{
    float theta = angle * 0.5f;
    return float4(sin(theta) * normalize(axis), cos(theta));
}

//derive the rotational quaternion of the angle of rotation from the 'from' vector to the 'to' vector
//得出从from向量旋转到to向量角度的旋转四元数
float4 getQuaternionFromTo(float3 from, float3 to)
{
    float3 crossProduct = cross(from, to);//得到两向量叉积
    float dotCross = dot(crossProduct, crossProduct);

    if (dotCross < EPSILON) return UNIT_QUATERNION;
    
    from = normalize(from);
    to = normalize(to);
    float3 axis = crossProduct / sqrt(dotCross);
    float theta = acos(clamp(dot(from, to), -1.0f, 1.0f));
    return getRotationalQuaternion(axis, theta);
}

//get the axis of rotation of quaternion
float3 getAxisOfQuaternion(float4 quaternion)
{
    float dotVector = dot(quaternion.xyz, quaternion.xyz);
    return dotVector < EPSILON ? float3(0.0f, 0.1f, 0.0f) : quaternion.xyz / sqrt(dotVector);
}

//get the angle of rotation of quaternion
float getAngleOfQuaternion(float4 quaternion)
{
    return 2.0f * acos(clamp(quaternion.w, -1.0f, 1.0f));
}

//using normalized linear interpolation for quaternion
//(对四元数来说，归一化线性插值其实一种平滑的非线性插值)(输入值必须为单位向量，否则结果不会经过初始、最终向量)
float4 nlerp(float4 quaternion1, float4 quaternion2, float t)
{
    float4 value = (1 - t) * quaternion1 + t * quaternion2;
    float dotValue = dot(value, value);
    return dotValue < EPSILON ? UNIT_QUATERNION : value / sqrt(dotValue);
}

//using spherical linear interpolation for quaternions
//(对四元数的真正线性插值)(比nlerp慢，在四元数夹角比较小时可以交给nlerp)
float4 slerp(float4 quaternion1, float4 quaternion2, float t)
{
    //检查四元数夹角是否过小，防止后续操作中sinTheta由于浮点数的性质被近似为0.0
    float cosTheta = dot(normalize(quaternion1), normalize(quaternion2));
    if (cosTheta > EPSILON_ONE_MINUS) return nlerp(quaternion1, quaternion2, t);

    float theta = acos(clamp(cosTheta, -1.0f, 1.0f));
    return ( sin((1-t) * theta) * quaternion1 + sin(t * theta) * quaternion2 ) / sin(theta);
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
