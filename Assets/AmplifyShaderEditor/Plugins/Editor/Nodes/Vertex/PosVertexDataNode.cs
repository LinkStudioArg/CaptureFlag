// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[VS] Vertex Pos", "Vertex Data", "Vertex position. Only works on Vertex Shaders ports ( p.e. Local Vertex Offset Port )." )]
	public sealed class PosVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "vertex";
		}
	}
}
