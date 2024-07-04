Shader "Unlit/Curve"
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
            float _curve;
            float _vignette;
           

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
                i.uv = i.uv *2 -1;

                float2 offset = (i.uv.yx) / _curve;
                i.uv = i.uv +  i.uv * offset*offset;
              
               float2 inverted = 1-abs(i.uv);
              
  i.uv = i.uv *0.5+0.5;

                 
                // float dist = saturate(length((abs((i.uv))))+_vignette);
                float2 vvector = float2(_vignette/ _ScreenParams.x, _vignette/ _ScreenParams.y);
               
              
                 //return  float4(inverted,0,1)  ;
                   if (i.uv.x >1 || (i.uv.x) <0 || i.uv.y >1 || (i.uv.y) <0 )
                {
                    return 0;
                }

                
                float2 vigette = smoothstep(0,(vvector), (inverted));
         
                
                
             //   return  float4(vigette,0,1)  ;
    
                
                fixed4 col = tex2D(_MainTex, i.uv);
            //smoothstep
              return (vigette.x*vigette.y)*col;
             
               // return lerp(col,0,dist );
            }
            ENDCG
        }
    }
}
