// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[VS] Vertex Normal", "Vertex Data", "Vertex Normal. Only works on Vertex Shaders ports ( p.e. Local Vertex Offset Port )." )]
	public sealed class NormalVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "normal";
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT3 );
			m_outputPorts[ 4 ].Visible = false;
		}
	}
}
