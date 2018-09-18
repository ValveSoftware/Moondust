// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "FX/PortalEdge" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Noise Texture", 2D) = "white" {}
	_Grad ("Color gradient", 2D) = "white" {}
	_Nrm ("Distortion", 2D) = "bump" {}
	_AllExp ("Overall Exponent", Range(0.01,5.0)) = 1.0
	_Dist ("Distortion Multiplier", Range(0.01,2.0)) = 1.0

}

Category {
	Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	Cull Back
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
			sampler2D _Nrm;

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
				float2 texcoord1 : TEXCOORD4;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _MainTex_ST;
			float4 _Nrm_ST;

			half _Dist;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord,_Nrm);
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
				float powrim =pow(i.color.r *1.06f,4);
				float softrim =saturate(pow(i.color.r*2,2));

				float3 nrm = UnpackNormal(tex2D (_Nrm, i.texcoord + float2(_Time.r*0.1,0))) * 0.02 * _Dist;

				float tex = (tex2D(_MainTex, i.texcoord + float2(0,_Time.r*0.2 ) + nrm.xy)/1+0.1) * i.color * 2;

				tex = pow(tex,_AllExp) + powrim*1;
				
				fixed4 col =(1+powrim);

				col.a = tex*0.8;

				col.rgb = tex2D(_Grad, float2(col.a - 0.6,0.5)).rgb *2;

				col.a *= softrim;

				col.a = pow(col.a,0.4);



				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				//float r
				return (col*1) ;
			}

			ENDCG 
		}
	}	
}
}
