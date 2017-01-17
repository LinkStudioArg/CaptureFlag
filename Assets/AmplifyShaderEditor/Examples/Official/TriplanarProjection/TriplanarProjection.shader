// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Triplanar"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TriplanarAlbedo("Triplanar Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_TopAlbedo("Top Albedo", 2D) = "white" {}
		_TopNormal("Top Normal", 2D) = "bump" {}
		_WorldtoObjectSwitch("World to Object Switch", Range( 0 , 1)) = 0
		_CoverageAmount("Coverage Amount", Range( -1 , 1)) = 0
		_CoverageFalloff("Coverage Falloff", Range( 0.01 , 2)) = 0.5
		_Specular("Specular", Range( 0 , 1)) = 0.02
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 2.5
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 localVertexPos;
			float2 uv_TopNormal;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform sampler2D _TopNormal;
		uniform fixed _CoverageAmount;
		uniform fixed _CoverageFalloff;
		uniform fixed _WorldtoObjectSwitch;
		uniform sampler2D _TriplanarAlbedo;
		uniform sampler2D _TopAlbedo;
		uniform fixed _Specular;
		uniform fixed _Smoothness;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.localVertexPos = vertexData.vertex.xyz ;
		}

		void surf( Input input , inout SurfaceOutputStandardSpecular output )
		{
			fixed3 BlendComponents = ( abs( mul( fixed4( WorldNormalVector( input , float3( 0,0,1 )) , 0.0 ) , unity_ObjectToWorld ) ) / dot( abs( mul( fixed4( WorldNormalVector( input , float3( 0,0,1 )) , 0.0 ) , unity_ObjectToWorld ) ) , fixed3(1,1,1) ) );
			fixed3 CalculatedNormal = lerp( ( ( ( UnpackNormal( tex2D( _Normal,float2( input.localVertexPos.y , input.localVertexPos.z )) ) * BlendComponents.x ) + ( UnpackNormal( tex2D( _Normal,float2( input.localVertexPos.x , input.localVertexPos.z )) ) * BlendComponents.y ) ) + ( UnpackNormal( tex2D( _Normal,float2( input.localVertexPos.x , input.localVertexPos.y )) ) * BlendComponents.z ) ) , UnpackNormal( tex2D( _TopNormal,input.uv_TopNormal) ) , pow( saturate( ( WorldNormalVector( input , float3( 0,0,1 )).y + _CoverageAmount ) ) , _CoverageFalloff ) );
			fixed3 PixelNormal = WorldNormalVector( input , CalculatedNormal );
			fixed WorldObjectSwitch = step( 0.5 , _WorldtoObjectSwitch );
			output.Normal = CalculatedNormal;
			fixed3 temp_cast_0 = _CoverageAmount;
			fixed3 temp_cast_1 = _CoverageFalloff;
			output.Albedo = lerp( ( ( ( tex2D( _TriplanarAlbedo,float2( input.localVertexPos.y , input.localVertexPos.z )) * BlendComponents.x ) + ( tex2D( _TriplanarAlbedo,float2( input.localVertexPos.x , input.localVertexPos.z )) * BlendComponents.y ) ) + ( tex2D( _TriplanarAlbedo,float2( input.localVertexPos.x , input.localVertexPos.y )) * BlendComponents.z ) ) , tex2D( _TopAlbedo,float2( lerp( input.worldPos , input.localVertexPos , WorldObjectSwitch ).x , lerp( input.worldPos , input.localVertexPos , WorldObjectSwitch ).z )) , pow( saturate( ( lerp( PixelNormal , mul( fixed4( PixelNormal , 0.0 ) , unity_ObjectToWorld ) , WorldObjectSwitch ) + temp_cast_0 ) ) , temp_cast_1 ).y ).xyz;
			fixed3 temp_cast_2 = _Specular;
			output.Specular = temp_cast_2;
			output.Smoothness = _Smoothness;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1249.407;-1584.739;1.9;True;False
Node;AmplifyShaderEditor.CommentaryNode;175;70.0921,528.1943;Float;224;239;Coverage in World mode;1;161
Node;AmplifyShaderEditor.CommentaryNode;174;54.4417,809.0455;Float;241;239;Coverage in Object mode;1;119
Node;AmplifyShaderEditor.CommentaryNode;172;-422.9069,762.4951;Float;317.8;243.84;Coverage in World mode;1;293
Node;AmplifyShaderEditor.CommentaryNode;170;-524.7089,1125.127;Float;436.2993;336.8007;Coverage in Object mode;3;149;150;313
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-247.4096,1245.228;Float;FLOAT3;0,0,0;FLOAT4x4;0.0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
Node;AmplifyShaderEditor.LerpOp;186;-10.50789,1060.395;Float;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;150;-439.7088,1344.429;Float
Node;AmplifyShaderEditor.StepOpNode;191;-455.1077,1008.095;Float;FLOAT;0.5;FLOAT;0.0
Node;AmplifyShaderEditor.LerpOp;105;1295.895,186.9972;Float;FLOAT4;0,0,0,0;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleDivideOpNode;75;-1493.199,250.0983;Float;FLOAT3;0.0,0,0;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1813.101,190.4999;Fixed;True;1;Fixed;ASEMaterialInspector;StandardSpecular;ASESampleShaders/Triplanar;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;485.5023,175.1985;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;97.5016,118.8984;Float;FLOAT4;0.0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;315.0017,-37.50143;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;238;-1060.186,102.4543;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AbsOpNode;72;-1836,248.4983;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.Vector3Node;264;-1873.118,436.4499;Float;Constant;_Vector0;Vector 0;-1;1,1,1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;126.101,-133.2017;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.BreakToComponentsNode;256;-671.9149,-160.55;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;257;-465.815,-170.0501;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.LocalVertexPosNode;98;-856.2065,-160.6024;Float
Node;AmplifyShaderEditor.LocalVertexPosNode;96;-823.2065,399.8978;Float
Node;AmplifyShaderEditor.LocalVertexPosNode;97;-798.8057,124.1976;Float
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;107.7027,378.6988;Float;FLOAT4;0.0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;252;197.6706,2099.901;Float;FLOAT3;0.0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;10.77018,1973.2;Float;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.BreakToComponentsNode;271;-811.2666,1860.002;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;272;-596.5656,1833.902;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.LocalVertexPosNode;242;-1000.75,1861.216;Float
Node;AmplifyShaderEditor.BreakToComponentsNode;275;-805.974,2132.695;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;276;-591.2729,2106.594;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.LocalVertexPosNode;277;-995.4575,2133.908;Float
Node;AmplifyShaderEditor.BreakToComponentsNode;278;-823.4946,2444.797;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;279;-608.7936,2418.696;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.LocalVertexPosNode;280;-1012.978,2446.01;Float
Node;AmplifyShaderEditor.SimpleAddOpNode;248;374.3712,2287.8;Float;FLOAT3;0.0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;239;-1068.265,239.3751;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;240;-1061.418,376.9223;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;259;-426.7649,86.99984;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.BreakToComponentsNode;258;-628.9649,105.5999;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.LocalVertexPosNode;119;104.4417,859.0455;Float
Node;AmplifyShaderEditor.WorldPosInputsNode;161;120.0921,578.1942;Float
Node;AmplifyShaderEditor.LerpOp;187;336.0916,690.0955;Float;FLOAT3;0,0,0;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.BreakToComponentsNode;287;518.2163,698.463;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;286;740.4164,707.363;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.DotProductOpNode;73;-1666.1,322.3978;Float;FLOAT3;0.0,0,0;FLOAT3;1,1,1
Node;AmplifyShaderEditor.BreakToComponentsNode;260;-656.5151,404.3495;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.AppendNode;261;-450.2144,386.6494;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.GetLocalVarNode;245;-1607.314,2243.473;Float;147
Node;AmplifyShaderEditor.WireNode;120;267.1915,260.2962;Float;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.WireNode;190;1188.091,343.0952;Float;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.WireNode;250;173.9606,2384.1;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.WireNode;296;-818.8829,585.063;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;193;82.09288,1493.795;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;181;350.9923,1673.196;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;180;441.7923,1577.996;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;300;1680.917,300.2614;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;268;712.8824,1055.651;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.LerpOp;284;972.6819,2279.452;Float;FLOAT3;0.0,0,0;FLOAT3;0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;301;777.1167,2375.862;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.BreakToComponentsNode;273;-1296.918,2236.915;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;18.57068,2246.701;Float;FLOAT3;0.0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;249;7.971807,2541.301;Float;FLOAT3;0.0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-1997.113,255.6272;Float;FLOAT3;0,0,0;FLOAT4x4;0.0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
Node;AmplifyShaderEditor.WorldNormalInputsNode;144;-2256.413,185.5275;Float;True
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;146;-2224.413,352.3276;Float
Node;AmplifyShaderEditor.SimpleAddOpNode;153;212.3922,1063.395;Float;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SaturateNode;152;387.1914,1072.796;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.PowerNode;155;557.6918,1070.396;Float;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-1325.309,265.0945;Float;BlendComponents;1;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.WireNode;308;377.6202,2089.66;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WorldNormalInputsNode;304;258.7195,1802.285;Float;True
Node;AmplifyShaderEditor.WireNode;312;415.6202,2061.163;Float;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleAddOpNode;303;504.7183,2067.186;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SaturateNode;305;664.1176,2178.087;Float;FLOAT;0.0
Node;AmplifyShaderEditor.PowerNode;306;819.4181,2225.687;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;309;24.42033,1855.761;Float;FLOAT;0.0
Node;AmplifyShaderEditor.RegisterLocalVarNode;292;1180.313,2288.762;Float;CalculatedNormal;2;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-320.3076,1021.095;Float;WorldObjectSwitch;4;FLOAT;0.0
Node;AmplifyShaderEditor.GetLocalVarNode;293;-353.8859,869.5626;Float;315
Node;AmplifyShaderEditor.GetLocalVarNode;313;-501.9794,1212.861;Float;315
Node;AmplifyShaderEditor.WorldNormalVector;314;1473.82,2344.061;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;315;1699.42,2298.961;Float;PixelNormal;3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.WireNode;310;51.32038,1820.762;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;307;451.2203,2246.361;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;316;633.6203,2276.96;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;317;-29.97948,820.2599;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;188;17.79165,780.8954;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;297;24.91654,562.5632;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;318;-14.17962,582.1601;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;298;-850.8782,563.062;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;295;27.6165,263.8628;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;319;-0.8795109,289.5601;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;89;-7.104836,10.99753;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;320;40.92054,-1.139977;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;90;-819.6044,12.49749;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;198;-850.1791,33.64586;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;321;1175.53,1025.136;Float;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;1;-252.6997,-172.9003;Float;Property;_TriplanarAlbedo;Triplanar Albedo;1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;243;-400.5435,1841.018;Float;Property;_Normal;Normal;2;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;104;893.8926,679.8981;Float;Property;_TopAlbedo;Top Albedo;3;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;285;393.683,2492.652;Float;Property;_TopNormal;Top Normal;4;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;185;-734.309,1024.496;Float;Property;_WorldtoObjectSwitch;World to Object Switch;5;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;110;-339.4074,1528.998;Float;Property;_CoverageAmount;Coverage Amount;6;0;-1;1
Node;AmplifyShaderEditor.RangedFloatNode;115;-335.5072,1649.297;Float;Property;_CoverageFalloff;Coverage Falloff;7;0.5;0.01;2
Node;AmplifyShaderEditor.RangedFloatNode;212;1324.42,324.6465;Float;Property;_Specular;Specular;8;0.02;0;1
Node;AmplifyShaderEditor.RangedFloatNode;213;1317.922,409.1468;Float;Property;_Smoothness;Smoothness;9;0.5;0;1
Node;AmplifyShaderEditor.WireNode;299;1510.917,2008.36;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.WireNode;323;-1064.197,2591.897;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;322;-1093.91,2570.107;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;324;-1042.408,2025.374;Float;FLOAT;0.0
Node;AmplifyShaderEditor.BreakToComponentsNode;270;-1288.002,2105.134;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.WireNode;325;-1074.101,2043.202;Float;FLOAT;0.0
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-1296.747,2361.49;Float;FLOAT3;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.SamplerNode;302;-257.7821,82.56178;Float;Property;_TextureSample0;Texture Sample 0;-1;None;True;0;False;white;Auto;False;Instance;1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;33;-254.9974,360.4987;Float;Property;_TextureSample2;Texture Sample 2;-1;None;True;0;False;white;Auto;False;Instance;1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;274;-395.2509,2113.71;Float;Property;_TextureSample4;Texture Sample 4;1;None;True;0;True;bump;Auto;True;Instance;243;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;281;-412.7716,2425.812;Float;Property;_TextureSample5;Texture Sample 5;1;None;True;0;True;bump;Auto;True;Instance;243;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;149;0;313;0
WireConnection;149;1;150;0
WireConnection;186;0;293;0
WireConnection;186;1;149;0
WireConnection;186;2;192;0
WireConnection;191;1;185;0
WireConnection;105;0;35;0
WireConnection;105;1;190;0
WireConnection;105;2;321;0
WireConnection;75;0;72;0
WireConnection;75;1;73;0
WireConnection;0;0;105;0
WireConnection;0;1;300;0
WireConnection;0;3;212;0
WireConnection;0;4;213;0
WireConnection;35;0;32;0
WireConnection;35;1;120;0
WireConnection;31;0;302;0
WireConnection;31;1;295;0
WireConnection;32;0;28;0
WireConnection;32;1;31;0
WireConnection;238;0;147;0
WireConnection;72;0;145;0
WireConnection;28;0;1;0
WireConnection;28;1;320;0
WireConnection;256;0;98;0
WireConnection;257;0;256;1
WireConnection;257;1;256;2
WireConnection;34;0;33;0
WireConnection;34;1;297;0
WireConnection;252;0;253;0
WireConnection;252;1;251;0
WireConnection;253;0;243;0
WireConnection;253;1;324;0
WireConnection;271;0;242;0
WireConnection;272;0;271;1
WireConnection;272;1;271;2
WireConnection;275;0;277;0
WireConnection;276;0;275;0
WireConnection;276;1;275;2
WireConnection;278;0;280;0
WireConnection;279;0;278;0
WireConnection;279;1;278;1
WireConnection;248;0;252;0
WireConnection;248;1;250;0
WireConnection;239;0;147;0
WireConnection;240;0;147;0
WireConnection;259;0;258;0
WireConnection;259;1;258;2
WireConnection;258;0;97;0
WireConnection;187;0;161;0
WireConnection;187;1;119;0
WireConnection;187;2;188;0
WireConnection;287;0;187;0
WireConnection;286;0;287;0
WireConnection;286;1;287;2
WireConnection;73;0;72;0
WireConnection;73;1;264;0
WireConnection;260;0;96;0
WireConnection;261;0;260;0
WireConnection;261;1;260;1
WireConnection;120;0;34;0
WireConnection;190;0;104;0
WireConnection;250;0;249;0
WireConnection;296;0;298;0
WireConnection;193;0;110;0
WireConnection;181;0;115;0
WireConnection;180;0;181;0
WireConnection;300;0;299;0
WireConnection;268;0;155;0
WireConnection;284;0;248;0
WireConnection;284;1;301;0
WireConnection;284;2;306;0
WireConnection;301;0;285;0
WireConnection;273;0;245;0
WireConnection;251;0;274;0
WireConnection;251;1;273;1
WireConnection;249;0;281;0
WireConnection;249;1;323;0
WireConnection;145;0;144;0
WireConnection;145;1;146;0
WireConnection;153;0;186;0
WireConnection;153;1;193;0
WireConnection;152;0;153;0
WireConnection;155;0;152;0
WireConnection;155;1;180;0
WireConnection;147;0;75;0
WireConnection;308;0;309;0
WireConnection;312;0;310;0
WireConnection;303;0;304;2
WireConnection;303;1;312;0
WireConnection;305;0;303;0
WireConnection;306;0;305;0
WireConnection;306;1;316;0
WireConnection;309;0;115;0
WireConnection;292;0;284;0
WireConnection;192;0;191;0
WireConnection;314;0;292;0
WireConnection;315;0;314;0
WireConnection;310;0;110;0
WireConnection;307;0;308;0
WireConnection;316;0;307;0
WireConnection;317;0;192;0
WireConnection;188;0;317;0
WireConnection;297;0;318;0
WireConnection;318;0;296;0
WireConnection;298;0;240;2
WireConnection;295;0;319;0
WireConnection;319;0;239;1
WireConnection;89;0;90;0
WireConnection;320;0;89;0
WireConnection;90;0;198;0
WireConnection;198;0;238;0
WireConnection;321;0;268;1
WireConnection;1;1;257;0
WireConnection;243;1;272;0
WireConnection;104;1;286;0
WireConnection;299;0;292;0
WireConnection;323;0;322;0
WireConnection;322;0;282;2
WireConnection;324;0;325;0
WireConnection;270;0;245;0
WireConnection;325;0;270;0
WireConnection;282;0;245;0
WireConnection;302;1;259;0
WireConnection;33;1;261;0
WireConnection;274;1;276;0
WireConnection;281;1;279;0
ASEEND*/
//CHKSM=71A7A17E2FBC43A11D6D101BF0257D4FDF9038B7