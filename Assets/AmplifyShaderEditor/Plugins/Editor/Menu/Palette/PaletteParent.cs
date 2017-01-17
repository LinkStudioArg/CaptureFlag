// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public class PaletteFilterData
	{
		public bool Visible;
		public List<ContextMenuItem> Contents;
		public PaletteFilterData( bool visible )
		{
			Visible = visible;
			Contents = new List<ContextMenuItem>();
		}
	}

	public class PaletteParent : MenuParent
	{
		private const float ItemSize = 18;
		public delegate void OnPaletteNodeCreate( Type type, string name );
		public event OnPaletteNodeCreate OnPaletteNodeCreateEvt;

		private string m_searchFilterStr = "Search";
		protected string m_searchFilterControl = "SHADERNAMETEXTFIELDCONTROLNAME";
		protected bool m_focusOnSearch = false;
		protected bool m_defaultCategoryVisible = false;

		protected List<ContextMenuItem> m_allItems;
		protected List<ContextMenuItem> m_currentItems;
		protected Dictionary<string, PaletteFilterData> m_currentCategories;
		private bool m_forceUpdate = true;


		protected string m_searchFilter;
		private Vector2 m_currentScrollPos;
		private float m_searchLabelSize = -1;
		private GUIStyle m_buttonStyle;
		private GUIStyle m_foldoutStyle;


		protected int m_validButtonId = 0;
		protected int m_initialSeparatorAmount = 1;

		private Vector2 _currScrollBarDims = new Vector2( 1, 1 );


		public PaletteParent( List<ContextMenuItem> items, float x, float y, float width, float height, string name, MenuAnchor anchor = MenuAnchor.NONE, MenuAutoSize autoSize = MenuAutoSize.NONE ) : base( x, y, width, height, name, anchor, autoSize )
		{
			m_searchFilter = string.Empty;
			m_currentScrollPos = Vector2.zero;

			m_currentCategories = new Dictionary<string, PaletteFilterData>();
			m_allItems = new List<ContextMenuItem>();
			m_currentItems = new List<ContextMenuItem>();
			for ( int i = 0; i < items.Count; i++ )
			{
				m_allItems.Add( items[ i ] );
			}
		}

		public virtual void OnEnterPressed() { }
		public virtual void OnEscapePressed() { }

		public void SortElements()
		{
			m_allItems.Sort( ( x, y ) => x.Category.CompareTo( y.Category ) );
		}

		public void FireNodeCreateEvent( Type type, string name )
		{
			OnPaletteNodeCreateEvt( type, name );
		}

		public override void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId )
		{
			base.Draw( parentPosition, mousePosition, mouseButtonId );
			if ( m_searchLabelSize < 0 )
			{
				m_searchLabelSize = GUI.skin.label.CalcSize( new GUIContent( m_searchFilterStr ) ).x;
			}

			if ( m_foldoutStyle == null )
			{
				m_foldoutStyle = new GUIStyle( GUI.skin.GetStyle( "foldout" ) );
				m_foldoutStyle.fontStyle = FontStyle.Bold;
			}

			if ( m_buttonStyle == null )
			{
				m_buttonStyle = UIUtils.CurrentWindow.CustomStylesInstance.Label;
			}

			bool isMouseInside = IsInside( mousePosition );
			GUILayout.BeginArea( m_transformedArea, m_content, m_style );
			{

				for ( int i = 0; i < m_initialSeparatorAmount; i++ )
				{
					EditorGUILayout.Separator();
				}

				if ( Event.current.type == EventType.keyDown )
				{
					if ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter )
						OnEnterPressed();

					if ( Event.current.keyCode == KeyCode.Escape )
						OnEscapePressed();
				}

				float width = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = m_searchLabelSize;
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName( m_searchFilterControl );
				m_searchFilter = EditorGUILayout.TextField( m_searchFilterStr, m_searchFilter );
				if ( EditorGUI.EndChangeCheck() )
					m_forceUpdate = true;

				EditorGUIUtility.labelWidth = width;

				_currScrollBarDims.x = m_transformedArea.width;
				_currScrollBarDims.y = m_transformedArea.height;
				m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
				{
					if ( m_forceUpdate )
					{
						m_forceUpdate = false;

						m_currentItems.Clear();
						m_currentCategories.Clear();

						if ( m_searchFilter.Length == 0 )
						{
							for ( int i = 0; i < m_allItems.Count; i++ )
							{
								m_currentItems.Add( m_allItems[ i ] );
								if ( !m_currentCategories.ContainsKey( m_allItems[ i ].Category ) )
								{
									m_currentCategories.Add( m_allItems[ i ].Category, new PaletteFilterData( m_defaultCategoryVisible ) );
								}
								m_currentCategories[ m_allItems[ i ].Category ].Contents.Add( m_allItems[ i ] );
							}
						}
						else
						{
							for ( int i = 0; i < m_allItems.Count; i++ )
							{
								if ( m_allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||
										m_allItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0
									)
								{
									m_currentItems.Add( m_allItems[ i ] );
									if ( !m_currentCategories.ContainsKey( m_allItems[ i ].Category ) )
									{
										m_currentCategories.Add( m_allItems[ i ].Category, new PaletteFilterData( m_defaultCategoryVisible ) );
									}
									m_currentCategories[ m_allItems[ i ].Category ].Contents.Add( m_allItems[ i ] );
								}
							}
						}
						var categoryEnumerator = m_currentCategories.GetEnumerator();
						while ( categoryEnumerator.MoveNext() )
						{
							categoryEnumerator.Current.Value.Contents.Sort( ( x, y ) => x.Name.CompareTo( y.Name ) );
						}
					}
					float currPos = 0;
					var enumerator = m_currentCategories.GetEnumerator();
					while ( enumerator.MoveNext() )
					{
						var current = enumerator.Current;
						current.Value.Visible = GUILayout.Toggle( current.Value.Visible, current.Key, m_foldoutStyle );

						currPos += ItemSize;
						if ( m_searchFilter.Length > 0 || current.Value.Visible )
						{
							for ( int i = 0; i < current.Value.Contents.Count; i++ )
							{
								if ( !IsItemVisible( currPos ) )
								{
									// Invisible
									GUILayout.Space( ItemSize );
								}
								else
								{
									// Visible
									EditorGUILayout.BeginHorizontal();
									GUILayout.Space( 16 );
									if ( isMouseInside )
									{
										if ( CheckButton( current.Value.Contents[ i ].ItemUIContent, m_buttonStyle, mouseButtonId ) )
										{
											int controlID = GUIUtility.GetControlID( FocusType.Passive );
											GUIUtility.hotControl = controlID;
											OnPaletteNodeCreateEvt( current.Value.Contents[ i ].NodeType, current.Value.Contents[ i ].Name );
										}
									}
									else
									{
										GUILayout.Label( current.Value.Contents[ i ].ItemUIContent, m_buttonStyle );
									}
									EditorGUILayout.EndHorizontal();
								}
								currPos += ItemSize;
							}
						}
					}
				}
				EditorGUILayout.EndScrollView();
			}
			GUILayout.EndArea();

			//CheckDumpTest();

			if ( m_focusOnSearch )
			{
				m_focusOnSearch = false;
				EditorGUI.FocusTextInControl( m_searchFilterControl );
			}
		}

		void CheckDumpTest()
		{
			if ( Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.S )
			{
				string nodesDump = string.Empty;
				var enumerator = m_currentCategories.GetEnumerator();
				while ( enumerator.MoveNext() )
				{
					var current = enumerator.Current;
					for ( int i = 0; i < current.Value.Contents.Count; i++ )
					{
						nodesDump += string.Format( Constants.NodesDumpFormat, current.Key, current.Value.Contents[ i ].ItemUIContent.text, current.Value.Contents[ i ].ItemUIContent.tooltip );
					}
				}
				Debug.Log( nodesDump );
			}
		}
		public virtual bool CheckButton( GUIContent content, GUIStyle style, int buttonId )
		{
			if ( buttonId != m_validButtonId )
			{
				GUILayout.Label( content, style );
				return false;
			}

			return GUILayout.RepeatButton( content, style );
		}

		private bool IsItemVisible( float currPos )
		{
			if ( ( currPos < m_currentScrollPos.y && ( currPos + ItemSize ) < m_currentScrollPos.y ) ||
									( currPos > ( m_currentScrollPos.y + _currScrollBarDims.y ) &&
									( currPos + ItemSize ) > ( m_currentScrollPos.y + _currScrollBarDims.y ) ) )
			{
				return false;
			}
			return true;
		}

		public override void Destroy()
		{
			base.Destroy();

			m_allItems.Clear();
			m_allItems = null;

			m_currentItems.Clear();
			m_currentItems = null;

			m_currentCategories.Clear();
			m_currentCategories = null;

			OnPaletteNodeCreateEvt = null;
			m_buttonStyle = null;
			m_foldoutStyle = null;
		}
	}
}
