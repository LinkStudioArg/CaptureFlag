// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum WirePortDataType
	{
		OBJECT = 1 << 1,
		FLOAT = 1 << 2,
		FLOAT2 = 1 << 3,
		FLOAT3 = 1 << 4,
		FLOAT4 = 1 << 5,
		FLOAT3x3 = 1 << 6,
		FLOAT4x4 = 1 << 7,
		COLOR = 1 << 8,
		INT = 1 << 9,
		SAMPLER1D = 1 << 10,
		SAMPLER2D = 1 << 11,
		SAMPLER3D = 1 << 12,
		SAMPLERCUBE = 1 << 13
	}

	[System.Serializable]
	public class WirePort
	{
		private Vector2 m_labelSize;
		protected bool m_dirtyLabelSize;

		[SerializeField]
		private Rect m_position;

		[SerializeField]
		protected int m_nodeId = -1;

		[SerializeField]
		protected int m_portId = -1;

		[SerializeField]
		protected int m_orderId = -1;

		[SerializeField]
		protected WirePortDataType m_dataType = WirePortDataType.FLOAT;

		[SerializeField]
		protected string m_name;

		[SerializeField]
		protected List<WireReference> m_externalReferences;

		[SerializeField]
		protected bool m_locked = false;

		[SerializeField]
		protected bool m_visible = true;

		[SerializeField]
		protected bool m_hasCustomColor = false;

		[SerializeField]
		protected Color m_customColor = Color.white;

		[SerializeField]
		protected Rect m_activePortArea;

		public WirePort( int nodeId, int portId, WirePortDataType dataType, string name, int orderId = -1 )
		{
			m_nodeId = nodeId;
			m_portId = portId;
			m_orderId = orderId;
			m_dataType = dataType;
			m_name = name;
			m_externalReferences = new List<WireReference>();
		}

		public void Destroy()
		{
			m_externalReferences.Clear();
			m_externalReferences = null;
		}

		public bool ConnectTo( WireReference port )
		{
			if ( m_locked )
				return false;

			if ( m_externalReferences.Contains( port ) )
				return false;

			m_externalReferences.Add( port );
			return true;
		}

		public bool ConnectTo( int nodeId, int portId )
		{
			if ( m_locked )
				return false;


			foreach ( WireReference reference in m_externalReferences )
			{
				if ( reference.NodeId == nodeId && reference.PortId == portId )
				{
					return false;
				}
			}
			m_externalReferences.Add( new WireReference( nodeId, portId, m_dataType, false ) );
			return true;
		}

		public bool ConnectTo( int nodeId, int portId, WirePortDataType dataType, bool typeLocked )
		{
			if ( m_locked )
				return false;

			foreach ( WireReference reference in m_externalReferences )
			{
				if ( reference.NodeId == nodeId && reference.PortId == portId )
				{
					return false;
				}
			}
			m_externalReferences.Add( new WireReference( nodeId, portId, dataType, typeLocked ) );
			return true;
		}

		public void DummyAdd( int nodeId, int portId )
		{
			m_externalReferences.Insert( 0, new WireReference( nodeId, portId, WirePortDataType.OBJECT, false ) );
		}

		public void DummyRemove()
		{
			m_externalReferences.RemoveAt( 0 );
		}


		public WireReference GetConnection( int connID = 0 )
		{
			if ( connID < m_externalReferences.Count )
				return m_externalReferences[ connID ];
			return null;
		}

		public void ChangeProperties( string newName, WirePortDataType newType, bool invalidateConnections )
		{
			m_name = newName;
			if ( m_dataType != newType )
			{
				DataType = newType;
				if ( invalidateConnections )
				{
					InvalidateAllConnections();
				}
				else
				{
					NotifyExternalRefencesOnChange();
				}
			}
		}

		public void ChangeType( WirePortDataType newType, bool invalidateConnections )
		{
			if ( m_dataType != newType )
			{
				DataType = newType;
				if ( invalidateConnections )
				{
					InvalidateAllConnections();
				}
				else
				{
					NotifyExternalRefencesOnChange();
				}
			}
		}

		public virtual void NotifyExternalRefencesOnChange() { }

		public void UpdateInfoOnExternalConn( int nodeId, int portId, WirePortDataType type )
		{
			for ( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if ( m_externalReferences[ i ].NodeId == nodeId && m_externalReferences[ i ].PortId == portId )
				{
					m_externalReferences[ i ].DataType = type;
				}
			}
		}

		public void InvalidateConnection( int nodeId, int portId )
		{
			int id = -1;
			for ( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if ( m_externalReferences[ i ].NodeId == nodeId && m_externalReferences[ i ].PortId == portId )
				{
					id = i;
					break;
				}
			}

			if ( id > -1 )
				m_externalReferences.RemoveAt( id );
		}

		public void RemoveInvalidConnections()
		{
			Debug.Log( "Cleaning invalid connections" );
			List<WireReference> validConnections = new List<WireReference>();
			for ( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if ( m_externalReferences[ i ].IsValid )
				{
					validConnections.Add( m_externalReferences[ i ] );
				}
				else
				{
					Debug.Log( "Detected invalid connection on node " + m_nodeId + " port " + m_portId );
				}
			}
			m_externalReferences.Clear();
			m_externalReferences = validConnections;
		}

		public void InvalidateAllConnections()
		{
			m_externalReferences.Clear();
		}

		public bool IsConnectedTo( int nodeId, int portId )
		{
			if ( m_locked )
				return false;
			 
			for ( int i = 0; i < m_externalReferences.Count; i++ )
			{
				if ( m_externalReferences[ i ].NodeId == nodeId && m_externalReferences[ i ].PortId == portId )
					return true;
			}
			return false;
		}

		public WirePortDataType ConnectionType( int id = 0 )
		{
			return ( id < m_externalReferences.Count ) ? m_externalReferences[ id ].DataType : DataType;
		}

		public bool CheckMatchConnectionType( int id = 0 )
		{
			if ( id < m_externalReferences.Count )
				return m_externalReferences[ id ].DataType == DataType;

			return false;
		}

		public void MatchPortToConnection( int id = 0 )
		{
			if ( id < m_externalReferences.Count )
			{
				DataType = m_externalReferences[ id ].DataType;
			}
		}

		public void ResetWireReferenceStatus()
		{
			for ( int i = 0; i < m_externalReferences.Count; i++ )
			{
				m_externalReferences[ i ].WireStatus = WireStatus.Default;
			}
		}

		public bool InsideActiveArea( Vector2 pos )
		{
			return m_activePortArea.Contains( pos );
		}

		public virtual void ForceClearConnection() { }

		public bool IsConnected
		{
			get { return ( m_externalReferences.Count > 0  && !m_locked ); }
		}

		public List<WireReference> ExternalReferences
		{
			get { return m_externalReferences; }
		}

		public int ConnectionCount
		{
			get { return m_externalReferences.Count; }
		}

		public Rect Position
		{
			get { return m_position; }

			set { m_position = value; }
		}

		public int PortId
		{
			get { return m_portId; }
			set { m_portId = value; }
		}

		public int OrderId
		{
			get { return m_orderId; }
			set { m_orderId = value; }
		}


		public int NodeId
		{
			get { return m_nodeId; }
			set { m_nodeId = value; }
		}

		public virtual WirePortDataType DataType
		{
			get { return m_dataType; }
			set { m_dataType = value; }
		}

		public bool Visible
		{
			get { return m_visible; }
			set
			{
				m_visible = value;
				if ( !m_visible && IsConnected )
				{
					ForceClearConnection();
				}
			}
		}

		public bool Locked
		{
			get { return m_locked; }
			set
			{
				//if ( m_locked && IsConnected )
				//{
				//	ForceClearConnection();
				//}
				m_locked = value;
			}
		}

		public string Name
		{
			get { return m_name; }
			set { m_name = value; m_dirtyLabelSize = true; }
		}

		public bool DirtyLabelSize
		{
			get { return m_dirtyLabelSize; }
			set { m_dirtyLabelSize = value; }
		}

		public bool HasCustomColor
		{
			get { return m_hasCustomColor; }
		}

		public Color CustomColor
		{
			get { return m_customColor; }
			set
			{
				m_hasCustomColor = true;
				m_customColor = value;
			}
		}

		public Rect ActivePortArea
		{
			get { return m_activePortArea; }
			set { m_activePortArea = value; }
		}

		public Vector2 LabelSize
		{
			get { return m_labelSize; }
			set { m_labelSize = value; m_dirtyLabelSize = true; }
		}

		public bool Available { get { return m_visible && !m_locked; } }
		public override string ToString()
		{
			string dump = "";
			dump += "Order: " + m_orderId + "\n";
			dump += "Name: " + m_name + "\n";
			dump += " Type: " + m_dataType;
			dump += " NodeId : " + m_nodeId;
			dump += " PortId : " + m_portId;
			dump += "\nConnections:\n";
			foreach ( WireReference wirePort in m_externalReferences )
			{
				dump += wirePort + "\n";
			}
			return dump;
		}

	}
}
