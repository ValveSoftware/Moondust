Shader "FX/FlashingArrows"
{
    Properties
    {
		[HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Flash("Flashing Gradient", 2D) = "white" {}
		_Speed("Scroll Speed (W = Flash Intensity)", Vector) = (0,1,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100

		Blend One One
		ZWrite Off
		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _Flash;
            float4 _Flash_ST;
			fixed4 _Color;
			float4 _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _Flash) + (_Time.g * _Speed.xy);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				col.rgb *= col.a;
				//return col.a;
				float flash = tex2D(_Flash, i.uv2).r;
				col *= lerp(1, flash, _Speed.w);
                // apply fog
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
                return col;
            }
            ENDCG
        }
    }
}
