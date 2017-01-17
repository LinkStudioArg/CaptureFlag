// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/CubemapReflections"
{
	Properties
	{
		_Metallic("Metallic", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		_Cubemap("Cubemap", CUBE) = "white" {}
		_Normals("Normals", 2D) = "bump" {}
		_Albedo("Albedo", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_Normals;
			float2 uv_Albedo;
			float3 worldRefl;
			INTERNAL_DATA
			float2 uv_Metallic;
		};

		uniform sampler2D _Normals;
		uniform sampler2D _Albedo;
		uniform samplerCUBE _Cubemap;
		uniform sampler2D _Metallic;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			float3 tex2DNode10 = UnpackNormal( tex2D( _Normals,input.uv_Normals) );
			output.Normal = tex2DNode10;
			output.Albedo = ( tex2D( _Albedo,input.uv_Albedo) * 0.5 ).xyz;
			output.Emission = ( ( 1.0 - 0.5 ) * texCUBE( _Cubemap,WorldReflectionVector( input , tex2DNode10 )) ).xyz;
			output.Metallic = tex2D( _Metallic,input.uv_Metallic).x;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
392;92;1247;695;977.3164;225.4564;1;True;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-151.5,-27;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-150.5,174;Float;FLOAT;0.0;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.OneMinusNode;6;-292.5,81;Float;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;4;-466.5,11;Float;Constant;_Float0;Float 0;-1;0.5;0;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;71,2;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/CubemapReflections;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.WorldReflectionVector;9;-844.6999,324.6002;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;1;-497.5,242;Float;Property;_Cubemap;Cubemap;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;13;-210.7001,302.3;Float;Property;_Metallic;Metallic;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;2;-636.5,-254;Float;Property;_Albedo;Albedo;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;10;-1193.5,-7.000012;Float;Property;_Normals;Normals;-1;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;3;0;2;0
WireConnection;3;1;4;0
WireConnection;7;0;6;0
WireConnection;7;1;1;0
WireConnection;6;0;4;0
WireConnection;0;0;3;0
WireConnection;0;1;10;0
WireConnection;0;2;7;0
WireConnection;0;3;13;0
WireConnection;9;0;10;0
WireConnection;1;1;9;0
ASEEND*/
//CHKSM=E44D4F3410A71AF101316A957D9EF911A0F5F2FF