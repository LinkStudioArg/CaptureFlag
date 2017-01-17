// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class PortLegendInfo : EditorWindow
	{
		private const string m_lockedStr = "Locked Port";
		protected GUIStyle m_portStyle;
		protected GUIStyle m_labelStyle;
		private GUIContent m_content = new GUIContent( "Port Color", "Shows which data type is specified by each color" );
		private bool m_init = true;

		public void Init()
		{
			m_init = false;
			wantsMouseMove = false;
			m_portStyle = UIUtils.CurrentWindow.CustomStylesInstance.Button;
			m_labelStyle = new GUIStyle( UIUtils.CurrentWindow.CustomStylesInstance.Label );
			m_labelStyle.fontSize = 13;
			minSize = maxSize = new Vector2( 170, 240 );
			titleContent = m_content;
		}

		void OnGUI()
		{
			if ( m_init )
			{
				Init();
			}

			Color originalColor = GUI.color;
			EditorGUILayout.BeginVertical();

			GUILayout.Space( 5 );
			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.OBJECT ,false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.OBJECT ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
			
			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.FLOAT, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.FLOAT ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.FLOAT2, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.FLOAT2 ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.FLOAT3, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.FLOAT3 ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.FLOAT4, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.FLOAT4 ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.FLOAT3x3, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.FLOAT3x3 ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.FLOAT4x4, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.FLOAT4x4 ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.COLOR, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.COLOR ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = UIUtils.GetColorForDataType( WirePortDataType.INT, false );
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( UIUtils.GetNameForDataType( WirePortDataType.INT ), m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 5 );

			EditorGUILayout.BeginHorizontal();
			GUI.color = Constants.LockedPortColor;
			GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
			GUI.color = Color.white;
			EditorGUILayout.LabelField( m_lockedStr, m_labelStyle );
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
			GUI.color = originalColor;
		}
	}
}
