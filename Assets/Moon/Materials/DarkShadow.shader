// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/FogPlane" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Fade("Fade Intensity", Float) = 30
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
	#pragma multi_compile_particles

	#include "UnityCG.cginc"
			
		fixed4 _TintColor;

		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;

			float4 projPos : TEXCOORD1;

		};


		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			o.projPos = ComputeScreenPos(o.vertex);
			COMPUTE_EYEDEPTH(o.projPos.z);

			o.color = v.color;
			o.texcoord = v.texcoord;
			return o;
		}

		sampler2D _CameraDepthTexture;
		float _Fade;

		fixed4 frag(v2f i) : COLOR
		{
		
		float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
		float partZ = i.projPos.z;
		float e = 2.7;
		float fade = 1-saturate(1 / pow(e, pow((sceneZ - partZ)*(_Fade/30),2)));

		float4 col = _TintColor;

		col.a *= fade;

		return col;
		}
			ENDCG
		}
		}
		}
}