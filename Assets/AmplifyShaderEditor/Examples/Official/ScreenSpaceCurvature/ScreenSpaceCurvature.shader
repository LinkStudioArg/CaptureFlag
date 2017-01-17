// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ScreenSpaceCurvature"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ScaleFactor("ScaleFactor", Float) = 4
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 localVertexPos;
		};

		uniform float _ScaleFactor;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.localVertexPos = vertexData.vertex.xyz ;
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			float3 normalValue33 = WorldNormalVector( input , output.Normal );
			float3 temp_output_2_0 = ddx( normalValue33 );
			float3 temp_output_7_0 = ddy( normalValue33 );
			float3 temp_cast_3 = ( ( ( cross( ( normalValue33 - temp_output_2_0 ) , ( normalValue33 + temp_output_2_0 ) ).y - cross( ( normalValue33 - temp_output_7_0 ) , ( normalValue33 + temp_output_7_0 ) ).x ) * _ScaleFactor ) / length( input.localVertexPos ) );
			output.Emission = ( temp_cast_3 + float3(0.5,0.5,0.5) );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;242.501;133.4995;1;True;False
Node;AmplifyShaderEditor.SimpleSubtractOpNode;19;-531.6992,126.5998;Float;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.CrossProductOpNode;21;-373.0967,-141.1999;Float;FLOAT3;0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-536.899,289.1003;Float;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;24;-212.5965,203.3003;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;23;-206.0973,-189.2997;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;8.402822,-21.59954;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;173.5023,10.90066;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.LengthOpNode;16;92.40041,431.3006;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;479.1028,217.2994;Float;FLOAT;0.0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.DdyOpNode;7;-820.8997,285.6999;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.DdxOpNode;2;-835.1998,-79.59997;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-573.0988,-142.6001;Float;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-575.8983,-12.50004;Float;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.CrossProductOpNode;22;-375.2969,147;Float;FLOAT3;0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.WorldNormalVector;33;-1093.197,86.89935;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;27;19.00231,144.3006;Float;Property;_ScaleFactor;ScaleFactor;-1;4;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;28;305.8015,139.5998;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.Vector3Node;30;288.9025,325.6991;Float;Constant;_Vector0;Vector 0;-1;0.5,0.5,0.5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;652.9998,-97.9001;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/ScreenSpaceCurvature;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.LocalVertexPosNode;14;-108.1999,367.4005;Float
WireConnection;19;0;33;0
WireConnection;19;1;7;0
WireConnection;21;0;17;0
WireConnection;21;1;18;0
WireConnection;20;0;33;0
WireConnection;20;1;7;0
WireConnection;24;0;22;0
WireConnection;23;0;21;0
WireConnection;25;0;23;1
WireConnection;25;1;24;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;16;0;14;0
WireConnection;29;0;28;0
WireConnection;29;1;30;0
WireConnection;7;0;33;0
WireConnection;2;0;33;0
WireConnection;17;0;33;0
WireConnection;17;1;2;0
WireConnection;18;0;33;0
WireConnection;18;1;2;0
WireConnection;22;0;19;0
WireConnection;22;1;20;0
WireConnection;28;0;26;0
WireConnection;28;1;16;0
WireConnection;0;13;29;0
ASEEND*/
//CHKSM=8016CE2F315CF6AF7A804FDF7CFB467C15DBE431