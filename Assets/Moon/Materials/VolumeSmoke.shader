// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FX/Zulubo's smokin' hot smoke shader" {
	Properties {
		_MainColor ("Base Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Emission ("Emission", Range(0.0,20.0)) = 0.0
		_FresnelP ("Fresnel Power", Range(0.1,4)) = 1
		_FresnelO ("Fresnel Offset", Range(-0.9,-0.2)) = -0.7

			_Fade("Soft Particles Distance", Range(0.01,20.0)) = 1.0
			_NFade("Near Fade Distance", Range(0.01,20.0)) = 1.0
			_FR("Near Fade Range", Range(0.01,20.0)) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200

		Cull Back
		
		CGPROGRAM
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
		#pragma exclude_renderers gles
		#pragma surface surf WrapLambert alpha
		#pragma vertex vert

#pragma target 4.0


#include "UnityCG.cginc"


		
		half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten)
		{
          half NdotL = dot (s.Normal, lightDir);
          half diff = NdotL * 0.3 + 0.7;
          half4 c;
          c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
          c.a = s.Alpha;
          return c;
      	}
		
		fixed4 _MainColor;
		sampler2D _MainTex;
		half _FresnelP;
		half _FresnelO;
		half _Emission;

		half _NFade;
		half _FR;
		half _Fade;


		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
			float4 color : COLOR;
			float4 posWorld : TEXCOORD1;
			float4 projPos : TEXCOORD2;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
		    UNITY_INITIALIZE_OUTPUT(Input,o);
		    o.color = v.color;
		    o.posWorld = mul(unity_ObjectToWorld, v.vertex);
			o.projPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
			COMPUTE_EYEDEPTH(o.projPos.z);
		}

		sampler2D _CameraDepthTexture;
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			float dist = length(IN.posWorld.xyz - _WorldSpaceCameraPos.xyz);
			float f = pow((saturate(dot(normalize(IN.viewDir),o.Normal)+ _FresnelO)),_FresnelP);

			
			float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projPos))));
			float partZ = IN.projPos.z;
			float fade = saturate((sceneZ - partZ) / _Fade);
			fade *= saturate((partZ - _NFade) / _FR);


			if(dot(normalize(IN.viewDir),o.Normal) < 0){
				f = pow((saturate(-dot(normalize(IN.viewDir),o.Normal)+ _FresnelO)),_FresnelP);;
			}
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _MainColor * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = f * c.a * 1.5f * fade;
			o.Emission = c.rgb * _Emission * fade;
			//o.Alpha = 1;
			//o.Emission = -dot(normalize(IN.viewDir),o.Normal);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
