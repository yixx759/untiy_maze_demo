Shader "Unlit/Decal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-400" "DisableBatching"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // turn z test off
            
            

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 scrn : TEXCOORD1;
                float d : TEXCOORD2; 
                float3 ray : TEXCOORD3; 
              
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _CameraDepthTexture;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(unity_ObjectToWorld,v.vertex);
                o.ray = (o.vertex - _WorldSpaceCameraPos);

                o.vertex = UnityWorldToClipPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               o.scrn = ComputeScreenPos(o.vertex) ;
                o.d = o.vertex.z / o.vertex.w;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
              
                float d = tex2D(_CameraDepthTexture, i.scrn/i.scrn.w).x;
             
                d = LinearEyeDepth(d);
                i.ray = normalize(i.ray);
                float4 fr = float4(0,0,1,0);

                i.ray /= dot(i.ray, -unity_MatrixV[2]);
                
               float3 wrld = _WorldSpaceCameraPos + i.ray*d;
               float3 obj = mul(unity_WorldToObject, float4(wrld,1)).xyz;
                clip(0.5-abs(obj));
              // return float4(obj ,1);
               return tex2D(_MainTex,obj.zy+=0.5);
               // return float4(d,0,0,1);
            }
            ENDCG
        }
    }
}
