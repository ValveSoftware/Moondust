// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PortalCenter" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Pattern Texture", 2D) = "white" {}
		_Bump("Flow Normals Texture", 2D) = "bump" {}
		_Speed("Flow Speed", Range(-10,10)) = -0.5
		_Distort("Flow Distortion", Range(0.01,1)) = 0.1
	}

		Category{
		Tags{  "IgnoreProjector" = "True" }
		
		
		ColorMask RGB
		Lighting Off Fog{ Color(0,0,0,0) }
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
			sampler2D _Bump;
			half _Distort;
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

		};

		float4 _MainTex_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);


			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}


		fixed4 frag(v2f i) : COLOR
		{
		half glow = 0;
		float2 flow = UnpackNormal(tex2D(_Bump, i.texcoord / 5 + float2(0, _Time.r* _Speed))).xy;


		glow = tex2D(_MainTex, i.texcoord + flow * _Distort + float2(0, _Time.r* _Speed * -2)).r;

		
		glow *= glow;
		//i.color *= i.color;

		return 2.0f * _TintColor * glow;


		}
			ENDCG
		}
		}
		}
}