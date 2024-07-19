Shader "Unlit/Noiseer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Col ("Texture", 2D) = "white" {}
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
   
            #define PI 3.14159265358979323846264
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

            sampler2D _MainTex, _Col;
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
        
   float  a, b;

            
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
                float lum = dot(col.xyz,float3( 0.2125, 0.7154, 0.0721));
                float4 noise = CreateNoise( lum,a,b,1,-1,i.uv);
                 fixed4 chroma = tex2D(_Col, i.uv*3);
             

            return col+(((noise-lum)*chroma));
   
       
            }
            ENDCG
        }
    }
}
