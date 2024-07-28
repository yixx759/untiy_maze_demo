Shader "Unlit/BadOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
           


            struct LumaNeighbour
            {
              float m, n,e,s,w;
                float big, small,diff;
                
            };

            LumaNeighbour getNeighbours(float2 uv)
            {
                LumaNeighbour m;
                m.m = tex2D(_MainTex, uv).g;
                m.n = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(1,0)).g;
                m.e = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(0,1)).g;
                m.s = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(-1,0)).g;
                m.w = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(0,-1)).g;
                m.big = max( max( max( max(m.m,m.n),m.e),m.s),m.w);
                m.small = min( min( min( min(m.m,m.n),m.e),m.s),m.w);
                m.diff = m.big - m.small;
                return m;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
               // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                LumaNeighbour neigh = getNeighbours(i.uv);

                return neigh.diff;
            }
            ENDCG
        }
    }
}
