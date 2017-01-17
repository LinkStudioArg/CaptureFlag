// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleRefraction"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Distortion("Distortion", Range( 0 , 1)) = 0.292
		_BrushedMetalNormal("BrushedMetalNormal", 2D) = "bump" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		ZTest LEqual
		GrabPass{ "_ScreenGrab0" }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:premul keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float2 uv_BrushedMetalNormal;
		};

		uniform sampler2D _ScreenGrab0;
		uniform sampler2D _BrushedMetalNormal;
		uniform float _Distortion;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Emission = tex2D( _ScreenGrab0,( ( input.screenPos/input.screenPos.w ).xy + ( UnpackNormal( tex2D( _BrushedMetalNormal,input.uv_BrushedMetalNormal) ) * _Distortion ).xy )).rgb;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;951.9801;204.001;1;True;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;536.7999,-33.8;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/SimpleRefraction;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;36.62508,137.2995;Float;FLOAT2;0.0,0;FLOAT2;0.0,0
Node;AmplifyShaderEditor.ScreenColorNode;8;224.0004,85.8997;Float;Global;_ScreenGrab0;Screen Grab 0;-1;Object;-1;FLOAT2;0,0
Node;AmplifyShaderEditor.SamplerNode;29;-855.48,221.599;Float;Property;_BrushedMetalNormal;BrushedMetalNormal;-1;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;31;-749.975,387.8984;Float;Property;_Distortion;Distortion;-1;0.292;0;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-441.6739,287.2988;Float;FLOAT3;0.0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.ComponentMaskNode;36;-248.5805,285.0987;Float;True;True;False;True;FLOAT3;0,0,0
Node;AmplifyShaderEditor.ComponentMaskNode;39;-191.7806,65.19897;Float;True;True;False;False;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.ScreenPosInputsNode;4;-381.0954,24.99997;Float;True
WireConnection;0;13;8;0
WireConnection;30;0;39;0
WireConnection;30;1;36;0
WireConnection;8;0;30;0
WireConnection;32;0;29;0
WireConnection;32;1;31;0
WireConnection;36;0;32;0
WireConnection;39;0;4;0
ASEEND*/
//CHKSM=6DA612D9DB382113E6A2F4A30BABAF5F82F36240