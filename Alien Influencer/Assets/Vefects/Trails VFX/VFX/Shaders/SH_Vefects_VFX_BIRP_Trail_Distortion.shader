// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_BIRP_Trail_Distortion"
{
	Properties
	{
		_DistortionIntensity("Distortion Intensity", Float) = 1
		_NoiseMultiply("Noise Multiply", Float) = 1
		_Erosion("Erosion", Float) = 0
		_ErosionSmoothness("Erosion Smoothness", Float) = 1
		_OpacityMultiply("Opacity Multiply", Float) = 1
		[Space(33)][Header(Noise)][Space(13)]_NoiseTexture("Noise Texture", 2D) = "white" {}
		_Noise01UVS("Noise 01 UV S", Vector) = (1,1,0,0)
		_Noise01UVP("Noise 01 UV P", Vector) = (0.1,0.3,0,0)
		_DistortionLerp("Distortion Lerp", Float) = 1
		_Noise01Intensity("Noise 01 Intensity", Float) = -1
		[Space(33)][Header(Intensity Mask)][Space(13)]_IntensityMask("Intensity Mask", 2D) = "black" {}
		_IntensityMaskUVS("Intensity Mask UV S", Vector) = (1,1,0,0)
		_IntensityMaskPanSpeed("Intensity Mask Pan Speed", Vector) = (0,0,0,0)
		_IntensityMaskOffset("Intensity Mask Offset", Vector) = (0,0,0,0)
		_IntensityMaskPower("Intensity Mask Power", Float) = 1
		_IntensityMaskMultiply("Intensity Mask Multiply", Float) = 1
		[Space(33)][Header(Depth Fade)][Space(13)]_DepthFade("Depth Fade", Float) = 0
		[Space(33)][Header(Camera Depth Fade)][Space(13)]_CameraDepthFadeLength("Camera Depth Fade Length", Float) = 3
		_CameraDepthFadeOffset("Camera Depth Fade Offset", Float) = 0.1
		[Space(33)][Header(Opacity Cutout Mask)][Space(13)]_OpacityMask("Opacity Mask", 2D) = "white" {}
		_OpacityMaskPower("Opacity Mask Power", Float) = 1
		_OpacityMaskMultiply("Opacity Mask Multiply", Float) = 1
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 0
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_Src] [_Dst]
		
		GrabPass{ }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19701
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		struct Input
		{
			float4 screenPos;
			float2 uv2_texcoord2;
			float2 uv_texcoord;
			float eyeDepth;
			float4 vertexColor : COLOR;
		};

		uniform float _Dst;
		uniform float _Src;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Erosion;
		uniform float _ErosionSmoothness;
		uniform sampler2D _IntensityMask;
		uniform float2 _IntensityMaskPanSpeed;
		uniform float2 _IntensityMaskUVS;
		uniform float2 _IntensityMaskOffset;
		uniform sampler2D _NoiseTexture;
		uniform float2 _Noise01UVP;
		uniform float2 _Noise01UVS;
		uniform float _Noise01Intensity;
		uniform float _NoiseMultiply;
		uniform float _IntensityMaskPower;
		uniform float _IntensityMaskMultiply;
		uniform sampler2D _OpacityMask;
		uniform float4 _OpacityMask_ST;
		uniform float _OpacityMaskPower;
		uniform float _OpacityMaskMultiply;
		uniform float _CameraDepthFadeLength;
		uniform float _CameraDepthFadeOffset;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFade;
		uniform float _OpacityMultiply;
		uniform float _DistortionLerp;
		uniform float _DistortionIntensity;


inline float4 ASE_ComputeGrabScreenPos( float4 pos )
{
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	float4 o = pos;
	o.y = pos.w * 0.5f;
	o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
	return o;
}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult8 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
			float eros67 = _Erosion;
			float2 temp_output_65_0 = ( i.uv2_texcoord2 * _IntensityMaskUVS );
			float2 panner199 = ( 1.0 * _Time.y * _IntensityMaskPanSpeed + ( temp_output_65_0 + _IntensityMaskOffset ));
			float2 panner59 = ( 1.0 * _Time.y * _Noise01UVP + ( i.uv_texcoord * _Noise01UVS ));
			float randomOffset163 = 0.0;
			float noiseMultiply51 = _NoiseMultiply;
			float smoothstepResult82 = smoothstep( eros67 , ( eros67 + _ErosionSmoothness ) , tex2D( _IntensityMask, ( panner199 + ( tex2D( _NoiseTexture, ( panner59 + ( randomOffset163 * 0.173 ) ) ).r * ( _Noise01Intensity * noiseMultiply51 ) ) ) ).g);
			float temp_output_100_0 = saturate( ( saturate( pow( smoothstepResult82 , _IntensityMaskPower ) ) * _IntensityMaskMultiply ) );
			float2 uv_OpacityMask = i.uv_texcoord * _OpacityMask_ST.xy + _OpacityMask_ST.zw;
			float temp_output_159_0 = saturate( ( saturate( pow( tex2D( _OpacityMask, uv_OpacityMask ).g , _OpacityMaskPower ) ) * _OpacityMaskMultiply ) );
			float cameraDepthFade187 = (( i.eyeDepth -_ProjectionParams.y - _CameraDepthFadeOffset ) / _CameraDepthFadeLength);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth111 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth111 = saturate( ( screenDepth111 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			float temp_output_141_0 = saturate( ( saturate( ( saturate( ( saturate( ( temp_output_100_0 * temp_output_159_0 ) ) * saturate( cameraDepthFade187 ) ) ) * distanceDepth111 ) ) * _OpacityMultiply ) );
			float2 temp_cast_0 = (temp_output_141_0).xx;
			float distortionIntensity137 = _DistortionIntensity;
			float2 lerpResult33 = lerp( float2( 0,0 ) , temp_cast_0 , ( _DistortionLerp * distortionIntensity137 ));
			float4 screenColor9 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( appendResult8 + lerpResult33 ));
			o.Emission = screenColor9.rgb;
			o.Alpha = i.vertexColor.a;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
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
				float4 customPack1 : TEXCOORD1;
				float1 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
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
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv2_texcoord2;
				o.customPack1.xy = v.texcoord1;
				o.customPack1.zw = customInputData.uv_texcoord;
				o.customPack1.zw = v.texcoord;
				o.customPack2.x = customInputData.eyeDepth;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.uv2_texcoord2 = IN.customPack1.xy;
				surfIN.uv_texcoord = IN.customPack1.zw;
				surfIN.eyeDepth = IN.customPack2.x;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
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
Node;AmplifyShaderEditor.RangedFloatNode;196;-3712,-640;Inherit;False;Constant;_Float0;Float 0;32;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-11904,-256;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;49;-11520,-128;Inherit;False;Property;_Noise01UVS;Noise 01 UV S;10;0;Create;True;0;0;0;False;0;False;1,1;0.3,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-3712,-768;Inherit;False;randomOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-11520,-256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-3968,-1024;Inherit;False;Property;_NoiseMultiply;Noise Multiply;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;53;-11264,-128;Inherit;False;Property;_Noise01UVP;Noise 01 UV P;11;0;Create;True;0;0;0;False;0;False;0.1,0.3;-0.5,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;169;-11008,-512;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-3712,-1024;Inherit;False;noiseMultiply;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;59;-11264,-256;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-11008,-384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.173;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;-9984,-640;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;62;-9472,-512;Inherit;False;Property;_IntensityMaskUVS;Intensity Mask UV S;19;0;Create;True;0;0;0;False;0;False;1,1;1,1.333;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;58;-10240,128;Inherit;False;51;noiseMultiply;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;-11008,-256;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;60;-10624,-512;Inherit;True;Property;_NoiseTexture;Noise Texture;9;0;Create;True;0;0;0;False;3;Space(33);Header(Noise);Space(13);False;3327b545e30728e47b34b9588ff64aef;3327b545e30728e47b34b9588ff64aef;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;54;-10240,0;Inherit;False;Property;_Noise01Intensity;Noise 01 Intensity;16;0;Create;True;0;0;0;False;0;False;-1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-9472,-640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;198;-9216,-512;Inherit;False;Property;_IntensityMaskOffset;Intensity Mask Offset;21;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;63;-10624,-256;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-9984,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;-3968,-896;Inherit;False;Property;_Erosion;Erosion;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;197;-9216,-640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;200;-8960,-512;Inherit;False;Property;_IntensityMaskPanSpeed;Intensity Mask Pan Speed;20;0;Create;True;0;0;0;False;0;False;0,0;-1,0.113;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-10112,-256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-3712,-896;Inherit;False;eros;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;199;-8960,-640;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-7424,0;Inherit;False;Property;_ErosionSmoothness;Erosion Smoothness;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-7424,-384;Inherit;False;67;eros;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-8576,-640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-7424,-128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;77;-8320,-640;Inherit;True;Property;_IntensityMask;Intensity Mask;18;0;Create;True;0;0;0;False;3;Space(33);Header(Intensity Mask);Space(13);False;-1;fa6235bc65ac4064f8d49fcc0c8ff503;fa6235bc65ac4064f8d49fcc0c8ff503;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;79;-7040,-384;Inherit;False;Property;_IntensityMaskPower;Intensity Mask Power;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;82;-7424,-256;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-6784,1792;Inherit;False;Property;_OpacityMaskPower;Opacity Mask Power;28;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;101;-7168,1536;Inherit;True;Property;_OpacityMask;Opacity Mask;27;0;Create;True;0;0;0;False;3;Space(33);Header(Opacity Cutout Mask);Space(13);False;-1;171b695a70c8bb2439da87f69178c81b;171b695a70c8bb2439da87f69178c81b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PowerNode;83;-7040,-256;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;156;-6784,1664;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-6528,-384;Inherit;False;Property;_IntensityMaskMultiply;Intensity Mask Multiply;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;86;-6784,-256;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;157;-6656,1664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-6528,1792;Inherit;False;Property;_OpacityMaskMultiply;Opacity Mask Multiply;29;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-6528,-256;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-6528,1664;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;100;-6272,-256;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;159;-6400,1664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-5376,1920;Inherit;False;Property;_CameraDepthFadeLength;Camera Depth Fade Length;25;0;Create;True;0;0;0;False;3;Space(33);Header(Camera Depth Fade);Space(13);False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;193;-5376,2048;Inherit;False;Property;_CameraDepthFadeOffset;Camera Depth Fade Offset;26;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;187;-5376,1664;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-6016,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;190;-5120,1664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;113;-5760,1536;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-5376,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-4736,1792;Inherit;False;Property;_DepthFade;Depth Fade;24;0;Create;True;0;0;0;False;3;Space(33);Header(Depth Fade);Space(13);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;111;-4736,1664;Inherit;False;True;True;False;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;189;-5120,1536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-3968,-1152;Inherit;False;Property;_DistortionIntensity;Distortion Intensity;1;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-4736,1536;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-4096,1792;Inherit;False;Property;_OpacityMultiply;Opacity Multiply;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-3712,-1152;Inherit;False;distortionIntensity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;123;-4480,1536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-4096,1536;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1024,512;Inherit;False;Property;_DistortionLerp;Distortion Lerp;12;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-1024,640;Inherit;False;137;distortionIntensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;141;-3840,1536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-768,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;31;-1280,128;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GrabScreenPosition;7;-1664,0;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;8;-1408,0;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;33;-1024,256;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;45;462,-50;Inherit;False;1253;162.95;Lush was here! <3;5;3;2;4;5;1;Lush was here! <3;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-1024,0;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-4224,384;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;140;-3840,384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;9;-768,0;Inherit;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;16;-384,256;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;1024,0;Inherit;False;Property;_Dst;Dst;32;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;768,0;Inherit;False;Property;_Src;Src;31;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;1280,0;Inherit;False;Property;_ZWrite;ZWrite;33;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;1537,0;Inherit;False;Property;_ZTest;ZTest;34;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;512,0;Inherit;False;Property;_Cull;Cull;30;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;164;-3968,-768;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-3328,384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-11264,0;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-10752,0;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-11008,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.31;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;136;-7424,768;Inherit;True;Property;_TextureSample5;Texture Sample 3;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexturePropertyNode;112;-7424,1024;Inherit;True;Property;_BaseTexture;Base Texture;6;0;Create;True;0;0;0;False;3;Space(33);Header(Base Texture);Space(13);False;3327b545e30728e47b34b9588ff64aef;3327b545e30728e47b34b9588ff64aef;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-6016,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;195;-5760,1280;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;-9456,-928;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;-9456,-1056;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;15;-3088,400;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;70;-11904,1024;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;71;-11520,1152;Inherit;False;Property;_NoiseDistUVS;Noise Dist UV S;14;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-11520,1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;84;-10624,1024;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ComponentMaskNode;89;-10240,1024;Inherit;True;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;91;-9728,1408;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;95;-9984,1024;Inherit;True;ConstantBiasScale;-1;;1;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;-0.5;False;2;FLOAT;2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-9344,1408;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-9344,1024;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;103;-8832,1408;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;76;-11264,1152;Inherit;False;Property;_NoiseDistUVP;Noise Dist UV P;15;0;Create;True;0;0;0;False;0;False;-0.2,-0.1;0.02,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;80;-11264,1024;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;171;-11008,1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;174;-8576,1408;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-8576,1152;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;107;-8064,1024;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-8576,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3.333;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;90;-9728,1664;Inherit;False;Property;_BaseTextureUVS;Base Texture UV S;7;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;98;-9088,1664;Inherit;False;Property;_BaseTextureUVP;Base Texture UV P;8;0;Create;True;0;0;0;False;0;False;-0.2,-0.1;-0.05,-0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;96;-9728,1152;Inherit;False;Property;_BaseTextureDistortionIntensity;Base Texture Distortion Intensity;17;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-11008,768;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;-11008,896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.777;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-11008,1408;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;178;-11520,1408;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-9088,1792;Inherit;False;163;randomOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;181;-8576,1792;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;-11264,1408;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.137;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-8832,1792;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.07;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;81;-10624,768;Inherit;True;Property;_NoiseDistortionTexture;Noise Distortion Texture;13;1;[Normal];Create;True;0;0;0;False;3;Space(33);Header(Noise Distortion);Space(13);False;23d93a568484acf42a53f25e56beb838;23d93a568484acf42a53f25e56beb838;True;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;202;0,0;Float;False;True;-1;3;ASEMaterialInspector;0;0;Unlit;Vefects/SH_Vefects_VFX_BIRP_Trail_Distortion;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;True;_ZWrite;0;True;_ZTest;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;True;_Src;10;True;_Dst;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;163;0;196;0
WireConnection;52;0;48;0
WireConnection;52;1;49;0
WireConnection;51;0;154;0
WireConnection;59;0;52;0
WireConnection;59;2;53;0
WireConnection;170;0;169;0
WireConnection;168;0;59;0
WireConnection;168;1;170;0
WireConnection;65;0;61;0
WireConnection;65;1;62;0
WireConnection;63;0;60;0
WireConnection;63;1;168;0
WireConnection;64;0;54;0
WireConnection;64;1;58;0
WireConnection;197;0;65;0
WireConnection;197;1;198;0
WireConnection;69;0;63;1
WireConnection;69;1;64;0
WireConnection;67;0;155;0
WireConnection;199;0;197;0
WireConnection;199;2;200;0
WireConnection;72;0;199;0
WireConnection;72;1;69;0
WireConnection;78;0;74;0
WireConnection;78;1;73;0
WireConnection;77;1;72;0
WireConnection;82;0;77;2
WireConnection;82;1;74;0
WireConnection;82;2;78;0
WireConnection;83;0;82;0
WireConnection;83;1;79;0
WireConnection;156;0;101;2
WireConnection;156;1;160;0
WireConnection;86;0;83;0
WireConnection;157;0;156;0
WireConnection;93;0;86;0
WireConnection;93;1;85;0
WireConnection;162;0;157;0
WireConnection;162;1;161;0
WireConnection;100;0;93;0
WireConnection;159;0;162;0
WireConnection;187;0;192;0
WireConnection;187;1;193;0
WireConnection;106;0;100;0
WireConnection;106;1;159;0
WireConnection;190;0;187;0
WireConnection;113;0;106;0
WireConnection;188;0;113;0
WireConnection;188;1;190;0
WireConnection;111;0;105;0
WireConnection;189;0;188;0
WireConnection;118;0;189;0
WireConnection;118;1;111;0
WireConnection;137;0;152;0
WireConnection;123;0;118;0
WireConnection;125;0;123;0
WireConnection;125;1;120;0
WireConnection;141;0;125;0
WireConnection;42;0;44;0
WireConnection;42;1;138;0
WireConnection;8;0;7;1
WireConnection;8;1;7;2
WireConnection;33;0;31;0
WireConnection;33;1;141;0
WireConnection;33;2;42;0
WireConnection;32;0;8;0
WireConnection;32;1;33;0
WireConnection;130;0;136;2
WireConnection;130;1;100;0
WireConnection;140;0;130;0
WireConnection;9;0;32;0
WireConnection;139;0;140;0
WireConnection;139;1;141;0
WireConnection;185;0;53;0
WireConnection;185;1;186;0
WireConnection;186;0;184;0
WireConnection;136;0;112;0
WireConnection;136;1;107;0
WireConnection;194;0;136;2
WireConnection;194;1;159;0
WireConnection;195;0;194;0
WireConnection;165;0;65;0
WireConnection;165;1;167;0
WireConnection;15;0;139;0
WireConnection;75;0;70;0
WireConnection;75;1;71;0
WireConnection;84;0;81;0
WireConnection;84;1;171;0
WireConnection;89;0;84;0
WireConnection;95;3;89;0
WireConnection;97;0;91;0
WireConnection;97;1;90;0
WireConnection;102;0;95;0
WireConnection;102;1;96;0
WireConnection;103;0;97;0
WireConnection;103;2;98;0
WireConnection;80;0;75;0
WireConnection;80;2;76;0
WireConnection;171;0;80;0
WireConnection;171;1;172;0
WireConnection;174;0;103;0
WireConnection;174;1;176;0
WireConnection;107;0;102;0
WireConnection;107;1;103;0
WireConnection;176;0;175;0
WireConnection;172;0;173;0
WireConnection;180;0;76;0
WireConnection;180;1;179;0
WireConnection;181;0;98;0
WireConnection;181;1;182;0
WireConnection;179;0;178;0
WireConnection;182;0;183;0
WireConnection;202;2;9;0
WireConnection;202;9;16;4
ASEEND*/
//CHKSM=2955C3F1959DCA4F4E2F44C35B470150912659E5