// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[VS] Vertex TexCoord", "Vertex Data", "Vertex texture coordinates. Only works on Vertex Shaders ports ( p.e. Local Vertex Offset Port )." )]
	public sealed class TexCoordVertexDataNode : VertexDataNode
	{
		[SerializeField]
		private int m_index = 0;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "texcoord";
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
				m_index = EditorGUILayout.IntPopup( Constants.AvailableUVChannelLabel, m_index, Constants.AvailableUVChannelsStr, Constants.AvailableUVChannels );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_currentVertexData = ( m_index == 0 ) ? "texcoord" : "texcoord" + Constants.AvailableUVChannelsStr[ m_index ];
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2502 )
			{
				m_index = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void WriteInputDataToString( ref string nodeInfo )
		{
			base.WriteInputDataToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_index );
		}
	}
}
