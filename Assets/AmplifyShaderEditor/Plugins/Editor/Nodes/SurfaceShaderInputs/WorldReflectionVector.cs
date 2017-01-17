// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Per Pixel World Reflection Vector", "Surface Standard Inputs", "Per pixel world reflection vector", null, KeyCode.R )]
	public sealed class WorldReflectionVector : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );

			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{

			dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_REFL ), true );
			dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );
			string result = "WorldReflectionVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar ) + " )";
			return GetOutputVectorItem( 0, outputId, result );
		}

	}
}
