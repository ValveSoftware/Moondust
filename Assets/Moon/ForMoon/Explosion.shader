Shader "Custom/Explosion"
{
    Properties 
    {
    	_Color("Color", Color) = (1,1,1,1)
        _RampTex ("Color Ramp", 2D) = "white" {}
        _DispTex ("Displacement Texture", 2D) = "gray" {}
        _Displacement ("Displacement", Range(0, 10.0)) = 0.9
        _Speed ("Speed", Range(0, 10.0)) = 1
        _Bright ("Brightness", Range(0, 10.0)) = 2
        _ChannelFactor ("ChannelFactor (r,g,b)", Vector) = (1,0,0)
        _Range ("Range (min,max)", Vector) = (0,0.5,0)
        _ClipRange ("ClipRange [0,1]", float) = 0.8
        _Flow ("Flow (x and y)", Vector) = (0,0,0,0)

    }
 
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        LOD 300
 
        CGPROGRAM
        #pragma surface surf Lambert vertex:disp nolightmap
        #pragma target 4.0
        #pragma glsl
 		float4 _Color;
        sampler2D _DispTex;
        float _Displacement;
        float _Speed;
        float _Bright;
        float3 _ChannelFactor;
        float2 _Range;
        float _ClipRange;
        float2 _Flow;
 		
        struct Input 
        {
            float2 uv_DispTex;
            float4 Time;
        };


 
        void disp (inout appdata_full v)
        {
       		float PI = 3.14159265359;
        	float r = sin((_Time.g / _Speed) * (2 * PI)) * 0.5f + 0.25f;
			float g = sin((_Time.g / _Speed + 0.33333333f) * 2 * PI) * 0.5f + 0.25f;
			float b = sin((_Time.g / _Speed + 0.66666667f) * 2 * PI) * 0.5f + 0.25f;
			float correction = 1 / (r + g + b);
			r *= correction;
			g *= correction;
			b *= correction;
			_ChannelFactor= float4(r,g,b,0);

			float2 off = _Flow.xy * _Time.r;
            float3 dcolor = tex2Dlod (_DispTex, float4(v.texcoord.xy+off,0,0));
            float d = (dcolor.r*_ChannelFactor.r + dcolor.g*_ChannelFactor.g + dcolor.b*_ChannelFactor.b);
            v.vertex.xyz += v.normal * d * _Displacement;
        }
 
        sampler2D _RampTex;
 
        void surf (Input IN, inout SurfaceOutput o) 
        {
        	float PI = 3.14159265359;
        	float r = sin((_Time.g / _Speed) * (2 * PI)) * 0.5f + 0.25f;
			float g = sin((_Time.g / _Speed + 0.33333333f) * 2 * PI) * 0.5f + 0.25f;
			float b = sin((_Time.g / _Speed + 0.66666667f) * 2 * PI) * 0.5f + 0.25f;
			float correction = 1 / (r + g + b);
			r *= correction;
			g *= correction;
			b *= correction;
			_ChannelFactor= float4(r,g,b,0);
			float2 off = _Flow.xy * _Time.r;
            float3 dcolor = tex2D (_DispTex, IN.uv_DispTex+off);
            float d = (dcolor.r*_ChannelFactor.r + dcolor.g*_ChannelFactor.g + dcolor.b*_ChannelFactor.b) * (_Range.y-_Range.x) + _Range.x;
           // clip (_ClipRange-d);
            half4 c = tex2D (_RampTex, float2(d,0.5));
            o.Albedo = 0;
            o.Emission = c.rgb*c.a * _Bright * _Color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}