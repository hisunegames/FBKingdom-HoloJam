Shader "Custom/UnlitCorruptionEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Point1 ("Corruption Start Point", Vector) = (0, 0, 0, 0)
        _Point2 ("Corruption Mid Point", Vector) = (1, 0, 0, 0)
        _Point3 ("Corruption End Point", Vector) = (2, 0, 0, 0)

        _CorruptionColor ("Corruption Color", Color) = (1, 0, 0, 1)
        _CorruptionStrength ("Corruption Strength", Range(0, 1)) = 0.5
        _PosterizeLevels ("Posterization Levels", Range(2, 256)) = 8
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

            sampler2D _MainTex;
            float4 _Point1;
            float4 _Point2;
            float4 _Point3;
            float4 _CorruptionColor;
            float _CorruptionStrength;
            int _PosterizeLevels;

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

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the original color from the texture
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                // Check if the current position is before the midpoint
                if (i.worldPos.x < _Point2.x)
                {
                    return baseColor; // No corruption before the midpoint
                }

                // Calculate distance factors from midpoint to endpoint
                float distToMid = abs(i.worldPos.x - _Point2.x);
                float distToEnd = abs(i.worldPos.x - _Point3.x);
                float totalDistance = distToMid + distToEnd;
                float corruptionFactor = saturate((i.worldPos.x - _Point2.x) / totalDistance) * _CorruptionStrength;

                // Desaturate base color gradually as we approach the endpoint
                float desaturationLevel = corruptionFactor * 0.8;
                fixed3 grayscale = dot(baseColor.rgb, fixed3(0.3, 0.59, 0.11));
                fixed3 desaturatedColor = lerp(baseColor.rgb, grayscale.xxx, desaturationLevel);

                // Hard Light Blending
                fixed3 corruptColor = _CorruptionColor.rgb;
                fixed3 hardLightBlend;
                for (int channel = 0; channel < 3; channel++)
                {
                    hardLightBlend[channel] = desaturatedColor[channel] < 0.5 
                        ? 2.0 * desaturatedColor[channel] * corruptColor[channel] 
                        : 1.0 - 2.0 * (1.0 - desaturatedColor[channel]) * (1.0 - corruptColor[channel]);
                }
                
                // Blend the result with the corruption factor
                fixed3 blendedColor = lerp(desaturatedColor, hardLightBlend, corruptionFactor);

                // Improved Posterization
                float stepSize = 1.0 / _PosterizeLevels;
                blendedColor = floor((blendedColor + stepSize * 0.5) / stepSize) * stepSize;

                // Return the final color with original alpha
                return fixed4(blendedColor, baseColor.a);
            }
            ENDCG
        }
    }
}
