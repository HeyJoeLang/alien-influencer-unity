Shader "Custom/UnlitTransparentToon"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color (RGBA = tint + alpha)", Color) = (1,1,1,1)

        _Steps ("Toon Steps", Range(1,8)) = 3
        _EdgeSmooth ("Band Smoothness", Range(0,0.25)) = 0.04

        _RimIntensity ("Rim Intensity", Range(0,2)) = 0.0
        _RimPower ("Rim Power", Range(0.5,8)) = 3.0

        [Toggle] _AlphaClip ("Enable Alpha Cutoff", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        // Outline (optional)
        [Toggle] _UseOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness (screen-ish)", Range(0,5)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        // ---------- Outline Pass (optional) ----------
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode"="Always" }
            Cull Front
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ _USEOUTLINE
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            fixed4 _BaseColor;
            float _UseOutline;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 col : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // Expand along view-space normals so thickness stays visually consistent
                float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);

                // Scale thickness with perspective
                float thickness = _OutlineThickness * 0.002; // tweak factor
                viewPos.xyz += viewNormal * thickness;

                o.pos = mul(UNITY_MATRIX_P, viewPos);

                // Outline respects base alpha
                o.col = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_UseOutline < 0.5) discard;
                return i.col;
            }
            ENDCG
        }

        // ---------- Main Pass ----------
        Pass
        {
            Name "FORWARD"
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            fixed4 _BaseColor;

            float _Steps;
            float _EdgeSmooth;

            float _RimIntensity;
            float _RimPower;

            float _AlphaClip;
            float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos        : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 worldN     : TEXCOORD1;
                float3 worldPos   : TEXCOORD2;
                float3 viewDir    : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                o.worldPos = worldPos;
                o.worldN = worldN;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            // Quantize NdotL into bands with optional edge smoothing
            float toonBand(float ndotl, float steps, float smooth)
            {
                steps = max(1.0, steps);
                float t = saturate(ndotl);
                float x = t * steps;
                float baseBand = floor(x);

                // Smooth the edge between bands using fractional part
                float f = x - baseBand;
                float s = smoothstep(0.5 - smooth, 0.5 + smooth, f);
                float q = (baseBand + s) / steps;
                return saturate(q);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_BaseMap, i.uv) * _BaseColor;

                if (_AlphaClip > 0.5 && baseCol.a < _Cutoff) discard;

                // Compute a single main light direction (works for directional lights)
                float3 L;
                // _WorldSpaceLightPos0.w == 0 for directional; otherwise point/spot
                if (_WorldSpaceLightPos0.w == 0)
                {
                    L = normalize(-_WorldSpaceLightPos0.xyz);
                }
                else
                {
                    L = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                }

                float NdotL = max(0, dot(normalize(i.worldN), L));

                // Toon banding
                float litFactor = toonBand(NdotL, _Steps, _EdgeSmooth);

                // Optional rim light
                float rim = pow(1.0 - saturate(dot(normalize(i.worldN), normalize(i.viewDir))), _RimPower) * _RimIntensity;

                float shade = saturate(litFactor + rim);

                fixed3 rgb = baseCol.rgb * shade;
                return fixed4(rgb, baseCol.a);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}
