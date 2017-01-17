// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Rotator
// Donated by kebrus

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Rotator", "Textures", "Rotates UVs with time but can also be used to rotate other Vector2 values", null, KeyCode.None, true, false, null, null, true )]
	public sealed class RotatorNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Coordinate" );
			AddInputPort( WirePortDataType.FLOAT2, false, "Anchor" );
			m_inputPorts[ 1 ].Vector2InternalData = new Vector2( 0.5f, 0.5f );
			AddInputPort( WirePortDataType.FLOAT, false, "Time" );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_useInternalPortData = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.HelpBox("Rotates UVs but can also be used to rotate other Vector2 values\n\nAnchor is the rotation point in UV space from which you rotate the UVs\nAngle is the amount of rotation applied [0,1], if less unconnected it will use time as the default value", MessageType.None);
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			string result = string.Empty;
			string uv = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, false );
			string anchor = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false );

			string time = string.Empty;
			if ( m_inputPorts[ 2 ].IsConnected )
			{
				time = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, false );
			}
			else
			{
				dataCollector.AddToIncludes( m_uniqueId, Constants.UnityShaderVariables );
				time = "_Time[1]";
			}

			result += uv;

			string cosVar = "cos" + m_uniqueId;
			string sinVar = "sin" + m_uniqueId;
			dataCollector.AddToLocalVariables( m_uniqueId, "float " + cosVar + " = cos( "+time+" );");
			dataCollector.AddToLocalVariables( m_uniqueId, "float " + sinVar + " = sin( "+time+" );");


			string rotatorVar = "rotator" + m_uniqueId;
			dataCollector.AddToLocalVariables( m_uniqueId, "float2 " + rotatorVar + " = mul(" + result + " - "+anchor+", float2x2("+cosVar+",-"+sinVar+","+sinVar+","+cosVar+")) + "+anchor+";" );

			return GetOutputVectorItem( 0, outputId, rotatorVar );
		}
	}
}
