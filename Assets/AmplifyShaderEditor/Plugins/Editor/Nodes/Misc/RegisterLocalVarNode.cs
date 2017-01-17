// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Register Local Var", "Misc", "Forces a local variable to be written" )]
	public sealed class RegisterLocalVarNode : SignalGeneratorNode
	{
		private const string LocalDefaultNameStr = "myVarName";
		private const string LocalVarNameStr = "Var Name";
		private const string OrderIndexStr = "Order Index";
		
		[SerializeField]
		private string m_variableName = LocalDefaultNameStr;

		[SerializeField]
		private int m_orderIndex = -1;

		private bool m_forceUpdate = true;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 80;
			m_variableName += UIUtils.LocalVarNodeAmount();
			UIUtils.RegisterLocalVarNode( this );
			UpdateTitle();
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterLocalVarNode( this );
		}

		void UpdateTitle()
		{
			m_additionalContent.text = string.Format( Constants.PropertyValueLabel, m_variableName );
			m_titleLineAdjust = 5;
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_variableName = EditorGUILayout.TextField( LocalVarNameStr, m_variableName );
			if ( EditorGUI.EndChangeCheck() )
			{

				m_variableName = UIUtils.RemoveInvalidCharacters( m_variableName );
				if ( string.IsNullOrEmpty( m_variableName ) )
				{
					m_variableName = LocalDefaultNameStr + m_uniqueId;
				}

				UIUtils.UpdateLocalVarDataNode( m_uniqueId, m_variableName );
				UpdateTitle();
				m_sizeIsDirty = true;
			}
			m_orderIndex = EditorGUILayout.IntField( OrderIndexStr, m_orderIndex );
			DrawPrecisionProperty();
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_forceUpdate )
			{
				m_forceUpdate = false;
				UpdateTitle();
				m_sizeIsDirty = true;
			}
		}
		
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return m_variableName;
		}

		public string CreateLocalVariable( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string type = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				string result = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, true, 0, false );
				return string.Format( Constants.LocalValueDec, type, m_variableName, result );
			}
			else
			{
				return string.Format( Constants.SimpleLocalValueDec, type, m_variableName );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_variableName = GetCurrentParam( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 14 )
				m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			
			UIUtils.UpdateLocalVarDataNode( m_uniqueId, m_variableName );
			m_forceUpdate = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_variableName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
		}

		public override string DataToArray { get { return m_variableName; } }
		public int OrderIndex { get { return m_orderIndex; } }
	}
}
