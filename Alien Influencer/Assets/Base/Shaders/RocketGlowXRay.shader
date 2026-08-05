Shader "Custom/RocketGlowXRay"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _GlowColor ("Glow Color", Color) = (1,0.45,0.05,1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 1.0

        _HotColor ("Hot Core Color", Color) = (1,0.85,0.5,1)
        _HotPower ("Hot Core Sharpness", Range(0.1,8)) = 3.0

        [Enum(Off,0,Static,1,Pulsing,2)] _RimMode ("Rim Mode", Float) = 2
        _RimColor ("Rim Color", Color) = (1,0.35,0,1)
        _RimIntensity ("Rim Intensity", Range(0,10)) = 2.5
        _RimPower ("Rim Width (Power)", Range(0.1,8)) = 2.5
        _RimPulseSpeed ("Rim Pulse Speed (Hz)", Range(0.0,8.0)) = 2.0

        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.4

        [Toggle] _XRayEnabled ("Show Through Occluders", Float) = 1
        _XRayColor ("X-Ray Glow Color", Color) = (1,0.35,0,1)
        _XRayIntensity ("X-Ray Intensity", Range(0,5)) = 1.5
        _XRayFillAlpha ("X-Ray Fill Opacity", Range(0,1)) = 0.85
        _XRayPulseSpeed ("X-Ray Pulse Speed (Hz)", Range(0.0,8.0)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        // Pass 0: normal, depth-tested lit rocket with rim + hot-core glow.
        // Opaque queue is sorted front-to-back, so closer occluders (e.g. buildings)
        // reliably write their depth before this renders - required for Pass 1 below.
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _Color;

        float4 _GlowColor;
        half _GlowIntensity;

        float4 _HotColor;
        half _HotPower;

        float _RimMode;
        float4 _RimColor;
        half _RimIntensity;
        half _RimPower;
        half _RimPulseSpeed;

        half _Metallic;
        half _Smoothness;

        struct Input
        {
            float2 uv_MainTex;
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
            o.Alpha = 1;

            // Hot-core-to-cool-edge: brighter texture regions read as a hotter (near-white) glow,
            // darker regions stay in the base orange.
            half luminance = dot(baseCol.rgb, half3(0.299, 0.587, 0.114));
            half hotMask = pow(saturate(luminance), _HotPower);
            float3 glow = lerp(_GlowColor.rgb, _HotColor.rgb, hotMask) * _GlowIntensity * luminance;

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

        // Pass 1: X-ray glow. ZTest Greater only draws where an already-rendered
        // occluder is nearer to the camera than this surface - i.e. only the silhouette
        // that's currently hidden behind something, like a building.
        // Uses a mostly-opaque alpha-blended fill rather than pure additive: additive glow
        // washes out to white over bright backgrounds (sky, light walls), while a solid,
        // near-opaque fill reads as a clear colored shape no matter what's behind it.
        Pass
        {
            Name "XRAY_GLOW"
            Tags { "LightMode"="Always" }
            Cull Back
            ZWrite Off
            ZTest Greater
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float4 _XRayColor;
            half _XRayIntensity;
            half _XRayFillAlpha;
            half _XRayPulseSpeed;
            half _XRayEnabled;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                clip(_XRayEnabled - 0.5);

                float3 n = normalize(i.worldNormal);
                float fres = pow(1.0 - saturate(dot(normalize(i.viewDir), n)), 1.5);
                float t = _Time.y * 6.2831853 * max(_XRayPulseSpeed, 0.0);
                half pulse = 0.85h + 0.15h * sin(t);

                // Solid base fill so it reads clearly at any angle, plus a brighter rim on top.
                float3 col = _XRayColor.rgb * _XRayIntensity * pulse;
                col += _XRayColor.rgb * fres * _XRayIntensity * 0.75h;

                return fixed4(col, _XRayFillAlpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
