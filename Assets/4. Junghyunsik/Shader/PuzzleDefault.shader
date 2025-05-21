Shader "Custom/VerticalThreeColorGradient_Local"
{
    Properties
    {
        _TopColor    ("Top Color",    Color) = (0.988, 0.631, 0.420, 1)  // 보랏빛 #673AB7
        _MiddleColor ("Middle Color", Color) = (0.992, 0.973, 0.757, 1)  // 밝은 살구톤 #F5D28F
        _BottomColor ("Bottom Color", Color) = (0.960, 0.820, 0.560, 1)  // 연한 베이지 #F8EAC1
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