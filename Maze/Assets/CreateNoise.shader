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
            float seed, a, b,seed2;
            float2 random (float2 uv )
            {
                            
                            
              
                float x = dot(uv, float2(123.4, 234.5));
                float y = dot(uv, float2(234.5, 345.6));
                float2 gradient = float2(x,y);
                
               
               // return sin(sin(gradient* 46728.5453)) ;
               // return (sin(sin(gradient* 43758.5453+seed*frac(_Time.y)))) ;
                return (sin(sin(gradient* 43758.5453+seed))) ;
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
          float r = frac(sin(dot(co.xy, float2(12.2345,64.22343))) * 32546.222);

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
   
        float4 np = (gaussrand(lum,a*lum,uv));

        
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
              
                   float lum = dot(col.xyz,float3( 0.2125, 0.7154, 0.0721));
                     //float4 noise = CreateNoise( lum,a,b,1,-1,i.uv);

              //  float num = (i.uv.x);
             //float2 uver = ((int)(i.uv*a))/a ;
              
             float2 uver = float2(i.uv.y,i.uv.y ) ;
            
              //  uver = ((int)(uver*a))/a ;
            //    float2 uver = (((i.uv))) ;
           
              //float num = (gaussrand(0,100,uver));
              float num = (random(uver));
                return num;
               // num *= 100;
                int numer= num;
                // if(num > 50)
                // {
                //  return float4(0,(int)(num%2),0,1);
                //     
                // }
               
                if((numer % 3) == 0)
                {
                  return float4(0.89,0.2,0.2,1);
                    
                }
                else if ((numer % 3) == 1)
                {
                    return float4(0.2,0.89,0.2,1);
                    
                }
                else  
                {
                     return float4(0.2,0.2,0.89,1);
                    
                }
            
              
               // return col+ (noise-lum);
            }
            ENDCG
        }
    }
}
