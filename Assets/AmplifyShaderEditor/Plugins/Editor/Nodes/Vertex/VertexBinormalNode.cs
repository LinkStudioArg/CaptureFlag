// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex Binormal World
// Donated by Community Member Kebrus

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex Binormal World", "Vertex Data", "Tranfers vertex binormal into pixel shader" )]
	public sealed class VertexBinormalNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToInput( m_uniqueId, "float3 worldBinormal", true );
			dataCollector.AddVertexInstruction( "o.worldBinormal = normalize( cross("+Constants.VertexShaderInputStr+".normal," + Constants.VertexShaderInputStr + ".tangent) * "+ Constants.VertexShaderInputStr+".tangent.w)", m_uniqueId );

			return "input.worldBinormal";
		}
	}
}

