// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "PI", "Constants", "PI constant : 3.14159265359" )]
	public sealed class PiNode : ParentNode
	{
		[SerializeField]
		private float m_value = 1;

		public PiNode() : base() { }
		public PiNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, true, "Multiplier" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 40;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			{
				m_value = EditorGUILayout.FloatField( Constants.ValueLabel, m_value );
			}
			EditorGUILayout.EndVertical();
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			string finalValue = string.Empty;
			finalValue = InputPorts[ 0 ].IsConnected ? ( InputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) + "*" + Mathf.PI.ToString() ) : ( m_value * Mathf.PI ).ToString();

			if ( finalValue.Equals( string.Empty ) )
			{
				UIUtils.ShowMessage( "PINode generating empty code", MessageSeverity.Warning );
			}
			return finalValue;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_value = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_value );
		}

	}
}
