// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Hologram" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Color ("Color", Color) = (0,0,0,0)
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _Brightness ("Brightness", Range(0.0,6.0)) = 1.0
	  _Bp("Normal Strength", Range(0.0,2.0)) = 1.0
    }


    SubShader {
    cull off
    	
    


      Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
      Blend SrcAlpha One
      ZWrite Off
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
      #pragma target 3.0

     
      
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
          float4 screenPos;
          float dist;
		  float3 color;
      };

      float maxD;
      float3 center;
      void vert (inout appdata_full v, out Input o) {
     		UNITY_INITIALIZE_OUTPUT(Input,o);
            float mix = clamp(abs(_SinTime.b) * 100 - 90,0,1);
			o.color = v.color;
            o.dist = clamp(1 - (length(mul(unity_ObjectToWorld, v.vertex).xyz - center.xyz) / maxD),0,1);
            //o.dist = v.vertex.x;
           // v.vertex.xyz += v.normal * lerp(0,(sin((_Time.b + (v.vertex.y * _Time.b)+ (v.vertex.x * _Time.b)) * 100))* _Amount, mix);
            
        } 
      sampler2D _MainTex;
      sampler2D _BumpMap;
      float4 _RimColor;
      float4 _Color;
      float _RimPower;
      float _Brightness;
	  half _Bp;


      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = float4(0,0,0,0);
          o.Alpha = 0;
          o.Normal = lerp(float3(0,0,1),UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap)),_Bp);
         
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
         half backside = 0;
          if(rim > 0.99){
         	 rim = 1.0 - saturate(dot (normalize(IN.viewDir), -o.Normal));
          }
          rim = rim * tex2D (_MainTex, IN.uv_MainTex).a;
          float bright = IN.dist;
          if(abs(center.x) < 0.01){
          	bright = 1;
          }

         
          o.Emission = _Color.a * 0.4 * lerp((_RimColor.rgb * pow (rim, _RimPower) * 20),  tex2D (_MainTex, IN.uv_MainTex).rgb*tex2D (_MainTex, IN.uv_MainTex).a * _Color  * 3 - (backside * 0.2), -(_RimColor.rgb * pow (rim, _RimPower)) + 1) * bright  * _Brightness * IN.color;

         // o.Emission = IN.dist;

           //o.Alpha = lerp((_RimColor.rgb * pow (rim, _RimPower) * ((tex2D (_ScanTex, screenUV).r)  - 0.5) * 20) * _Brightness,  tex2D (_MainTex, IN.uv_MainTex).rgb * _Color * (tex2D (_ScanTex, screenUV).r * 0.5 * 2) * 2 - (backside * 0.2), -(_RimColor.rgb * pow (rim, _RimPower)) + 1);
      }
      ENDCG
    } 
    Fallback "Transparent/Diffuse"
  }
