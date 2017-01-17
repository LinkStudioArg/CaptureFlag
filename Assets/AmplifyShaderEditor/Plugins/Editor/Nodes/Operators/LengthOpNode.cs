// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Length", "Operators", "Scalar Euclidean length of a vector" )]
	public sealed class LengthOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "length";
			m_validTypes = ( int ) WirePortDataType.OBJECT |
							( int ) WirePortDataType.FLOAT |
							( int ) WirePortDataType.FLOAT2 |
							( int ) WirePortDataType.FLOAT3 |
							( int ) WirePortDataType.FLOAT4 |
							( int ) WirePortDataType.COLOR |
							( int ) WirePortDataType.INT;
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_autoUpdateOutputPort = false;
		}
	}
}
