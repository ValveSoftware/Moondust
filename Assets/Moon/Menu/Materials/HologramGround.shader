Shader "FX/HoloTerrain"
{
    Properties
    {
		[HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Flash("Extrusion Texture", 2D) = "white" {}
		_Speed("Scroll Speed (Z = Extrusion Intensity)", Vector) = (0,1,1,0)
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
				float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 flash : TEXCOORD1;
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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float2 uv2 = TRANSFORM_TEX(v.uv, _Flash) + (_Time.g * _Speed.xy);
				float flash = tex2Dlod(_Flash, float4(uv2,0,0)).r * v.color.r;
				v.vertex.y += flash * _Speed.z;
				o.flash = flash;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				col.rgb *= col.a;
				//return col.a;
				
				col *= i.flash;
                // apply fog
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
                return col;
            }
            ENDCG
        }
    }
}
