// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Ext]Max", "Operators", "Maximum of multiple scalars or each respective component of multiple vectors", null, KeyCode.None, false )]
	class MaxOpNode : ExtensibleInputPortNode
	{
		private List<string> m_validObjects = new List<string>();

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( InputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( InputPorts.Count == 1 && InputPorts[ 0 ].IsConnected )
			{
				return InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			}

			switch ( m_selectedType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				{
					m_validObjects.Clear();
					for ( int i = 0; i < InputPorts.Count; i++ )
					{
						if ( InputPorts[ i ].IsConnected )
							m_validObjects.Add( InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) );
					}
					if ( m_validObjects.Count == 1 )
						return m_validObjects[ 0 ];

					string result = "max( ";
					bool firstOp = true;
					for ( int portId = 0; portId < m_validObjects.Count; portId++ )
					{

						if ( firstOp )
						{
							firstOp = false;
							result += m_validObjects[ portId ] + " , ";
						}
						else
						{
							if ( portId < ( m_validObjects.Count - 1 ) )
							{
								result += " max(  " + m_validObjects[ portId ] + " , ";
							}
							else
								result += m_validObjects[ portId ];
						}
					}

					for ( int portId = 0; portId < ( m_validObjects.Count - 1 ); portId++ )
					{
						result += " )";
					}

					return result;
				}
			}

			return UIUtils.InvalidParameter( this );
		}

	}
}
