// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World Space Light Pos", "Light", "Light Position" )]
	public sealed class WorldSpaceLightPos : ShaderVariablesNode
	{
		private const string m_lightPosValue = "WorldSpaceLightPos0";
		private const string m_lightPosDeclaration = "uniform float4 WorldSpaceLightPos0;";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT4 );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalVar );
			dataCollector.AddToUniforms( m_uniqueId, m_lightPosDeclaration );
			return m_lightPosValue;
		}

	}
}
