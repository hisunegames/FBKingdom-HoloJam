Shader "Custom/ParticleWithNoiseColorPosterizationAndNoiseMap" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FlickerIntensity ("Flicker Intensity", Range(0, 1)) = 0.5
        _TimeScale ("Time Scale", Range(0.1, 10)) = 1.0
        _Color ("Tint Color", Color) = (1, 1, 1, 1) // Color property for tinting
        _PosterizationLevels ("Posterization Levels", Range(1, 256)) = 16 // Number of color levels
        _NoiseMap ("Noise Map", 2D) = "white" {} // Noise texture with alpha
        _AlphaAdjustment ("Alpha Adjustment", Range(0, 1)) = 1.0 // New property for overall alpha adjustment
    }
    SubShader {
        Tags { "RenderType" = "TransparentCutout" }
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1; // World position for noise
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1; // Pass world position to fragment
            };

            sampler2D _MainTex;
            sampler2D _NoiseMap; // New noise map texture
            float4 _MainTex_ST;

            float _FlickerIntensity;
            float _TimeScale;
            float4 _Color; // Tint color
            int _PosterizationLevels; // Number of posterization levels
            float _AlphaAdjustment; // Overall alpha adjustment

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calculate world position
                return o;
            }

            // Simple gradient noise function based on world position and time
            float gradientNoise(float3 pos, float time) {
                return frac(sin(dot(pos * 12.9898, float3(78.233, 45.643, 83.542))) * 43758.5453 + time);
            }

            // Posterization function
            float3 posterizeColor(float3 color, int levels) {
                float invLevels = (float)levels - 1.0; // Inverse of levels
                return floor(color * invLevels + 0.5) / invLevels; // Quantize the color
            }

            fixed4 frag(v2f i) : SV_Target {
                // Sample the main texture
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                
                // Apply brightness adjustment and flicker using gradient noise
                float noise = gradientNoise(i.worldPos, _TimeScale * _Time.y) * _FlickerIntensity;
                baseColor.rgb *= 1.0 - noise; // Brightness adjustment with noise flicker

                // Apply tint color
                baseColor.rgb *= _Color.rgb; // Tint the color

                // Sample the noise map and use its alpha for additional modification
                fixed4 noiseColor = tex2D(_NoiseMap, i.uv);
                baseColor.a *= noiseColor.a; // Modulate alpha by noise map alpha

                // Apply overall alpha adjustment
                baseColor.a *= _AlphaAdjustment; // Adjust overall alpha based on slider value

                // Apply posterization
                baseColor.rgb = posterizeColor(baseColor.rgb, _PosterizationLevels); // Posterize the color

                // Return the modified color
                return baseColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse" // Optional: specify a fallback shader
}
