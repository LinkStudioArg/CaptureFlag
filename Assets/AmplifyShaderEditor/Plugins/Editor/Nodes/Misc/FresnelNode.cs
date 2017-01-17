// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// http://kylehalladay.com/blog/tutorial/2014/02/18/Fresnel-Shaders-From-The-Ground-Up.html
// http://http.developer.nvidia.com/CgTutorial/cg_tutorial_chapter07.html

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Fresnel", "Misc", "Simple Fresnel effect" )]
	public sealed class FresnelNode : ParentNode
	{

		private const string FresnedDotVar = "fresnelDotVal";
		private const string FresnedFinalVar = "fresnelFinalVal";

		private const string FresnesDotOp = "float {0} = dot( {1},{2} );";
		private const string FresnesFinalOp = "float {0} = {1};";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddInputPort( WirePortDataType.FLOAT, false, "Bias" );
			AddInputPort( WirePortDataType.FLOAT, false, "Scale" );
			AddInputPort( WirePortDataType.FLOAT, false, "Power" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_useInternalPortData = true;
			m_inputPorts[ 2 ].FloatInternalData = 1;
			m_inputPorts[ 3 ].FloatInternalData = 1;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToIncludes( m_uniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToInput( m_uniqueId, Constants.IncidentVecDecStr, true );
			dataCollector.AddVertexInstruction( Constants.VertexVecDecStr + " = mul(unity_ObjectToWorld," + Constants.VertexShaderInputStr + ".vertex)", m_uniqueId );
			dataCollector.AddVertexInstruction( Constants.IncidentVecDefStr, m_uniqueId );


			string normal = string.Empty;
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				normal = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, 0, true );
			}
			else
			{
				dataCollector.AddToInput( m_uniqueId, Constants.NormalVecDecStr, true );
				dataCollector.AddVertexInstruction( Constants.NormalVecVertStr + " = UnityObjectToWorldNormal(" + Constants.VertexShaderInputStr + ".normal)", m_uniqueId );
				normal = Constants.NormalVecFragStr;
			}

			string bias = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, 0, true );
			string scale = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, 0, true );
			string power = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, 0, true );

			string fresnelLocalDotVar = FresnedDotVar + m_uniqueId;

			dataCollector.AddToLocalVariables( m_uniqueId, string.Format( FresnesDotOp, fresnelLocalDotVar, Constants.IncidentVecFragStr, normal ) );

			string fresnalFinalVar = FresnedFinalVar + m_uniqueId;
			dataCollector.AddToLocalVariables( m_uniqueId, string.Format( FresnesFinalOp, fresnalFinalVar, string.Format( "({0} + {1}*pow( 1.0 + {2} , {3}))", bias, scale, fresnelLocalDotVar, power ) ) );
			return CreateOutputLocalVariable( 0, fresnalFinalVar, ref dataCollector );
		}
	}
}
