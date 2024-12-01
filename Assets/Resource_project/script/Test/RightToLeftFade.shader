Shader "Custom/LeftToRightFadeOut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "Null" {}
        _Fade ("Fade Amount", Range(-1.0, 1.0)) = 0.7 // ��l�����z���B�k�z��
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Fade;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                //col = fixed4(0, 0, 0, 1); // �w�]�¦�

                // ����G���䤣�z���B�k��z���A�H�X�q����k
                float alphaFactor = 1.0 - smoothstep(_Fade, 1.0, i.uv.x);
                col.a *= alphaFactor;

                return col;
            }
            ENDCG
        }
    }
}
