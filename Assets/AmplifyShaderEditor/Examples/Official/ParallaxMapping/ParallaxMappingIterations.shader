// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ParallaxMappingIterations"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Tiling("Tiling", Float) = 4
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 2)) = 1
		_Metallic("Metallic", 2D) = "white" {}
		_MetallicAmount("Metallic Amount", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", 2D) = "white" {}
		_SmoothnessScale("Smoothness Scale", Float) = 0
		_Occlusion("Occlusion", 2D) = "white" {}
		_HeightMap("HeightMap", 2D) = "white" {}
		_Parallax("Parallax", Range( 0 , 0.1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 uv_HeightMap;
			float3 viewDir;
		};

		uniform fixed _Tiling;
		uniform sampler2D _HeightMap;
		uniform fixed _Parallax;
		uniform fixed _NormalScale;
		uniform sampler2D _Normal;
		uniform sampler2D _Albedo;
		uniform sampler2D _Metallic;
		uniform fixed _MetallicAmount;
		uniform sampler2D _Smoothness;
		uniform fixed _SmoothnessScale;
		uniform sampler2D _Occlusion;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			fixed2 temp_cast_0 = _Tiling;
			o.texcoord_0.xy = vertexData.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			float2 Offset4 = ( ( tex2D( _HeightMap,input.uv_HeightMap).r - 1.0 ) * input.viewDir.xy * _Parallax ) + input.texcoord_0;
			float2 Offset49 = ( ( tex2D( _HeightMap,Offset4).r - 1.0 ) * input.viewDir.xy * _Parallax ) + Offset4;
			float2 Offset52 = ( ( tex2D( _HeightMap,Offset49).r - 1.0 ) * input.viewDir.xy * _Parallax ) + Offset49;
			float2 Offset54 = ( ( tex2D( _HeightMap,Offset52).r - 1.0 ) * input.viewDir.xy * _Parallax ) + Offset52;
			fixed2 Offset = Offset54;
			output.Normal = UnpackScaleNormal( tex2D( _Normal,Offset) ,_NormalScale );
			output.Albedo = tex2D( _Albedo,Offset).xyz;
			output.Metallic = ( tex2D( _Metallic,Offset).r + _MetallicAmount );
			output.Smoothness = ( tex2D( _Smoothness,Offset).r * _SmoothnessScale );
			output.Occlusion = tex2D( _Occlusion,Offset).r;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
392;92;1247;695;3198.203;106.9992;1.3;True;False
Node;AmplifyShaderEditor.WireNode;14;-969.1005,34.99995;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;15;-967.2003,194.6;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;16;-959.6003,321.9;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;18;-967.2002,475.7998;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;17;-962.8002,615.8001;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.SamplerNode;5;-617.7998,105.7;Float;Property;_Normal;Normal;2;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;6;-909.7999,213.8;Float;Property;_NormalScale;Normal Scale;3;1;0;2
Node;AmplifyShaderEditor.RangedFloatNode;21;-2596,-60.60016;Float;Property;_Tiling;Tiling;-1;4;0;0
Node;AmplifyShaderEditor.WireNode;57;-1835.593,83.00086;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;59;-1843.193,366.1007;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;63;-1854.594,615.0007;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;72;-1563.894,96.3009;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;74;-1525.893,109.6009;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;73;-1797.594,172.3009;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;76;-1583.393,374.9009;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;77;-1806.994,439.9008;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;78;-1552.194,397.0009;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;80;-1583.393,625.8008;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;82;-1565.193,659.6006;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;81;-1784.893,697.3006;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.ParallaxMappingNode;4;-1751.3,-56.79994;Float;Normal;FLOAT2;FLOAT2;FLOAT;FLOAT2;FLOAT;FLOAT;FLOAT3;0,0
Node;AmplifyShaderEditor.ParallaxMappingNode;49;-1765.997,212.9008;Float;Normal;FLOAT2;FLOAT2;FLOAT;FLOAT2;FLOAT;FLOAT;FLOAT3;0,0
Node;AmplifyShaderEditor.ParallaxMappingNode;52;-1767.744,467.1004;Float;Normal;FLOAT2;FLOAT2;FLOAT;FLOAT2;FLOAT;FLOAT;FLOAT3;0,0
Node;AmplifyShaderEditor.SamplerNode;1;-582,-129.1001;Float;Property;_Albedo;Albedo;1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.WireNode;71;-2155.494,143.7008;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;75;-2173.593,407.4007;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.SamplerNode;53;-2167.34,712.5007;Float;Property;_TextureSample3;Texture Sample 3;7;None;True;0;False;white;Auto;False;Instance;10;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;51;-2167.34,442.501;Float;Property;_TextureSample2;Texture Sample 2;7;None;True;0;False;white;Auto;False;Instance;10;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;50;-2163.993,189.9015;Float;Property;_TextureSample1;Texture Sample 1;7;None;True;0;False;white;Auto;False;Instance;10;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.WireNode;70;-1856.694,894.2998;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;69;-2222.694,867.3996;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;79;-2182.694,676.5005;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;64;-2200.494,611.6006;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;62;-2192.794,371.8004;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.ParallaxMappingNode;54;-1769.344,735.5002;Float;Normal;FLOAT2;FLOAT2;FLOAT;FLOAT2;FLOAT;FLOAT;FLOAT3;0,0
Node;AmplifyShaderEditor.WireNode;58;-2173.793,109.0009;Float;FLOAT3;FLOAT3,0,0
Node;AmplifyShaderEditor.WireNode;48;-1847.899,-125.1993;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.WireNode;47;-2158.197,-127.1994;Float;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1293.3,291.4999;Float;Offset;1;FLOAT2;FLOAT2,0
Node;AmplifyShaderEditor.SamplerNode;11;-596.6002,275.0999;Float;Property;_Metallic;Metallic;4;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;41;-546.0994,444.3003;Float;Property;_MetallicAmount;Metallic Amount;5;0;0;1
Node;AmplifyShaderEditor.SamplerNode;12;-589.1003,545.7;Float;Property;_Smoothness;Smoothness;6;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;38;-477.2994,728.0001;Float;Property;_SmoothnessScale;Smoothness Scale;7;0;0;0
Node;AmplifyShaderEditor.SamplerNode;7;-593.6,815;Float;Property;_Occlusion;Occlusion;8;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;10;-2139.3,-93.09997;Float;Property;_HeightMap;HeightMap;9;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;FLOAT2;FLOAT2;FLOAT2;FLOAT;FLOAT;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.WireNode;113;-1869.302,875.4011;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;112;-2222.9,840.2011;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;111;-2203.7,584.2009;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;114;-1858.101,595.4011;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;115;-1853.301,347.4012;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;110;-2197.3,344.2014;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;116;-1848.502,64.2011;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;109;-2192.501,83.40113;Float;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-2793.601,269.6991;Float;Property;_Parallax;Parallax;10;0;0;0.1
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;119.6998,201.8002;Fixed;True;2;Fixed;ASEMaterialInspector;Standard;ASESampleShaders/ParallaxMappingIterations;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;FLOAT3;FLOAT3;FLOAT3;FLOAT3;FLOAT3;FLOAT;0,0,0;FLOAT;FLOAT3;FLOAT;FLOAT3;FLOAT3;FLOAT;FLOAT3;0,0,0;FLOAT;FLOAT;OBJECT;FLOAT3;OBJECT;FLOAT;OBJECT;0,0,0;OBJECT;FLOAT;FLOAT3;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-243.8992,299.2003;Float;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-237.3988,473.5015;Float;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;117;-2409.904,-72.99911;Float;0;-1;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;8;-2659.698,344.3994;Float;Tangent
WireConnection;14;0;13;0
WireConnection;15;0;13;0
WireConnection;16;0;13;0
WireConnection;18;0;13;0
WireConnection;17;0;13;0
WireConnection;5;1;15;0
WireConnection;5;5;6;0
WireConnection;57;0;58;0
WireConnection;59;0;62;0
WireConnection;63;0;64;0
WireConnection;72;0;4;0
WireConnection;74;0;4;0
WireConnection;73;0;74;0
WireConnection;76;0;49;0
WireConnection;77;0;78;0
WireConnection;78;0;49;0
WireConnection;80;0;52;0
WireConnection;82;0;52;0
WireConnection;81;0;82;0
WireConnection;4;0;48;0
WireConnection;4;1;10;1
WireConnection;4;2;116;0
WireConnection;4;3;57;0
WireConnection;49;0;73;0
WireConnection;49;1;50;1
WireConnection;49;2;115;0
WireConnection;49;3;59;0
WireConnection;52;0;77;0
WireConnection;52;1;51;1
WireConnection;52;2;114;0
WireConnection;52;3;63;0
WireConnection;1;1;14;0
WireConnection;71;0;72;0
WireConnection;75;0;76;0
WireConnection;53;1;79;0
WireConnection;51;1;75;0
WireConnection;50;1;71;0
WireConnection;70;0;69;0
WireConnection;69;0;8;0
WireConnection;79;0;80;0
WireConnection;64;0;8;0
WireConnection;62;0;8;0
WireConnection;54;0;81;0
WireConnection;54;1;53;1
WireConnection;54;2;113;0
WireConnection;54;3;70;0
WireConnection;58;0;8;0
WireConnection;48;0;47;0
WireConnection;47;0;117;0
WireConnection;13;0;54;0
WireConnection;11;1;16;0
WireConnection;12;1;18;0
WireConnection;7;1;17;0
WireConnection;113;0;112;0
WireConnection;112;0;9;0
WireConnection;111;0;9;0
WireConnection;114;0;111;0
WireConnection;115;0;110;0
WireConnection;110;0;9;0
WireConnection;116;0;109;0
WireConnection;109;0;9;0
WireConnection;0;0;1;0
WireConnection;0;1;5;0
WireConnection;0;3;40;0
WireConnection;0;4;100;0
WireConnection;0;5;7;1
WireConnection;40;0;11;1
WireConnection;40;1;41;0
WireConnection;100;0;12;1
WireConnection;100;1;38;0
WireConnection;117;0;21;0
ASEEND*/
//CHKSM=39C17E8B7D2EE9E45C50D66D15F8974DEC5F9032