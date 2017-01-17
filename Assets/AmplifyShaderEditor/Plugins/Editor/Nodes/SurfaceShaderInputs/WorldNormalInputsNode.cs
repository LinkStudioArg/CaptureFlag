// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex Normal World", "Surface Standard Inputs", "Vertex Normal World" )]
	public sealed class WorldNormalInputsNode : SurfaceShaderINParentNode
	{
		private const string PerPixelLabelStr = "Per Pixel";

		[SerializeField]
		private bool m_perPixel = true;

		[SerializeField]
		private string m_precisionString;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.WORLD_NORMAL;
			InitialSetup();
			UIUtils.AddNormalDependentCount();
			m_precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.RemoveNormalDependentCount();
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_perPixel = EditorGUILayout.ToggleLeft( PerPixelLabelStr, m_perPixel );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
			dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );
			if ( dataCollector.PortCategory != MasterNodePortCategory.Debug && m_perPixel && dataCollector.DirtyNormal )
			{

				//string result = "WorldNormalVector( " + Constants.InputVarStr + " , float3( 0,0,1 ))";
				m_precisionString = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
				string result = string.Format( Constants.WorldNormalLocalDecStr, m_precisionString );
				int count = 0;
				for ( int i = 0; i < m_outputPorts.Count; i++ )
				{
					if ( m_outputPorts[ i ].IsConnected )
					{
						if ( m_outputPorts[ i ].ConnectionCount > 2 )
						{
							count = 2;
							break;
						}
						count += 1;
						if ( count > 1 )
							break;
					}
				}
				if ( count > 1 )
				{
					string localVarName = "WorldNormal" + m_uniqueId;
					dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, m_outputPorts[ 0 ].DataType, localVarName, result );
					return GetOutputVectorItem( 0, outputId, localVarName );
				}
				else
				{
					return GetOutputVectorItem( 0, outputId, result );
				}
			}
			else
			{
				return base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalVar );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2504 )
				m_perPixel = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_perPixel );
		}
	}
}
