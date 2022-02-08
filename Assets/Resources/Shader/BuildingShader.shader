Shader "Unlit/BuildingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "UnityCG.cginc" 
            #include "Lighting.cginc" 
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // �V���h�E�̃f�[�^�� TEXCOORD1 �Ɋi�[
                float3 worldNormal : TEXCOORD2;
                half3 halfDir : TEXCOORD3;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // �n�[�t�x�N�g�������߂�
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                half3 eyeDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
                o.halfDir = normalize(_WorldSpaceLightPos0.xyz + eyeDir);

                // �V���h�E�̃f�[�^���v�Z���܂�
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag(v2f i ) : SV_Target
            {
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 normal = normalize(i.worldNormal);
                float NL = dot(normal, lightDir);

                float3 baseColor = tex2D(_MainTex, i.uv);
                
                float3 lightColor = _LightColor0;
                lightColor += max(0, dot(i.worldNormal, i.halfDir));

                fixed4 col = fixed4(baseColor * lightColor * max(NL, 0.2), 0);


                // �V���h�E�̌������v�Z���܂� (1.0 = ���S�ɏƎ˂����, 0.0 = ���S�ɉe�ɂȂ�)
                fixed shadow = SHADOW_ATTENUATION(i);

                // �X�y�L�����̋��������߂�
                col.rgb = col.rgb * max(0.0, dot(i.worldNormal, i.halfDir)) * shadow;

                return col;

            }
            ENDCG
        }
        Pass
        {
            Tags{ "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
