Shader "Unlit/CreateNoise"
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
            float seed, a, b;
            float2 random (float2 uv )
            {
                            
                            
              
                float x = dot(uv, float2(123.4, 234.5));
                float y = dot(uv, float2(234.5, 345.6));
                float2 gradient = float2(x,y);
                
               
               // return sin(sin(gradient* 46728.5453)) ;
                return sin(sin(gradient* 43758.5453+seed)) ;
            }



      float possionInvertedSearch(float lambda, float2 uv)
    {

        float x = 0;
        float p = (exp( -lambda));
        
        float s = p;
        float u = random(uv);
       // print(u);
        while (u > s)
        {
            x += 1;
            p = p * lambda / x;
            s += p;
        }

        return x;

    }

    float getGaussianNum( float2 uv)
    {
        float v1, v2, s;

        do
        {
            v1 = 2 *  random(uv) - 1.0f;
            v2 = 2 *  random(uv) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0);

        s = sqrt((-2.0f * log(s)) / s);
        return v1 * s;
        

    }

    float getGaussian(float mean, float sd, float max, float min, float2 uv)
    {
        float x;

       do
       {
            x = mean + getGaussianNum(uv) * sd;
        } while (x < min || x > max);

        return x;


    }

   float CreateNoise(float lum,float a, float b, float max, float min, float2 uv)
    {
   
        float np = possionInvertedSearch(a*lum,uv);
  
        float ng = getGaussian(0,b*b,max,min,uv);
  
   
        return np / a + ng;


    }






            
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
                    // apply fog
                
                float noise = CreateNoise( dot(col.xyz,float3( 0.2125, 0.7154, 0.0721)),a,b,1,-1,i.uv);

                
                return col *noise;
            }
            ENDCG
        }
    }
}
