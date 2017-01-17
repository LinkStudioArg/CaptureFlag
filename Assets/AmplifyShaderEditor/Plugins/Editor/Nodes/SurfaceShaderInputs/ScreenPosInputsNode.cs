// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Position", "Surface Standard Inputs", "Screen space position" )]
	public sealed class ScreenPosInputsNode : SurfaceShaderINParentNode
	{
		private const string NormalizeStr = "Normalize";

		[SerializeField]
		private bool m_normalize = false;
		
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.SCREEN_POS;
			InitialSetup();
			m_textLabelWidth = 65;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_normalize = EditorGUILayout.Toggle( NormalizeStr, m_normalize );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string result = base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalVar );
			if ( m_normalize )
			{
				result += "/" + GetOutputVectorItem( 0, 4, m_currentInputValueStr );
				result = UIUtils.AddBrackets( result );
			}
			return result;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2400 )
				m_normalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalize );
		}
	}
}
