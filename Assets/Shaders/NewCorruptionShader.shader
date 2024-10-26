Shader "Custom/MaskShaderWithJitter" {
    Properties {
        _JitterAmount ("Jitter Amount", Range(0, 1)) = 0.1
        _JitterSpeed ("Jitter Speed", Range(0, 10)) = 1.0
    }
    SubShader {
        Tags { "RenderType" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _JitterAmount;
            float _JitterSpeed;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Simple hash function for jitter
            float GradientNoise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target {
                // Compute time-based offset
                float time = _Time.y * _JitterSpeed; // Use _Time.y for consistent time value
                float2 jitter = GradientNoise(i.uv + time) * _JitterAmount;

                // Apply jitter to UV coordinates
                float2 jitteredUV = i.uv + jitter;

                // Output color (black for mask)
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse" // Optional: specify a fallback shader
}
