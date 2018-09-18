Shader "Custom/ConveyorBelt" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Metallic ("Metallic/Roughness", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Belt("Belt Mask", 2D) = "white" {}

		_Move("Movement", Vector) = (0,1,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Metallic;
		sampler2D _Normal;
		sampler2D _Belt;

		struct Input {
			float2 uv_MainTex;
		};
		fixed4 _Color;
		float4 _Move;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

			float2 uv = IN.uv_MainTex;
			float belt = tex2D(_Belt, uv).r;

			if (belt > 0.5) {
				float t = frac(_Time.r);
				uv += float2(_Move.x *t, _Move.y*t);
			}

			fixed4 c = tex2D (_MainTex, uv) * _Color;
			fixed4 m = tex2D(_Metallic, uv);
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_Normal, uv));
			// Metallic and smoothness come from slider variables
			o.Metallic = m.r;
			o.Smoothness = m.a;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
