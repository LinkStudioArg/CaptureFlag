// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	public class HelperParentNode : ParentNode
	{
		[SerializeField]
		protected string m_funcType = string.Empty;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.OBJECT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
		}

		public override string GetIncludes()
		{
			return Constants.UnityCgLibFuncs;
		}
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToIncludes( m_uniqueId, Constants.UnityCgLibFuncs );
			string concatResults = string.Empty;
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				string result = string.Empty;
				if ( m_inputPorts[ i ].IsConnected )
				{
					result = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
					if ( !m_inputPorts[ i ].CheckMatchConnectionType() )
					{
						result = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), result, m_inputPorts[ i ].ConnectionType(), m_inputPorts[ i ].DataType, result );
					}
				}
				else
				{
					result = m_inputPorts[ i ].WrappedInternalData;
				}

				concatResults += result;
				if ( i != ( m_inputPorts.Count - 1 ) )
					concatResults += " , ";
			}
			string finalResult = m_funcType + "( " + concatResults + " )";
			return CreateOutputLocalVariable( 0, finalResult, ref dataCollector );
		}
	}
}
