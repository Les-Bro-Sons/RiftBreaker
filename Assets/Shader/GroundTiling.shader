Shader "Custom/GroundTiling"
{
    Properties
    {
        _Texture1 ("Texture 1", 2D) = "white" {}
        _Texture2 ("Texture 2", 2D) = "white" {}
        _Texture3 ("Texture 3", 2D) = "white" {}
        _Texture4 ("Texture 4", 2D) = "white" {}
        _Scale ("Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;
            sampler2D _Texture4;
            float _Scale;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float randValue : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);

                // Utiliser les coordonnées de position pour générer une valeur aléatoire par face
                o.randValue = frac(sin(dot(o.worldPos.xy + o.worldPos.z, float2(12.9898, 78.233))) * 43758.5453);

                return o;
            }

            fixed4 SampleTriplanarTexture(float3 worldPos, float3 worldNormal, sampler2D tex)
            {
                float3 blend = abs(worldNormal);
                blend = normalize(max(blend, 0.00001)); //to avoid division by zero
                blend /= (blend.x + blend.y + blend.z);

                fixed4 xTex = tex2D(tex, worldPos.yz * _Scale);
                fixed4 yTex = tex2D(tex, worldPos.xz * _Scale);
                fixed4 zTex = tex2D(tex, worldPos.xy * _Scale);

                return xTex * blend.x + yTex * blend.y + zTex * blend.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = i.worldPos * _Scale;
                float3 worldNormal = normalize(i.worldNormal);

                float n = i.randValue * 4.0;

                fixed4 tex = fixed4(0, 0, 0, 1);

                if (n < 1.0)
                    tex = SampleTriplanarTexture(worldPos, worldNormal, _Texture1);
                else if (n < 2.0)
                    tex = SampleTriplanarTexture(worldPos, worldNormal, _Texture2);
                else if (n < 3.0)
                    tex = SampleTriplanarTexture(worldPos, worldNormal, _Texture3);
                else
                    tex = SampleTriplanarTexture(worldPos, worldNormal, _Texture4);

                return tex;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
