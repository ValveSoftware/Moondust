Shader "Custom/JoeJeffRecolorable"
{
    Properties
    {
        _Color ("Skin Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB) Skin Mask (A)", 2D) = "white" {}
        [NoScaleOffset]_Metallic ("Metallic/Smoothness", 2D) = "black" {}
        [NoScaleOffset]_Normal ("Normal", 2D) = "bump" {}

    }
    SubShader
    {
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

        struct Input
        {
            float2 uv_MainTex;
        };


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 met = tex2D (_Metallic, IN.uv_MainTex);

            o.Albedo = lerp(tex.rgb * UNITY_ACCESS_INSTANCED_PROP(Props, _Color), tex.rgb, tex.a);
            o.Metallic = met.r;
            o.Smoothness = met.a;
			o.Normal = UnpackNormal(tex2D (_Normal, IN.uv_MainTex));

        }
        ENDCG
    }
    FallBack "Diffuse"
}
