// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Component Mask", "Misc", "Mask certain channels from vectors/color components" )]
	public sealed class ComponentMaskNode : ParentNode
	{
		[SerializeField]
		private bool[] m_selection = { true, true, true, true };

		[SerializeField]
		private int m_outputPortCount = 4;

		[SerializeField]
		private string[] m_labels;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_labels = new string[] { "X", "Y", "Z", "W" };
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdatePorts();
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			UpdatePorts();
		}

		void UpdatePorts()
		{
			m_inputPorts[ 0 ].MatchPortToConnection();
			int count = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			int activeCount = 0;
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					if ( m_selection[ i ] )
						activeCount += 1;
				}
			}

			m_outputPortCount = activeCount;
			switch ( activeCount )
			{
				case 0: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
				case 1: ChangeOutputType( WirePortDataType.FLOAT, false ); break;
				case 2: ChangeOutputType( WirePortDataType.FLOAT2, false ); break;
				case 3: ChangeOutputType( WirePortDataType.FLOAT3, false ); break;
				case 4: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
			}
			
		}
		
		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();

			int count = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			int activeCount = 0;
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					m_selection[ i ] = EditorGUILayout.ToggleLeft( m_labels[i], m_selection[ i ] );
					m_labels[ i ] = UIUtils.GetComponentForPosition( i, m_inputPorts[ 0 ].DataType ).ToUpper();
					if ( m_selection[ i ] )
						activeCount += 1;
				}
			}

			if ( activeCount != m_outputPortCount )
			{
				m_outputPortCount = activeCount;
				switch ( activeCount )
				{
					case 0: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
					case 1: ChangeOutputType( WirePortDataType.FLOAT, false ); break;
					case 2: ChangeOutputType( WirePortDataType.FLOAT2, false ); break;
					case 3: ChangeOutputType( WirePortDataType.FLOAT3, false ); break;
					case 4: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
				}
				SetSaveIsDirty();
			}

			EditorGUILayout.EndVertical();
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string value = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalVar );

			int count = 0;

			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				{
					count = 0;
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			if ( count > 0 )
			{
				value += ".";
				for ( int i = 0; i < count; i++ )
				{
					if ( m_selection[ i ] )
					{
						value += UIUtils.GetComponentForPosition( i, m_outputPorts[ 0 ].DataType );
					}
				}
			}
			
			return value;
		}

		public string GetComponentForPosition( int i )
		{
			switch ( i )
			{
				case 0:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "r" : "x" );
				}
				case 1:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "g" : "y" );
				}
				case 2:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "b" : "z" );
				}
				case 3:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "a" : "w" );
				}
			}
			return string.Empty;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			for ( int i = 0; i < 4; i++ )
			{
				m_selection[ i ] = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}

		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			for ( int i = 0; i < 4; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_selection[ i ] );
			}
		}
	}
}
