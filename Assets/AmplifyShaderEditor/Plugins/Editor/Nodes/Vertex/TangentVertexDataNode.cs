// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[VS] Vertex Tangent", "Vertex Data", "Vertex tangent vector. Only works on Vertex Shaders ports ( p.e. Local Vertex Offset Port )." )]
	public sealed class TangentVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "tangent";
		}
	}
}
