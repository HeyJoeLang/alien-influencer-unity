Shader "Custom/IndicatorGlowXRay"
{
    Properties
    {
        // MissileIndicator.cs drives this every frame via Renderer.material.color -
        // both the distance fade (alpha) and the white-to-red danger gradient (rgb).
        _Color ("Color", Color) = (1,1,1,1)

        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimIntensity ("Rim Intensity", Range(0,5)) = 1.0
        _RimPower ("Rim Power", Range(0.1,8)) = 2.0

        [Toggle] _XRayEnabled ("Show Through Occluders", Float) = 1
        _XRayIntensity ("X-Ray Intensity", Range(0,5)) = 1.6
        _XRayFillAlpha ("X-Ray Fill Opacity", Range(0,1)) = 0.85
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 200

        // Pass 0: normal, depth-tested unlit fill with a soft fresnel rim.
        // Unlit on purpose - this is a HUD-style warning signal, so it should read as a
        // consistent color/alpha rather than shift with scene lighting.
        Pass
        {
            Name "FORWARD"
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float4 _Color;
            float4 _RimColor;
            half _RimIntensity;
            half _RimPower;

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
                float3 n = normalize(i.worldNormal);
                float fres = pow(1.0 - saturate(dot(normalize(i.viewDir), n)), _RimPower);
                float3 col = _Color.rgb + _RimColor.rgb * fres * _RimIntensity;
                return fixed4(col, _Color.a);
            }
            ENDCG
        }

        // Pass 1: X-ray glow, same technique as RocketGlowXRay.shader - ZTest Greater only
        // draws where an already-rendered occluder (e.g. a building) is nearer to the
        // camera than the indicator. Reads _Color.a so it automatically respects the same
        // distance fade the script already drives - no indicator "ghosts" through walls
        // once it should be fully invisible.
        Pass
        {
            Name "XRAY_GLOW"
            Tags { "LightMode"="Always" }
            Cull Off
            ZWrite Off
            ZTest Greater
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float4 _Color;
            half _XRayIntensity;
            half _XRayFillAlpha;
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
                clip(_Color.a - 0.001h);

                float3 n = normalize(i.worldNormal);
                float fres = pow(1.0 - saturate(dot(normalize(i.viewDir), n)), 1.5);

                float3 col = _Color.rgb * _XRayIntensity;
                col += _Color.rgb * fres * _XRayIntensity * 0.5h;

                float alpha = saturate(_XRayFillAlpha * _Color.a);
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
