Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0,0,0,0);
                float2 uv = i.uv;
                float2 blurOffset = _BlurSize / _ScreenParams.xy;

                // Sample the texture with a 9-tap Gaussian blur
                color += tex2D(_MainTex, uv + float2(-blurOffset.x, -blurOffset.y)) * 0.05;
                color += tex2D(_MainTex, uv + float2(-blurOffset.x, 0)) * 0.09;
                color += tex2D(_MainTex, uv + float2(-blurOffset.x, blurOffset.y)) * 0.05;
                color += tex2D(_MainTex, uv + float2(0, -blurOffset.y)) * 0.09;
                color += tex2D(_MainTex, uv) * 0.16;
                color += tex2D(_MainTex, uv + float2(0, blurOffset.y)) * 0.09;
                color += tex2D(_MainTex, uv + float2(blurOffset.x, -blurOffset.y)) * 0.05;
                color += tex2D(_MainTex, uv + float2(blurOffset.x, 0)) * 0.09;
                color += tex2D(_MainTex, uv + float2(blurOffset.x, blurOffset.y)) * 0.05;

                return color;
            }
            ENDCG
        }
    }
}
