Shader "Custom/GageShader"
{
    Properties
    {
        _TopColor("Top Color", Color) = (1,1,1,1)         // �㑤�̃}�e���A���F�i�����F���j
        _BottomColor("Bottom Color", Color) = (1,0,0,1)    // �����̃}�e���A���F�i�����F�ԁj
        _BlendAmount("Blend Amount", Range(0,1)) = 0       // �ǂ��܂Ő؂�ւ��邩�i0=�ゾ��TopColor�A1=�S��BottomColor�j
        _ObjectHeight("Object Height", Float) = 1.0        // �I�u�W�F�N�g�̍����i�蓮�Œ��� or �����ŃZ�b�g�j
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
                float3 worldPos : TEXCOORD0;     // ���[���h�ʒu
                float3 worldNormal : TEXCOORD1;  // �@��
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

            // ���[�J��Y�����𐢊E��Ԃɕϊ�
            float3 worldLocalUp = normalize(mul((float3x3)unity_ObjectToWorld, float3(0, 1, 0)));

            // worldPos ����I�u�W�F�N�g�̌��_�܂ł̃x�N�g��������āAlocalUp�ɓ��e
            float3 worldToOrigin = i.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
            float projectedHeight = dot(worldToOrigin, worldLocalUp); // �X���ɒǏ]���鍂��

            // �u�����h���E�ŐF�𕪂���
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