Shader "Custom/PixelatedShader"
{
    Properties
    {
        _MainTex("Base Texture (RGB)", 2D) = "white" { }
        _PixelSize("Pixel Size", Range(1, 128)) = 8
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                uniform float _PixelSize;
                sampler2D _MainTex;
                float4 _MainTex_TexelSize;  // This is the texel size (for resolution per pixel)
                float4 _MainTex_ST;         // Texture transform for _MainTex

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    // Pixelate the base texture by scaling the UVs
                    float2 pixelSize = _PixelSize;
                    float2 uvPixelated = floor(i.uv * pixelSize) / pixelSize;

                    // Sample the pixelated base texture
                    return tex2D(_MainTex, uvPixelated);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}