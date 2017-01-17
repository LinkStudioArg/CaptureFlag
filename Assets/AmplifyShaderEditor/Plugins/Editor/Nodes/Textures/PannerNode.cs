// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Panner", "Textures", "Pans UV texture coordinates according to its inputs" )]
	public sealed class PannerNode : ParentNode
	{
		private const string _speedXStr = "Speed X";
		private const string _speedYStr = "Speed Y";
		[SerializeField]
		private float m_speedX = 1f;

		[SerializeField]
		private float m_speedY = 1f;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Coordinate" );
			AddInputPort( WirePortDataType.FLOAT, false, "Time" );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_textLabelWidth = 55;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_speedX = EditorGUILayout.FloatField( _speedXStr, m_speedX );
			m_speedY = EditorGUILayout.FloatField( _speedYStr, m_speedY );

		}
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			string timePort = string.Empty;
			if ( m_inputPorts[ 1 ].IsConnected )
			{
				timePort = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			}
			else
			{
				dataCollector.AddToIncludes( m_uniqueId, Constants.UnityShaderVariables );
				timePort = "_Time[1]";
			}

			string result = "(abs( " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) + "+" + timePort + " * float2(" + m_speedX + "," + m_speedY + " )))";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_speedX = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_speedY = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
		}
		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_speedX );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_speedY );
		}
	}
}
