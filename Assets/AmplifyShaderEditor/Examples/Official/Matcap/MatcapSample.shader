// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Matcap"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Matcap("Matcap", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha  
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Matcap;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			fixed3 temp_cast_0 = 0.5;
			output.Emission = tex2D( _Matcap,( ( mul( UNITY_MATRIX_V , fixed4( input.worldNormal , 0.0 ) ) * 0.5 ) + temp_cast_0 ).xy).xyz;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
392;92;1247;695;1586.941;360.4467;1.6;True;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-793.4805,113.6614;Float;FLOAT3;0.0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-960.7327,77.2156;Float;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.ViewMatrixNode;21;-1119.276,47.42967;Float
Node;AmplifyShaderEditor.RangedFloatNode;23;-971.5502,205.5904;Float;Constant;_Float0;Float 0;-1;0.5;0;0
Node;AmplifyShaderEditor.WorldNormalInputsNode;26;-1199.464,141.5635;Float;True
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-618.1711,163.7383;Float;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-49.57976,164.356;Fixed;True;2;Fixed;ASEMaterialInspector;Standard;ASESampleShaders/Matcap;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0.0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;1;-422.7262,121.5546;Float;Property;_Matcap;Matcap;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;22;0;18;0
WireConnection;22;1;23;0
WireConnection;18;0;21;0
WireConnection;18;1;26;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;0;2;1;0
WireConnection;1;1;24;0
ASEEND*/
//CHKSM=02BD4B69FA341B1F51BB61BDB4CF80A292433915