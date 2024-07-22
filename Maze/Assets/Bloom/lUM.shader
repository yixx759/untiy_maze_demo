Shader "Unlit/lUM"
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
            float thresh =0;
            float softthresh =0;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
         
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

               
                
                   float lum = max(col.x,max(col.y,col.z) );
                float k = thresh *softthresh;
 float s = pow( min(max(0,(lum-thresh+k)), 2*k),2) /(4*k);
                   float cont = max(s,lum - thresh);
                cont /= max(0.000001, lum);

                
                return col*cont;
            }
            ENDCG
        }
    }
}
