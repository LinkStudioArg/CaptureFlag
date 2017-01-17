// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Radians", "Trigonometry", "Converts values of scalars and vectors from degrees to radians" )]
	public sealed class RadiansOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "radians";
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
