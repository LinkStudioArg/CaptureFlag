// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum MenuAnchor
	{
		TOP_LEFT = 0,
		TOP_CENTER,
		TOP_RIGHT,
		MIDDLE_LEFT,
		MIDDLE_CENTER,
		MIDDLE_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT,
		NONE
	}

	public enum MenuAutoSize
	{
		MATCH_VERTICAL = 0,
		MATCH_HORIZONTAL,
		NONE
	}

	public class MenuParent
	{
		protected const float MINIMIZE_BUTTON_X_SPACING = 5;
		protected const float MINIMIZE_BUTTON_Y_SPACING = 5.5f;

		protected const float MinimizeCollisionAdjust = 5;

		protected GUIStyle m_style;
		protected GUIContent m_content;
		protected Rect m_maximizedArea;
		protected Rect m_transformedArea;
		protected MenuAnchor m_anchor;
		protected MenuAutoSize m_autoSize;
		protected bool m_isActive = true;
		protected bool m_isMaximized = true;

		protected bool m_lockOnMinimize = false;
		protected bool m_preLockState = false;

		protected Rect m_minimizedArea;
		protected Rect m_minimizeButtonPos;
		protected float m_realWidth;
		protected GUIStyle m_empty = new GUIStyle();

		public MenuParent( float x, float y, float width, float height, string name, MenuAnchor anchor = MenuAnchor.NONE, MenuAutoSize autoSize = MenuAutoSize.NONE )
		{
			m_anchor = anchor;
			m_autoSize = autoSize;
			m_maximizedArea = new Rect( x, y, width, height );
			m_content = new GUIContent( GUIContent.none );
			m_content.text = name;
			m_transformedArea = new Rect();
		}

		public void SetMinimizedArea( float x, float y, float width, float height )
		{
			m_minimizedArea = new Rect( x, y, width, height );
		}

		protected void InitDraw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId )
		{
			if ( m_style == null )
			{
				m_style = new GUIStyle( UIUtils.CurrentWindow.CustomStylesInstance.TextArea );
				m_style.stretchHeight = true;
				m_style.stretchWidth = true;
				m_style.fontSize = ( int ) Constants.DefaultTitleFontSize;
				m_style.fontStyle = FontStyle.Normal;
				Texture minimizeTex = UIUtils.CustomStyle( CustomStyle.MaximizeButton ).normal.background;
				m_minimizeButtonPos = new Rect( 0, 0, minimizeTex.width, minimizeTex.height );
			}

			Rect currentArea = m_isMaximized ? m_maximizedArea : m_minimizedArea;

			if ( m_isMaximized )
			{
				m_realWidth = m_maximizedArea.width;
			}
			else
			{
				if ( currentArea.x < 0 )
				{
					m_realWidth = currentArea.width + currentArea.x;
				}
				else if ( ( currentArea.x + currentArea.width ) > parentPosition.width )
				{
					m_realWidth = parentPosition.width - currentArea.x;
				}
				if ( m_realWidth < 0 )
					m_realWidth = 0;
			}

			switch ( m_anchor )
			{
				case MenuAnchor.TOP_LEFT:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = currentArea.y;
					if ( m_isMaximized )
					{
						m_minimizeButtonPos.x = m_transformedArea.x + m_transformedArea.width - m_minimizeButtonPos.width - MINIMIZE_BUTTON_X_SPACING;
						m_minimizeButtonPos.y = m_transformedArea.y + MINIMIZE_BUTTON_Y_SPACING;
					}
					else
					{
						float width = ( m_transformedArea.width - m_transformedArea.x );
						m_minimizeButtonPos.x = m_transformedArea.x + width * 0.5f - m_minimizeButtonPos.width * 0.5f;
						m_minimizeButtonPos.y = m_transformedArea.height * 0.5f - m_minimizeButtonPos.height * 0.5f;
					}
				}
				break;
				case MenuAnchor.TOP_CENTER:
				{
					m_transformedArea.x = parentPosition.width * 0.5f + currentArea.x;
					m_transformedArea.y = currentArea.y;
				}
				break;
				case MenuAnchor.TOP_RIGHT:
				{
					m_transformedArea.x = parentPosition.width - currentArea.x - currentArea.width;
					m_transformedArea.y = currentArea.y;
					if ( m_isMaximized )
					{
						m_minimizeButtonPos.x = m_transformedArea.x + MINIMIZE_BUTTON_X_SPACING;
						m_minimizeButtonPos.y = m_transformedArea.y + MINIMIZE_BUTTON_Y_SPACING;
					}
					else
					{
						float width = ( parentPosition.width - m_transformedArea.x );
						m_minimizeButtonPos.x = m_transformedArea.x + width * 0.5f - m_minimizeButtonPos.width * 0.5f;
						m_minimizeButtonPos.y = m_transformedArea.height * 0.5f - m_minimizeButtonPos.height * 0.5f;
					}
				}
				break;
				case MenuAnchor.MIDDLE_LEFT:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = parentPosition.height * 0.5f + currentArea.y;
				}
				break;
				case MenuAnchor.MIDDLE_CENTER:
				{
					m_transformedArea.x = parentPosition.width * 0.5f + currentArea.x;
					m_transformedArea.y = parentPosition.height * 0.5f + currentArea.y;
				}
				break;
				case MenuAnchor.MIDDLE_RIGHT:
				{
					m_transformedArea.x = parentPosition.width - currentArea.x - currentArea.width;
					m_transformedArea.y = parentPosition.height * 0.5f + currentArea.y;
				}
				break;
				case MenuAnchor.BOTTOM_LEFT:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = parentPosition.height - currentArea.y - currentArea.height;
				}
				break;
				case MenuAnchor.BOTTOM_CENTER:
				{
					m_transformedArea.x = parentPosition.width * 0.5f + currentArea.x;
					m_transformedArea.y = parentPosition.height - currentArea.y - currentArea.height;
				}
				break;
				case MenuAnchor.BOTTOM_RIGHT:
				{
					m_transformedArea.x = parentPosition.width - currentArea.x - currentArea.width;
					m_transformedArea.y = parentPosition.height - currentArea.y - currentArea.height;
				}
				break;

				case MenuAnchor.NONE:
				{
					m_transformedArea.x = currentArea.x;
					m_transformedArea.y = currentArea.y;
				}
				break;
			}

			switch ( m_autoSize )
			{
				case MenuAutoSize.MATCH_HORIZONTAL:
				{
					m_transformedArea.width = parentPosition.width - m_transformedArea.x;
					m_transformedArea.height = currentArea.height;
				}
				break;

				case MenuAutoSize.MATCH_VERTICAL:
				{
					m_transformedArea.width = currentArea.width;
					m_transformedArea.height = parentPosition.height - m_transformedArea.y;
				}
				break;
				case MenuAutoSize.NONE:
				{
					m_transformedArea.width = currentArea.width;
					m_transformedArea.height = currentArea.height;
				}
				break;
			}
		}
		public virtual void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId )
		{
			InitDraw( parentPosition, mousePosition, mouseButtonId );
		}

		public void PostDraw()
		{
			if ( !m_isMaximized )
			{
				m_transformedArea.height = 35;
				GUI.Box( m_transformedArea, m_content, m_style );
			}

			Color colorBuffer = GUI.color;
			GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			bool guiEnabledBuffer = GUI.enabled;
			GUI.enabled = !m_lockOnMinimize;
			Rect buttonArea = m_minimizeButtonPos;

			buttonArea.x -= MinimizeCollisionAdjust;
			buttonArea.width += 2* MinimizeCollisionAdjust;

			buttonArea.y -= MinimizeCollisionAdjust;
			buttonArea.height += 2* MinimizeCollisionAdjust;

			GUI.Box( m_minimizeButtonPos, string.Empty, UIUtils.CustomStyle( m_isMaximized ? CustomStyle.MinimizeButton : CustomStyle.MaximizeButton ) );
			if ( GUI.Button( buttonArea, string.Empty , m_empty ) )
			{
				m_isMaximized = !m_isMaximized;
			}

			GUI.enabled = guiEnabledBuffer;
			GUI.color = colorBuffer;
		}

		virtual public void Destroy()
		{
			m_empty = null;
		}

		public float InitialX
		{
			get { return m_maximizedArea.x; }
			set { m_maximizedArea.x = value; }
		}

		public float Width
		{
			get { return m_maximizedArea.width; }
			set { m_maximizedArea.width = value; }
		}

		public float RealWidth
		{
			get { return m_realWidth; }
		}
		public float Height
		{
			get { return m_maximizedArea.height; }
			set { m_maximizedArea.height = value; }
		}

		public Rect Size
		{
			get { return m_maximizedArea; }
		}

		public virtual bool IsInside( Vector2 position )
		{
			if ( !m_isActive )
				return false;

			return m_transformedArea.Contains( position );
		}

		public bool IsMaximized
		{
			get { return m_isMaximized; }
			set { m_isMaximized = value; }
		}

		public Rect TransformedArea
		{
			get { return m_transformedArea; }
		}


		public bool LockOnMinimize
		{
			set
			{
				if ( m_lockOnMinimize == value )
					return;

				m_lockOnMinimize = value;
				if ( value )
				{
					m_preLockState = m_isMaximized;
					m_isMaximized = false;
				}
				else
				{
					m_isMaximized = m_preLockState;
				}
			}
		}
	}
}
