Shader "Custom/GageShader"
{
    Properties
    {
        _TopColor("Top Color", Color) = (1,1,1,1)         // 上側のマテリアル色（初期：白）
        _BottomColor("Bottom Color", Color) = (1,0,0,1)    // 下側のマテリアル色（初期：赤）
        _BlendAmount("Blend Amount", Range(0,1)) = 0       // どこまで切り替えるか（0=上だけTopColor、1=全部BottomColor）
        _ObjectHeight("Object Height", Float) = 1.0        // オブジェクトの高さ（手動で調整 or 自動でセット可）
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            float4 _TopColor;
            float4 _BottomColor;
            float _BlendAmount;
            float _ObjectHeight;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;     // ワールド位置
                float3 worldNormal : TEXCOORD1;  // 法線
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = worldPos;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float cutoff = _BlendAmount * _ObjectHeight;

            // ローカルY方向を世界空間に変換
            float3 worldLocalUp = normalize(mul((float3x3)unity_ObjectToWorld, float3(0, 1, 0)));

            // worldPos からオブジェクトの原点までのベクトルを取って、localUpに投影
            float3 worldToOrigin = i.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
            float projectedHeight = dot(worldToOrigin, worldLocalUp); // 傾きに追従する高さ

            // ブレンド境界で色を分ける
            if (projectedHeight < cutoff)
            {
                return _BottomColor;
            }
            else
            {
                return _TopColor;
            }
            }

        ENDCG
    }
    }
}