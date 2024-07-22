Shader "Unlit/Box"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
  float intensity = 1;
        ENDCG
        
        
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
            float2 upDelta;
            float4 _MainTex_TexelSize;

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                 float4 box =0;
                float2 space =  float2(upDelta.x*_MainTex_TexelSize.x,upDelta.y*_MainTex_TexelSize.y);
              
                fixed4 up = tex2D(_MainTex, i.uv+ float2(0,1) *space);
                fixed4 down = tex2D(_MainTex, i.uv+ float2(0,-1) *space);
                fixed4 left = tex2D(_MainTex, i.uv+ float2(-1,0) *space);
                fixed4 right = tex2D(_MainTex, i.uv+ float2(1,0) *space);
                // apply fog
                box = up + down + left + right;
                
                box /=4;
                return box;
            }
            ENDCG
        }
        
        
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
            float2  downDelta;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

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
                float4 box =0;
                float2 space =  float2(downDelta.x*_MainTex_TexelSize.x,downDelta.y*_MainTex_TexelSize.y);
                
                fixed4 up = tex2D(_MainTex, i.uv+ float2(0,1) *space);
                fixed4 down = tex2D(_MainTex, i.uv+ float2(0,-1) *space);
                fixed4 left = tex2D(_MainTex, i.uv+ float2(-1,0) *space);
                fixed4 right = tex2D(_MainTex, i.uv+ float2(1,0) *space);
                // apply fog
                box =  up + down + left + right;
                
                box /=4;
                return box;
            }
            ENDCG
        }
        
        
         Pass
        {
            
            Blend One One
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
             
                return tex2D(_MainTex, i.uv) * intensity;
            }
            ENDCG
        }
    }
    }
    
    
    

