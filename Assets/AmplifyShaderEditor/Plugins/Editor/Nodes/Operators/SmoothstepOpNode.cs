// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Smoothstep", "Operators", "Interpolate smoothly between two input values based on a third" )]
	public sealed class SmoothstepOpNode : ParentNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.OBJECT, false, "alpha" );
			AddInputPort( WirePortDataType.OBJECT, false, "min" );
			AddInputPort( WirePortDataType.OBJECT, false, "max" );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_textLabelWidth = 40;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if ( portId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );

				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
			//else
			//{
			//	_inputPorts[ portId ].MatchPortToConnection();
			//}
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			if ( outputPortId == 0 )
			{
				m_inputPorts[ 0 ].MatchPortToConnection();
				m_inputPorts[ 1 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
				m_inputPorts[ 2 ].ChangeType( m_inputPorts[ 0 ].DataType, false );

				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			//if ( !InputPorts[ 0 ].IsConnected || !InputPorts[ 1 ].IsConnected || !InputPorts[ 2 ].IsConnected )
			//{
			//	return UIUtils.NoConnection( this );
			//}

			WirePortDataType alphaType = m_inputPorts[ 0 ].ConnectionType();
			WirePortDataType minType = m_inputPorts[ 1 ].ConnectionType();
			WirePortDataType maxType = m_inputPorts[ 2 ].ConnectionType();

			string alpha = InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );

			string min = InputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
			if ( minType != alphaType )
			{
				min = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), min, InputPorts[ 1 ].DataType, m_inputPorts[ 0 ].DataType, min );
			}

			string max = InputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
			if ( maxType != alphaType )
			{
				max = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), max, InputPorts[ 2 ].DataType, m_inputPorts[ 0 ].DataType, max );
			}
			string result = string.Empty;
			switch ( alphaType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.INT:
				case WirePortDataType.COLOR:
				case WirePortDataType.OBJECT:
				{
					result = "smoothstep( " + min + " , " + max + " , " + alpha + " )";
				}break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					result = UIUtils.InvalidParameter( this );
				} break;
			}

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
