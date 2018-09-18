// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "FX/Aura" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Noise Texture", 2D) = "white" {}
	_Grad ("Color gradient", 2D) = "white" {}
	_AllExp ("Overall Exponent", Range(0.01,5.0)) = 1.0

}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	Blend One One
	ColorMask RGB
	Cull Off
	Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

				
			sampler2D _MainTex;
			sampler2D _Grad;
			fixed4 _TintColor;

			struct appdata_t {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				//float3 viewDir;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


			struct v2f {
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				//if(o.rim>1)
				//o.rim = 0;
				return o;
			}

			float _InvFade;

			float _NearFade;

			float _AllExp;

			fixed4 frag (v2f i) : SV_Target

			{
				i.color = clamp(i.color, 0.0001,1);
				float powrim =pow(i.color.r *1,1);

				float tex = (tex2D(_MainTex, i.texcoord + float2(0,_Time.r*4 ))+0.1)* powrim ;

				tex = pow(tex,_AllExp);

				fixed4 col = float4(tex2D(_Grad, float2(tex,0.5)).rgb,1);

				//col.a = tex;

				if(tex*tex<0.3){
				col=0;
				}


				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				//float r
				return (saturate(col/2)*2) * _TintColor ;
			}

			ENDCG 
		}
	}	
}
}
