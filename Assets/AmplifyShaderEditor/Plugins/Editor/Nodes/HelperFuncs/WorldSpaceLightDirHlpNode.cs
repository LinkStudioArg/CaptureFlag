// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Space Light Dir", "Forward Render", "Computes world space direction (not normalized) to light, given object space vertex position" )]
	public sealed class WorldSpaceLightDirHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "WorldSpaceLightDir";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_useInternalPortData = false;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToIncludes( m_uniqueId, Constants.UnityCgLibFuncs );
			string result = string.Empty;
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				result = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, 0, true );
			}
			else
			{
				string input = UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS );
				dataCollector.AddToInput( m_uniqueId, input, true );
				result = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + Constants.InputVarStr + "." + UIUtils.GetInputValueFromType( AvailableSurfaceInputs.WORLD_POS ) + ", 0)";
			}
			result = m_funcType + "( " + result + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
