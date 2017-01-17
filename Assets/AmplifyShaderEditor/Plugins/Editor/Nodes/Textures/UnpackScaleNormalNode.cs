// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[NodeAttributes( "Unpack Scale Normal", "Textures", "Applies UnpackNormal/UnpackScaleNormal function" )]
	[Serializable]
	public class UnpackScaleNormalNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, "Value" );
			AddInputPort( WirePortDataType.FLOAT, false, "Normal Scale" );
			m_inputPorts[ 1 ].FloatInternalData = 1;
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			WirePortDataType dataType0 = m_inputPorts[ 0 ].ConnectionType();
			string src = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			if ( dataType0 != WirePortDataType.COLOR && dataType0 != WirePortDataType.FLOAT4 )
			{
				src = UIUtils.CastPortType( m_currentPrecisionType, new NodeCastInfo( m_uniqueId, outputId ), null, dataType0, WirePortDataType.FLOAT4, src );
			}

			bool isScaledNormal = false;
			if ( m_inputPorts[ 1 ].IsConnected )
			{
				isScaledNormal = true;
			}
			else
			{
				if ( m_inputPorts[ 1 ].FloatInternalData != 1 )
				{
					isScaledNormal = true;
				}
			}

			string normalMapUnpackMode = string.Empty;
			if ( isScaledNormal )
			{
				string scaleValue = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
				dataCollector.AddToIncludes( m_uniqueId, Constants.UnityStandardUtilsLibFuncs );
				normalMapUnpackMode = "UnpackScaleNormal( " + src + " ," + scaleValue + " )";
			}
			else
			{
				normalMapUnpackMode = "UnpackNormal( " + src + " )";
			}

			int outputUsage = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
					outputUsage += 1;
			}


			if ( outputUsage > 1 )
			{
				string varName = "localUnpackNormal" + m_uniqueId;
				dataCollector.AddToLocalVariables( m_uniqueId, "float3 " + varName + " = " + normalMapUnpackMode + ";" );
				return GetOutputVectorItem( 0, outputId, varName );
			}
			else
			{
				return GetOutputVectorItem( 0, outputId, normalMapUnpackMode );
			}
		}
	}
}
