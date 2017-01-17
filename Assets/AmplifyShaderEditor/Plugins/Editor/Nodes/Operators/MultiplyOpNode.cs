// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[Ext]Multiply", "Operators", "Simple scalar multiplication or per component vector multiplication or matrix multiplication", null, KeyCode.None, false )]
	class MultiplyOpNode : ExtensibleInputPortNode
	{
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( InputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( InputPorts.Count == 1 && InputPorts[ 0 ].IsConnected )
				return InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );

			string result = string.Empty;
			switch ( m_selectedType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					result = "( ";
					if ( InputPorts.Count == 1 && InputPorts[ 0 ].IsConnected )
					{
						result += InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
					}
					else
					{
						bool firstOp = true;
						for ( int portId = 0; portId < InputPorts.Count; portId++ )
						{
							if ( InputPorts[ portId ].IsConnected )
							{
								if ( firstOp )
								{
									firstOp = false;
									result += InputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
								}
								else
								{
									result += " * " + InputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
								}
							}
						}
					}
					result += " )";
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{

					result = "mul( " +
							InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, InputPorts[ 0 ].GetConnection().DataType, ignoreLocalvar ) + " , " +
							InputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, InputPorts[ 1 ].GetConnection().DataType, ignoreLocalvar ) + " )";
				}
				break;
			}

			if ( String.IsNullOrEmpty( result ) )
			{
				return UIUtils.InvalidParameter( this );
			}
			return result;
		}

		protected override void OnTypeChange()
		{
			m_freeInputCountNb = !( m_selectedType == WirePortDataType.FLOAT3x3 || m_selectedType == WirePortDataType.FLOAT4x4 );

			if ( !m_freeInputCountNb )
				m_inputCount = 2;
		}

	}
}
