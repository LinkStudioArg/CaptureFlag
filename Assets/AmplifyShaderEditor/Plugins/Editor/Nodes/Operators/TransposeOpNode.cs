// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Transpose", "Matrix", "Transpose matrix of a matrix" )]
	public sealed class TransposeOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "transpose";
			m_validTypes = ( int ) WirePortDataType.OBJECT |
						  ( int ) WirePortDataType.FLOAT3x3 |
						  ( int ) WirePortDataType.FLOAT4x4;
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4x4, false );
		}
	}
}
