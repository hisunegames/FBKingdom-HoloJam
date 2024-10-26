Shader "Custom/UnlitCorruptionEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CorruptionColor ("Corruption Color", Color) = (1, 0, 0, 1)
        _CorruptionStrength ("Corruption Strength", Range(0, 1)) = 0.5
        _PosterizeLevels ("Posterization Levels", Range(2, 256)) = 8
        _FlickerIntensity ("Flicker Intensity", Range(0, 1)) = 0.2
        _TimeScale ("Time Scale", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _CorruptionColor;
            float _CorruptionStrength;
            float _PosterizeLevels;
            float _FlickerIntensity;
            float _TimeScale;

            // Fixed max corruption points
            #define MAX_CORRUPTION_POINTS 10
            struct CorruptionPoint
            {
                float3 position;
                float radius;
            };
            StructuredBuffer<CorruptionPoint> _CorruptionPoints;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Simple gradient noise function based on world position and time
            float gradientNoise(float3 pos, float time)
            {
                return frac(sin(dot(pos * 12.9898, float3(78.233, 45.643, 83.542))) * 43758.5453 + time);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                float corruptionFactor = 0.0;

                // Manually unroll up to MAX_CORRUPTION_POINTS
                [unroll(MAX_CORRUPTION_POINTS)]
                for (int j = 0; j < MAX_CORRUPTION_POINTS; j++)
                {
                    if (j >= _CorruptionPoints.Length) break;

                    float3 pointPos = _CorruptionPoints[j].position;
                    float radius = _CorruptionPoints[j].radius;
                    float distToPoint = distance(i.worldPos, pointPos);

                    if (distToPoint < radius)
                    {
                        float pointCorruption = saturate((radius - distToPoint) / radius);
                        corruptionFactor = max(corruptionFactor, pointCorruption);
                    }
                }

                // Corruption desaturation
                float desaturationLevel = corruptionFactor * _CorruptionStrength;
                fixed3 grayscale = dot(baseColor.rgb, fixed3(0.3, 0.59, 0.11));
                fixed3 desaturatedColor = lerp(baseColor.rgb, grayscale.xxx, desaturationLevel);

                // Hard Light Blend for corruption color
                fixed3 corruptColor = _CorruptionColor.rgb;
                fixed3 hardLightBlend;
                for (int channel = 0; channel < 3; channel++)
                {
                    hardLightBlend[channel] = desaturatedColor[channel] < 0.5 
                        ? 2.0 * desaturatedColor[channel] * corruptColor[channel] 
                        : 1.0 - 2.0 * (1.0 - desaturatedColor[channel]) * (1.0 - corruptColor[channel]);
                }
                fixed3 blendedColor = lerp(desaturatedColor, hardLightBlend, corruptionFactor);

                // Posterization effect
                float stepSize = 1.0 / _PosterizeLevels;
                blendedColor = floor((blendedColor + stepSize * 0.5) / stepSize) * stepSize;

                // Apply brightness adjustment and flicker using gradient noise
                float noise = gradientNoise(i.worldPos, _TimeScale * _Time.y) * _FlickerIntensity * corruptionFactor;
                blendedColor *= 1.0 - noise; // Brightness adjustment with noise flicker

                return fixed4(blendedColor, baseColor.a);
            }
            ENDCG
        }
    }
}
