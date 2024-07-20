Shader "Unlit/DefRange"
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
            float start , end;
            float sharpness;

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
                float4 up =  tex2D(_MainTex, i.uv + float2(0,1)*_MainTex_TexelSize.xy);
                float4 down =  tex2D(_MainTex, i.uv - float2(0,1)*_MainTex_TexelSize.xy);
                float4 left =  tex2D(_MainTex, i.uv - float2(1,0)*_MainTex_TexelSize.xy);
                float4 right =  tex2D(_MainTex, i.uv + float2(1,0)*_MainTex_TexelSize.xy);
                
                fixed4 centre = tex2D(_MainTex, i.uv);
                
                float amount = sharpness * -1;
                float4 col = centre*(1+4*sharpness) + (amount*up +amount*right +amount*left +amount*down);
                col = lerp(start,end,col);


                
                return col;
            }
            ENDCG
        }
    }
}
