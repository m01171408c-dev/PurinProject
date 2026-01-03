Shader "Custom/LightPuddleAnimated"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.3)
        _Mask ("Mask", 2D) = "white" {}
        _Noise ("Noise", 2D) = "gray" {}
        _Distort ("Distort Amount", Range(0,0.1)) = 0.02
        _ScaleSpeed ("Scale Speed", Range(0,1)) = 0.2
        _DistortSpeed ("Distort Speed", Range(0,1)) = 0.1
        _RotateSpeed ("Rotate Speed", Range(-5,5)) = 1.0   // ← 回転速度追加
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Mask;
            sampler2D _Noise;
            float4 _Color;
            float _Distort;
            float _ScaleSpeed;
            float _DistortSpeed;
            float _RotateSpeed;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // UV 回転関数
            float2 RotateUV(float2 uv, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);

                // 中心(0.5,0.5)基準で回転
                uv -= 0.5;
                float2 rotated = float2(
                    uv.x * c - uv.y * s,
                    uv.x * s + uv.y * c
                );
                return rotated + 0.5;
            }

            fixed4 frag(v2f i):SV_Target
            {
                // 形状の変化（広がり・縮み）
                float scale = 1.0 + sin(_Time.y * _ScaleSpeed) * 0.05;
                float2 uv = i.uv * scale;

                // ★ 回転を追加
                float angle = _Time.y * _RotateSpeed;
                uv = RotateUV(uv, angle);

                // ゆらぎ（ノイズでUVを揺らす）
                float2 noise = tex2D(_Noise, uv * 2 + _Time.y * _DistortSpeed).rg;
                uv += (noise - 0.5) * _Distort;

                // マスク
                float mask = tex2D(_Mask, uv).r;

                // 色と透明度
                float4 col = _Color;
                col.a *= mask;

                return col;
            }
            ENDCG
        }
    }
}
