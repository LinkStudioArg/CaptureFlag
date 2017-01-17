// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum TexturePropertyValues
	{
		white,
		black,
		gray,
		bump
	}

	public enum TextureType
	{
		Texture1D,
		Texture2D,
		Texture3D,
		Cube
	}

	public enum AutoCastType
	{
		Auto,
		LockedToTexture1D,
		LockedToTexture2D,
		LockedToTexture3D,
		LockedToCube
	}


	[Serializable]
	[NodeAttributes( "Texture Object", "Constants", "Texture Object" )]
	public class TexturePropertyNode : PropertyNode
	{
		protected const int OriginalFontSizeUpper = 9;
		protected const int OriginalFontSizeLower = 9;

		protected const string DefaultTextureStr = "Default value";
		protected const string AutoCastModeStr = "Auto-Cast Mode";

		protected const string AutoUnpackNormalsStr = "Normal";

		[SerializeField]
		protected Texture m_defaultValue;

		[SerializeField]
		protected Texture m_materialValue;

		[SerializeField]
		protected TexturePropertyValues m_defaultTextureValue;

		[SerializeField]
		protected bool m_isNormalMap;

		[SerializeField]
		protected Type m_textureType;

		[SerializeField]
		protected bool m_isTextureFetched;

		[SerializeField]
		protected string m_textureFetchedValue;

		[SerializeField]
		protected TextureType m_currentType = TextureType.Texture2D;

		[SerializeField]
		protected AutoCastType m_autocastMode = AutoCastType.Auto;

		protected int PreviewSizeX = 110;
		protected int PreviewSizeY = 110;

		protected TexturePropertyNode m_textureProperty = null;

		protected bool m_drawPicker;

		protected bool m_forceNodeUpdate = false;

		public TexturePropertyNode() : base() { }
		public TexturePropertyNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_defaultTextureValue = TexturePropertyValues.white;
			m_insideSize.Set( PreviewSizeX, PreviewSizeY + 5);
			m_extraSize.Set( 0, 0 );
			AddOutputPort( WirePortDataType.SAMPLER2D, Constants.EmptyPortValue );
			m_currentParameterType = PropertyType.Property;
			m_useCustomPrefix = true;
			m_customPrefix = "Texture ";
			m_drawPrecisionUI = false;
			m_freeType = false;
			m_drawPicker = true;
			m_textLabelWidth = 100;
			ConfigTextureData( TextureType.Texture2D );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_textureProperty = this;
			UIUtils.RegisterPropertyNode( this );
			UIUtils.RegisterTexturePropertyNode( this );
		}

		protected void ConfigTextureData( TextureType type )
		{
			switch ( m_autocastMode )
			{
				case AutoCastType.Auto:
					{
						m_currentType = type;
					}
					break;
				case AutoCastType.LockedToTexture1D:
					{
						m_currentType = TextureType.Texture1D;
					}
					break;
				case AutoCastType.LockedToTexture2D:
					{
						m_currentType = TextureType.Texture2D;
					}
					break;
				case AutoCastType.LockedToTexture3D:
					{
						m_currentType = TextureType.Texture3D;
					}
					break;
				case AutoCastType.LockedToCube:
					{
						m_currentType = TextureType.Cube;
					}
					break;
			}

			switch ( m_currentType )
			{
				case TextureType.Texture1D:
					{
						m_textureType = typeof( Texture );
					}
					break;
				case TextureType.Texture2D:
					{
						m_textureType = typeof( Texture2D );
					}
					break;
				case TextureType.Texture3D:
					{
						m_textureType = typeof( Texture3D );
					}
					break;
				case TextureType.Cube:
					{
						m_textureType = typeof( Cubemap );
					}
					break;
			}
		}

		// Texture1D
		public string GetTexture1DPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 2D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture1DUniformValue()
		{
			return "uniform sampler1D " + PropertyName + ";";
		}

		// Texture2D
		public string GetTexture2DPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 2D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture2DUniformValue()
		{
			return "uniform sampler2D " + PropertyName + ";";
		}

		//Texture3D
		public string GetTexture3DPropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", 3D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture3DUniformValue()
		{
			return "uniform sampler3D " + PropertyName + ";";
		}

		// Cube
		public string GetCubePropertyValue()
		{
			return PropertyName + "(\"" + m_propertyInspectorName + "\", CUBE) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetCubeUniformValue()
		{
			return "uniform samplerCUBE " + PropertyName + ";";
		}
		//

		public override void DrawSubProperties()
		{
			ShowDefaults();

			EditorGUI.BeginChangeCheck();
			m_defaultValue = ( Texture )EditorGUILayout.ObjectField( Constants.DefaultValueLabel, m_defaultValue, m_textureType, false );
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
			}
		}

		public override void DrawMaterialProperties()
		{
			ShowDefaults();

			EditorGUI.BeginChangeCheck();
			m_materialValue = ( Texture )EditorGUILayout.ObjectField( Constants.MaterialValueLabel, m_materialValue, m_textureType, false );
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
			}
		}

		void ShowDefaults()
		{
			m_defaultTextureValue = ( TexturePropertyValues )EditorGUILayout.EnumPopup( DefaultTextureStr, m_defaultTextureValue );
			AutoCastType newAutoCast = ( AutoCastType )EditorGUILayout.EnumPopup( AutoCastModeStr, m_autocastMode );
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

		private void ConfigurePortsFromReference()
		{
			m_sizeIsDirty = true;
		}

		public virtual void ConfigureOutputPorts()
		{
			switch ( m_currentType )
			{
				case TextureType.Texture1D:
					m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER1D, false );
					break;
				case TextureType.Texture2D:
					m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER2D, false );
					break;
				case TextureType.Texture3D:
					m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLER3D, false );
					break;
				case TextureType.Cube:
					m_outputPorts[ 0 ].ChangeType( WirePortDataType.SAMPLERCUBE, false );
					break;
			}

			m_sizeIsDirty = true;
		}

		public virtual void ConfigureInputPorts()
		{
		}

		public virtual void AdditionalCheck()
		{
		}

		public virtual void CheckTextureImporter( bool additionalCheck )
		{
			m_requireMaterialUpdate = true;
			Texture texture = m_materialMode ? m_materialValue : m_defaultValue;
			TextureImporter importer = AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( texture ) ) as TextureImporter;
			if ( importer != null )
			{
#if UNITY_5_5_OR_NEWER
				m_isNormalMap = importer.textureType == TextureImporterType.NormalMap;
#else
				m_isNormalMap = importer.normalmap;
#endif
				
				if( m_defaultTextureValue == TexturePropertyValues.bump && !m_isNormalMap )
					m_defaultTextureValue = TexturePropertyValues.white;
				else if( m_isNormalMap )
					m_defaultTextureValue = TexturePropertyValues.bump;

				if( additionalCheck )
					AdditionalCheck();
			}

			if ( ( texture as Texture2D ) != null )
			{
				ConfigTextureData( TextureType.Texture2D );
			}
			else if ( ( texture as Texture3D ) != null )
			{
				ConfigTextureData( TextureType.Texture3D );
			}
			else if ( ( texture as Cubemap ) != null )
			{
				ConfigTextureData( TextureType.Cube );
			}

			//ConfigureInputPorts();
			//ConfigureOutputPorts();
			//ResizeNodeToPreview();
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

		protected void ConfigFromObject( UnityEngine.Object obj )
		{
			Texture texture = obj as Texture;
			if ( texture )
			{
				m_materialValue = texture;
				m_defaultValue = texture;
				CheckTextureImporter( false );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
		}

		public override void Draw( DrawInfo drawInfo )
		{
			EditorGUI.BeginChangeCheck();
			base.Draw( drawInfo );

			if ( EditorGUI.EndChangeCheck() )
			{
				OnPropertyNameChanged();
			}

			if ( m_forceNodeUpdate )
			{
				m_forceNodeUpdate = false;
				ResizeNodeToPreview();
			}

			if ( m_isVisible && m_drawPicker)
			{
				DrawTexturePicker( drawInfo );
			}

			//GUI.Box( m_remainingBox, string.Empty, UIUtils.CustomStyle( CustomStyle.MainCanvasTitle ) );
		}

		protected void DrawTexturePicker( DrawInfo drawInfo )
		{
			int fontSizeUpper = GUI.skin.customStyles[ 265 ].fontSize;
			int fontSizeLower = GUI.skin.customStyles[ 266 ].fontSize;

			//cached values
			//int overflowTop = GUI.skin.customStyles[ 266 ].overflow.top;
			//int overflowLeft = GUI.skin.customStyles[ 266 ].overflow.left;

			//int borderTop = GUI.skin.customStyles[ 266 ].border.top;
			//int borderBottom = GUI.skin.customStyles[ 266 ].border.bottom;
			//int borderLeft = GUI.skin.customStyles[ 266 ].border.left;
			//int borderRight = GUI.skin.customStyles[ 266 ].border.right;

			//UIUtils.MainSkin.customStyles[ ( int )CustomStyle.ObjectPicker ] = GUI.skin.customStyles[ 266 ];

			Rect newRect = m_globalPosition;

			newRect.width = PreviewSizeX * drawInfo.InvertedZoom; //PreviewSizeX * drawInfo.InvertedZoom;
			newRect.height = PreviewSizeY * drawInfo.InvertedZoom; //PreviewSizeY * drawInfo.InvertedZoom;
			newRect.x = m_remainingBox.x;
			newRect.y = m_remainingBox.y;


			GUI.skin.customStyles[ 265 ].fontSize = ( int )( OriginalFontSizeUpper * drawInfo.InvertedZoom );
			GUI.skin.customStyles[ 266 ].fontSize = ( int )( OriginalFontSizeLower * drawInfo.InvertedZoom );

			GUI.skin.customStyles[ 266 ].overflow.top = ( int )( -newRect.height + 15 * drawInfo.InvertedZoom );
			GUI.skin.customStyles[ 266 ].overflow.left = ( int )( -newRect.width + 75 * drawInfo.InvertedZoom );

			GUI.skin.customStyles[ 266 ].border.top = 0;
			GUI.skin.customStyles[ 266 ].border.bottom = 0;
			GUI.skin.customStyles[ 266 ].border.left = 5;
			GUI.skin.customStyles[ 266 ].border.right = 0;

			EditorGUI.BeginChangeCheck();
			if ( m_materialMode )
			{
				m_materialValue = ( Texture )EditorGUI.ObjectField( newRect, m_materialValue, m_textureType, false );
			}
			else
			{
				m_defaultValue = ( Texture )EditorGUI.ObjectField( newRect, m_defaultValue, m_textureType, false );
			}

			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				ConfigureInputPorts();
				ConfigureOutputPorts();
				ResizeNodeToPreview();
				BeginDelayedDirtyProperty();
			}

			GUI.skin.customStyles[ 265 ].fontSize = fontSizeUpper;
			GUI.skin.customStyles[ 266 ].fontSize = fontSizeLower;

			//fixed values from original skin
			GUI.skin.customStyles[ 266 ].overflow.top = 0;
			GUI.skin.customStyles[ 266 ].overflow.left = -24;

			//fixed values from original skin
			GUI.skin.customStyles[ 266 ].border.top = 0;
			GUI.skin.customStyles[ 266 ].border.bottom = 16;
			GUI.skin.customStyles[ 266 ].border.left = 8;
			GUI.skin.customStyles[ 266 ].border.right = 31;
		}

		public virtual void ResizeNodeToPreview()
		{
			
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalVar );
			return PropertyName;
		}

		public override void ResetOutputLocals()
		{
			base.ResetOutputLocals();
			m_isTextureFetched = false;
			m_textureFetchedValue = string.Empty;
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				OnPropertyNameChanged();
				if ( mat.HasProperty( PropertyName ) )
				{
					mat.SetTexture( PropertyName, m_materialValue );
				}
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_materialMode && UIUtils.IsProperty( m_currentParameterType ) )
			{
				if ( mat.HasProperty( PropertyName ) )
				{
					m_materialValue = mat.GetTexture( PropertyName );
					CheckTextureImporter( false );
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( PropertyName ) )
			{
				m_materialValue = material.GetTexture( PropertyName );
				CheckTextureImporter( false );
			}
		}

		public override bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol/* ref string metaStr */)
		{
			if ( m_defaultValue != null )
			{
				defaultCol.AddValue( PropertyName, m_defaultValue );
			}

			return true;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			ReadAdditionalData( ref nodeParams );
		}

		public virtual void ReadAdditionalData( ref string[] nodeParams )
		{
			string textureName = GetCurrentParam( ref nodeParams );
			m_defaultValue = AssetDatabase.LoadAssetAtPath<Texture>( textureName );
			m_isNormalMap = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_defaultTextureValue = ( TexturePropertyValues )Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_autocastMode = ( AutoCastType )Enum.Parse( typeof( AutoCastType ), GetCurrentParam( ref nodeParams ) );

			m_forceNodeUpdate = true;

			ConfigFromObject( m_defaultValue );
			ConfigureInputPorts();
			ConfigureOutputPorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			WriteAdditionalToString( ref nodeInfo, ref connectionsInfo );
		}

		public virtual void WriteAdditionalToString( ref string nodeInfo, ref string connectionsInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultValue != null ) ? AssetDatabase.GetAssetPath( m_defaultValue ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isNormalMap.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autocastMode );
		}

		public override void Destroy()
		{
			base.Destroy();
			m_defaultValue = null;
			m_materialValue = null;
			m_textureProperty = null;
			UIUtils.UnregisterPropertyNode( this );
			UIUtils.UnregisterTexturePropertyNode( this );
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialValue != null ? m_materialValue.name : IOUtils.NO_TEXTURES ) : ( m_defaultValue != null ? m_defaultValue.name : IOUtils.NO_TEXTURES );
		}

		public override string GetPropertyValue()
		{
			switch ( m_currentType )
			{
				case TextureType.Texture1D:
					{
						return GetTexture1DPropertyValue();
					}
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
			switch ( m_currentType )
			{
				case TextureType.Texture1D:
					{
						return GetTexture1DUniformValue();
					}
				case TextureType.Texture2D:
					{
						return GetTexture2DUniformValue();
					}
				case TextureType.Texture3D:
					{
						return GetTexture3DUniformValue();
					}
				case TextureType.Cube:
					{
						return GetCubeUniformValue();
					}
			}

			return string.Empty;
		}

		public virtual string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
					propertyName = PropertyName;
				return propertyName;
			}
		}

		public Texture Value
		{
			get { return m_materialMode ? m_materialValue : m_defaultValue; }
			set
			{
				if ( m_materialMode )
				{
					m_materialValue = value;
				}
				else
				{
					m_defaultValue = value;
				}
			}
		}

		public bool IsNormalMap
		{
			get {
				return m_isNormalMap;
			}
		}

		public override void OnPropertyNameChanged()
		{
			base.OnPropertyNameChanged();
			UIUtils.UpdateTexturePropertyDataNode( m_uniqueId, PropertyInspectorName );
		}

		public override string DataToArray { get { return PropertyInspectorName; } }
	}
}
