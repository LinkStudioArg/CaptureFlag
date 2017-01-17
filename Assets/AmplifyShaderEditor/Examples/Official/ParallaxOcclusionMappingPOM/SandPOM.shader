// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SandPom"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0.5
		_Metallic("Metallic", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		_RoughScale("Rough Scale", Float) = 0.5
		_Occlusion("Occlusion", 2D) = "white" {}
		_HeightMap("HeightMap", 2D) = "white" {}
		_Scale("Scale", Range( 0 , 1)) = 0.4247461
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 4.0
		#pragma surface surf Standard keepalpha  vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_Albedo;
			float3 viewDir;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldViewDir;
		};

		uniform sampler2D _HeightMap;
		uniform float _Scale;
		uniform float _NormalScale;
		uniform sampler2D _Normal;
		uniform sampler2D _Albedo;
		uniform sampler2D _Metallic;
		uniform float _RoughScale;
		uniform sampler2D _Roughness;
		uniform sampler2D _Occlusion;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane )
		{
			int stepIndex = 0;
			int numSteps = ( int )lerp( maxSamples, minSamples, length( fwidth( uvs ) ) * 10 );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
				currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r;
				if ( currHeight > currRayZ )
				{
					stepIndex = numSteps + 1;
				}
				else
				{
					stepIndex++;
					prevTexOffset = currTexOffset;
					prevRayZ = currRayZ;
					prevHeight = currHeight;
					currTexOffset += deltaTex;
					currRayZ -= layerHeight;
				}
			}
			int sectionSteps = 5;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
				intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
				finalTexOffset = prevTexOffset + intersection * deltaTex;
				newZ = prevRayZ - intersection * layerHeight;
				newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
				if ( newHeight > newZ )
				{
					currTexOffset = finalTexOffset;
					currHeight = newHeight;
					currRayZ = newZ;
					deltaTex = intersection * deltaTex;
					layerHeight = intersection * layerHeight;
				}
				else
				{
					prevTexOffset = finalTexOffset;
					prevHeight = newHeight;
					prevRayZ = newZ;
					deltaTex = ( 1 - intersection ) * deltaTex;
					layerHeight = ( 1 - intersection ) * layerHeight;
				}
				sectionIndex++;
			}
			return uvs + finalTexOffset;
		}


		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.worldViewDir = normalize( _WorldSpaceCameraPos - vertexData.vertex );
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			float2 OffsetPOM8 = POM( _HeightMap, input.uv_Albedo, ddx(input.uv_Albedo), ddx(input.uv_Albedo), WorldNormalVector( input, float3( 0, 0, 1 ) ), input.worldViewDir, input.viewDir, 64, 64, _Scale, 0 );
			float2 customUVs = OffsetPOM8;
			float2 temp_output_40_0 = ddx( input.uv_Albedo );
			float2 temp_output_41_0 = ddy( input.uv_Albedo );
			output.Normal = UnpackScaleNormal( tex2D( _Normal,customUVs, temp_output_40_0, temp_output_41_0) ,_NormalScale );
			output.Albedo = tex2D( _Albedo,customUVs, temp_output_40_0, temp_output_41_0).xyz;
			output.Metallic = tex2D( _Metallic,customUVs, temp_output_40_0, temp_output_41_0).r;
			output.Smoothness = ( 1.0 - ( _RoughScale * tex2D( _Roughness,customUVs, temp_output_40_0, temp_output_41_0).r ) );
			output.Occlusion = tex2D( _Occlusion,customUVs, temp_output_40_0, temp_output_41_0).r;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
392;92;1247;695;2361.934;854.0019;2.8;True;False
Node;AmplifyShaderEditor.RangedFloatNode;25;204.7048,210.9306;Float;Property;_RoughScale;Rough Scale;5;0.5;0;0
Node;AmplifyShaderEditor.WireNode;29;620.7496,395.8717;Float;FLOAT;0.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;780.7046,-269.0693;Float;True;4;Float;ASEMaterialInspector;Standard;ASESampleShaders/SandPom;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;396.7047,210.9306;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;11;60.70477,-301.0693;Float;Property;_Albedo;Albedo;0;None;True;0;False;white;Auto;False;Object;-1;Derivative;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;14;60.70477,-125.0694;Float;Property;_Normal;Normal;1;None;True;0;True;bump;Auto;True;Object;-1;Derivative;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.TexturePropertyNode;9;-1729,0.5;Float;Property;_HeightMap;HeightMap;7;None;False;white;Auto
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;15;-1288,170.5;Float;Tangent
Node;AmplifyShaderEditor.DdyOpNode;41;-655.7364,-88.30183;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.DdxOpNode;40;-661.1364,-156.5017;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.SamplerNode;23;60.70477,34.93072;Float;Property;_Metallic;Metallic;3;None;True;0;False;white;Auto;False;Object;-1;Derivative;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;20;60.70477,306.9308;Float;Property;_Roughness;Roughness;4;None;True;0;False;white;Auto;False;Object;-1;Derivative;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;21;60.70477,482.9311;Float;Property;_Occlusion;Occlusion;6;None;True;0;False;white;Auto;False;Object;-1;Derivative;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1384,-81.5;Float;0;11;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.RangedFloatNode;24;-292.5285,-343.4738;Float;Property;_NormalScale;Normal Scale;2;0.5;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-693.8807,-232.9194;Float;customUVs;1;FLOAT2;0.0,0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1448,78.5;Float;Property;_Scale;Scale;8;0.4247461;0;1
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;8;-1038.6,-21.1;Float;0;64;64;5;0.02;0;FLOAT2;0,0;SAMPLER2D;;FLOAT;0.0;FLOAT3;0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.OneMinusNode;42;532.664,38.29838;Float;FLOAT;0.0
WireConnection;29;0;21;1
WireConnection;0;0;11;0
WireConnection;0;1;14;0
WireConnection;0;3;23;1
WireConnection;0;4;42;0
WireConnection;0;5;29;0
WireConnection;26;0;25;0
WireConnection;26;1;20;1
WireConnection;11;1;39;0
WireConnection;11;3;40;0
WireConnection;11;4;41;0
WireConnection;14;1;39;0
WireConnection;14;3;40;0
WireConnection;14;4;41;0
WireConnection;14;5;24;0
WireConnection;41;0;10;0
WireConnection;40;0;10;0
WireConnection;23;1;39;0
WireConnection;23;3;40;0
WireConnection;23;4;41;0
WireConnection;20;1;39;0
WireConnection;20;3;40;0
WireConnection;20;4;41;0
WireConnection;21;1;39;0
WireConnection;21;3;40;0
WireConnection;21;4;41;0
WireConnection;39;0;8;0
WireConnection;8;0;10;0
WireConnection;8;1;9;0
WireConnection;8;2;13;0
WireConnection;8;3;15;0
WireConnection;42;0;26;0
ASEEND*/
//CHKSM=C05DB21458FE1312BA55D551576555033F4368BE