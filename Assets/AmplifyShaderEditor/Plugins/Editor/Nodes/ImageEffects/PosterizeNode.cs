// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://www.shadertoy.com/view/ldX3D4
using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Posterize", "Image Effects", "Converts a continuous gradation of tones to multiple regions of fewer tones" )]
	public sealed class PosterizeNode : ParentNode
	{
		private const string PosterizationPowerStr = "Posterization Power";
		[SerializeField]
		private int m_posterizationPower = 1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.INT, false, "Posterization Power [1-256]" );
			AddInputPort( WirePortDataType.COLOR, false, "Color" );
			AddOutputPort( WirePortDataType.COLOR, Constants.EmptyPortValue );
			m_textLabelWidth = 120;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			{
				m_posterizationPower = EditorGUILayout.IntSlider( PosterizationPowerStr, m_posterizationPower, 1, 256 );
			}
			EditorGUILayout.EndVertical();
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string posterizationPower = "1";
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				WirePortDataType dataType0 = m_inputPorts[ 0 ].ConnectionType();
				posterizationPower = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
				if ( m_inputPorts[ 0 ].ConnectionType() != WirePortDataType.INT )
				{
					posterizationPower = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), null, dataType0, WirePortDataType.INT, posterizationPower );
				}
			}
			else
			{
				posterizationPower = m_posterizationPower.ToString();
			}

			string colorTarget = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
			WirePortDataType dataType1 = m_inputPorts[ 1 ].ConnectionType();
			if ( dataType1 != WirePortDataType.COLOR && dataType1 != WirePortDataType.FLOAT4 )
			{
				colorTarget = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), null, dataType1, WirePortDataType.FLOAT4, colorTarget );
			}
			string divVar = "div" + m_uniqueId;
			dataCollector.AddToLocalVariables( m_uniqueId, "float " + divVar + "=256.0/float(" + posterizationPower + ");" );
			string result = "( floor( " + colorTarget + " * " + divVar + " ) / " + divVar + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_posterizationPower );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_posterizationPower = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}
	}
}
