// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum eResizeAxis
	{
		X_AXIS,
		Y_AXIS,
		ALL
	}
	
	[Serializable]
	public sealed class CommentaryNode : ParentNode, ISerializationCallbackReceiver
	{
		private const string CommentaryTitle = "Commentary";
		private const float BORDER_SIZE_X = 50;
		private const float BORDER_SIZE_Y = 50;
		private const float MIN_SIZE_X = 100;
		private const float MIN_SIZE_Y = 100;
		private const float COMMENTARY_BOX_HEIGHT = 30;


		private readonly Vector2 ResizeButtonPos = new Vector2( 1, 1 );

		[SerializeField]
		private string m_commentText = "Comment";

		[SerializeField]
		private eResizeAxis m_resizeAxis = eResizeAxis.ALL;

		[SerializeField]
		private List<ParentNode> m_nodesOnCommentary = new List<ParentNode>();
		private Dictionary<int, ParentNode> m_nodesOnCommentaryDict = new Dictionary<int, ParentNode>();
		private bool m_reRegisterNodes = false;

		[SerializeField]
		private Rect m_resizeLeftIconCoords;

		[SerializeField]
		private Rect m_resizeRightIconCoords;


		private Texture2D m_resizeIconTex;

		private bool m_isResizingRight = false;
		private bool m_isResizingLeft = false;

		private Vector2 m_resizeStartPoint = Vector2.zero;


		private List<int> m_nodesIds = new List<int>();
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_reorderLocked = true;
			m_rmbIgnore = true;
			m_defaultInteractionMode = InteractionMode.Both;
			m_headerColor = UIUtils.GetColorFromCategory( "Commentary" );
			m_connStatus = NodeConnectionStatus.Island;
			m_textLabelWidth = 80;
		}


		public void CreateFromSelectedNodes( Vector2 mousePosOnCanvasCoords, List<ParentNode> selectedNodes )
		{
			if ( selectedNodes.Count == 0 )
			{
				m_position = new Rect( mousePosOnCanvasCoords, new Vector2( 100, 100 ) );
				return;
			}

			Vector2 minPos = new Vector2( float.MaxValue, float.MaxValue );
			Vector2 maxPos = new Vector2( float.MinValue, float.MinValue );

			for ( int i = 0; i < selectedNodes.Count; i++ )
			{
				//Check min
				if ( selectedNodes[ i ].Position.x < minPos.x )
					minPos.x = selectedNodes[ i ].Position.x;

				if ( selectedNodes[ i ].Position.y < minPos.y )
					minPos.y = selectedNodes[ i ].Position.y;

				//check max
				float nodeXMax = selectedNodes[ i ].Position.x + selectedNodes[ i ].Position.width;
				if ( nodeXMax > maxPos.x )
				{
					maxPos.x = nodeXMax;
				}

				float nodeYMax = selectedNodes[ i ].Position.y + selectedNodes[ i ].Position.height;
				if ( nodeYMax > maxPos.y )
				{
					maxPos.y = nodeYMax;
				}

				//_nodesOnCommentary.Add( selectedNodes[ i ] );
				//selectedNodes[ i ].OnNodeStoppedMovingEvent += NodeStoppedMoving;
				AddNodeToCommentary( selectedNodes[ i ] );
			}

			Vector2 dims = maxPos - minPos + new Vector2( 2 * BORDER_SIZE_X, 2 * BORDER_SIZE_Y );
			m_position = new Rect( minPos.x - BORDER_SIZE_X, minPos.y - BORDER_SIZE_Y, dims.x, dims.y );
		}

		public override void Move( Vector2 delta , bool snap )
		{
			if ( m_isResizingRight || m_isResizingLeft )
				return;

			base.Move( delta , snap );
			for ( int i = 0; i < m_nodesOnCommentary.Count; i++ )
			{
				m_nodesOnCommentary[ i ].Move( delta , snap );
			}
		}

		public void NodeStoppedMoving( ParentNode node, bool testOnlySelected, InteractionMode useTargetInteraction )
		{
			if ( !m_position.Contains( node.Vec2Position ) && !m_position.Contains( node.Corner ) )
			{
				RemoveNode( node );
			}
		}

		public void RemoveNode( ParentNode node )
		{
			if ( m_nodesOnCommentaryDict.ContainsKey( node.UniqueId ) )
			{
				m_nodesOnCommentary.Remove( node );
				m_nodesOnCommentaryDict.Remove( node.UniqueId );
				node.OnNodeStoppedMovingEvent -= NodeStoppedMoving;
				node.IsCommentaryParented = false;
			}
		}

		public void RemoveAllNodes()
		{
			for ( int i = 0; i < m_nodesOnCommentary.Count; i++ )
			{
				m_nodesOnCommentary[ i ].OnNodeStoppedMovingEvent -= NodeStoppedMoving;
				m_nodesOnCommentary[ i ].IsCommentaryParented = false;
			}
			m_nodesOnCommentary.Clear();
			m_nodesOnCommentaryDict.Clear();
		}

		public override void Destroy()
		{
			base.Destroy();
			RemoveAllNodes();
		}

		public void AddNodeToCommentary( ParentNode node )
		{
			if ( !node.IsCommentaryParented && !m_nodesOnCommentaryDict.ContainsKey( node.UniqueId ) )
			{
				if ( node.Depth <= m_depth )
				{
					ActivateNodeReordering( node.Depth );
				}

				m_nodesOnCommentary.Add( node );
				m_nodesOnCommentaryDict.Add( node.UniqueId, node );
				node.OnNodeStoppedMovingEvent += NodeStoppedMoving;
				node.IsCommentaryParented = true;
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_commentText = EditorGUILayout.TextField( CommentaryTitle, m_commentText );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_initialized )
			{
				Color bufferedColor = GUI.color;
				if ( m_nodesIds.Count > 0 )
				{
					for ( int i = 0; i < m_nodesIds.Count; i++ )
					{
						ParentNode node = UIUtils.CurrentWindow.CurrentGraph.GetNode( m_nodesIds[ i ] );
						if ( node )
						{
							AddNodeToCommentary( node );
						}
					}
					m_nodesIds.Clear();
				}

				if ( m_reRegisterNodes )
				{
					m_reRegisterNodes = false;
					m_nodesOnCommentaryDict.Clear();
					for ( int i = 0; i < m_nodesOnCommentary.Count; i++ )
					{
						if ( m_nodesOnCommentary[ i ] != null )
						{
							m_nodesOnCommentary[ i ].OnNodeStoppedMovingEvent += NodeStoppedMoving;
							m_nodesOnCommentaryDict.Add( m_nodesOnCommentary[ i ].UniqueId, m_nodesOnCommentary[ i ] );
						}
					}
				}

				CalculatePositionAndVisibility( drawInfo );
				// Render Node
				GUI.color = Constants.NodeBodyColor;
				GUI.Box( m_globalPosition, string.Empty, UIUtils.CustomStyle( CustomStyle.CommentaryBackground ) );

				GUI.color = Color.white;
				Rect commentArea = m_globalPosition;
				commentArea.y -= 1;
				commentArea.height = COMMENTARY_BOX_HEIGHT * drawInfo.InvertedZoom;
				commentArea.width *= 0.93f;

				GUI.color = m_headerColor;
				m_headerPosition = m_globalPosition;
				m_headerPosition.height = UIUtils.CurrentHeaderHeight;
				GUI.Box( m_headerPosition, string.Empty, UIUtils.CustomStyle( CustomStyle.NodeHeader ) );

				GUI.color = bufferedColor;
				if ( m_selected )
				{
					GUI.Box( m_globalPosition, string.Empty, UIUtils.CustomStyle( CustomStyle.NodeWindowOn ) );
				}


				m_commentText = EditorGUI.TextField( commentArea, string.Empty, m_commentText, UIUtils.CustomStyle( CustomStyle.CommentaryTitle ) );

				if ( m_resizeIconTex == null )
				{
					m_resizeIconTex = UIUtils.CustomStyle( CustomStyle.CommentaryResizeButton ).normal.background;
				}


				// RIGHT RESIZE BUTTON

				m_resizeRightIconCoords = m_globalPosition;
				m_resizeRightIconCoords.x = m_globalPosition.x + m_globalPosition.width - 1 - ( m_resizeIconTex.width + ResizeButtonPos.x ) * drawInfo.InvertedZoom;
				m_resizeRightIconCoords.y = m_globalPosition.y + m_globalPosition.height - 2 - ( m_resizeIconTex.height + ResizeButtonPos.y ) * drawInfo.InvertedZoom;
				m_resizeRightIconCoords.width = m_resizeIconTex.width * drawInfo.InvertedZoom;
				m_resizeRightIconCoords.height = m_resizeIconTex.height * drawInfo.InvertedZoom;

				if ( GUI.RepeatButton( m_resizeRightIconCoords, string.Empty, UIUtils.CustomStyle( CustomStyle.CommentaryResizeButton ) ) )
				{
					if ( !m_isResizingRight && !m_isResizingLeft )
					{
						m_isResizingRight = true;
						UIUtils.CurrentWindow.ForceAutoPanDir = true;
						m_resizeStartPoint = drawInfo.TransformedMousePos;
						UIUtils.CurrentWindow.CurrentGraph.MarkToDeselect();
						UIUtils.CurrentWindow.CurrentGraph.MarkToSelect( m_uniqueId );
					}
				}

				if ( m_isResizingRight )
				{
					if ( drawInfo.CurrentEventType == EventType.mouseUp )
					{
						m_isResizingRight = false;
						UIUtils.CurrentWindow.ForceAutoPanDir = false;
						RemoveAllNodes();
						FireStoppedMovingEvent( false, InteractionMode.Target );
					}
					else
					{
						Vector2 currSize = ( drawInfo.TransformedMousePos - m_resizeStartPoint ) / drawInfo.InvertedZoom;
						m_resizeStartPoint = drawInfo.TransformedMousePos;
						if ( m_resizeAxis != eResizeAxis.Y_AXIS )
						{
							m_position.width += currSize.x;
							if ( m_position.width < MIN_SIZE_X )
							{
								m_position.width = MIN_SIZE_X;
							}
						}

						if ( m_resizeAxis != eResizeAxis.X_AXIS )
						{
							m_position.height += currSize.y;
							if ( m_position.height < MIN_SIZE_Y )
							{
								m_position.height = MIN_SIZE_Y;
							}
						}
					}
				}


				/// LEFT RESIZE BUTTON
				m_resizeLeftIconCoords = m_globalPosition;
				m_resizeLeftIconCoords.x = m_globalPosition.x + 2;
				m_resizeLeftIconCoords.y = m_globalPosition.y + m_globalPosition.height - 2 - ( m_resizeIconTex.height + ResizeButtonPos.y ) * drawInfo.InvertedZoom;
				m_resizeLeftIconCoords.width = m_resizeIconTex.width * drawInfo.InvertedZoom;
				m_resizeLeftIconCoords.height = m_resizeIconTex.height * drawInfo.InvertedZoom;

				if ( GUI.RepeatButton( m_resizeLeftIconCoords, string.Empty, UIUtils.CustomStyle( CustomStyle.CommentaryResizeButtonInv ) ) )
				{
					if ( !m_isResizingRight && !m_isResizingLeft )
					{
						m_isResizingLeft = true;
						UIUtils.CurrentWindow.ForceAutoPanDir = true;
						m_resizeStartPoint = drawInfo.TransformedMousePos;
						UIUtils.CurrentWindow.CurrentGraph.MarkToDeselect();
						UIUtils.CurrentWindow.CurrentGraph.MarkToSelect( m_uniqueId );
					}
				}

				if ( m_isResizingLeft )
				{
					if ( drawInfo.CurrentEventType == EventType.mouseUp )
					{
						m_isResizingLeft = false;
						UIUtils.CurrentWindow.ForceAutoPanDir = false;
						RemoveAllNodes();
						FireStoppedMovingEvent( false, InteractionMode.Target );
					}
					else
					{
						Vector2 currSize = ( drawInfo.TransformedMousePos - m_resizeStartPoint ) / drawInfo.InvertedZoom;
						m_resizeStartPoint = drawInfo.TransformedMousePos;
						if ( m_resizeAxis != eResizeAxis.Y_AXIS )
						{
							m_position.x += currSize.x;
							m_position.width -= currSize.x;
							if ( m_position.width < MIN_SIZE_X )
							{
								m_position.x -= ( MIN_SIZE_X - m_position.width );
								m_position.width = MIN_SIZE_X;
							}
						}

						if ( m_resizeAxis != eResizeAxis.X_AXIS )
						{
							m_position.height += currSize.y;
							if ( m_position.height < MIN_SIZE_Y )
							{
								m_position.height = MIN_SIZE_Y;
							}
						}
					}
				}
				GUI.color = bufferedColor;
			}
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_reRegisterNodes = true;
		}

		public override bool OnNodeInteraction( ParentNode node )
		{
			if ( node == null || m_uniqueId == node.UniqueId )
				return false;

			for ( int i = 0; i < m_nodesOnCommentary.Count; i++ )
			{
				if ( m_nodesOnCommentary[ i ] && m_nodesOnCommentary[ i ].OnNodeInteraction( node ) )
				{
					return false;
				}
			}

			if ( m_position.Contains( node.Vec2Position ) && m_position.Contains( node.Corner ) )
			{
				AddNodeToCommentary( node );
				return true;
			}
			return false;
		}

		public override void OnSelfStoppedMovingEvent()
		{
			FireStoppedMovingEvent( false, InteractionMode.Both );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_position.width = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_position.height = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_commentText = GetCurrentParam( ref nodeParams );
			int count = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			for ( int i = 0; i < count; i++ )
			{
				m_nodesIds.Add( Convert.ToInt32( GetCurrentParam( ref nodeParams ) ) );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_position.width );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_position.height );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_commentText );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_nodesOnCommentary.Count );
			for ( int i = 0; i < m_nodesOnCommentary.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_nodesOnCommentary[ i ].UniqueId );
			}
		}

	}
}
