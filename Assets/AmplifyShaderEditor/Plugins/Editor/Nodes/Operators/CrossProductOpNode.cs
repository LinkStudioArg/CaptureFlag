// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Cross", "Vector", "Cross product of two three-component vectors" )]
	public sealed class CrossProductOpNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "lhs" );
			AddInputPort( WirePortDataType.FLOAT3, false, "rhs" );
			AddOutputPort( WirePortDataType.FLOAT3, "out" );
			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string lhsStr = InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar, 0, true );
			string rhsStr = InputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar, 0, true );

			string result = "cross( " + lhsStr + " , " + rhsStr + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

	}
}
