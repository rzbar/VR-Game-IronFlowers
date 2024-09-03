Shader "IronFlower"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options nolightprobe	nolightmap
            #pragma instancing_options procedural:setup//表示每次实例渲染时，都会执行setup这个函数（但这里setup什么都没做）
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UniversalDOTSInstancing.hlsl" 

            void setup() {}

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                struct Particle_Color
                {
                    float3 position;
                    float scale;
                    float2 life; //(age lifeTime)

                    float3 velocity;

                    float4 color;
                };
                StructuredBuffer<Particle_Color> _ParticleDataBuffer;
            #endif

            half4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
            };


            v2f vert(appdata v)
            {
                v2f o;
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    Particle_Color particleData = _ParticleDataBuffer[v.instanceID];
                    v.vertex += float4(particleData.position, 0.0f);
                #endif
                
                o.posCS = TransformObjectToHClip(v.vertex);
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}