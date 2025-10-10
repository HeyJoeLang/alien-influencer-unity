Shader "Custom/TransparentRimGlow_Standard"
{
    Properties
    {
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Tint (RGBA drives overall alpha)", Color) = (1,1,1,0.5)

        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 1.0
        _GlowMask ("Glow Mask (R)", 2D) = "white" {}

        [Enum(Off,0,Static,1,Pulsing,2)] _RimMode ("Rim Mode", Float) = 1
        _RimColor ("Rim Color", Color) = (0.5,0.9,1,1)
        _RimIntensity ("Rim Intensity", Range(0,10)) = 2.0
        _RimPower ("Rim Width (Power)", Range(0.1,8)) = 2.0
        _RimPulseSpeed ("Rim Pulse Speed (Hz)", Range(0.0,8.0)) = 1.0

        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5

        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcBlend ("Src Blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlend ("Dst Blend", Float) = 10
        [Toggle] _ZWrite ("ZWrite (Off=0 / On=1)", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull ("Cull", Float) = 2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 300

        Cull [_Cull]
        ZWrite [_ZWrite]
        Blend [_SrcBlend] [_DstBlend]

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _Color;

        sampler2D _GlowMask;
        float4 _GlowColor;
        half _GlowIntensity;

        half _Metallic;
        half _Smoothness;

        float _RimMode;
        float4 _RimColor;
        half _RimIntensity;
        half _RimPower;
        half _RimPulseSpeed;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_GlowMask;
            float3 viewDir;
            INTERNAL_DATA
        };

        inline half FresnelTerm(float3 n, float3 v, half power)
        {
            half ndv = saturate(dot(normalize(n), normalize(v)));
            return pow(saturate(1.0h - ndv), power);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 baseCol = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = baseCol.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = saturate(baseCol.a);

            float mask = tex2D(_GlowMask, IN.uv_GlowMask).r;
            float3 glow = _GlowColor.rgb * _GlowIntensity * mask;

            float3 worldN = normalize(WorldNormalVector(IN, o.Normal));
            half fres = FresnelTerm(worldN, IN.viewDir, _RimPower);

            if (_RimMode > 0.5)
            {
                half pulse = 1.0h;
                if (_RimMode > 1.5)
                {
                    float t = _Time.x * 6.2831853 * max(_RimPulseSpeed, 0.0);
                    pulse = 0.5h + 0.5h * sin(t);
                }
                glow += _RimColor.rgb * (_RimIntensity * pulse) * fres;
            }

            o.Emission = glow;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
