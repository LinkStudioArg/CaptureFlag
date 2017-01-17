// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Pixel Normal World", "Surface Standard Inputs", "Per pixel world normal vector" )]
	public sealed class WorldNormalVector : ParentNode
	{
		private const string NormalVecValStr = "normalValue";
		private const string NormalVecDecStr = "float3 {0} = {1};";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			UIUtils.AddNormalDependentCount();
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.RemoveNormalDependentCount();
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
			dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );
			string result = string.Empty;
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				result = "WorldNormalVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + " )";
				if ( !dataCollector.DirtyNormal )
				{
					dataCollector.ForceNormal = true;
				}
			}
			else
			{
				result = "WorldNormalVector( " + Constants.InputVarStr + " , " + Constants.OutputVarStr + ".Normal )";
			}

			if ( m_outputPorts[ 0 ].ConnectionCount > 1 )
			{
				string localVar = NormalVecValStr + m_uniqueId;
				dataCollector.AddToLocalVariables( m_uniqueId, string.Format( NormalVecDecStr, localVar, result ) );
				return GetOutputVectorItem( 0, outputId, localVar );
			}
			else
			{
				return GetOutputVectorItem( 0, outputId, result );
			}
		}
	}
}
