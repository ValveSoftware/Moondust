// Copyright (c) Valve Corporation, All rights reserved. ======================================================================================================

Shader "Valve/WormholeAlwaysVisible"
{
    Properties
	{
        _Cubemap( "Cubemap", Cube ) = "_Skybox" {}
		_OutlineColor( "Outline Color", Color ) = ( 0.1, 0.5, 0.8, 1.0 )
		_TintColor( "Tint Color", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		[HideInInspector] _GlowAmount ( "Glow Amount", Float ) = 0.0
    }
    SubShader
	{
        Tags
		{
            "RenderType"="Transparent"
			"Queue"="Transparent+1"
        }
		Pass
		{
            Name "InFrontOfGeometry"
            Tags
			{
                "LightMode"="Always"
            }

			Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
			ZTest Less // In front of geometry
            
            CGPROGRAM

            #pragma vertex MainVs
            #pragma fragment MainPs
            
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl

			// Includes -------------------------------------------------------------------------------------------------------------------------------------------------
			#include "UnityCG.cginc"
			#include "WormholeShading.cginc"

            float4 MainPs( PS_INPUT i ) : COLOR
			{
				float4 shading = WormholeShading( i.vPositionWs.xyz, i.vNormalWs.xyz );
				return shading;
            }
            ENDCG
        }
        Pass
		{
            Name "BehindGeometry"
            Tags
			{
                "LightMode"="Always"
            }

			Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
			ZTest Greater // Behind geometry
            
            CGPROGRAM

            #pragma vertex MainVs
            #pragma fragment MainPs
            
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl

			// Includes -------------------------------------------------------------------------------------------------------------------------------------------------
			#include "UnityCG.cginc"
			#include "WormholeShading.cginc"

            float4 MainPs( PS_INPUT i ) : COLOR
			{
				float4 shading = WormholeShading( i.vPositionWs.xyz, i.vNormalWs.xyz );
				shading.a *= 0.25;
				return shading;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
