Shader "Unlit/NewUnlitShader"
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
   
            #define PI 3.14159265358979323846264
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


             float rand(float2 co)
        {
          // This one-liner can be found in many places, including:
          // http://stackoverflow.com/questions/4200224/random-noise-functions-for-glsl
          // I can't find any explanation for it, but experimentally it does seem to
          // produce approximately uniformly distributed values in the interval [0,1].
          float r = frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);

          // Make sure that we don't return 0.0
          if(r == 0.0)
          {
    return 0.000000000001;
              
          }
        
          else
          {

                 return r;
          }
         
        }



float offsetx, offsety;
            int detailp, detailp2;
   float seed, a, b,seed2, test, blur, blur2;

            
  float4 gaussrand(float mean, float sd,float2 co)
    {
          // Box-Muller method for sampling from the normal distribution
          // http://en.wikipedia.org/wiki/Normal_distribution#Generating_values_from_normal_distribution
          // This method requires 2 uniform random inputs and produces 2 
          // Gaussian random outputs.  We'll take a 3rd random variable and use it to
          // switch between the two outputs.

          float U, V, R, Z;
          // Add in the CPU-supplied random offsets to generate the 3 random values that
          // we'll use.
          U = rand(co+ float2(offsetx, offsety) );
          V = rand(co + float2(offsetx, offsety));
          R = rand(co + float2(offsetx, offsety));
          // Switch between the two random outputs.
          if(R < 0.5)
            Z = sqrt(-2.0 * log(U)) * sin(2.0 * PI * V);
          else
            Z = sqrt(-2.0 * log(U)) * cos(2.0 * PI * V);

          // Apply the stddev and mean.
          Z = Z * sd + mean;

          // Return it as a vec4, to be added to the input ("true") color.
          return float4(Z, Z, Z, 0.0);
        }



            
float4 CreateNoise(float lum,float a, float b, float max, float min, float2 uv)
    {
   
        float4 np = (gaussrand(lum,a*(1-lum),uv));

        
        float4 ng = gaussrand(0,b*b,uv);
              
      
   //return ng;
               // float finalc = np  + ng;
        
                float finalc = (np-a*lum)  + ng;
            
        return finalc;


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
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 final = 1;
        
        
                int detail =detailp;
                int detail2 =detailp2;
        
                 
                float lum = dot(col.xyz,float3( 0.2125, 0.7154, 0.0721));
               // float4 noise = CreateNoise( lum,a,b,1,-1,i.uv);
                float4 tnoise =0;
                float4 noise[9];
                int it2 = 0;

          
        
                 
                 float2 space = float2(_MainTex_TexelSize.x,_MainTex_TexelSize.y)*blur;
                 float2 space2 = float2(_MainTex_TexelSize.x,_MainTex_TexelSize.y)*blur2;
        
                 
              float2 uver = float2(i.uv.x,i.uv.y ) +float2(0,0) ;
              float2 uvertl = float2(i.uv.x,i.uv.y ) +float2(-space.x,space.y) ;
              float2 uverl = float2(i.uv.x,i.uv.y ) +float2(-space.x,0) ;
              float2 uverbl = float2(i.uv.x,i.uv.y ) +float2(-space.x,-space.y) ;
              float2 uverb = float2(i.uv.x,i.uv.y ) +float2(0,-space.y) ;
              float2 uverbr = float2(i.uv.x,i.uv.y )+float2(space.x,-space.y)  ;
              float2 uverr = float2(i.uv.x,i.uv.y ) +float2(space.x,0) ;
              float2 uvertr = float2(i.uv.x,i.uv.y )+float2(space.x,space.y)  ;
              float2 uvert = float2(i.uv.x,i.uv.y )+float2(0,space.y)  ;
        


               float2 uverp =  float2(i.uv.x,i.uv.y ) +float2(0,0) ;
              float2      uvertlp =float2(i.uv.x,i.uv.y ) +float2(-space2.x,space2.y)  ;
         float2 uverlp = float2(i.uv.x,i.uv.y ) +float2(-space2.x,0)   ;
         float2  uverblp = float2(i.uv.x,i.uv.y ) +float2(-space2.x,-space2.y)  ;
          float2 uverbp= float2(i.uv.x,i.uv.y ) +float2(0,-space2.y)  ;
         float2   uverbrp=float2(i.uv.x,i.uv.y )+float2(space2.x,-space2.y)  ;
         float2 uverrp = float2(i.uv.x,i.uv.y ) +float2(space2.x,0)   ;
          float2 uvertrp =float2(i.uv.x,i.uv.y )+float2(space2.x,space2.y)  ;
          float2 uvertp= float2(i.uv.x,i.uv.y )+float2(0,space2.y)  ;


                 uverp = ((trunc((uverp*detail2)))) ;
             uvertlp = ((trunc((uvertlp*detail2)))) ;
             uverlp = ((trunc((uverlp*detail2)))) ;
             uverblp = ((trunc((uverblp*detail2)))) ;
             uverbp = ((trunc((uverbp*detail2)))) ;
             uverbrp = ((trunc((uverbrp*detail2)))) ;
             uverrp = ((trunc((uverrp*detail2)))) ;
             uvertrp = ((trunc((uvertrp*detail2)))) ;
             uvertp = ((trunc((uvertp*detail2)))) ;
                 
             uver = ((trunc((uver*detail)))) ;
             uvertl = ((trunc((uvertl*detail)))) ;
             uverl = ((trunc((uverl*detail)))) ;
             uverbl = ((trunc((uverbl*detail)))) ;
             uverb = ((trunc((uverb*detail)))) ;
             uverbr = ((trunc((uverbr*detail)))) ;
             uverr = ((trunc((uverr*detail)))) ;
             uvertr = ((trunc((uvertr*detail)))) ;
             uvert = ((trunc((uvert*detail)))) ;

                
            noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverp));
             
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uvertlp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverlp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverblp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverbp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverbrp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverrp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uvertrp));
             noise[it2++] = (CreateNoise( lum,a,b,1,-1,uvertp));
        
        float num[9];
                 int it= 0;
             num[it++] = (gaussrand(3456,1346,uver));
             num[it++] = (gaussrand(3456,1346,uvertl));
             num[it++] = (gaussrand(3456,1346,uverl));
             num[it++] = (gaussrand(3456,1346,uverbl));
             num[it++] = (gaussrand(3456,1346,uverb));
             num[it++] = (gaussrand(3456,1346,uverbr));
             num[it++] = (gaussrand(3456,1346,uverr));
             num[it++] = (gaussrand(3456,1346,uvertr));
             num[it++] = (gaussrand(3456,1346,uvert));
           

            float4 total =0;
             
              
        
                
                 for(int i=0; i <9;i++)
                 {
                   tnoise += noise[i];
                  if(((int)num[i] % 3) == 0)
                 {
                 total += float4(0.89,0.1,0.1,1);
                     
                 }
                 else if (((int)num[i] % 3) == 1)
                 {
                      total += float4(0.1,0.89,0.1,1);
                     
                 }
                 else  
                 {
                       total += float4(0.1,0.1,0.89,1);
                     
                 }
        
        
                     
                 }
                
                 total /= 9;

                tnoise /=9;

            return total;
            // return total;
           //   return (((tnoise-lum)))*total;
            return col+ (((tnoise-lum)))*total;
              return col;
            }
            ENDCG
        }
    }
}
