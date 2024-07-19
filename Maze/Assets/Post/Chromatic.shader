Shader "Unlit/Chromatic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RedOffset ("redoffstet", vector) = (0.1,0.1, 0, 0)
        _GreenOffset ("greenoffstet", vector) = (0.1,0.1, 0, 0)
        _BlueOffset ("blueoffstet", vector) = (0.1,0.1, 0, 0)
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
            float2 _RedOffset , _GreenOffset , _BlueOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
              
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // make extreme on edges make 1 to disable
                float scale2Screen = length( (i.uv.xy - float2(0.5,0.5)));
                
               
                
                float redcol = tex2D(_MainTex, float2(i.uv + _RedOffset*scale2Screen)).x;
                float greencol = tex2D(_MainTex, float2(i.uv + _GreenOffset*scale2Screen)).y;
                float bluecol = tex2D(_MainTex, float2(i.uv+ _BlueOffset*scale2Screen)).z;
              
                return float4(redcol,greencol,bluecol,1);
            }
            ENDCG
        }
    }
}
