// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/EmancipationGrill" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Pattern Texture", 2D) = "white" {}
		_Mask("Noise Texture", 2D) = "white" {}
		_Bump("Flow Normals Texture", 2D) = "bump" {}
		_Fade("Soft Particles Distance", Range(0.01,20.0)) = 1.0
		_Speed("Flow Speed", Range(-10,10)) = -0.5
		_Distort("Flow Distortion", Range(0.01,1)) = 0.1

		_NearMul("Near Object Distance", Float) = 5
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }
		BindChannels{
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

			// ---- Fragment program cards
			SubShader{
			Pass{

			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile_particles

	#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Bump;
			sampler2D _Mask;
			half _Distort;
			half _NearMul;
		fixed4 _TintColor;

		half _Speed;

		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			float3 worldPos : TEXCOORD2;
			float4 projPos : TEXCOORD1;

		};

		float4 _MainTex_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			o.projPos = ComputeScreenPos(o.vertex);
			COMPUTE_EYEDEPTH(o.projPos.z);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}

		float3 hotPos;

		sampler2D _CameraDepthTexture;
		float _Fade;

		fixed4 frag(v2f i) : COLOR
		{
		half glow = 0;

		float near = pow(saturate(1-(length(i.worldPos - hotPos) / _NearMul)),1);

		float addnear = 1 + near * 2;


		float2 flow = UnpackNormal(tex2D(_Bump, i.texcoord / 7)).xy;

		float mask = tex2D(_Mask, i.texcoord / 10 + float2(0, _Time.r* _Speed / 2)).r + 0.3f;

		glow = tex2D(_MainTex, i.texcoord + flow * _Distort + float2(0,_Time.r* _Speed)).r * mask * addnear + (near*5*_TintColor);


			//tex2D(_MainTex, i.texcoord)
		float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
		float partZ = i.projPos.z;
		float fade = saturate((sceneZ - partZ) / (_Fade / lerp(1,10,near)));
		i.color.a *= fade;

		i.color *= i.color;

		return 2.0f * i.color * _TintColor * glow;
		}
			ENDCG
		}
		}
		}
}