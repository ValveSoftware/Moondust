// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (c) Valve Corporation, All rights reserved. ======================================================================================================

#ifndef WORLMHOLE_SHADING_INCLUDED
#define WORLMHOLE_SHADING_INCLUDED

// !!! HACK !!! Workaround for bug in Unity 5.4.0b9 where _WorldSpaceCameraPos appears to be set incorrectly breaking every view-dependent line of shader code
#define _WorldSpaceCameraPos ( float3( unity_CameraToWorld[0][3], unity_CameraToWorld[1][3], unity_CameraToWorld[2][3] ) )
// !!! END HACK !!!

uniform samplerCUBE _Cubemap;
float4 _OutlineColor;
float4 _TintColor;
float _GlowAmount;

#define REMAP( t, low1, high1, low2, high2 ) ( low2 + ( t - low1 ) * ( high2 - low2 ) / ( high1 - low1 ) )
#define REMAP_CLAMPED( t, low1, high1, low2, high2 ) clamp( REMAP( t, low1, high1, low2, high2 ), low2, high2 )

struct VS_INPUT
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct PS_INPUT
{
	float4 pos : SV_POSITION;
	float4 vPositionWs : TEXCOORD0;
	float3 vNormalWs : TEXCOORD1;
};

PS_INPUT MainVs(VS_INPUT v)
{
	PS_INPUT o = (PS_INPUT)0;
	o.vNormalWs = UnityObjectToWorldNormal(v.normal);
	o.vPositionWs = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);

	return o;
}

float4 WormholeShading( float3 vPositionWs, float3 vNormalWs )
{
	vNormalWs.xyz = normalize( vNormalWs.xyz );

	float3 vWormholeCenterWs = mul( unity_ObjectToWorld, float4( 0.0, 0.0, 0.0, 1.0 ) ).xyz;
	float3 vCameraToPositionDirWs = normalize( vPositionWs.xyz - _WorldSpaceCameraPos.xyz );
	float flNDotV = saturate( dot( -vCameraToPositionDirWs.xyz, vNormalWs.xyz ) );

	// lend between interior reflection ( inverted cubemap ) and plain view direction lookup
	// Giving wormhole a semi-solid semi-portal type look
	float3 vExteriorReflectionDirWs = reflect( vNormalWs.xyz, vCameraToPositionDirWs.xyz );
	float3 vInteriorReflectionDirWs = reflect( -vCameraToPositionDirWs.xyz, vExteriorReflectionDirWs.xyz );
	float flInteriorReflectionAmount = 2.0f * flNDotV - 0.2f;
	float3 vWormholeDirWs = normalize( lerp( vCameraToPositionDirWs, vInteriorReflectionDirWs, flInteriorReflectionAmount ) );

	float flEdgeHighlightStrength = 1.1 - flNDotV;
	flEdgeHighlightStrength *= flEdgeHighlightStrength * flEdgeHighlightStrength;
			
	float4 vWormholeTexel = texCUBE( _Cubemap, vWormholeDirWs.xyz ).rgba;

	float4 vOutputColor;
	vOutputColor.rgb = lerp( vWormholeTexel.rgb, _OutlineColor.rgb, flEdgeHighlightStrength );
	vOutputColor.a = flInteriorReflectionAmount;
	vOutputColor.rgba *= _TintColor.rgba * (1+_GlowAmount);

	return saturate(vOutputColor.rgba);
}

#endif
