// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public sealed class OutputPort : WirePort
	{
		[SerializeField]
		private bool m_connectedToMasterNode;

		[SerializeField]
		private bool m_isLocalValue;

		[SerializeField]
		private string m_localOutputValue;

		public OutputPort( int nodeId, int portId, WirePortDataType dataType, string name ) : base( nodeId, portId, dataType, name ) { LabelSize = Vector2.zero; }

		public bool ConnectedToMasterNode
		{
			get { return m_connectedToMasterNode; }
			set { m_connectedToMasterNode = value; }
		}

		public override void NotifyExternalRefencesOnChange()
		{
			for ( int i = 0; i < m_externalReferences.Count; i++ )
			{
				ParentNode node = UIUtils.GetNode( m_externalReferences[ i ].NodeId );
				if ( node )
				{
					InputPort port = node.GetInputPortById( m_externalReferences[ i ].PortId );
					port.UpdateInfoOnExternalConn( m_nodeId, m_portId, m_dataType );
					node.OnConnectedOutputNodeChanges( m_externalReferences[ i ].PortId, m_nodeId, m_portId, m_name, m_dataType );
				}
			}
		}

		public string ConfigOutputLocalValue( PrecisionType precisionType, string value )
		{
			m_localOutputValue = "temp_output_" + m_nodeId + "_" + PortId;
			m_isLocalValue = true;
			return string.Format( Constants.LocalValueDecWithoutIdent, UIUtils.PrecisionWirePortToCgType( precisionType, DataType ), m_localOutputValue, value );
		}

		public void SetLocalValue( string value )
		{
			m_isLocalValue = true;
			m_localOutputValue = value;
		}

		public void ResetLocalValue()
		{
			m_isLocalValue = false;
			m_localOutputValue = string.Empty;
		}

		public override void ForceClearConnection()
		{
			UIUtils.DeleteConnection( false, m_nodeId, m_portId, false, true );
		}

		public bool IsLocalValue
		{
			get { return m_isLocalValue; }
		}

		public string LocalValue
		{
			get { return m_localOutputValue; }
		}
	}
}
