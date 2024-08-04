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
            #pragma multi_compile Diagonal No_Diagonal
            #pragma multi_compile _ LOW MID
            // make fog work


         #if defined(LOW)
            
        #define SampleC 4
        #define EdgeMove 1.0, 1.0, 2.0, 2.0
            
        #elif defined(MID)

             #define SampleC 8
        #define EdgeMove 1.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 4.0
            
            #else
            
            #define SampleC 10
        #define EdgeMove 1.0, 1.0, 1.0, 1.0, 1.0, 2.0, 2.0, 2.0, 2.0, 4.0
            
            #endif


            const static float edgeStepS[SampleC] = {EdgeMove};

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
 float filterMult;
 bool enab;
        

            struct LumaNeighbour
            {
              float m, n,e,s,w, ne, nw, se,sw;
                float big, small,diff;
                
            };

            LumaNeighbour getNeighbours(float2 uv)
            {
                
                LumaNeighbour m;
                m.m = tex2D(_MainTex, uv).g;
                m.n = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(0,1)).g;
                m.e = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(1,0)).g;
                m.s = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(0,-1)).g;
                m.w = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(-1,0)).g;


                #if defined(Diagonal)

                  m.ne = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(1,1)).g;
                m.nw = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(-1,1)).g;
                m.sw = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(-1,-1)).g;
                m.se = tex2D(_MainTex, uv + _MainTex_TexelSize.xy * float2(1,-1)).g;

                
                #endif
                
                
                
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

            #if defined(Diagonal)
            {
                     filter *= 2;
                filter += body.ne +body.nw +body.se +body.nw ;
                filter *= 1.0/12;
            }
                #else
                {
                    
                    filter *= 1.0/4; 
                }
                #endif
           
                return filter;
            }


            bool edgeOr(LumaNeighbour LUM)
            {
                float Hor =1;
                float Ver =1;
                #if defined(Diagonal)
                {

                 Hor = 2 *abs( LUM.n+LUM.s - LUM.m*2) +
                   abs (LUM.ne + LUM.se - 2*LUM.e)+
                   abs (LUM.nw + LUM.sw - 2*LUM.w);

                
                 Ver = 2*abs( LUM.e+LUM.w - LUM.m*2) +
                   abs (LUM.ne + LUM.nw - 2*LUM.n)+
                   abs (LUM.se + LUM.sw - 2*LUM.s);


                    
                }
                #else
                {

                   Hor = abs( LUM.n+LUM.s - LUM.m*2) ;

                
                 Ver = abs( LUM.e+LUM.w - LUM.m*2) ;
            }
                #endif
                
               
                    return Hor >= Ver;
                
            }
            
                
            float getEdgeBlend(float2 uv, LumaNeighbour Luma, bool H, float pixelS, float otherLuma, float GradiaentLuma)
            {



              
                float2 edgeuv = uv;
                float2 uvStep =0.0;


//make func go across and check toip bottom or other way round  not contine up or across
                
                if(H)
                {
                   // edgeuv.y += 0.5*pixelS;
                    edgeuv.y += pixelS;
                    uvStep.x = _MainTex_TexelSize.x;
                    
                }
                else
                {
                   // edgeuv.x += 0.5*pixelS;
                    edgeuv.x +=pixelS;
                    uvStep.y = _MainTex_TexelSize.y;
                }
                float edgeLuma = 0.5 * (Luma.m+otherLuma);
                float gradientThresh = 0.25*GradiaentLuma;
          
                float2 uvP = edgeuv + uvStep;
                float2 prevp = uv+uvStep;
                float inbe =  lerp(tex2D(_MainTex, uvP).g,tex2D(_MainTex, prevp).g, 0.5 );
	            float lumaGradientP =( inbe  - edgeLuma);
	                bool atEndP = abs(lumaGradientP) >= gradientThresh;
                
                UNITY_UNROLL
                for(int i =0; i < SampleC && !atEndP;i++ )
                {
                    prevp += uvStep * edgeStepS[i] ;
                     uvP += uvStep;
                    inbe =  lerp(tex2D(_MainTex, uvP).g,tex2D(_MainTex, prevp).g, 0.5 );
	             lumaGradientP = ( inbe - edgeLuma);
	                 atEndP = abs(lumaGradientP) >= gradientThresh;


                    
                }
              
                float2 uvN = edgeuv - uvStep;
                prevp = uv - uvStep;
                inbe = lerp(tex2D(_MainTex, uvN).g,tex2D(_MainTex, prevp).g, 0.5 );
	            float lumaGradientN = (inbe - edgeLuma);
	                bool atEndN = abs(lumaGradientN) >= gradientThresh;
                
                UNITY_UNROLL
                for(int i =0; i < SampleC && !atEndN;i++ )
                {

                     uvN -= uvStep * edgeStepS[i];
                     prevp -= uvStep;
                     inbe = lerp(tex2D(_MainTex, uvN).g,tex2D(_MainTex, prevp).g, 0.5 );
	                  lumaGradientN = (inbe - edgeLuma);
	                 atEndN = abs(lumaGradientN) >= gradientThresh;


                    
                }





                
            float distFrom, distTo, dist;
                bool deltaSighn;
                if(H)
                {
                    distFrom = uvP.x - uv.x;
                     distTo=   uv.x-uvN.x;
                }
                else
                {
                    
                    distFrom = uvP.y - uv.y;
                     distTo =  uv.y-uvN.y ;
                }
             
                if(distFrom <= distTo)
                {
                    dist = distFrom;
                    deltaSighn = lumaGradientP >= 0;
                    
                }
                else
                {
                      dist = distTo;
                     deltaSighn = lumaGradientN >= 0;
                }
                
                
                if (deltaSighn == (Luma.m - edgeLuma >= 0)) {
		            return 0.0;
	            }
	            else {
		           
		            return 0.5 - dist / (distTo +distFrom ) ;
	            }

                
                
            }






            
            
            fixed4 frag (v2f i) : SV_Target
            {
                // if(i.uv.x > 0.5 && i.uv.x <0.50 + _MainTex_TexelSize.x && i.uv.y > 0.5 && i.uv.y <0.50 + _MainTex_TexelSize.y)
                // {
                //     return float4(1,0,0,1);
                //     
                // }


            
           
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                LumaNeighbour neigh = getNeighbours(i.uv);
              
                // if (neigh.diff < max(FixThresh, neigh.big*RelThresh))
                // {
                //     return 0;
                // }

                
                float filtere =  abs(filter(neigh) - neigh.m);
              
               
               filtere = saturate(filtere/neigh.diff);
                filtere = smoothstep(0,1, filtere);
              
                filtere *= filtere;

                
            bool Horizontal = edgeOr(neigh);
           
            float2 nuuv = i.uv;
                float lp, ln;
                float pixelstep;
                if (Horizontal)
                {
                   
                    pixelstep = _MainTex_TexelSize.y;
                    lp = neigh.n;
                    ln = neigh.s;
                    
                }
                else
                {
                    
                    pixelstep = _MainTex_TexelSize.x;
                    lp = neigh.e;
                    ln = neigh.w;
                }

                float gradp = abs(lp - neigh.m);
                float gradn = abs(ln - neigh.m);
                float grad ;
                float l ;
                if((gradp < gradn))
                {

                pixelstep = -pixelstep ;
                    grad = gradn;
                    l = ln;
                }
                else
                {
                    grad = gradp;
                    l = lp;
                }

             
            float fl = getEdgeBlend(i.uv, neigh,Horizontal,pixelstep, l,grad);
      if(enab)
      {

        filtere = max(filtere, saturate(fl))*filterMult;   
          
      }
      else
      {
           filtere *= filterMult;
      }
           
                if (Horizontal)
                {
                   
                    nuuv.y +=  pixelstep;
                    
                }
                else
                {
                         nuuv.x += pixelstep;
                }





                

                if (neigh.diff < max(FixThresh, neigh.big*RelThresh))
                {
             
                  return col;
                  
                }
             
              
             
                
            
               
                    return  lerp( tex2D(_MainTex, i.uv),tex2D(_MainTex, nuuv),filtere );
              
              
              //  float4 horcol = Horizontal ? float4(1,0,0,1) :   float4(0,1,0,1);
             //   return neigh.diff < max(FixThresh, neigh.big*RelThresh) ? 0 :horcol;;
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
