Shader "Custom/Shadow" 
{
	Properties
	{
		_MainTex ("Base (RGB) Mask (A)", 2D) = "black" {}
		_Color ("Tint Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		LOD 200
		Cull Off
		Lighting Off
		Fog { Mode Off }
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			
			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv = v.vertex.xy;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				float2 q = abs(i.uv)/1600;
	        	float val = sqrt(q.x*q.x + q.y*q.y);
	        	
				return half4(0,0,0,val+i.color.a);
			}
			ENDCG
		}
	}
}