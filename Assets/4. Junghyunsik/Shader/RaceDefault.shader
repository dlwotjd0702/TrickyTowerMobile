Shader "UI/VerticalThreeColorGradient"
{
    Properties
    {
        _MainTex      ("Sprite Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (0.427, 0.561, 0.847, 1)
        _MiddleColor ("Middle Color", Color) = (0.518, 0.796, 0.980, 1)
        _BottomColor ("Bottom Color", Color) = (0.588, 0.663, 0.918, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"               = "Transparent"
            "IgnoreProjector"     = "True"
            "RenderType"          = "Transparent"
            "PreviewType"         = "Plane"
            "CanUseSpriteAtlas"   = "True"
        }
        Cull Off  
        ZWrite Off  
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // UI 전용 입출력 구조체
            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;      // Image.color
                float2 texcoord : TEXCOORD0;  // UV
            };
            struct v2f {
                float4 pos      : SV_POSITION;
                float4 color    : COLOR0;
                float2 uv       : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _TopColor;
            fixed4    _MiddleColor;
            fixed4    _BottomColor;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.pos   = UnityObjectToClipPos(IN.vertex);
                OUT.uv    = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // (1) 기본 스프라이트 텍스처 샘플링 (없으면 흰색)
                fixed4 sprite = tex2D(_MainTex, IN.uv) * IN.color;

                // (2) UV.y 기준으로 0~1 정규화
                float t = IN.uv.y;

                // (3) 세 구간 그라데이션
                fixed4 grad;
                if (t < 0.4)
                {
                    float tt = smoothstep(0.0, 0.4, t);
                    grad = lerp(_BottomColor, _MiddleColor, tt);
                }
                else if (t < 0.6)
                {
                    grad = _MiddleColor;
                }
                else
                {
                    float tt = smoothstep(0.6, 1.0, t);
                    grad = lerp(_MiddleColor, _TopColor, tt);
                }

                // (4) 텍스처 × 그라데이션 (혹은 grad만 쓰려면 sprite 대신 grad로 교체)
                return sprite * grad;
            }
            ENDCG
        }
    }
}

