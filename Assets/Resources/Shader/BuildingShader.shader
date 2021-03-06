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
                SHADOW_COORDS(1) // シャドウのデータを TEXCOORD1 に格納
                float3 normal : TEXCOORD2;
                half3 halfDir : TEXCOORD3;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // ハーフベクトルを求める
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                half3 eyeDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
                o.halfDir = normalize(_WorldSpaceLightPos0.xyz + eyeDir);

                // シャドウのデータを計算します
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag(v2f i ) : SV_Target
            {
                // テクスチャカラー
                fixed4 col = tex2D(_MainTex, i.uv);

                // ディフューズライト
                //col.rgb *= max(0.2, dot(i.normal, _WorldSpaceLightPos0.xyz));
                //col *= _LightColor0;

                // ハーフランバート
                col.rgb *= dot(i.normal, _WorldSpaceLightPos0.xyz)* 0.5 + 0.5;
                //col *= _LightColor0;

                // スペキュラーライト
                //col.rgb += col.rgb * max(0.6, dot(i.normal, i.halfDir));

                // シャドウの減衰を計算します (1.0 = 完全に照射される, 0.0 = 完全に影になる)
                col.rgb *= max( 0.6, SHADOW_ATTENUATION(i));




                //float3 lightDir = _WorldSpaceLightPos0.xyz;
                //float3 normal = normalize(i.normal);
                //float NL = dot(normal, lightDir);

                //
                //float3 lightColor = _LightColor0;
                //lightColor += max(0, dot(i.normal, i.halfDir));

                //fixed4 col = fixed4(baseColor * lightColor * max(NL, 0), 0);


                

                // スペキュラの強さを求める
                //col.rgb = col.rgb * max(0.0, dot(i.worldNormal, i.halfDir)) * shadow;

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
