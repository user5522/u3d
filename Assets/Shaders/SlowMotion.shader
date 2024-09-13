Shader "Custom/SlowMotionURPDimmingResistant"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SlowMotionColor ("Slow Motion Color", Color) = (0,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,1)) = 0.5
        _SlowMotionIntensity ("Slow Motion Intensity", Range(0,1)) = 0
        _GlobalDimmingIntensity ("Global Dimming Intensity", Range(-1,1)) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _SlowMotionColor;
                half _GlowIntensity;
                half _SlowMotionIntensity;
                half _GlobalDimmingIntensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 albedo = lerp(c.rgb, _SlowMotionColor.rgb, _SlowMotionIntensity);
                half3 emission = _SlowMotionColor.rgb * _GlowIntensity * _SlowMotionIntensity;
                
                // Counteract global dimming
                half3 finalColor = albedo + emission;
                finalColor *= exp2(-_GlobalDimmingIntensity);
                
                return half4(finalColor, c.a);
            }
            ENDHLSL
        }
    }
}