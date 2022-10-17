Shader "Unlit/Signed Distance Field Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
    

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        

            #include "UnityCG.cginc"
            float sdHexPrism( float3 p, float2 h )
           {
             float3 q = abs(p);
             return max(q.z-h.y,max((q.x*0.866025+q.y*0.5),q.y)-h.x);
          }

        float map(float3 rp)
        {
            float ret = sdHexPrism(rp, float2(4, 5));

            return ret;
        }
        fixed4 raymarching(float3 rayOrigin,float3 rayDirection)
        {
          fixed4 ret=fixed4(0,0,0,0);
          int maxStep=64;
          float rayDistance=0;
          for(int i=0;i<maxStep;i++)
          {
              float3 p=rayOrigin+rayDirection*rayDistance;
              float surfaceDistance=map(p);
              if(surfaceDistance<0.001)
              {
               ret=fixed4(1,0,0,1);
               break;
              }
              rayDistance+=surfaceDistance;
          }
          return  ret;
        }
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
              
              
                float4 vertex : SV_POSITION;
                  float2 uv : TEXCOORD0;
                float3 ray:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4x4 _Corners;
            float4 camPos;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                int index=v.uv.x + (2*v.uv.y);

                o.ray= _Corners[index].xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
         
                fixed4 col = tex2D(_MainTex, i.uv);
                col=   raymarching(camPos.xyz,i.ray);
                // apply fog
           
                return col;
            }
            ENDCG
        }
    }
}
