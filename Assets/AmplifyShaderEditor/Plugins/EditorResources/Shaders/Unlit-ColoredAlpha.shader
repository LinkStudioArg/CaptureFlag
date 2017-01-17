Shader "Unlit/Colored Transparent" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_Position;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					#if UNITY_VERSION >= 540
					o.vertex = UnityObjectToClipPos(v.vertex);
					#else
					o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz,1));
					#endif
					o.color = v.color * _Color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					//cuts the lines 19 pixels from the top of the current window/screen, otherwise it renders on top of the title
					clip(i.vertex.y - 19);
					return i.color * tex2D(_MainTex, i.texcoord);
				}
			ENDCG
		}
	}
}
