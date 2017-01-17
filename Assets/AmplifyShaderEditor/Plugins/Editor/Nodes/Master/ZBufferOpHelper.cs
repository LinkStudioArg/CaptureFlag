using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum ZWriteMode
	{
		On,
		Off
	}

	public enum ZTestMode
	{
		Less,
		Greater,
		LEqual,
		GEqual,
		Equal,
		NotEqual,
		Always
	}

	[Serializable]
	class ZBufferOpHelper
	{
		private const string DepthParametersStr = "Depth";
		private const string ZWriteModeStr = "ZWrite Mode";
		private const string ZTestModeStr = "ZTest Mode";
		private const string OffsetStr = "Offset";
		private const string OffsetFactorStr = "Factor";
		private const string OffsetUnitsStr = "Units";

		private readonly string[] ZTestModeLabels = {   "<Default>",
														"Less",
														"Greater",
														"Less or Equal",
														"Greater or Equal",
														"Equal",
														"Not Equal",
														"Always" };

		private readonly string[] ZTestModeValues = {   "<Default>",
														"Less",
														"Greater",
														"LEqual",
														"GEqual",
														"Equal",
														"NotEqual",
														"Always"};

		private readonly string[] ZWriteModeValues = {  "<Default>",
														"On",
														"Off"};
		[SerializeField]
		private int m_zTestMode = 0;

		[SerializeField]
		private int m_zWriteMode = 0;

		[SerializeField]
		private float m_offsetFactor;

		[SerializeField]
		private float m_offsetUnits;

		[SerializeField]
		private bool m_offsetEnabled;

		[SerializeField]
		private bool m_foldout = false;

		private GUIStyle m_foldoutStyle;

		public string CreateDepthInfo()
		{
			string result = string.Empty;
			if ( m_zWriteMode != 0 )
			{
				MasterNode.AddRenderState( ref result, "ZWrite", ZWriteModeValues[ m_zWriteMode ] );
			}

			if ( m_zTestMode != 0 )
			{
				MasterNode.AddRenderState( ref result, "ZTest", ZTestModeValues[ m_zTestMode ] );
			}

			if ( m_offsetEnabled )
			{
				MasterNode.AddRenderState( ref result, "Offset ", m_offsetFactor + " , " + m_offsetUnits );
			}

			return result;
		}

		public void Draw()
		{
			if ( m_foldoutStyle == null )
			{
				m_foldoutStyle = GUI.skin.GetStyle( "foldout" );
			}

			EditorGUILayout.Separator();

			m_foldout = GUILayout.Toggle( m_foldout, DepthParametersStr, m_foldoutStyle );
			if ( m_foldout )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.Separator();
				m_zWriteMode = EditorGUILayout.Popup( ZWriteModeStr, m_zWriteMode, ZWriteModeValues );
				EditorGUILayout.Separator();
				m_zTestMode = EditorGUILayout.Popup( ZTestModeStr, m_zTestMode, ZTestModeLabels );
				EditorGUILayout.Separator();
				m_offsetEnabled = EditorGUILayout.Toggle( OffsetStr, m_offsetEnabled );
				EditorGUILayout.Separator();
				if ( m_offsetEnabled )
				{
					EditorGUI.indentLevel++;
					m_offsetFactor = EditorGUILayout.FloatField( OffsetFactorStr, m_offsetFactor );
					EditorGUILayout.Separator();
					m_offsetUnits = EditorGUILayout.FloatField( OffsetUnitsStr, m_offsetUnits );
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			if ( UIUtils.CurrentShaderVersion() < 2502 )
			{
				string zWriteMode = nodeParams[ index++ ];
				m_zWriteMode = zWriteMode.Equals( "Off" ) ? 2 : 0;

				string zTestMode = nodeParams[ index++ ];
				for ( int i = 0; i < ZTestModeValues.Length; i++ )
				{
					if ( zTestMode.Equals( ZTestModeValues[ i ] ) )
					{
						m_zTestMode = i;
						break;
					}
				}
			}
			else
			{
				m_zWriteMode = Convert.ToInt32( nodeParams[ index++ ] );
				m_zTestMode = Convert.ToInt32( nodeParams[ index++ ] );
				m_offsetEnabled = Convert.ToBoolean( nodeParams[ index++ ] );
				m_offsetFactor = Convert.ToSingle( nodeParams[ index++ ] );
				m_offsetUnits = Convert.ToSingle( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zWriteMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zTestMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetEnabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetFactor );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetUnits );
		}
		public bool IsActive { get { return m_zTestMode != 0 || m_zWriteMode != 0 || m_offsetEnabled; } }
	}
}
