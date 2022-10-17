Shader "Unlit/SDF"
{
    Properties
    { 
        _Color("Color",COLOR)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff("Cutoff",Range(0,1))=0.5
        _Smoothness("Smoothness",Range(0,1))=0.1
        _SDFsize("SDFsize",float)=1
    }
    SubShader
    {
      	//��͸��shaderͨ����������3����ǩ
		Tags { 
			"Queue"="Transparent" //ѡ����Ⱦ����ΪTransparent������������Ⱦ��͸������֮������Ⱦ������
			"IgnoreProjector"="True" //������ӰͶ����Ӱ��
			"RenderType"="Transparent"//����shader����Ԥ�����õ��࣬��������shader�滻
			}

       Pass{
        
                ZWrite On  //  д�����  Ϊ��ȷ����Ⱦ˳��
                ColorMask 0  //  ��������  �������passͨ����д���κ���ɫֵ

       }

        Pass
        {
        
        ZWrite Off  //  �ر�ZWrite�����д�룩
                Blend SrcAlpha OneMinusSrcAlpha  //  Դ��ɫ����  ����͸�����
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            float smoothstep1 (float edge0, float edge1, float x)
{
   	if (x < edge0)
      	return 1;

   	if (x > edge1)
      	return 0;
    
	// ����ӳ��
    x = (x - edge0) / (edge1 - edge0);
	
    // �⻬
   	return x * x * (3 - 2 * x);
}

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            float _Cutoff;
            float _Smoothness;
            float _SDFsize;
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
         fixed4 color = _Color;

    // sdf distance from edge (scalar)
    float dist = (_Cutoff - tex2D(_MainTex, (i.uv)).r);

    // sdf distance per pixel (gradient vector)
    float2 ddist = float2(ddx(dist), ddy(dist));

    // distance to edge in pixels (scalar)
    float pixelDist = dist / length(ddist);

    color.a = saturate(0.5 - pixelDist); 

    return color;
            }
            ENDCG
        }
    }
}
