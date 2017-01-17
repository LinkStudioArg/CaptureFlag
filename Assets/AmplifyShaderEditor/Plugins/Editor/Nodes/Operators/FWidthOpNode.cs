// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "FWidth", "Operators", "Sum of approximate window-space partial derivatives magnitudes" )]
	public sealed class FWidthOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "fwidth";
			m_validTypes = ( int ) WirePortDataType.OBJECT |
							( int ) WirePortDataType.FLOAT |
							( int ) WirePortDataType.FLOAT2 |
							( int ) WirePortDataType.FLOAT3 |
							( int ) WirePortDataType.FLOAT4 |
							( int ) WirePortDataType.COLOR |
							( int ) WirePortDataType.INT;
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
		}
	}
}
