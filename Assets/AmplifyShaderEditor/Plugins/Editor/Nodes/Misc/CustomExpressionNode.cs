using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Custom Expression", "Misc", "Creates Custom Expression" )]
	public sealed class CustomExpressionNode : ParentNode
	{
		private const string CodeTitleStr = "Code";
		private const string OutputTypeStr = "Output Type";
		private const string InputsStr = "Inputs";
		private const string InputNameStr = "Name";
		private const string InputTypeStr = "Type";
		private const string InputValueStr = "Value";
		private readonly string[] AvailableWireTypesStr =   {   "Int",
																"Float",
																"Float2",
																"Float3",
																"Float4",
																"Color",
																"Float3x3",
																"Float4x4"};

		private readonly WirePortDataType[] AvailableWireTypes = {  WirePortDataType.INT,
																	WirePortDataType.FLOAT,
																	WirePortDataType.FLOAT2,
																	WirePortDataType.FLOAT3,
																	WirePortDataType.FLOAT4,
																	WirePortDataType.COLOR,
																	WirePortDataType.FLOAT3x3,
																	WirePortDataType.FLOAT4x4};

		private readonly Dictionary<WirePortDataType, int> WireToIdx = new Dictionary<WirePortDataType, int> {  { WirePortDataType.INT,     0},
																												{ WirePortDataType.FLOAT,   1},
																												{ WirePortDataType.FLOAT2,  2},
																												{ WirePortDataType.FLOAT3,  3},
																												{ WirePortDataType.FLOAT4,  4},
																												{ WirePortDataType.COLOR,   5},
																												{ WirePortDataType.FLOAT3x3,6},
																												{ WirePortDataType.FLOAT4x4,7}};
		[SerializeField]
		private List<bool> m_foldoutValuesFlags = new List<bool>();
		[SerializeField]
		private List<string> m_foldoutValuesLabels = new List<string>();

		[SerializeField]
		private string m_code =" ";
		[SerializeField]
		private int m_outputTypeIdx = 1;
		[SerializeField]
		private bool m_visibleInputsFoldout = true;

		private GUIStyle m_addItemStyle;
		private GUIStyle m_removeItemStyle;
		private GUIStyle m_smallAddItemStyle;
		private GUIStyle m_smallRemoveItemStyle;

		private int m_markedToDelete = -1;
		private const float ButtonLayoutWidth = 15;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "In0" );
			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[0]" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 80;
		}

		public override void DrawProperties()
		{
			if ( m_addItemStyle == null )
				m_addItemStyle = UIUtils.PlusStyle;

			if ( m_removeItemStyle == null )
				m_removeItemStyle = UIUtils.MinusStyle;

			if ( m_smallAddItemStyle == null )
				m_smallAddItemStyle = UIUtils.PlusStyle;

			if ( m_smallRemoveItemStyle == null )
				m_smallRemoveItemStyle = UIUtils.MinusStyle;

			base.DrawProperties();
			DrawPrecisionProperty();
			
			EditorGUILayout.LabelField( CodeTitleStr );
			m_code = EditorGUILayout.TextArea( m_code, UIUtils.MainSkin.textArea );
			
			EditorGUI.BeginChangeCheck();
			m_outputTypeIdx = EditorGUILayout.Popup( OutputTypeStr, m_outputTypeIdx, AvailableWireTypesStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			}
			
			EditorGUILayout.BeginHorizontal();
			{
				m_visibleInputsFoldout = EditorGUILayout.Foldout( m_visibleInputsFoldout, InputsStr );

				// Add new port
				if ( GUILayout.Button( string.Empty, m_addItemStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
				{
					AddPortAt( m_inputPorts.Count );
				}

				//Remove port
				if ( GUILayout.Button( string.Empty, m_removeItemStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
				{
					RemovePortAt( m_inputPorts.Count - 1 );
				}
			}
			EditorGUILayout.EndHorizontal();

			if ( m_visibleInputsFoldout )
			{
				EditorGUI.indentLevel += 1;
				int count = m_inputPorts.Count;
				for ( int i = 0; i < count; i++ )
				{
					
					m_foldoutValuesFlags[ i ] = EditorGUILayout.Foldout( m_foldoutValuesFlags[ i ], m_foldoutValuesLabels[ i ] );


					if ( m_foldoutValuesFlags[ i ] )
					{
						EditorGUI.indentLevel += 1;
						EditorGUI.BeginChangeCheck();
						m_inputPorts[ i ].Name = EditorGUILayout.TextField( InputNameStr, m_inputPorts[ i ].Name );
						if ( EditorGUI.EndChangeCheck() )
						{
							m_inputPorts[ i ].Name = UIUtils.RemoveInvalidCharacters( m_inputPorts[ i ].Name );
							if ( string.IsNullOrEmpty( m_inputPorts[ i ].Name ) )
							{
								m_inputPorts[ i ].Name = "In" + i;
							}
						}

						int typeIdx = WireToIdx[ m_inputPorts[ i ].DataType ];
						EditorGUI.BeginChangeCheck();
						typeIdx = EditorGUILayout.Popup( InputTypeStr, typeIdx, AvailableWireTypesStr );
						if ( EditorGUI.EndChangeCheck() )
						{
							m_inputPorts[ i ].ChangeType( AvailableWireTypes[ typeIdx ], false );
						}

						if ( !m_inputPorts[ i ].IsConnected )
						{
							m_inputPorts[ i ].ShowInternalData( true, InputValueStr );
						}

						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label( " " );
							// Add new port
							if ( GUILayout.Button( string.Empty, m_smallAddItemStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
							{
								AddPortAt( i );
							}

							//Remove port
							if ( GUILayout.Button( string.Empty, m_smallRemoveItemStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
							{
								m_markedToDelete = i;
							}
						}
						EditorGUILayout.EndHorizontal();

						EditorGUI.indentLevel -= 1;
					}
				}
				EditorGUI.indentLevel -= 1;
			}

			if ( m_markedToDelete > -1 )
			{
				DeleteInputPort( m_markedToDelete );
				m_markedToDelete = -1;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_addItemStyle = null;
			m_removeItemStyle = null;
			m_smallAddItemStyle = null;
			m_smallRemoveItemStyle = null;
		}

		void AddPortAt( int idx )
		{
			AddInputPortAt( idx, WirePortDataType.FLOAT, false, "In" + idx );
			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[" + idx + "]" );
		}

		void RemovePortAt( int idx )
		{
			if ( m_inputPorts.Count > 0 )
			{
				DeleteInputPort( idx );
				m_foldoutValuesFlags.RemoveAt( idx );
				m_foldoutValuesLabels.RemoveAt( idx );
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			int count = m_inputPorts.Count;
			if ( m_inputPorts.Count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					if ( m_inputPorts[ i ].IsConnected )
					{
						string result = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ i ].DataType, true, 0, true );
						dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, m_inputPorts[ i ].Name, result );
					}
					else
					{
						dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, m_inputPorts[ i ].Name, m_inputPorts[ i ].WrappedInternalData );
					}
				}
			}
			return m_code;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			// This node is, by default, created with one input port 
			base.ReadFromString( ref nodeParams );
			m_code = GetCurrentParam( ref nodeParams );
			m_outputTypeIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			int count = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( count == 0 )
			{
				DeleteInputPort( 0 );
				m_foldoutValuesLabels.Clear();
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					m_foldoutValuesFlags.Add( Convert.ToBoolean( GetCurrentParam( ref nodeParams ) ) );
					string name = GetCurrentParam( ref nodeParams );
					WirePortDataType type = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
					string internalData = GetCurrentParam( ref nodeParams );
					if ( i == 0 )
					{
						m_inputPorts[ 0 ].ChangeProperties( name, type, false );
					}
					else
					{
						m_foldoutValuesLabels.Add( "[" + i + "]" );
						AddInputPort( type, false, name );
					}
					m_inputPorts[ i ].InternalData = internalData;
				}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_code );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeIdx );
			int count = m_inputPorts.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, count );
			for ( int i = 0; i < count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_foldoutValuesFlags[ i ] );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].Name );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].DataType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].InternalData );
			}
		}
	}
}
