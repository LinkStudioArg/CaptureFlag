// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Distance", "Operators", "Euclidean distance between two points" )]
	public sealed class DistanceOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
			m_dynamicOutputType = false;
			m_useInternalPortData = true;
		}

		public override string BuildResults( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			switch ( m_mainDataType )
			{

				case WirePortDataType.OBJECT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				{
					return "distance( " + m_inputA + " , " + m_inputB + " )";
				}
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					UIUtils.ShowMessage( "Distance Op does not work with matrices." );
				}
				break;
			}
			return UIUtils.InvalidParameter( this );
		}
	}
}
