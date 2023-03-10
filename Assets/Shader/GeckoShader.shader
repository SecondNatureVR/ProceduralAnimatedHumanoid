Shader "Custom/GeckoShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Fatness ("Fatness", Range(0, 100)) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf NoLighting noambient vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }

        float _Fatness;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float InverseLerp(float a, float b, float v) {
			return (v - a) / (b - a);
		}

        float calculateFatness() {
            float minFat = -0.03;
            float maxFat = 0.17;

            float t = saturate(InverseLerp(0, 100, _Fatness));
			return lerp(minFat, maxFat, t);
        }

	    void vert (inout appdata_full v) {
		    v.vertex.xyz += v.normal * calculateFatness();
		}

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
