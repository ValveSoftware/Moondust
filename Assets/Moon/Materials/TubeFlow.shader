Shader "Custom/TubeFlow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Emission("Emission", Color) = (0,0,0,1)
		_MainTex ("Alpha", 2D) = "white" {}
		_Normal("Bump", 2D) = "bump" {}
		_Noise("Noise", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_Pos("Position", Range(0,1)) = 0.5
		_Size("segment Size", Range(0.001,1)) = 0.1
	}
	SubShader {
			Tags{ "Queue" = "Transparent-1" "RenderType" = "Transparent" }
			LOD 200

			CGPROGRAM

#pragma surface surf Standard fullforwardshadows alpha:fade
#pragma target 3.0


		sampler2D _MainTex;
		sampler2D _Noise;
		sampler2D _Normal;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Noise;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _Emission;
		half _Pos;
		half _Size;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

			float Overlay(float a, float b) {
			if (a < 0.5) {
				a = a*b * 2;
			}
			else {
				a = 1 - 2 * (1 - b)*(1 - a);
			}
			return (a);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color;
			o.Albedo = c.rgb*1;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Noise));


			float2 uv = IN.uv_MainTex;
			_Pos = lerp(0 - _Size, 1 + _Size, _Pos);
			uv = float2(saturate((uv.x-_Pos)/_Size), 0.5);
			half a = tex2D(_MainTex, uv).r;

			half b = tex2D(_Noise, IN.uv_Noise).r;

			a = Overlay(a, b);

			a = Overlay(a, a);
			a = Overlay(a, a);
			

			o.Emission = _Emission;

			clip(a - 0.1);
			o.Alpha = a;
		}

		
		ENDCG
	}
	FallBack "Diffuse"
}
