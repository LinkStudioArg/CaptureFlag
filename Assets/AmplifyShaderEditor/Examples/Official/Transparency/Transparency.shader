// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Transparency"
{
	Properties
	{
		_UVOffset1("UVOffset1", Vector) = (-0.1,-0.1,-0.1,0)
		_TextureSample3("Texture Sample 3", CUBE) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_TextureSample1("Texture Sample 1", CUBE) = "white" {}
		_char_woodman_normals("char_woodman_normals", 2D) = "bump" {}
		_Cubemap("Cubemap", CUBE) = "white" {}
		_UVOffset0("UVOffset0", Vector) = (0.07,0.1,0.1,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_char_woodman_normals;
			float3 worldRefl;
			INTERNAL_DATA
		};

		uniform sampler2D _char_woodman_normals;
		uniform samplerCUBE _TextureSample1;
		uniform float3 _UVOffset0;
		uniform samplerCUBE _Cubemap;
		uniform samplerCUBE _TextureSample3;
		uniform float3 _UVOffset1;
		uniform float _Opacity;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			float3 tex2DNode10 = UnpackNormal( tex2D( _char_woodman_normals,input.uv_char_woodman_normals) );
			output.Normal = tex2DNode10;
			output.Albedo = float4(1,1,1,0).rgb;
			output.Emission = ( 1.0 * float4( texCUBE( _TextureSample1,( WorldReflectionVector( input , tex2DNode10 ) + _UVOffset0 )).r , texCUBE( _Cubemap,WorldReflectionVector( input , tex2DNode10 )).g , texCUBE( _TextureSample3,( WorldReflectionVector( input , tex2DNode10 ) + _UVOffset1 )).b , 0 ) ).xyz;
			output.Metallic = 1.0;
			output.Alpha = _Opacity;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1476.315;448.0558;1.6;True;False
Node;AmplifyShaderEditor.SamplerNode;10;-1193.5,-7.000012;Float;Property;_char_woodman_normals;char_woodman_normals;-1;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;20;-566.8156,525.7439;Float;Property;_TextureSample3;Texture Sample 3;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.WorldReflectionVector;9;-1042.699,194.6001;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-741.8156,623.7439;Float;FLOAT3;0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;295.4,-92.59999;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/Transparency;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Fade;0.5;True;True;0;True;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;47.89995,31.40003;Float;FLOAT;0.0;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.ColorNode;16;94.68337,-258.8558;Float;Constant;_Color0;Color 0;-1;1,1,1,0
Node;AmplifyShaderEditor.RangedFloatNode;17;158.6833,358.1439;Float;Constant;_Metallic;Metallic;-1;1;0;1
Node;AmplifyShaderEditor.RangedFloatNode;18;159.6833,282.1439;Float;Property;_Smoothness;Smoothness;-1;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;4;-306.5,52;Float;Constant;_Float0;Float 0;-1;1;0;0
Node;AmplifyShaderEditor.AppendNode;31;-159.3181,274.3439;Float;FLOAT4;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.Vector3Node;24;-1001.816,470.7439;Float;Property;_UVOffset0;UVOffset0;-1;0.07,0.1,0.1
Node;AmplifyShaderEditor.Vector3Node;25;-1009.816,641.7439;Float;Property;_UVOffset1;UVOffset1;-1;-0.1,-0.1,-0.1
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-726.8156,352.7439;Float;FLOAT3;0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SamplerNode;1;-559.5,152;Float;Property;_Cubemap;Cubemap;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;19;-573.8156,343.7439;Float;Property;_TextureSample1;Texture Sample 1;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;14;7.680939,452.8441;Float;Property;_Opacity;Opacity;-1;1;0;1
WireConnection;20;1;26;0
WireConnection;9;0;10;0
WireConnection;26;0;9;0
WireConnection;26;1;25;0
WireConnection;0;0;16;0
WireConnection;0;1;10;0
WireConnection;0;2;7;0
WireConnection;0;3;17;0
WireConnection;0;8;14;0
WireConnection;7;0;4;0
WireConnection;7;1;31;0
WireConnection;31;0;19;1
WireConnection;31;1;1;2
WireConnection;31;2;20;3
WireConnection;23;0;9;0
WireConnection;23;1;24;0
WireConnection;1;1;9;0
WireConnection;19;1;23;0
ASEEND*/
//CHKSM=9A12D8EA2C753CD72B7C683F74388C3342739295