Shader "IronFlower"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup//表示每次实例渲染时，都会执行setup这个函数（但这里setup什么都没做）

            void setup() {}
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UniversalDOTSInstancing.hlsl" 

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
            };

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                struct Particle
                {
                    float3 position;
                    float scale;
                    float2 life; //(age lifeTime)

                    float3 velocity;
                };
                StructuredBuffer<Particle> particlesBuffer;
            #endif

            half4 _Color;

            v2f vert(appdata v, uint instanceID : SV_InstanceID)
            {
                v2f o;
                
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    Particle particleData = particlesBuffer[instanceID];
                    //v.vertex *= particleData.scale;
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