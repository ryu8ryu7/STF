Shader "3D/Bullding"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
        _Spec1Power("Specular Power", Range(0, 30)) = 1
        _Spec1Color("Specular Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "LightMode" = "ForwardBase" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float4 vertexW : TEXCOORD3;
                fixed4 diff : COLOR0;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            sampler2D _BumpMap;
            float4 _MainTex_ST;

            uniform float _Spec1Power;
            uniform float4 _Spec1Color;

            float4x4 InvTangentMatrix(float3 tan, float3 bin, float3 nor)
            {
                float4x4 mat = float4x4(
                    float4(tan, 0),
                    float4(bin, 0),
                    float4(nor, 0),
                    float4(0, 0, 0, 1)
                    );

                // 正規直交系行列なので、逆行列は転置行列で求まる
                return transpose(mat);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexW = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

                // ワールド位置にあるライトをローカル空間へ変換する
                float3 localLight = mul(unity_WorldToObject, _WorldSpaceLightPos0);

                float3 t = v.tangent;
                float3 b = cross(v.normal, t);

                o.lightDir = mul(localLight, InvTangentMatrix(t, b, v.normal));

                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Diffuse
                col *= i.diff;
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                //float3 NdotL = dot(i.normal, L);
                //float3 diffuse = (NdotL * 0.5 + 0.5) * _LightColor0.rgb;
                //col.xyz *= diffuse.xyz;

                // Speculer
                float3 V = normalize(_WorldSpaceCameraPos - i.vertexW.xyz);
                float3 specular = pow(max(0.0, dot(reflect(-L, i.normal), V)), _Spec1Power) * _Spec1Color.xyz;  // reflection
                //col.xyz += specular.xyz;

                // Normal mapping
                float3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 light = normalize(i.lightDir);
                float diff = max(0, dot(normal, light));
                col *= diff;

                col.w = 1.0 - i.vertexW.y;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
