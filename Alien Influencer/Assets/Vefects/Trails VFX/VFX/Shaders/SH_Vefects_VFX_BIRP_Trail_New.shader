// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_BIRP_Trail_New"
{
	Properties
	{
		_EmissiveIntensity("Emissive Intensity", Float) = 1
		_AlphaAffectsOpacity("Alpha Affects Opacity", Float) = 1
		_OverallSpeed("Overall Speed", Float) = 1
		[Space(33)][Header(Color)][Space(13)]_Color("Color", 2D) = "white" {}
		_ColorUVScale("Color UV Scale", Vector) = (1,1,0,0)
		_ColorPanSpeed("Color Pan Speed", Vector) = (0,0,0,0)
		_Color01("Color 01", Color) = (1,1,1,0)
		_Color02("Color 02", Color) = (1,1,1,0)
		_ColorSmoothstep("Color Smoothstep", Float) = 0
		_ColorSmoothstepSmoothness("Color Smoothstep Smoothness", Float) = 1
		[Space(33)][Header(Distortion)][Space(13)]_Distortion("Distortion", 2D) = "white" {}
		_DistortionUVScale("Distortion UV Scale", Vector) = (1,1,0,0)
		_DistortionPanSpeed("Distortion Pan Speed", Vector) = (0,0,0,0)
		_DistortionAmount("Distortion Amount", Float) = 0.1
		[Space(33)][Header(Erosion)][Space(13)]_Erosion("Erosion", 2D) = "white" {}
		_ErosionUVScale("Erosion UV Scale", Vector) = (1,1,0,0)
		_ErosionPanSpeed("Erosion Pan Speed", Vector) = (0,0,0,0)
		_ErosionSmoothstep("Erosion Smoothstep", Float) = 0
		_ErosionSmoothstepSmoothness("Erosion Smoothstep Smoothness", Float) = 1
		[Space(33)][Header(Mask)][Space(13)]_Mask("Mask", 2D) = "white" {}
		_MaskUVScale("Mask UV Scale", Vector) = (1,1,0,0)
		_MaskPanSpeed("Mask Pan Speed", Vector) = (0,0,0,0)
		_MaskDistortionIntensity("Mask Distortion Intensity", Float) = 1
		_MaskSmoothstep("Mask Smoothstep", Float) = 0
		_MaskSmoothstepSmoothness("Mask Smoothstep Smoothness", Float) = 1
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_Src] [_Dst]
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19701
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float _Dst;
		uniform float _Src;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		uniform float _MaskSmoothstep;
		uniform float _MaskSmoothstepSmoothness;
		uniform sampler2D _Mask;
		uniform sampler2D _Distortion;
		uniform float _OverallSpeed;
		uniform float2 _DistortionPanSpeed;
		uniform float2 _DistortionUVScale;
		uniform float _DistortionAmount;
		uniform float _MaskDistortionIntensity;
		uniform float2 _MaskPanSpeed;
		uniform float2 _MaskUVScale;
		uniform float _ErosionSmoothstep;
		uniform float _ErosionSmoothstepSmoothness;
		uniform sampler2D _Erosion;
		uniform float2 _ErosionPanSpeed;
		uniform float2 _ErosionUVScale;
		uniform float4 _Color01;
		uniform float4 _Color02;
		uniform float _ColorSmoothstep;
		uniform float _ColorSmoothstepSmoothness;
		uniform sampler2D _Color;
		uniform float2 _ColorPanSpeed;
		uniform float2 _ColorUVScale;
		uniform float _EmissiveIntensity;
		uniform float _AlphaAffectsOpacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float globalSpeed200 = ( _OverallSpeed * _Time.y );
			float2 panner79 = ( globalSpeed200 * _DistortionPanSpeed + ( i.uv_texcoord * _DistortionUVScale ));
			float Distortion64 = ( tex2D( _Distortion, panner79 ).g * _DistortionAmount );
			float2 uv_TexCoord216 = i.uv_texcoord + float2( 0.28,0 );
			float2 panner377 = ( globalSpeed200 * _MaskPanSpeed + ( uv_TexCoord216 * _MaskUVScale ));
			float4 tex2DNode214 = tex2D( _Mask, ( ( Distortion64 * _MaskDistortionIntensity ) + panner377 ) );
			float smoothstepResult454 = smoothstep( _MaskSmoothstep , ( _MaskSmoothstep + _MaskSmoothstepSmoothness ) , tex2DNode214.g);
			float2 panner78 = ( globalSpeed200 * _ErosionPanSpeed + ( i.uv_texcoord * _ErosionUVScale ));
			float smoothstepResult446 = smoothstep( _ErosionSmoothstep , ( _ErosionSmoothstep + _ErosionSmoothstepSmoothness ) , tex2D( _Erosion, ( panner78 + Distortion64 ) ).g);
			float noises205 = saturate( smoothstepResult446 );
			float temp_output_432_0 = saturate( ( saturate( smoothstepResult454 ) - saturate( ( noises205 - i.vertexColor.a ) ) ) );
			float2 panner320 = ( globalSpeed200 * _ColorPanSpeed + ( i.uv_texcoord * _ColorUVScale ));
			float smoothstepResult458 = smoothstep( _ColorSmoothstep , ( _ColorSmoothstep + _ColorSmoothstepSmoothness ) , tex2D( _Color, panner320 ).g);
			float4 lerpResult285 = lerp( _Color01 , _Color02 , saturate( smoothstepResult458 ));
			float4 Color329 = lerpResult285;
			o.Emission = ( ( ( (i.vertexColor).rgb * temp_output_432_0 ) * (Color329).rgb ) * _EmissiveIntensity );
			float temp_output_309_0 = saturate( temp_output_432_0 );
			float lerpResult444 = lerp( temp_output_309_0 , saturate( ( temp_output_309_0 * i.vertexColor.a ) ) , saturate( _AlphaAffectsOpacity ));
			float op433 = lerpResult444;
			o.Alpha = op433;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.CommentaryNode;296;-5056,-1936;Inherit;False;786;417;Global Speed;4;198;199;197;200;Global Speed;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;197;-5008,-1872;Inherit;False;Property;_OverallSpeed;Overall Speed;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;198;-5008,-1632;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;77;-5456,-2912;Inherit;False;2502.5;663.612;Distortion;10;50;64;52;43;79;32;204;30;31;301;Distortion;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-4752,-1888;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-5248,-2768;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;31;-5248,-2640;Inherit;False;Property;_DistortionUVScale;Distortion UV Scale;12;0;Create;True;0;0;0;False;0;False;1,1;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;-4496,-1888;Inherit;False;globalSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-4512,-2496;Inherit;False;200;globalSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-4848,-2768;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;32;-4608,-2656;Inherit;False;Property;_DistortionPanSpeed;Distortion Pan Speed;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;79;-4352,-2768;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;210;-5216,-1136;Inherit;False;3074.332;597.0625;Erosion;15;205;24;54;78;69;29;297;203;26;25;446;447;448;449;450;Erosion;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3456,-2560;Inherit;False;Property;_DistortionAmount;Distortion Amount;14;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-4096,-2816;Inherit;True;Property;_Distortion;Distortion;11;0;Create;True;0;0;0;False;3;Space(33);Header(Distortion);Space(13);False;-1;3327b545e30728e47b34b9588ff64aef;3327b545e30728e47b34b9588ff64aef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-4976,-1008;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-3456,-2784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;25;-5056,-848;Inherit;False;Property;_ErosionUVScale;Erosion UV Scale;16;0;Create;True;0;0;0;False;0;False;1,1;0.25,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;-4720,-1008;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;29;-4592,-880;Inherit;False;Property;_ErosionPanSpeed;Erosion Pan Speed;17;0;Create;True;0;0;0;False;0;False;0,0;0.25,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;203;-4336,-752;Inherit;False;200;globalSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;64;-3200,-2768;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-4080,-752;Inherit;False;64;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;78;-4336,-1008;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;313;-4771.846,206;Inherit;False;2128.653;489.2563;Mask;12;214;271;377;378;216;379;217;441;451;453;454;452;Mask;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-4080,-1008;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;447;-3712,-768;Inherit;False;Property;_ErosionSmoothstep;Erosion Smoothstep;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;448;-3712,-640;Inherit;False;Property;_ErosionSmoothstepSmoothness;Erosion Smoothstep Smoothness;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;216;-4656,272;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.28,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;217;-4448,448;Inherit;False;Property;_MaskUVScale;Mask UV Scale;21;0;Create;True;0;0;0;False;0;False;1,1;0.25,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;24;-3824,-1008;Inherit;True;Property;_Erosion;Erosion;15;0;Create;True;0;0;0;False;3;Space(33);Header(Erosion);Space(13);False;-1;3327b545e30728e47b34b9588ff64aef;3327b545e30728e47b34b9588ff64aef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;449;-3328,-640;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;272;-4416.335,-202.6369;Inherit;False;64;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;378;-4070.613,542.5769;Inherit;False;200;globalSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;379;-4082.303,374.3753;Inherit;False;Property;_MaskPanSpeed;Mask Pan Speed;22;0;Create;True;0;0;0;False;0;False;0,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;441;-4353.649,294.2913;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-4416.335,-74.63676;Inherit;False;Property;_MaskDistortionIntensity;Mask Distortion Intensity;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;446;-3328,-768;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;328;-2892.27,-2886.208;Inherit;False;2718.794;1253.04;Color;16;329;285;287;260;283;320;318;319;327;316;317;455;456;457;458;459;Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.PannerNode;377;-3835.945,269.9912;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;274;-3904.338,-202.6369;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;450;-2688,-896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;271;-3587,251;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-2432,-1008;Inherit;False;noises;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;451;-3200,432;Inherit;False;Property;_MaskSmoothstep;Mask Smoothstep;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;452;-3200,560;Inherit;False;Property;_MaskSmoothstepSmoothness;Mask Smoothstep Smoothness;25;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;317;-2801.907,-2313.482;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;316;-2801.907,-2185.482;Inherit;False;Property;_ColorUVScale;Color UV Scale;5;0;Create;True;0;0;0;False;0;False;1,1;0.1,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;214;-3444.538,224.8561;Inherit;True;Property;_Mask;Mask;20;0;Create;True;0;0;0;False;3;Space(33);Header(Mask);Space(13);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.VertexColorNode;306;-1408,-256;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;453;-2816,560;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;367;-896,0;Inherit;False;205;noises;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-2454.906,-2312.482;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;327;-2285.027,-2054.01;Inherit;False;200;globalSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;318;-2320,-2208;Inherit;False;Property;_ColorPanSpeed;Color Pan Speed;6;0;Create;True;0;0;0;False;0;False;0,0;0.2,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;368;-896,128;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;454;-2816,432;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;320;-2107.907,-2312.482;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;455;-1536,-2048;Inherit;False;Property;_ColorSmoothstep;Color Smoothstep;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;456;-1536,-1920;Inherit;False;Property;_ColorSmoothstepSmoothness;Color Smoothstep Smoothness;10;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;440;-768,128;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;439;-2560,256;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;283;-1920,-2304;Inherit;True;Property;_Color;Color;4;0;Create;True;0;0;0;False;3;Space(33);Header(Color);Space(13);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;457;-1152,-1920;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;394;-512,384;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;458;-1152,-2048;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;432;-256,384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;287;-1912.423,-2836.208;Inherit;False;Property;_Color01;Color 01;7;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,0.1132353,0.1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;260;-1912.423,-2580.208;Inherit;False;Property;_Color02;Color 02;8;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.1019608,0.01019608,0.01019608,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode;459;-896,-2048;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;309;-128,640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;285;-1528.422,-2836.208;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;442;128,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;445;640,896;Inherit;False;Property;_AlphaAffectsOpacity;Alpha Affects Opacity;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;329;-845.6066,-2835.616;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;443;256,1280;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;466;896,896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;405;-1024,-256;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;330;256,0;Inherit;False;329;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;444;640,768;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;364;512,0;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;412;0,-256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;426;2768,-48;Inherit;False;1253;162.95;Lush was here! <3;5;431;430;429;428;427;Lush was here! <3;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;433;1152,896;Inherit;False;op;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;512,-256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;240;1168,-144;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;461;-3712,1152;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;463;-3456,1280;Inherit;False;Property;_CutoutMaskOffset;Cutout Mask Offset;27;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;462;-3328,1152;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;460;-3072,1152;Inherit;True;Property;_CutoutMask;Cutout Mask;26;0;Create;True;0;0;0;False;3;Space(33);Header(Cutout Mask);Space(13);False;-1;7c48f5b92e0b90d40ba17c1418e880e7;7c48f5b92e0b90d40ba17c1418e880e7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;464;-2432,1152;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;465;-2176,1152;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;427;3328,0;Inherit;False;Property;_Dst;Dst;30;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;428;3072,0;Inherit;False;Property;_Src;Src;29;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;429;3584,0;Inherit;False;Property;_ZWrite;ZWrite;31;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;430;3840,0;Inherit;False;Property;_ZTest;ZTest;32;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;431;2816,0;Inherit;False;Property;_Cull;Cull;28;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;434;1792,256;Inherit;False;433;op;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;236;1408,-256;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;468;2081.931,3.282921;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Vefects/SH_Vefects_VFX_BIRP_Trail_New;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite;0;True;_ZTest;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;True;_Src;10;True;_Dst;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;199;0;197;0
WireConnection;199;1;198;0
WireConnection;200;0;199;0
WireConnection;301;0;30;0
WireConnection;301;1;31;0
WireConnection;79;0;301;0
WireConnection;79;2;32;0
WireConnection;79;1;204;0
WireConnection;43;1;79;0
WireConnection;50;0;43;2
WireConnection;50;1;52;0
WireConnection;297;0;26;0
WireConnection;297;1;25;0
WireConnection;64;0;50;0
WireConnection;78;0;297;0
WireConnection;78;2;29;0
WireConnection;78;1;203;0
WireConnection;54;0;78;0
WireConnection;54;1;69;0
WireConnection;24;1;54;0
WireConnection;449;0;447;0
WireConnection;449;1;448;0
WireConnection;441;0;216;0
WireConnection;441;1;217;0
WireConnection;446;0;24;2
WireConnection;446;1;447;0
WireConnection;446;2;449;0
WireConnection;377;0;441;0
WireConnection;377;2;379;0
WireConnection;377;1;378;0
WireConnection;274;0;272;0
WireConnection;274;1;273;0
WireConnection;450;0;446;0
WireConnection;271;0;274;0
WireConnection;271;1;377;0
WireConnection;205;0;450;0
WireConnection;214;1;271;0
WireConnection;453;0;451;0
WireConnection;453;1;452;0
WireConnection;319;0;317;0
WireConnection;319;1;316;0
WireConnection;368;0;367;0
WireConnection;368;1;306;4
WireConnection;454;0;214;2
WireConnection;454;1;451;0
WireConnection;454;2;453;0
WireConnection;320;0;319;0
WireConnection;320;2;318;0
WireConnection;320;1;327;0
WireConnection;440;0;368;0
WireConnection;439;0;454;0
WireConnection;283;1;320;0
WireConnection;457;0;455;0
WireConnection;457;1;456;0
WireConnection;394;0;439;0
WireConnection;394;1;440;0
WireConnection;458;0;283;2
WireConnection;458;1;455;0
WireConnection;458;2;457;0
WireConnection;432;0;394;0
WireConnection;459;0;458;0
WireConnection;309;0;432;0
WireConnection;285;0;287;0
WireConnection;285;1;260;0
WireConnection;285;2;459;0
WireConnection;442;0;309;0
WireConnection;442;1;306;4
WireConnection;329;0;285;0
WireConnection;443;0;442;0
WireConnection;466;0;445;0
WireConnection;405;0;306;0
WireConnection;444;0;309;0
WireConnection;444;1;443;0
WireConnection;444;2;466;0
WireConnection;364;0;330;0
WireConnection;412;0;405;0
WireConnection;412;1;432;0
WireConnection;433;0;444;0
WireConnection;307;0;412;0
WireConnection;307;1;364;0
WireConnection;462;0;461;0
WireConnection;462;1;463;0
WireConnection;460;1;462;0
WireConnection;464;0;214;2
WireConnection;464;1;460;2
WireConnection;465;0;464;0
WireConnection;236;0;307;0
WireConnection;236;1;240;0
WireConnection;468;2;236;0
WireConnection;468;9;434;0
ASEEND*/
//CHKSM=85DCA342F79D0415B1437346462D8D9174149AC4