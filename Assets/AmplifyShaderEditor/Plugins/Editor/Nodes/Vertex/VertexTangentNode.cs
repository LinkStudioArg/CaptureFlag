// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex Tangent World
// Donated by Community Member Kebrus
using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex Tangent World", "Vertex Data", "Tranfers vertex tangent into pixel shader", null, KeyCode.None, true, false, null, null, true )]
	public sealed class VertexTangentNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToInput( m_uniqueId, "float3 worldTangent", true );
			dataCollector.AddVertexInstruction( "o.worldTangent = normalize( mul(unity_ObjectToWorld," + Constants.VertexShaderInputStr + ".tangent.xyz))", m_uniqueId );

			return "input.worldTangent";
		}
	}
}
