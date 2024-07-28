Shader "Unlit/AntiAliasing"
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
       

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
           
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
 float FixThresh;
 float RelThresh;

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
          
                return o;
            }

            float filter(LumaNeighbour body)
            {

            float filter = (body.n+body.e+body.s+body.w);
                filter *= 1.0/4;
                return filter;
            }


            bool edgeOr(LumaNeighbour LUM)
            {
                float Hor = abs( LUM.n+LUM.s - LUM.m*2);
                float Ver = abs( LUM.e+LUM.w - LUM.m*2);

                    return Hor> Ver;
                
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                LumaNeighbour neigh = getNeighbours(i.uv);

                float filtere =  abs(filter(neigh) - neigh.m);
                filtere /= neigh.diff;
                filtere = smoothstep(0,1, filtere);
                filtere *= filtere;

bool Horizontal = edgeOr(neigh);
                float4 horcol = Horizontal ? float4(1,0,0,1) :   float4(0,1,0,1);
                return neigh.diff < max(FixThresh, neigh.big*RelThresh) ? 0 :horcol;;
                return col.g;
                //add diagonal
                //put in options to stitch to acc luma
                //adn one which stores luma in aplha
                return sqrt(dot(col,float3(0.2126, 0.7152, 0.0722) ));
            }
            ENDCG
        }
    }
}
