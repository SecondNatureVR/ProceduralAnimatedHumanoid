Shader "Unlit/GeckoShaderUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fatness ("Fatness", Range(0, 100)) = 20
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
			float _Fatness;

			float InverseLerp(float a, float b, float v) {
				return (v - a) / (b - a);
			}

			float calculateFatness() {
				float minFat = -0.03;
				float maxFat = 0.17;

				float t = saturate(InverseLerp(0, 100, _Fatness));
				return lerp(minFat, maxFat, t);
			}

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.vertex.xyz += v.normal * calculateFatness();
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
