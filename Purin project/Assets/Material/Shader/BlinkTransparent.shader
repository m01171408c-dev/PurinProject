Shader "Custom/BlinkTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _BlinkSpeed ("Blink Speed", Float) = 2.0
        _MinAlpha ("Min Alpha", Range(0,1)) = 0.1
        _MaxAlpha ("Max Alpha", Range(0,1)) = 1.0
    }

    SubShader
    {
        // 半透明オブジェクト用の設定
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        // 深度書き込みをオフ、アルファブレンド
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
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
            };

            float4 _Color;
            float _BlinkSpeed;
            float _MinAlpha;
            float _MaxAlpha;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 時間に応じて 0〜1 のサイン波を生成
                float t = (_Time.y * _BlinkSpeed);
                float s = (sin(t) + 1.0) * 0.5; // 0〜1

                // 最小～最大アルファの範囲にマッピング
                float alpha = lerp(_MinAlpha, _MaxAlpha, s);

                fixed4 col = _Color;
                col.a *= alpha;

                return col;
            }
            ENDHLSL
        }
    }
}
