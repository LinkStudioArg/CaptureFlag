// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Ext]Add", "Operators", "Simple scalar addition or vector per component addition", null, KeyCode.None, false )]
	class AddOpNode : ExtensibleInputPortNode
	{
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );

			if ( InputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( InputPorts.Count == 1 && InputPorts[ 0 ].IsConnected )
				return InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );

			string result = "( ";
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
								result += " + " + InputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
							}
						}
					}
				}
				break;
			}

			result += " )";
			return result;
		}

	}
}
