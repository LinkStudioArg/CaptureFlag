// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Texture Coordinates", "Surface Standard Inputs", "Texture UV coordinates set", null, KeyCode.U )]
	public sealed class TextureCoordinatesNode : ParentNode
	{
		private readonly string[] Dummy = { string.Empty };

		private const string TilingStr = "Tiling";
		private const string OffsetStr = "Offset";
		private const string TexCoordStr = "texcoord_";

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private int m_textureCoordChannel = 0;

		[SerializeField]
		private int m_texcoordId = -1;

		[SerializeField]
		private string m_surfaceTexcoordName = string.Empty;

		private bool m_forceNodeUpdate = false;
		private TexturePropertyNode m_referenceNode = null;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Tiling" );
			m_inputPorts[ 0 ].Vector2InternalData = new Vector2( 1, 1 );
			AddInputPort( WirePortDataType.FLOAT2, false, "Offset" );
			AddOutputVectorPorts( WirePortDataType.FLOAT2, Constants.EmptyPortValue );
			m_textLabelWidth = 75;
			m_useInternalPortData = true;
			m_inputPorts[ 0 ].Category = MasterNodePortCategory.Vertex;
			m_inputPorts[ 1 ].Category = MasterNodePortCategory.Vertex;
		}

		public override void Reset()
		{
			m_texcoordId = -1;
			m_surfaceTexcoordName = string.Empty;
		}

		void UpdateTitle()
		{
			if ( m_referenceArrayId > -1 && m_referenceNode != null )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				m_additionalContent.text = string.Format( "Value( {0} )", m_referenceNode.PropertyInspectorName );
				m_titleLineAdjust = 5;
				m_sizeIsDirty = true;
			}
			else
			{
				m_additionalContent.text = string.Empty;
				m_titleLineAdjust = 0;
				m_sizeIsDirty = true;
			}
		}

		void UpdatePorts()
		{
			if ( m_referenceArrayId > -1 )
			{
				m_inputPorts[ 0 ].Locked = true;
				m_inputPorts[ 1 ].Locked = true;
			}
			else
			{
				m_inputPorts[ 0 ].Locked = false;
				m_inputPorts[ 1 ].Locked = false;
			}
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			List<string> arr = new List<string>( UIUtils.TexturePropertyNodeArr() );
			bool guiEnabledBuffer = GUI.enabled;
			if ( arr != null && arr.Count > 0 )
			{
				arr.Insert( 0, "None" );
				GUI.enabled = true;
				m_referenceArrayId = EditorGUILayout.Popup( Constants.AvailableReferenceStr, m_referenceArrayId + 1, arr.ToArray() ) - 1;
			}
			else
			{
				m_referenceArrayId = -1;
				GUI.enabled = false;
				m_referenceArrayId = EditorGUILayout.Popup( Constants.AvailableReferenceStr, m_referenceArrayId + 1, Dummy );
			}



			GUI.enabled = guiEnabledBuffer;
			if ( EditorGUI.EndChangeCheck() )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if ( m_referenceNode != null )
				{
					m_referenceNodeId = m_referenceNode.UniqueId;
				}
				else
				{
					m_referenceNodeId = -1;
					m_referenceArrayId = -1;
				}

				UpdateTitle();
				UpdatePorts();
			}

			m_textureCoordChannel = EditorGUILayout.IntPopup( Constants.AvailableUVSetsLabel, m_textureCoordChannel, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );

			if ( m_referenceArrayId > -1 )
				GUI.enabled = false;

			base.DrawProperties();

			GUI.enabled = guiEnabledBuffer;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( m_forceNodeUpdate )
			{
				m_forceNodeUpdate = false;
				if ( UIUtils.CurrentShaderVersion() > 2404 )
				{
					m_referenceNode = UIUtils.GetNode( m_referenceNodeId ) as TexturePropertyNode;
					m_referenceArrayId = UIUtils.GetTexturePropertyNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
					if ( m_referenceNode != null )
					{
						m_referenceNodeId = m_referenceNode.UniqueId;
					}
				}
				UpdateTitle();
				UpdatePorts();
			}

			if ( m_referenceNode == null && m_referenceNodeId > -1 )
			{
				m_referenceNodeId = -1;
				m_referenceArrayId = -1;
				UpdateTitle();
				UpdatePorts();
			}
		}
		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_textureCoordChannel = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( UIUtils.CurrentShaderVersion() > 2402 )
			{
				if ( UIUtils.CurrentShaderVersion() > 2404 )
				{
					m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_referenceArrayId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				m_forceNodeUpdate = true;
			}
		}

		public override void PropagateNodeCategory( MasterNodePortCategory category )
		{
			UIUtils.SetCategoryInBitArray( ref m_category, category );

			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
				{
					//m_inputPorts[ i ].GetOutputNode().PropagateNodeCategory( category );
					m_inputPorts[ i ].GetOutputNode().PropagateNodeCategory( MasterNodePortCategory.Vertex );
				}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordChannel );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceNode != null ) ? m_referenceNode.UniqueId : -1 ) );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_referenceArrayId > -1 )
			{
				//TexturePropertyNode node = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if ( m_referenceNode != null )
				{
					string propertyName = m_referenceNode.PropertyName;
					int coordSet = ( ( m_textureCoordChannel < 0 ) ? 0 : m_textureCoordChannel );
					string uvChannelDeclaration = IOUtils.GetUVChannelDeclaration( propertyName, -1, coordSet );
					dataCollector.AddToInput( m_uniqueId, uvChannelDeclaration, true );
					return GetOutputVectorItem( 0, outputId, Constants.InputVarStr + "."+ IOUtils.GetUVChannelName( propertyName, coordSet ));

					//dataCollector.AddToInput( m_uniqueId, IOUtils.GetUVChannelDeclaration( node.PropertyName, -1, -1 ), true );
					//return Constants.InputVarStr+"."+ IOUtils.GetUVChannelName( node.PropertyName, -1 );
				}
			}

			if ( m_texcoordId < 0 )
			{
				m_texcoordId = dataCollector.AvailableUvIndex;
				string texcoordName = TexCoordStr + m_texcoordId;
				string uvChannel = m_textureCoordChannel == 0 ? ".xy" : m_textureCoordChannel + ".xy";

				MasterNodePortCategory portCategory = dataCollector.PortCategory;
				dataCollector.PortCategory = MasterNodePortCategory.Vertex;
				bool dirtyVarsBefore = dataCollector.DirtySpecialLocalVariables;
				string tiling = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, 0, true );
				string offset = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, 0, true );
				dataCollector.PortCategory = portCategory;

				string vertexUV = Constants.VertexShaderInputStr + ".texcoord" + uvChannel;
				dataCollector.AddToInput( m_uniqueId, "float2 " + texcoordName, true );

				// new texture coordinates are calculated on the vertex shader so we need to register its local vars
				if ( !dirtyVarsBefore && dataCollector.DirtySpecialLocalVariables )
				{
					dataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.SpecialLocalVariables, m_uniqueId, false );
					UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
				}

				dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + texcoordName + ".xy = " + vertexUV + " * " + tiling + " + " + offset, m_uniqueId );
				m_surfaceTexcoordName = Constants.InputVarStr + "." + texcoordName;
			}

			return GetOutputVectorItem( 0, outputId, m_surfaceTexcoordName );
		}

		public override void Destroy()
		{
			base.Destroy();
			m_referenceNode = null;
		}

	}
}
