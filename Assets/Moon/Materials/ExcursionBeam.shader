// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ExcursionFunnel" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Swirl Texture", 2D) = "white" {}
		_Particles("Particles Texture", 2D) = "black" {}
		_Fade("Soft Particles Distance", Range(0.01,20.0)) = 1.0
		_NFade("Near Fade Distance", Range(0.01,20.0)) = 1.0
		_FR("Near Fade Range", Range(0.01,20.0)) = 1.0
		_Speed("Flow Speed", Range(-10,10)) = -0.5
		_Distort("Flow Distortion", Range(0.01,1)) = 0.1
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
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
	

	#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Particles;
			half _NFade;
			half _FR;
		fixed4 _TintColor;

		half _Speed;

		struct appdata_t {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 vert : TEXCOORD2;
			float4 projPos : TEXCOORD1;
	

		};

		float4 _MainTex_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			o.projPos = ComputeScreenPos(o.vertex);
			COMPUTE_EYEDEPTH(o.projPos.z);
			o.vert = v.vertex;
			o.normal = v.normal;
			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}

		sampler2D _CameraDepthTexture;
		float _Fade;

		fixed4 frag(v2f i) : COLOR
		{
		half glow = 0;


		half2 swirl = tex2D(_MainTex, i.texcoord + float2(0, _Time.r* _Speed)).rg;
		half noise = tex2D(_MainTex, i.texcoord*2).b;

		half p_lines = tex2D(_Particles, i.texcoord*float2(2,1) + float2(0, _Time.r*- _Speed*0.4)).r;
		half p_dots = tex2D(_Particles, i.texcoord + float2(0, _Time.r*_Speed*1.5)).g;

		glow = swirl.r + lerp(swirl.g,swirl.g*noise*noise,0.7) + p_lines*0.5 + p_dots*0.7;



		float3 viewDir = normalize(ObjSpaceViewDir(i.vert));
		float dotProduct = abs(dot(i.normal, viewDir));
		float c = 1 - dotProduct;
		glow += c*0.3;
		float rimWidth = 1.5;
		half rim = smoothstep(1 - rimWidth, 1.0, dotProduct);




			//tex2D(_MainTex, i.texcoord)
		float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
		float partZ = i.projPos.z;
		float fade = saturate((sceneZ - partZ) / _Fade);
		fade *= saturate((partZ - _NFade) / _FR);

		i.color *= i.color;

		glow *= rim;

		//return rim;
		float ou = i.color * glow;
		half4 col = _TintColor;
		col.a *= ou * fade;
		return col;
		}
			ENDCG
		}
		}
		}
}