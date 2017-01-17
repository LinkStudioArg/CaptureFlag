// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Multiply", "Operators", "Simple multiplication of two variables", null, KeyCode.M )]
	public sealed class SimpleMultiplyOpNode : DynamicTypeNode
	{
		public override string BuildResults( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 ||
				m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4x4 ||
				m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT3x3 ||
				m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT4x4 )
			{
				m_inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
				m_inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );


				// Check matrix on first input
				if ( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 )
				{
					switch ( m_inputPorts[ 1 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR:
						{
							m_inputB = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, WirePortDataType.FLOAT3, m_inputB );
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							m_inputA = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, WirePortDataType.FLOAT4x4, m_inputA );
						}
						break;
						case WirePortDataType.FLOAT3:
						case WirePortDataType.FLOAT3x3: break;
					}
				}

				if ( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4x4 )
				{
					switch ( m_inputPorts[ 1 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						{
							m_inputB = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, WirePortDataType.FLOAT4, m_inputB );
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							m_inputB = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, WirePortDataType.FLOAT4x4, m_inputB );
						}
						break;
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR: break;
					}
				}

				// Check matrix on second input
				if ( m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT3x3 )
				{
					switch ( m_inputPorts[ 0 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR:
						{
							m_inputA = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, WirePortDataType.FLOAT3, m_inputA );
						}
						break;
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.FLOAT3x3: break;
					}
				}

				if ( m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT4x4 )
				{
					switch ( m_inputPorts[ 0 ].DataType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT:
						case WirePortDataType.INT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						{
							m_inputA = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, WirePortDataType.FLOAT4, m_inputA );
						}
						break;
						case WirePortDataType.FLOAT3x3:
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR: break;
					}
				}

				return "mul( " + m_inputA + " , " + m_inputB + " )";
			}
			else
			{

				m_inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
				if ( m_inputPorts[ 0 ].DataType != m_mainDataType && m_inputPorts[ 0 ].DataType != WirePortDataType.FLOAT )
				{
					m_inputA = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, m_mainDataType, m_inputA );
				}

				m_inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
				if ( m_inputPorts[ 1 ].DataType != m_mainDataType && m_inputPorts[ 1 ].DataType != WirePortDataType.FLOAT )
				{
					m_inputB = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, m_mainDataType, m_inputB );
				}

				return "( " + m_inputA + " * " + m_inputB + " )";
			}
		}
	}
}