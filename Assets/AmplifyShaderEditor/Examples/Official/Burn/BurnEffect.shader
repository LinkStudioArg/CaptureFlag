// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/BurnEffect"
{
	Properties
	{
		EmberColorTint("Ember Color Tint", Color) = (0.9926471,0.6777384,0,1)
		GlowColorIntensity("Glow Color Intensity", Range( 0 , 10)) = 0
		_AlbedoMix("Albedo Mix", Range( 0 , 1)) = 0.5
		BaseEmber("Base Ember", Range( 0 , 1)) = 0
		GlowEmissionMultiplier("Glow Emission Multiplier", Range( 0 , 30)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
		GlowBaseFrequency("Glow Base Frequency", Range( 0 , 5)) = 1.1
		GlowOverride("Glow Override", Range( 0 , 10)) = 1
		_CharcoalNormalTile("Charcoal Normal Tile", Range( 2 , 5)) = 5
		_CharcoalMix("Charcoal Mix", Range( 0 , 1)) = 1
		Normals("Normals", 2D) = "bump" {}
		BurntTileNormals("Burnt Tile Normals", 2D) = "white" {}
		_BurnTilling("Burn Tilling", Range( 0.1 , 1)) = 1
		Albedo("Albedo", 2D) = "white" {}
		Masks("Masks", 2D) = "white" {}
		_BurnOffset("Burn Offset", Range( 0 , 5)) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
		};

		uniform sampler2D Normals;
		uniform sampler2D BurntTileNormals;
		uniform fixed _CharcoalNormalTile;
		uniform fixed _CharcoalMix;
		uniform sampler2D Masks;
		uniform fixed _BurnOffset;
		uniform fixed _BurnTilling;
		uniform sampler2D Albedo;
		uniform fixed _AlbedoMix;
		uniform fixed BaseEmber;
		uniform fixed4 EmberColorTint;
		uniform fixed GlowColorIntensity;
		uniform fixed GlowBaseFrequency;
		uniform fixed GlowOverride;
		uniform fixed GlowEmissionMultiplier;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			fixed4 tex2DNode83 = tex2D( BurntTileNormals,( input.texcoord_0 * _CharcoalNormalTile ));
			fixed4 tex2DNode98 = tex2D( Masks,(abs( ( input.texcoord_1 * _BurnTilling )+_BurnOffset * float2(1,0.5 ))));
			float temp_output_19_0 = ( _CharcoalMix + tex2DNode98.r );
			output.Normal = lerp( fixed4( UnpackNormal( tex2D( Normals,input.texcoord_0) ) , 0.0 ) , tex2DNode83 , temp_output_19_0 ).xyz;
			fixed4 tex2DNode80 = tex2D( Albedo,input.texcoord_1);
			fixed4 temp_cast_4 = 0.0;
			output.Albedo = lerp( lerp( ( tex2DNode80 * _AlbedoMix ) , temp_cast_4 , temp_output_19_0 ) , ( ( lerp( ( fixed4(0.718,0.0627451,0,1) * ( tex2DNode83.a * 2.95 ) ) , ( fixed4(0.647,0.06297875,0,1) * ( tex2DNode83.a * 4.2 ) ) , tex2DNode98.g ) * tex2DNode98.r ) * BaseEmber ) , ( tex2DNode98.r * 1.0 ) ).rgb;
			output.Emission = clamp( ( ( tex2DNode98.r * ( ( ( ( EmberColorTint * GlowColorIntensity ) * ( ( sin( ( _Time.y * GlowBaseFrequency ) ) * 0.5 ) + ( GlowOverride * ( tex2DNode98.r * tex2DNode83.a ) ) ) ) * tex2DNode98.g ) * tex2DNode83.a ) ) * GlowEmissionMultiplier ) , 0.0 , 100.0 ).rgb;
			output.Smoothness = tex2DNode80.a;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=3001
392;92;1247;695;3880.498;665.773;1.9;True;False
Node;AmplifyShaderEditor.CommentaryNode;130;-2566.58,462.9727;Float;2529.991;765.4811;Emission;17;157;158;69;66;95;68;67;76;73;77;127;65;70;106;101;170;174
Node;AmplifyShaderEditor.CommentaryNode;128;-3118.95,-279.5554;Float;1648.54;574.2015;;9;7;9;11;10;98;13;19;129;180
Node;AmplifyShaderEditor.CommentaryNode;129;-1945.302,-227.6195;Float;471.6918;296.3271;Mix Base Albedo;0
Node;AmplifyShaderEditor.CommentaryNode;39;-2354.221,1634.534;Float;1333.056;554.484;Base + Burnt Detail Mix (1 Free Alpha channels if needed);6;82;6;5;40;103;179
Node;AmplifyShaderEditor.CommentaryNode;40;-1796.12,1849.328;Float;343.3401;246.79;Emission in Alpha;1;83
Node;AmplifyShaderEditor.CommentaryNode;38;-1752.147,-1032.491;Float;1183.903;527.3994;Albedo - Smoothness in Alpha;5;35;27;34;28;80
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1277.102,-982.4909;Float;FLOAT4;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1279.204,-773.5922;Float;Constant;_RangedFloatNode27;_RangedFloatNode27;-1;0;0;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1666.103,-951.4903;Float;Property;_AlbedoMix;Albedo Mix;-1;0.5;0;1
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-1635.11,-49.29237;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-1374.632,659.5688;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-1650.418,755.3741;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1833.5,705.4734;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-952.7081,532.5618;Float;FLOAT;0;COLOR;0,0,0,0
Node;AmplifyShaderEditor.LerpOp;28;-970.9127,-675.8198;Float;FLOAT4;0,0,0,0;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-735.1851,46.52425;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;147;-1134.788,-277.6757;Float;Constant;ColorNode39134147;ColorNode39134 147;-1;0.718,0.0627451,0,1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-864.0865,-85.57518;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-718.0855,-186.0759;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-118.0104,-148.7752;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;152;247.1904,-253.5751;Float;FLOAT4;0,0,0,0;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-1201.686,-84.4754;Float;Constant;R2144;R2 144;-1;2.95;0;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;41.58921,-45.27597;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-182.4112,109.8244;Float;Constant;RangedFloatNode156;RangedFloatNode 156;-1;1;0;2
Node;AmplifyShaderEditor.RangedFloatNode;150;-535.9109,125.925;Float;Property;BaseEmber;Base Ember;-1;0;0;1
Node;AmplifyShaderEditor.LerpOp;148;-532.6986,-105.8688;Float;COLOR;0,0,0,0;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-348.4115,-25.67536;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;521.204,-107.2724;Fixed;True;2;Fixed;;Standard;ASESampleShaders/BurnEffect;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0,0,0;FLOAT;0;FLOAT;0;FLOAT3;0;FLOAT3;0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;80;-1643.756,-782.4553;Float;Property;Albedo;Albedo;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;77;-2516.58,713.7126;Float;Property;GlowColorIntensity;Glow Color Intensity;-1;0;0;10
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1971.32,1872.128;Float;FLOAT2;0,0;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;103;-1187.304,1824.428;Float;FLOAT3;0,0,0;FLOAT4;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;67;-2487.243,814.3365;Float
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-2214.131,864.2569;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2048.016,1006.15;Float;Constant;GlowDuration;Glow Duration;-1;0.5;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1859.748,866.4651;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-597.8378,569.7058;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-2059.027,1470.798;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1863.427,1078.999;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;174;-1695.621,992.7978;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2304.22,1979.127;Float;Property;_CharcoalNormalTile;Charcoal Normal Tile;-1;5;2;5
Node;AmplifyShaderEditor.RangedFloatNode;158;-922.8376,723.2053;Float;Property;GlowEmissionMultiplier;Glow Emission Multiplier;-1;1;0;30
Node;AmplifyShaderEditor.RangedFloatNode;10;-3067.554,57.64606;Float;Property;_BurnOffset;Burn Offset;-1;1;0;5
Node;AmplifyShaderEditor.RangedFloatNode;11;-3066.507,-41.68358;Float;Property;_BurnTilling;Burn Tilling;-1;1;0.1;1
Node;AmplifyShaderEditor.RangedFloatNode;13;-1907.802,-165.1195;Float;Property;_CharcoalMix;Charcoal Mix;-1;1;0;1
Node;AmplifyShaderEditor.RangedFloatNode;76;-2501.525,1037.474;Float;Property;GlowBaseFrequency;Glow Base Frequency;-1;1.1;0;5
Node;AmplifyShaderEditor.RangedFloatNode;169;-2446.727,1216.798;Float;Property;GlowOverride;Glow Override;-1;1;0;10
Node;AmplifyShaderEditor.ClampOpNode;176;257.5815,221.0976;Float;COLOR;0,0,0,0;COLOR;0,0,0,0;COLOR;0,0,0,0
Node;AmplifyShaderEditor.ColorNode;73;-2500.298,512.9727;Float;Property;EmberColorTint;Ember Color Tint;-1;0.9926471,0.6777384,0,1
Node;AmplifyShaderEditor.ColorNode;134;-1253.789,180.1245;Float;Constant;ColorNode39134;ColorNode 39 134;-1;0.647,0.06297875,0,1
Node;AmplifyShaderEditor.RangedFloatNode;138;-1244.786,362.2247;Float;Constant;R2;R2;-1;4.2;0;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-877.9863,266.6246;Float;FLOAT;0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-204.6184,257.1976;Float;Constant;RangedFloatNode177;RangedFloatNode 177;-1;0;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;179;-2197.001,1725.1;Float;0;-1;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.SamplerNode;83;-1771.837,1899.235;Float;Property;BurntTileNormals;Burnt Tile Normals;-1;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;82;-1802.313,1678.685;Float;Property;Normals;Normals;-1;None;True;2;True;bump;Auto;True;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;98;-2193.674,88.78339;Float;Property;Masks;Masks;-1;None;True;1;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SinOpNode;66;-2005.042,836.0363;Float;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-1147.638,615.7524;Float;COLOR;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;178;30.78172,469.7978;Float;Constant;RangedFloatNode178;RangedFloatNode 178;-1;100;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2686.548,-127.2553;Float;FLOAT2;0,0;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;180;-3038.006,-242.6004;Float;0;-1;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.PannerNode;9;-2442.548,-69.0541;Float;1;0.5;FLOAT2;0,0;FLOAT;0
WireConnection;34;0;80;0
WireConnection;34;1;35;0
WireConnection;19;0;13;0
WireConnection;19;1;98;1
WireConnection;101;0;70;0
WireConnection;101;1;98;2
WireConnection;70;0;65;0
WireConnection;70;1;174;0
WireConnection;65;0;73;0
WireConnection;65;1;77;0
WireConnection;127;0;98;1
WireConnection;127;1;106;0
WireConnection;28;0;34;0
WireConnection;28;1;27;0
WireConnection;28;2;19;0
WireConnection;136;0;134;0
WireConnection;136;1;137;0
WireConnection;145;0;83;4
WireConnection;145;1;144;0
WireConnection;146;0;147;0
WireConnection;146;1;145;0
WireConnection;151;0;149;0
WireConnection;151;1;150;0
WireConnection;152;0;28;0
WireConnection;152;1;151;0
WireConnection;152;2;154;0
WireConnection;154;0;98;1
WireConnection;154;1;156;0
WireConnection;148;0;146;0
WireConnection;148;1;136;0
WireConnection;148;2;98;2
WireConnection;149;0;148;0
WireConnection;149;1;98;1
WireConnection;0;0;152;0
WireConnection;0;1;103;0
WireConnection;0;2;176;0
WireConnection;0;4;80;4
WireConnection;80;1;180;0
WireConnection;5;0;179;0
WireConnection;5;1;6;0
WireConnection;103;0;82;0
WireConnection;103;1;83;0
WireConnection;103;2;19;0
WireConnection;68;0;67;2
WireConnection;68;1;76;0
WireConnection;69;0;66;0
WireConnection;69;1;95;0
WireConnection;157;0;127;0
WireConnection;157;1;158;0
WireConnection;171;0;98;1
WireConnection;171;1;83;4
WireConnection;170;0;169;0
WireConnection;170;1;171;0
WireConnection;174;0;69;0
WireConnection;174;1;170;0
WireConnection;176;0;157;0
WireConnection;176;1;177;0
WireConnection;176;2;178;0
WireConnection;137;0;83;4
WireConnection;137;1;138;0
WireConnection;83;1;5;0
WireConnection;82;1;179;0
WireConnection;98;1;9;0
WireConnection;66;0;68;0
WireConnection;106;0;101;0
WireConnection;106;1;83;4
WireConnection;7;0;180;0
WireConnection;7;1;11;0
WireConnection;9;0;7;0
WireConnection;9;1;10;0
ASEEND*/
//CHKSM=38BE3F240CD0068B1A803DF34977DD127D55A984