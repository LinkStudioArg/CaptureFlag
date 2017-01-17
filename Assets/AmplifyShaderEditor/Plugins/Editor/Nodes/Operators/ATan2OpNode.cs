// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "ATan2", "Trigonometry", "Arctangent of y/x" )]
	public sealed class ATan2OpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_dynamicOutputType = true;
			m_useInternalPortData = true;
		}

		public override string BuildResults( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			string result = string.Empty;
			switch ( m_mainDataType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				{
					result =  "atan2( " + m_inputA + " , " + m_inputB + " )";
				}break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					result = UIUtils.InvalidParameter( this );
				}
				break;
			}
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
