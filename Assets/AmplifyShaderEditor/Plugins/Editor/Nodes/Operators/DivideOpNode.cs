// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Ext]Divide", "Operators", "Simple scalar division or vector per component division", null, KeyCode.None, false )]
	class DivideOpNode : ExtensibleInputPortNode
	{
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( InputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( InputPorts.Count == 1 && InputPorts[ 0 ].IsConnected )
				return InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );

			string result = "( ";
			switch ( m_selectedType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					if ( InputPorts.Count == 1 && InputPorts[ 0 ].IsConnected )
					{
						result += InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
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
									result += InputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
								}
								else
								{
									result += " / " + InputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );
								}
							}
						}
					}
					break;
				}
			}
			result += " )";
			return result;
		}

	}
}
