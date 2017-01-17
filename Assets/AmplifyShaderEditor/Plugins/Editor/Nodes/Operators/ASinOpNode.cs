// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "ASin", "Trigonometry", "Arcsine of scalars and vectors" )]
	public sealed class ASinOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "asin";
			m_validTypes = ( int ) WirePortDataType.OBJECT |
							( int ) WirePortDataType.FLOAT |
							( int ) WirePortDataType.FLOAT2 |
							( int ) WirePortDataType.FLOAT3 |
							( int ) WirePortDataType.FLOAT4 |
							( int ) WirePortDataType.COLOR |
							( int ) WirePortDataType.INT;
		}
	}
}
