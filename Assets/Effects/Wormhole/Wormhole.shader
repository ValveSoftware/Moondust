// Copyright (c) Valve Corporation, All rights reserved. ======================================================================================================

Shader "Valve/Wormhole"
{
    Properties
	{
        _Cubemap( "Cubemap", Cube ) = "_Skybox" {}
		_OutlineColor( "Outline Color", Color ) = ( 0.1, 0.5, 0.8, 1.0 )
		_TintColor( "Tint Color", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		 _GlowAmount ( "Glow Amount", Float ) = 0.0
    }
    SubShader
	{
        Tags
		{
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Pass
		{
            Name "FORWARD"
            Tags
			{
                "LightMode"="ForwardBase"
            }

			Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
            
            CGPROGRAM

            #pragma vertex MainVs
            #pragma fragment MainPs
            #pragma target 3.0

			// Includes -------------------------------------------------------------------------------------------------------------------------------------------------
			#include "UnityCG.cginc"
			#include "WormholeShading.cginc"

            float4 MainPs( PS_INPUT i ) : COLOR
			{
				return WormholeShading( i.vPositionWs.xyz, i.vNormalWs.xyz );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
