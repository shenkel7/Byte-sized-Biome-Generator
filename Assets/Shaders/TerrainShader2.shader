Shader "Custom/TerrainShader2"
{
    Properties
    {
        _Height ("Height", Float) = 1.0
        _Color ("Color", Color) = (1,1,1,1)
        _Color2 ("Color", Color) = (1,1,1,1)
        _Color3 ("High Elevation Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        _MainTex3 ("High Elevation Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
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
        sampler2D _MainTex2;
        sampler2D _MainTex3;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MainTex2;
            float uv_MainTex3;
            float3 worldNormal;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Color2;
        fixed4 _Color3;
        float _Height;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            // fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color 
            float3 up =  { 0, 1.0f, 0 };
            float blend = clamp (dot (IN.worldNormal, up), 0.0f, 1.0f);
            fixed4 c = lerp (tex2D (_MainTex, IN.uv_MainTex) * _Color, tex2D (_MainTex2, IN.uv_MainTex2) * _Color2, blend);
            c = lerp( c, tex2D (_MainTex3, IN.uv_MainTex3) * _Color3, clamp (IN.worldPos.y, 0, _Height) / _Height);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
