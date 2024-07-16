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
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
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
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            float seed, a, b,seed2, test, blur;
            float2 random (float2 uv )
            {
                            
                            
              
                float x = dot(uv, float2(123.4, 234.5));
                float y = dot(uv, float2(234.5, 345.6));
                float2 gradient = float2(x,y);
                
               
               // return sin(sin(gradient* 46728.5453)) ;
               // return (sin(sin(gradient* 43758.5453+seed*frac(_Time.y)))) ;
                return (sin(sin(gradient* 43758.5453))) ;
            }
            float2 random2 (float2 uv )
            {
                            
                            
              
                float x = dot(uv, float2(555.4, 234.5));
                float y = dot(uv, float2(457.5, 333.6));
                float2 gradient = float2(x,y);
                
               
               // return sin(sin(gradient* 46728.5453)) ;
                return (sin(sin(gradient* 3521.5453+seed2*frac(_Time.y)))) ;
            }
   
#define PI 3.14159265358979323846264

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


float offsetx, offsety;
            int detailp;

                
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

    float getGaussianNum( float2 uv)
    {
        float v1, v2, s;

        do
        {
            // v1 = 2 *  random(uv) - 1.0f;
            // v2 = 2 *  random(uv) - 1.0f;
            v1 = 2 *  random(uv) - 1.0f;
            v2 = 2 *   random2(uv) - 1.0f;
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
           
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
            
                
                fixed4 col = tex2D(_MainTex, i.uv);
             
                     // apply fog
                 float4 final = 1;
        
        
                int detail =detailp;
        
                 
                float lum = dot(col.xyz,float3( 0.2125, 0.7154, 0.0721));
                float4 noise = CreateNoise( lum,a,b,1,-1,i.uv);
        
               //  float num = (i.uv.x);
              //float2 uver = ((int)(i.uv*a))/a ;
              // i.uv.y = i.uv.x;
                // return i.uv.y;
        
        
        
             
        
        
        
        
                 
                 float2 space = float2(_MainTex_TexelSize.x,_MainTex_TexelSize.y)*blur;
        
                 
              float2 uver = float2(i.uv.x,i.uv.y ) +float2(0,0) ;
              float2 uvertl = float2(i.uv.x,i.uv.y ) +float2(-space.x,space.y) ;
              float2 uverl = float2(i.uv.x,i.uv.y ) +float2(-space.x,0) ;
              float2 uverbl = float2(i.uv.x,i.uv.y ) +float2(-space.x,-space.y) ;
              float2 uverb = float2(i.uv.x,i.uv.y ) +float2(0,-space.y) ;
              float2 uverbr = float2(i.uv.x,i.uv.y )+float2(space.x,-space.y)  ;
              float2 uverr = float2(i.uv.x,i.uv.y ) +float2(space.x,0) ;
              float2 uvertr = float2(i.uv.x,i.uv.y )+float2(space.x,space.y)  ;
              float2 uvert = float2(i.uv.x,i.uv.y )+float2(0,space.y)  ;
        
             
                 
             uver = ((trunc((uver*detail)))) ;
             uvertl = ((trunc((uvertl*detail)))) ;
             uverl = ((trunc((uverl*detail)))) ;
             uverbl = ((trunc((uverbl*detail)))) ;
             uverb = ((trunc((uverb*detail)))) ;
             uverbr = ((trunc((uverbr*detail)))) ;
             uverr = ((trunc((uverr*detail)))) ;
             uvertr = ((trunc((uvertr*detail)))) ;
             uvert = ((trunc((uvert*detail)))) ;
        
        
        
        
        
             
                  //   float4 noise[9];
              //   float4 lumtotal =0;
                //  int it2 = 0;
              
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uver));
             //
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uvertl));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverl));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverbl));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverb));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverbr));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uverr));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uvertr));
             // noise[it2++] = (CreateNoise( lum,a,b,1,-1,uvert));
             //
             //
        
        
        
        
        
                 
            
        
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
           
              // float num = (random(uver));
             //  num = uver;
            
             float4 total =0;
             
              
        
        
                 for(int i=0; i <9;i++)
                 {
                   //  lumtotal += noise[i];
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
               //  lumtotal /= 9;
                 
                 // if(num > 50)
                 // {
                 //  return float4(0,(int)(num%2),0,1);
                 //     
                 // }
              //  return lumtotal;
               
            return total;
        //      // return noise-lum;
        //        return col+ (lumtotal-lum)*total;
            }
            ENDCG
        }
    }
}
