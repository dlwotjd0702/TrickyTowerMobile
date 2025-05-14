Shader "Custom/VerticalThreeColorGradient_Local"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.427, 0.561, 0.847, 1)
        _MiddleColor ("Middle Color", Color) = (0.518, 0.796, 0.980, 1)
        _BottomColor ("Bottom Color", Color) = (0.588, 0.663, 0.918, 1)
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
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
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            fixed4 _TopColor;
            fixed4 _MiddleColor;
            fixed4 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float y = i.localPos.y;
                float t = (y + 0.5); // [0 ~ 1]로 정규화

                fixed4 color;
                
                if (t < 0.4)
                {
                    float t01 = smoothstep(0.0, 0.4, t); // Bottom → Middle
                    color = lerp(_BottomColor, _MiddleColor, t01);
                }
                else if (t < 0.6)
                {
                    color = _MiddleColor; // 중간 영역 유지
                }
                else
                {
                    float t12 = smoothstep(0.6, 1.0, t); // Middle → Top
                    color = lerp(_MiddleColor, _TopColor, t12);
                }
                return color;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}