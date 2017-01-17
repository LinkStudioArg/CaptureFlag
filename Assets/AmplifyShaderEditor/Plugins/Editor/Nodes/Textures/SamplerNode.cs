// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum TexReferenceType
	{
		Object,
		Instance
	}

	public enum MipType
	{
		Auto,
		MipLevel,
		MipBias,
		Derivative
	}

	[Serializable]
	[NodeAttributes( "Texture Sample", "Textures", "Textures", KeyCode.T, true, typeof( Texture ), typeof( Texture2D ), typeof( Texture3D ), typeof( Cubemap ) )]
	public sealed class SamplerNode : TexturePropertyNode
	{
		private const string MipModeStr = "Mip Mode";

		private const string DefaultTextureUseSematicsStr = "Use Semantics";
		private const string DefaultTextureIsNormalMapsStr = "Is Normal Map";

		private const string NormalScaleStr = "Normal Scale";

		private float InstanceIconWidth = 19;
		private float InstanceIconHeight = 19;

		private readonly Color ReferenceHeaderColor = new Color( 2.67f, 1.0f, 0.5f, 1.0f );

		[SerializeField]
		private int m_textureCoordSet = 0;

		[SerializeField]
		private string m_normalMapUnpackMode;

		[SerializeField]
		private bool m_autoUnpackNormals = false;

		[SerializeField]
		private bool m_useSemantics;

		[SerializeField]
		private string m_samplerType;

		[SerializeField]
		private MipType m_mipMode = MipType.Auto;

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		private SamplerNode m_referenceSampler = null;

		[SerializeField]
		private GUIStyle m_referenceStyle = null;

		[SerializeField]
		private GUIStyle m_referenceIconStyle = null;

		[SerializeField]
		private GUIContent m_referenceContent = null;

		[SerializeField]
		private float m_referenceWidth = -1;

		private bool m_forceSamplerUpdate = false;

		private InputPort m_texPort;
		private InputPort m_uvPort;
		private InputPort m_lodPort;
		private InputPort m_ddxPort;
		private InputPort m_ddyPort;
		private InputPort m_normalPort;

		private OutputPort m_colorPort;

		public SamplerNode() : base() { }
		public SamplerNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_defaultTextureValue = TexturePropertyValues.white;
			m_extraSize.Set( 10, 0 );
			AddInputPort( WirePortDataType.SAMPLER2D, false, "Tex" );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Level" );
			AddInputPort( WirePortDataType.FLOAT2, false, "DDX" );
			AddInputPort( WirePortDataType.FLOAT2, false, "DDY" );
			AddInputPort( WirePortDataType.FLOAT, false, "Normal Scale" );

			m_texPort = m_inputPorts[ 0 ];
			m_uvPort = m_inputPorts[ 1 ];
			m_lodPort = m_inputPorts[ 2 ];
			m_ddxPort = m_inputPorts[ 3 ];
			m_ddyPort = m_inputPorts[ 4 ];
			m_normalPort = m_inputPorts[ 5 ];

			m_lodPort.Visible = false;
			m_lodPort.FloatInternalData = 1.0f;
			m_ddxPort.Visible = false;
			m_ddyPort.Visible = false;
			m_normalPort.Visible = m_autoUnpackNormals;
			m_normalPort.FloatInternalData = 1.0f;

			//Remove output port (sampler)
			m_outputPorts.RemoveAt( 0 );

			AddOutputColorPorts( WirePortDataType.COLOR, "RGBA" );
			m_colorPort = m_outputPorts[ 0 ];
			m_currentParameterType = PropertyType.Property;
			m_useCustomPrefix = true;
			m_customPrefix = "Texture Sample ";
			m_referenceContent = new GUIContent( string.Empty );
			m_freeType = false;
			m_useSemantics = true;
			m_drawPicker = false;
			ConfigTextureData( TextureType.Texture2D );

		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if ( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.RegisterSamplerNode( this );
				UIUtils.RegisterPropertyNode( this );
			}
			m_textureProperty = this;
		}

		public void ConfigSampler()
		{
			switch ( m_currentType )
			{
				case TextureType.Texture1D:
				m_samplerType = "tex1D";
				break;
				case TextureType.Texture2D:
				m_samplerType = "tex2D";
				break;
				case TextureType.Texture3D:
				m_samplerType = "tex3D";
				break;
				case TextureType.Cube:
				m_samplerType = "texCUBE";
				break;
			}
		}

		public override void DrawSubProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			m_defaultValue = ( Texture ) EditorGUILayout.ObjectField( Constants.DefaultValueLabel, m_defaultValue, m_textureType, false );
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				ConfigureInputPorts();
				ConfigureOutputPorts();
				ResizeNodeToPreview();
			}
		}

		public override void DrawMaterialProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			m_materialValue = ( Texture ) EditorGUILayout.ObjectField( Constants.MaterialValueLabel, m_materialValue, m_textureType, false );
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				ConfigureInputPorts();
				ConfigureOutputPorts();
				ResizeNodeToPreview();
			}
		}

		void ShowDefaults()
		{
			m_textureCoordSet = EditorGUILayout.IntPopup( Constants.AvailableUVSetsLabel, m_textureCoordSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );
			m_defaultTextureValue = ( TexturePropertyValues ) EditorGUILayout.EnumPopup( DefaultTextureStr, m_defaultTextureValue );
			AutoCastType newAutoCast = ( AutoCastType ) EditorGUILayout.EnumPopup( AutoCastModeStr, m_autocastMode );
			if ( newAutoCast != m_autocastMode )
			{
				m_autocastMode = newAutoCast;
				if ( m_autocastMode != AutoCastType.Auto )
				{
					ConfigTextureData( m_currentType );
					ConfigureInputPorts();
					ConfigureOutputPorts();
					ResizeNodeToPreview();
				}
			}
		}

		public override void AdditionalCheck()
		{
			m_autoUnpackNormals = m_isNormalMap;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );

			if ( portId == m_texPort.PortId )
			{
				m_textureProperty = m_texPort.GetOutputNode( 0 ) as TexturePropertyNode;

				if ( m_textureProperty == null )
				{
					m_textureProperty = this;
				}
				else
				{
					AutoUnpackNormals = m_textureProperty.IsNormalMap;

					if ( m_textureProperty is VirtualTexturePropertyNode )
					{
						AutoUnpackNormals = ( m_textureProperty as VirtualTexturePropertyNode ).Channel == VirtualChannel.Normal;
					}

					UIUtils.UnregisterPropertyNode( this );
					UIUtils.UnregisterTexturePropertyNode( this );
				}

				ConfigureInputPorts();
				ConfigureOutputPorts();
				ResizeNodeToPreview();
			}
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );

			if ( portId == m_texPort.PortId )
			{
				m_textureProperty = this;

				if ( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterPropertyNode( this );
					UIUtils.RegisterTexturePropertyNode( this );
				}

				ConfigureOutputPorts();
				ResizeNodeToPreview();
			}
		}

		public override void ConfigureInputPorts()
		{
			m_normalPort.Visible = AutoUnpackNormals;

			switch ( m_mipMode )
			{
				case MipType.Auto:
				m_lodPort.Visible = false;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.MipLevel:
				m_lodPort.Name = "Level";
				m_lodPort.Visible = true;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.MipBias:
				m_lodPort.Name = "Bias";
				m_lodPort.Visible = true;
				m_ddxPort.Visible = false;
				m_ddyPort.Visible = false;
				break;
				case MipType.Derivative:
				m_lodPort.Visible = false;
				m_ddxPort.Visible = true;
				m_ddyPort.Visible = true;
				break;
			}

			switch ( m_currentType )
			{
				case TextureType.Texture2D:
				m_uvPort.ChangeType( WirePortDataType.FLOAT2, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT2, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT2, false );
				break;
				case TextureType.Texture3D:
				case TextureType.Cube:
				m_uvPort.ChangeType( WirePortDataType.FLOAT3, false );
				m_ddxPort.ChangeType( WirePortDataType.FLOAT3, false );
				m_ddyPort.ChangeType( WirePortDataType.FLOAT3, false );
				break;
			}

			m_sizeIsDirty = true;
		}

		public override void ConfigureOutputPorts()
		{
			m_outputPorts[ m_colorPort.PortId + 4 ].Visible = !AutoUnpackNormals;

			/*VirtualTexturePropertyNode vtex = ( TextureProperty as VirtualTexturePropertyNode );
			if ( vtex != null )
			{
				switch ( vtex.Channel )
				{
					default:
					case VirtualChannel.Albedo:
					case VirtualChannel.Base:
					case VirtualChannel.Specular:
					case VirtualChannel.SpecMet:
					case VirtualChannel.Material:
					case VirtualChannel.Normal:
						m_colorPort.Visible = true;
						m_outputPorts[ m_colorPort.PortId + 1 ].Visible = true;
						m_outputPorts[ m_colorPort.PortId + 2 ].Visible = true;
						m_outputPorts[ m_colorPort.PortId + 3 ].Visible = true;
						break;
					case VirtualChannel.Occlusion:
						m_autoUnpackNormals = false;
						m_colorPort.Visible = false;
						m_outputPorts[ m_colorPort.PortId + 1 ].Visible = true;
						m_outputPorts[ m_colorPort.PortId + 2 ].Visible = false;
						m_outputPorts[ m_colorPort.PortId + 3 ].Visible = false;
						m_outputPorts[ m_colorPort.PortId + 4 ].Visible = false;
						break;
					case VirtualChannel.Displacement:
					case VirtualChannel.Height:
						m_autoUnpackNormals = false;
						m_colorPort.Visible = false;
						m_outputPorts[ m_colorPort.PortId + 1 ].Visible = false;
						m_outputPorts[ m_colorPort.PortId + 2 ].Visible = false;
						m_outputPorts[ m_colorPort.PortId + 3 ].Visible = true;
						m_outputPorts[ m_colorPort.PortId + 4 ].Visible = false;
						break;
				}
			}
			else
			{
				m_colorPort.Visible = true;
				m_outputPorts[ m_colorPort.PortId + 1 ].Visible = true;
				m_outputPorts[ m_colorPort.PortId + 2 ].Visible = true;
				m_outputPorts[ m_colorPort.PortId + 3 ].Visible = true;
			}*/

			if ( !AutoUnpackNormals )
			{
				m_colorPort.ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
				m_outputPorts[ m_colorPort.PortId + 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );

			}
			else
			{
				m_colorPort.ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ m_colorPort.PortId + 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );
			}

			m_sizeIsDirty = true;
		}

		public override void ResizeNodeToPreview()
		{
			if ( AutoUnpackNormals )
			{
				PreviewSizeX = 88;
				PreviewSizeY = 88;
			}
			else
			{
				PreviewSizeX = 110;
				PreviewSizeY = 110;
			}

			m_insideSize.Set( PreviewSizeX, PreviewSizeY + 5 );

			m_sizeIsDirty = true;
		}

		public override void OnObjectDropped( UnityEngine.Object obj )
		{
			base.OnObjectDropped( obj );
			ConfigFromObject( obj );
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			ConfigFromObject( obj );
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		public void DrawSamplerOptions()
		{
			MipType newMipMode = ( MipType ) EditorGUILayout.EnumPopup( MipModeStr, m_mipMode );
			if ( newMipMode != m_mipMode )
			{
				m_mipMode = newMipMode;
				ConfigureInputPorts();
				ConfigureOutputPorts();
				ResizeNodeToPreview();
			}


			EditorGUI.BeginChangeCheck();
			bool autoUnpackNormals = EditorGUILayout.Toggle( AutoUnpackNormalsStr, m_autoUnpackNormals );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_autoUnpackNormals != autoUnpackNormals )
				{
					AutoUnpackNormals = autoUnpackNormals;

					ConfigureInputPorts();
					ConfigureOutputPorts();
					ResizeNodeToPreview();
				}
			}

			if ( m_autoUnpackNormals && !m_normalPort.IsConnected )
			{
				m_normalPort.FloatInternalData = EditorGUILayout.FloatField( NormalScaleStr, m_normalPort.FloatInternalData );
			}
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_referenceType = ( TexReferenceType ) EditorGUILayout.EnumPopup( Constants.ReferenceTypeStr, m_referenceType );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_sizeIsDirty = true;
				if ( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterSamplerNode( this );
					UIUtils.RegisterPropertyNode( this );
					if ( !m_texPort.IsConnected )
						UIUtils.RegisterTexturePropertyNode( this );

					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					m_referenceArrayId = -1;
					m_referenceNodeId = -1;
					m_referenceSampler = null;
				}
				else
				{
					UIUtils.UnregisterSamplerNode( this );
					UIUtils.UnregisterPropertyNode( this );
					if ( !m_texPort.IsConnected )
						UIUtils.UnregisterTexturePropertyNode( this );
				}
				UpdateHeaderColor();
			}

			if ( m_referenceType == TexReferenceType.Object )
			{
				EditorGUI.BeginChangeCheck();
				base.DrawProperties();
				if ( EditorGUI.EndChangeCheck() )
				{
					OnPropertyNameChanged();
				}
			}
			else
			{
				string[] arr = UIUtils.SamplerNodeArr();
				bool guiEnabledBuffer = GUI.enabled;
				if ( arr != null && arr.Length > 0 )
				{
					GUI.enabled = true;
				}
				else
				{
					m_referenceArrayId = -1;
					GUI.enabled = false;
				}

				m_referenceArrayId = EditorGUILayout.Popup( Constants.AvailableReferenceStr, m_referenceArrayId, arr );
				GUI.enabled = guiEnabledBuffer;

				DrawSamplerOptions();
			}
		}
		public override void OnPropertyNameChanged()
		{
			base.OnPropertyNameChanged();
			UIUtils.UpdateSamplerDataNode( m_uniqueId, PropertyInspectorName );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			EditorGUI.BeginChangeCheck();
			if ( m_forceSamplerUpdate )
			{
				m_forceSamplerUpdate = false;
				if ( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceSampler = UIUtils.GetNode( m_referenceNodeId ) as SamplerNode;
					//m_referenceArrayId = UIUtils.GetTexturePropertyNodeRegisterId( m_referenceNodeId );
					m_referenceArrayId = UIUtils.GetSamplerNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceSampler = UIUtils.GetSamplerNode( m_referenceArrayId );
					if ( m_referenceSampler != null )
					{
						m_referenceNodeId = m_referenceSampler.UniqueId;
					}
				}
			}

			if ( EditorGUI.EndChangeCheck() )
			{
				OnPropertyNameChanged();
			}

			if ( m_isVisible )
			{
				if ( SoftValidReference )
				{
					m_drawPicker = false;

					DrawTexturePropertyPreview( drawInfo, true );
				}
				else
				if ( m_texPort.IsConnected )
				{
					m_drawPicker = false;

					DrawTexturePropertyPreview( drawInfo, false );
				}
				else
				{
					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					m_drawPicker = true;
				}
			}
		}

		private void DrawTexturePropertyPreview( DrawInfo drawInfo, bool instance )
		{
			Rect newPos = m_remainingBox;

			TexturePropertyNode texProp = null;
			if ( instance )
				texProp = m_referenceSampler.TextureProperty;
			else
				texProp = TextureProperty;

			float previewSizeX = PreviewSizeX;
			float previewSizeY = PreviewSizeY;
			newPos.width = previewSizeX * drawInfo.InvertedZoom;
			newPos.height = previewSizeY * drawInfo.InvertedZoom;

			SetTitleText( texProp.PropertyInspectorName + ( instance ? Constants.InstancePostfixStr : " (Input)" ) );
			SetAdditonalTitleText( texProp.AdditonalTitleContent.text );

			if ( m_referenceStyle == null )
			{
				m_referenceStyle = UIUtils.CustomStyle( CustomStyle.SamplerTextureRef );
			}

			if ( m_referenceIconStyle == null || m_referenceIconStyle.normal == null )
			{
				m_referenceIconStyle = UIUtils.CustomStyle( CustomStyle.SamplerTextureIcon );
				if ( m_referenceIconStyle.normal != null )
				{
					InstanceIconWidth = m_referenceIconStyle.normal.background.width;
					InstanceIconHeight = m_referenceIconStyle.normal.background.height;
				}
			}

			Rect iconPos = m_globalPosition;
			iconPos.width = InstanceIconWidth * drawInfo.InvertedZoom;
			iconPos.height = InstanceIconHeight * drawInfo.InvertedZoom;

			iconPos.y += 10 * drawInfo.InvertedZoom;
			iconPos.x += m_globalPosition.width - iconPos.width - 7 * drawInfo.InvertedZoom;

			if ( GUI.Button( newPos, string.Empty, UIUtils.CustomStyle( CustomStyle.SamplerTextureRef )/* m_referenceStyle */) ||
				GUI.Button( iconPos, string.Empty, m_referenceIconStyle )
				)
			{
				UIUtils.FocusOnNode( texProp, 1, true );
			}

			newPos.x += 3 * drawInfo.InvertedZoom;
			newPos.y += 3 * drawInfo.InvertedZoom;

			newPos.width *= 0.94f;
			newPos.height *= 0.94f;

			if ( texProp.Value != null )
				EditorGUI.DrawPreviewTexture( newPos, texProp.Value );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			OnPropertyNameChanged();

			ConfigSampler();

			if ( m_texPort.IsConnected )
				m_texPort.GenerateShaderForOutput( ref dataCollector, m_texPort.DataType, ignoreLocalVar );

			if ( m_autoUnpackNormals )
			{
				bool isScaledNormal = false;
				if ( m_normalPort.IsConnected )
				{
					isScaledNormal = true;
				}
				else
				{
					if ( m_normalPort.FloatInternalData != 1 )
					{
						isScaledNormal = true;
					}
				}
				if ( isScaledNormal )
				{
					string scaleValue = m_normalPort.GenerateShaderForOutput( ref dataCollector, inputType, ignoreLocalVar );
					dataCollector.AddToIncludes( m_uniqueId, Constants.UnityStandardUtilsLibFuncs );
					m_normalMapUnpackMode = "UnpackScaleNormal( {0} ," + scaleValue + " )";
				}
				else
				{
					m_normalMapUnpackMode = "UnpackNormal( {0} )";
				}
			}

			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalVar );
			if ( !m_uvPort.IsConnected )
			{
				SetUVChannelDeclaration( ref dataCollector );
			}

			string valueName = SetFetchedData( ref dataCollector, ignoreLocalVar, inputType );
			return GetOutputColorItem( 0, outputId, valueName );
		}

		public string SetUVChannelDeclaration( ref MasterNodeDataCollector dataCollector, int customCoordSet = -1 )
		{
			string propertyName = CurrentPropertyReference;
			int coordSet = ( ( customCoordSet < 0 ) ? m_textureCoordSet : customCoordSet );
			string uvChannelDeclaration = IOUtils.GetUVChannelDeclaration( propertyName, -1, coordSet );
			dataCollector.AddToInput( m_uniqueId, uvChannelDeclaration, true );
			return IOUtils.GetUVChannelName( propertyName, coordSet );
		}

		public string SampleVirtualTexture( VirtualTexturePropertyNode node, string coord )
		{
			string sampler = string.Empty;
			switch ( node.Channel )
			{
				default:
				case VirtualChannel.Albedo:
				case VirtualChannel.Base:
				sampler = "VTSampleAlbedo( " + coord + " )";
				break;
				case VirtualChannel.Normal:
				case VirtualChannel.Height:
				case VirtualChannel.Occlusion:
				case VirtualChannel.Displacement:
				sampler = "VTSampleNormal( " + coord + " )";
				break;
				case VirtualChannel.Specular:
				case VirtualChannel.SpecMet:
				case VirtualChannel.Material:
				sampler = "VTSampleSpecular( " + coord + " )";
				break;
			}
			return sampler;
		}

		public string SetFetchedData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, WirePortDataType inputType )
		{
			m_precisionString = UIUtils.PrecisionWirePortToCgType( UIUtils.GetFinalPrecision( m_currentPrecisionType ), m_colorPort.DataType );
			string propertyName = CurrentPropertyReference;

			string mipType = "";
			if ( m_lodPort.IsConnected )
			{
				switch ( m_mipMode )
				{
					case MipType.Auto:
					break;
					case MipType.MipLevel:
					mipType = "lod";
					break;
					case MipType.MipBias:
					mipType = "bias";
					break;
					case MipType.Derivative:
					break;
				}
			}

			if ( ignoreLocalVar )
			{
				if ( TextureProperty is VirtualTexturePropertyNode )
					Debug.Log( "TODO" );

				if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
				{
					mipType = "lod";
				}

				string samplerValue = m_samplerType + mipType + "( " + propertyName + "," + GetUVCoords( ref dataCollector, ignoreLocalVar ) + ")";
				AddNormalMapTag( ref samplerValue );
				return samplerValue;
			}

			VirtualTexturePropertyNode vtex = ( TextureProperty as VirtualTexturePropertyNode );

			if ( vtex != null )
			{
				string atPathname = AssetDatabase.GUIDToAssetPath( Constants.ATSharedLibGUID );
				if ( string.IsNullOrEmpty( atPathname ) )
				{
					UIUtils.ShowMessage( "Could not find Amplify Texture on your project folder. Please install it and re-compile the shader.", MessageSeverity.Error );
				}
				else
				{
					//Need to see if the asset really exists because AssetDatabase.GUIDToAssetPath() can return a valid path if
					// the asset was previously imported and deleted after that
					UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( atPathname );
					if ( obj == null )
					{
						UIUtils.ShowMessage( "Could not find Amplify Texture on your project folder. Please install it and re-compile the shader.", MessageSeverity.Error );
					}
					else
					{
						if ( m_isTextureFetched )
							return m_textureFetchedValue;

						string remapPortR = ".r";
						string remapPortG = ".g";
						string remapPortB = ".b";
						string remapPortA = ".a";

						if ( vtex.Channel == VirtualChannel.Occlusion )
						{
							remapPortR = ".r"; remapPortG = ".r"; remapPortB = ".r"; remapPortA = ".r";
						}
						else if ( vtex.Channel == VirtualChannel.SpecMet && ( UIUtils.CurrentWindow.CurrentGraph.CurrentStandardSurface != null && UIUtils.CurrentWindow.CurrentGraph.CurrentStandardSurface.CurrentLightingModel == StandardShaderLightModel.Standard ) )
						{
							remapPortR = ".r"; remapPortG = ".r"; remapPortB = ".r";
						}
						else if ( vtex.Channel == VirtualChannel.Height || vtex.Channel == VirtualChannel.Displacement )
						{
							remapPortR = ".b"; remapPortG = ".b"; remapPortB = ".b"; remapPortA = ".b";
						}

						dataCollector.AddToPragmas( m_uniqueId, IOUtils.VirtualTexturePragmaHeader );
						dataCollector.AddToIncludes( m_uniqueId, atPathname );

						string lodBias = m_mipMode == MipType.MipLevel ? "Lod" : m_mipMode == MipType.MipBias ? "Bias" : "";
						int virtualCoordId = dataCollector.GetVirtualCoordinatesId( m_uniqueId, GetVirtualUVCoords( ref dataCollector, ignoreLocalVar ), lodBias );

						string virtualSampler = SampleVirtualTexture( vtex, Constants.VirtualCoordNameStr + virtualCoordId );
						AddNormalMapTag( ref virtualSampler );

						for ( int i = 0; i < m_outputPorts.Count; i++ )
						{
							if ( m_outputPorts[ i ].IsConnected )
							{

								//TODO: make the sampler not generate local variables at all times
								m_textureFetchedValue = "virtualNode" + m_uniqueId;
								m_isTextureFetched = true;

								dataCollector.AddToLocalVariables( m_uniqueId, m_precisionString + " " + m_textureFetchedValue + " = " + virtualSampler + ";" );
								m_colorPort.SetLocalValue( m_textureFetchedValue );
								m_outputPorts[ m_colorPort.PortId + 1 ].SetLocalValue( m_textureFetchedValue + remapPortR );
								m_outputPorts[ m_colorPort.PortId + 2 ].SetLocalValue( m_textureFetchedValue + remapPortG );
								m_outputPorts[ m_colorPort.PortId + 3 ].SetLocalValue( m_textureFetchedValue + remapPortB );
								m_outputPorts[ m_colorPort.PortId + 4 ].SetLocalValue( m_textureFetchedValue + remapPortA );
								return m_textureFetchedValue;
							}
						}

						return virtualSampler;
					}
				}
			}

			if ( m_isTextureFetched )
				return m_textureFetchedValue;

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
			{
				mipType = "lod";
			}

			string samplerOp = m_samplerType + mipType + "( " + propertyName + "," + GetUVCoords( ref dataCollector, ignoreLocalVar ) + ")";
			AddNormalMapTag( ref samplerOp );

			int connectedPorts = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					connectedPorts += 1;
					if ( connectedPorts > 1 || m_outputPorts[ i ].ConnectionCount > 1 || ( i > 0 && inputType != WirePortDataType.FLOAT )/*if some cast is going to happen the its better to save fetch*/ )
					{
						// Create common local var and mark as fetched
						m_textureFetchedValue = m_samplerType + "Node" + m_uniqueId;
						m_isTextureFetched = true;

						//dataCollector.AddToLocalVariables( m_uniqueId, ( ( /*m_isNormalMap && */m_autoUnpackNormals ) ? "float3 " : "float4 " ) + m_textureFetchedValue + " = " + samplerOp + ";" );
						dataCollector.AddToLocalVariables( m_uniqueId, m_precisionString + " " + m_textureFetchedValue + " = " + samplerOp + ";" );
						m_colorPort.SetLocalValue( m_textureFetchedValue );
						m_outputPorts[ m_colorPort.PortId + 1 ].SetLocalValue( m_textureFetchedValue + ".r" );
						m_outputPorts[ m_colorPort.PortId + 2 ].SetLocalValue( m_textureFetchedValue + ".g" );
						m_outputPorts[ m_colorPort.PortId + 3 ].SetLocalValue( m_textureFetchedValue + ".b" );
						m_outputPorts[ m_colorPort.PortId + 4 ].SetLocalValue( m_textureFetchedValue + ".a" );
						return m_textureFetchedValue;
					}
				}
			}
			return samplerOp;
		}

		private void AddNormalMapTag( ref string value )
		{
			if ( /*m_isNormalMap && */m_autoUnpackNormals )
			{
				value = string.Format( m_normalMapUnpackMode, value );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_defaultValue = AssetDatabase.LoadAssetAtPath<Texture>( textureName );
			m_useSemantics = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_textureCoordSet = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_isNormalMap = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_defaultTextureValue = ( TexturePropertyValues ) Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_autocastMode = ( AutoCastType ) Enum.Parse( typeof( AutoCastType ), GetCurrentParam( ref nodeParams ) );
			m_autoUnpackNormals = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if ( UIUtils.CurrentShaderVersion() > 12 )
			{
				m_referenceType = ( TexReferenceType ) Enum.Parse( typeof( TexReferenceType ), GetCurrentParam( ref nodeParams ) );
				if ( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_referenceArrayId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				if ( m_referenceType == TexReferenceType.Instance )
				{
					UIUtils.UnregisterSamplerNode( this );
					UIUtils.UnregisterPropertyNode( this );
					m_forceSamplerUpdate = true;
				}
				UpdateHeaderColor();
			}
			if ( UIUtils.CurrentShaderVersion() > 2405 )
				m_mipMode = ( MipType ) Enum.Parse( typeof( MipType ), GetCurrentParam( ref nodeParams ) );

			ConfigFromObject( m_defaultValue );
			ConfigureInputPorts();
			ConfigureOutputPorts();
			ResizeNodeToPreview();
		}
		public override void ReadAdditionalData( ref string[] nodeParams ) { }
		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultValue != null ) ? AssetDatabase.GetAssetPath( m_defaultValue ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_useSemantics.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordSet.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isNormalMap.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autocastMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoUnpackNormals );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceSampler != null ) ? m_referenceSampler.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mipMode );
		}

		public override void WriteAdditionalToString( ref string nodeInfo, ref string connectionsInfo ) { }

		public string GetVirtualUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string bias = "";
			if ( m_mipMode == MipType.MipBias || m_mipMode == MipType.MipLevel )
			{
				string lodLevel = m_lodPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar );
				bias += ", " + lodLevel;
			}

			if ( m_uvPort.IsConnected )
			{
				string uvs = m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
				return uvs + bias;
			}
			else
			{
				string uvCoord = string.Empty;

				if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
				{
					uvCoord = Constants.VertexShaderInputStr + ".texcoord";
					if ( m_textureCoordSet > 0 )
					{
						uvCoord += m_textureCoordSet.ToString();
					}
					uvCoord = UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Fixed, WirePortDataType.FLOAT4 ) + "( " + uvCoord + ",0,0 )";
				}
				else
				{
					string propertyName = CurrentPropertyReference;
					string uvChannelName = IOUtils.GetUVChannelName( propertyName, m_textureCoordSet );
					uvCoord = Constants.InputVarStr + "." + uvChannelName;
				}

				return uvCoord + bias;
			}
		}

		public string GetUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_uvPort.IsConnected )
			{
				if ( ( m_mipMode == MipType.MipLevel || m_mipMode == MipType.MipBias ) && m_lodPort.IsConnected )
				{
					string lodLevel = m_lodPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar );
					if ( m_currentType != TextureType.Texture2D )
					{
						string uvs = m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
						return UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + uvs + ", " + lodLevel + ")";
					}
					else
					{
						string uvs = m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
						return UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + uvs + ", 0, " + lodLevel + ")";
					}
				}
				else if ( m_mipMode == MipType.Derivative )
				{
					if ( m_currentType != TextureType.Texture2D )
					{
						string uvs = m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
						string ddx = m_ddxPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
						string ddy = m_ddyPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
						return uvs + ", " + ddx + ", " + ddy;
					}
					else
					{
						string uvs = m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
						string ddx = m_ddxPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
						string ddy = m_ddyPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
						return uvs + ", " + ddx + ", " + ddy;
					}

				}
				else
				{
					if ( m_currentType != TextureType.Texture2D )
						return m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
					else
						return m_uvPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
				}
			}
			else
			{
				if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
				{
					string coords = Constants.VertexShaderInputStr + ".texcoord";
					if ( m_textureCoordSet > 0 )
					{
						coords += m_textureCoordSet.ToString();
					}

					return UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Fixed, WirePortDataType.FLOAT4 ) + "( " + coords + ",0,0 )";
				}

				string propertyName = CurrentPropertyReference;
				string uvChannelName = IOUtils.GetUVChannelName( propertyName, m_textureCoordSet );
				string uvCoord = Constants.InputVarStr + "." + uvChannelName;
				if ( ( m_mipMode == MipType.MipLevel || m_mipMode == MipType.MipBias ) && m_lodPort.IsConnected )
				{
					string lodLevel = m_lodPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar );
					if ( m_currentType != TextureType.Texture2D )
					{
						string uvs = string.Format( "float3({0},0.0)", uvCoord ); ;
						return UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + uvs + ", " + lodLevel + ")";
					}
					else
					{
						string uvs = uvCoord;
						return UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT4 ) + "( " + uvs + ", 0, " + lodLevel + ")";
					}
				}
				else if ( m_mipMode == MipType.Derivative )
				{
					if ( m_currentType != TextureType.Texture2D )
					{
						string uvs = string.Format( "float3({0},0.0)", uvCoord );
						string ddx = m_ddxPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
						string ddy = m_ddyPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
						return uvs + ", " + ddx + ", " + ddy;
					}
					else
					{
						string uvs = uvCoord;
						string ddx = m_ddxPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
						string ddy = m_ddyPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
						return uvs + ", " + ddx + ", " + ddy;
					}
				}

				else
				{
					if ( m_currentType != TextureType.Texture2D )
					{
						return string.Format( "float3({0},0.0)", uvCoord );
					}
					else
					{
						return uvCoord;
					}
				}
			}
		}

		public override int VersionConvertInputPortId( int portId )
		{
			int newPort = portId;
			//change normal scale port to last
			if ( UIUtils.CurrentShaderVersion() < 2407 )
			{
				if ( portId == 1 )
					newPort = 4;
			}

			if ( UIUtils.CurrentShaderVersion() < 2408 )
			{
				newPort = newPort + 1;
			}

			return newPort;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_defaultValue = null;
			m_materialValue = null;
			m_referenceSampler = null;
			m_referenceStyle = null;
			m_referenceContent = null;
			m_texPort = null;
			m_uvPort = null;
			m_lodPort = null;
			m_ddxPort = null;
			m_ddyPort = null;
			m_normalPort = null;
			m_colorPort = null;

			if ( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterSamplerNode( this );
				UIUtils.UnregisterPropertyNode( this );
			}
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialValue != null ? m_materialValue.name : IOUtils.NO_TEXTURES ) : ( m_defaultValue != null ? m_defaultValue.name : IOUtils.NO_TEXTURES );
		}

		public TexturePropertyNode TextureProperty
		{
			get
			{
				if ( m_referenceSampler != null )
				{
					m_textureProperty = m_referenceSampler as TexturePropertyNode;
				}
				else if ( m_texPort.IsConnected )
				{
					m_textureProperty = m_texPort.GetOutputNode( 0 ) as TexturePropertyNode;
				}

				if ( m_textureProperty != null )
					return m_textureProperty;

				return this;
			}
		}



		public override string GetPropertyValue()
		{
			if ( SoftValidReference )
			{
				if ( m_referenceSampler.IsConnected )
					return string.Empty;

				return m_referenceSampler.TextureProperty.GetPropertyValue();
			}
			else
			if ( m_texPort.IsConnected )
			{
				return TextureProperty.GetPropertyValue();
			}

			switch ( m_currentType )
			{
				case TextureType.Texture2D:
				{
					return GetTexture2DPropertyValue();
				}
				case TextureType.Texture3D:
				{
					return GetTexture3DPropertyValue();
				}
				case TextureType.Cube:
				{
					return GetCubePropertyValue();
				}
			}
			return string.Empty;
		}

		public override string GetUniformValue()
		{
			if ( SoftValidReference )
			{
				if ( m_referenceSampler.IsConnected )
					return string.Empty;

				return m_referenceSampler.TextureProperty.GetUniformValue();
			}
			else if ( m_texPort.IsConnected )
			{
				return TextureProperty.GetUniformValue();
			}

			return base.GetUniformValue();
		}
		public string UVCoordsName { get { return Constants.InputVarStr + "." + IOUtils.GetUVChannelName( CurrentPropertyReference, m_textureCoordSet ); } }

		public override string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
				if ( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					SamplerNode node = UIUtils.GetSamplerNode( m_referenceArrayId );
					propertyName = ( node != null ) ? node.TextureProperty.PropertyName : PropertyName;
				}
				else if ( m_texPort.IsConnected )
				{
					propertyName = TextureProperty.PropertyName;
				}
				else
				{
					propertyName = PropertyName;
				}
				return propertyName;
			}
		}

		public bool SoftValidReference
		{
			get
			{
				if ( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					m_referenceSampler = UIUtils.GetSamplerNode( m_referenceArrayId );

					m_texPort.Locked = true;

					if ( m_referenceContent == null )
						m_referenceContent = new GUIContent();


					if ( m_referenceSampler != null )
					{
						m_referenceContent.image = m_referenceSampler.Value;
						if ( m_referenceWidth != m_referenceSampler.Position.width )
						{
							m_referenceWidth = m_referenceSampler.Position.width;
							m_sizeIsDirty = true;
						}
					}
					else
					{
						m_referenceArrayId = -1;
						m_referenceWidth = -1;
					}

					return m_referenceSampler != null;
				}
				m_texPort.Locked = false;
				return false;
			}
		}
		//public override string DataToArray { get { return PropertyName; } }

		public bool AutoUnpackNormals
		{
			get { return m_autoUnpackNormals; }
			set
			{
				m_autoUnpackNormals = value;
				m_defaultTextureValue = value ? TexturePropertyValues.bump : TexturePropertyValues.white;
			}
		}
	}
}
