// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class SurfaceShaderINParentNode : ParentNode
	{
		[SerializeField]
		protected AvailableSurfaceInputs m_currentInput;

		[SerializeField]
		protected string m_currentInputValueStr;

		[SerializeField]
		protected string m_currentInputDecStr;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.UV_COORDS;
			m_textLabelWidth = 65;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			DrawPrecisionProperty();
		}
		//This needs to be called on the end of the CommonInit on all children
		protected void InitialSetup()
		{
			m_currentInputValueStr = Constants.InputVarStr + "." + UIUtils.GetInputValueFromType( m_currentInput );
			
			string outputName = "Out";
			switch ( m_currentInput )
			{
				case AvailableSurfaceInputs.DEPTH:
				{
					AddOutputPort( WirePortDataType.FLOAT, outputName );
				}
				break;
				case AvailableSurfaceInputs.UV_COORDS:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT2, outputName );
				}
				break;
				case AvailableSurfaceInputs.UV2_COORDS:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT2, outputName );
				}
				break;
				case AvailableSurfaceInputs.VIEW_DIR:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
				case AvailableSurfaceInputs.COLOR:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT4, outputName );
				}
				break;
				case AvailableSurfaceInputs.SCREEN_POS:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT4, outputName );
				}
				break;
				case AvailableSurfaceInputs.WORLD_POS:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
				case AvailableSurfaceInputs.WORLD_REFL:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
				case AvailableSurfaceInputs.WORLD_NORMAL:
				{
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			m_currentInputDecStr = UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, m_currentInput );
			dataCollector.AddToInput( m_uniqueId, m_currentInputDecStr, true );
			switch ( m_currentInput )
			{
				case AvailableSurfaceInputs.WORLD_POS:
				case AvailableSurfaceInputs.WORLD_REFL:
				case AvailableSurfaceInputs.WORLD_NORMAL:
				{
					dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );
				}
				break;
				case AvailableSurfaceInputs.DEPTH:
				case AvailableSurfaceInputs.UV_COORDS:
				case AvailableSurfaceInputs.UV2_COORDS:
				case AvailableSurfaceInputs.VIEW_DIR:
				case AvailableSurfaceInputs.COLOR:
				case AvailableSurfaceInputs.SCREEN_POS: break;
			};

			return GetOutputVectorItem( 0, outputId, m_currentInputValueStr );
		}

	}
}
