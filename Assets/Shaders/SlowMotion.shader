Shader "Custom/SlowMotion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SlowMotionColor ("Slow Motion Color", Color) = (0,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,1)) = 0.5
        _SlowMotionIntensity ("Slow Motion Intensity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        fixed4 _SlowMotionColor;
        float _GlowIntensity;
        float _SlowMotionIntensity;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = lerp(c.rgb, _SlowMotionColor.rgb, _SlowMotionIntensity);
            o.Emission = _SlowMotionColor.rgb * _GlowIntensity * _SlowMotionIntensity;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}