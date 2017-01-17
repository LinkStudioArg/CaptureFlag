// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{

	public enum RenderPath
	{
		All,
		ForwardOnly,
		DeferredOnly
	}

	public enum StandardShaderLightModel
	{
		Standard,
		StandardSpecular,
		Lambert,
		BlinnPhong
	}

	public enum CullMode
	{
		Back,
		Front,
		Off
	}



	public enum AlphaMode
	{
		Opaque = 0,
		Masked,
		Fade,
		Transparent
	}

	public enum RenderType
	{
		Opaque,
		Transparent,
		TransparentCutout,
		Background,
		Overlay,
		TreeOpaque,
		TreeTransparentCutout,
		TreeBillboard,
		Grass,
		GrassBillboard
	}

	public enum RenderQueue
	{
		Background,
		Geometry,
		AlphaTest,
		Transparent,
		Overlay
	}

	public enum RenderPlatforms
	{
		d3d9,
		d3d11,
		glcore,
		gles,
		gles3,
		metal,
		d3d11_9x,
		xbox360,
		xboxone,
		ps4,
		psp2,
		n3ds,
		wiiu
	}

	[Serializable]
	public class NodeCache
	{
		public int TargetNodeId = -1;
		public int TargetPortId = -1;

		public NodeCache( int targetNodeId, int targetPortId )
		{
			SetData( targetNodeId, targetPortId );
		}

		public void SetData( int targetNodeId, int targetPortId )
		{
			TargetNodeId = targetNodeId;
			TargetPortId = targetPortId;
		}

		public void Invalidate()
		{
			TargetNodeId = -1;
			TargetPortId = -1;
		}

		public bool IsValid
		{
			get { return ( TargetNodeId >= 0 ); }
		}

		public override string ToString()
		{
			return "TargetNodeId " + TargetNodeId + " TargetPortId " + TargetPortId;
		}
	}

	[Serializable]
	public class CacheNodeConnections
	{
		public Dictionary<string, NodeCache> NodeCacheArray;

		public CacheNodeConnections()
		{
			NodeCacheArray = new Dictionary<string, NodeCache>();
		}

		public void Add( string key, NodeCache value )
		{
			NodeCacheArray.Add( key, value );
		}

		public NodeCache Get( string key )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
				return NodeCacheArray[ key ];
			return null;
		}

		public void Clear()
		{
			NodeCacheArray.Clear();
		}
	}

	[Serializable]
	[NodeAttributes( "Standard Surface Output", "Master", "Surface shader generator output", null, KeyCode.None, false )]
	public sealed class StandardSurfaceOutputNode : MasterNode, ISerializationCallbackReceiver
	{
		private readonly string[] RenderingPlatformsLabels =    {   "Direct3D 9",
																	"Direct3D 11/12",
																	"OpenGL 3.x/4.x",
																	"OpenGL ES 2.0",
																	"OpenGL ES 3.x",
																	"iOS/Mac Metal",
																	"Direct3D 11 9.x",
																	"Xbox 360",
																	"Xbox One",
																	"PlayStation 4",
																	"PlayStation Vita",
																	"Nintendo 3DS",
																	"Nintendo Wii U" };

		private const string RenderingPlatformsStr = "Rendering Platforms";
		private const string CustomInspectorStr = "Custom Editor";
		private GUIContent RenderPathContent = new GUIContent( "Render Path", "Selects and generates passes for the supported rendering paths\nDefault: All" );
		private const string ShaderModelStr = "Shader Model";
		//private GUIContent ShaderModelContent = new GUIContent( "Shader Model", "Selects the shader model to compile for, higher models have more features but are less supported in all platforms\nDefault: 3.0" );
		private const string LightModelStr = "Light Model";
		private GUIContent LightModelContent = new GUIContent( "Light Model", "Surface shader lighting model defines how the surface reflects light\nDefault: Standard" );
		private GUIContent CullModeContent = new GUIContent( "Cull Mode", "Polygon culling mode prevents rendering of either back-facing or front-facing polygons to save performance, turn it off if you want to render both sides\nDefault: Back" );
		private const string ZWriteModeStr = "ZWrite Mode";
		private const string ZTestModeStr = "ZTest Mode";
		private const string ShaderNameStr = "Shader Name";

		private const string DiscardStr = "Opacity Mask";
		private const string VertexDisplacementStr = "Local Vertex Offset";
		private const string PerVertexDataStr = "Per Vertex Data";
		private const string CustomLightModelStr = "C. Light Model";
		private const string AlbedoStr = "Albedo";
		private const string NormalStr = "Normal";
		private const string EmissionStr = "Emission";
		private const string MetallicStr = "Metallic";
		private const string SmoothnessStr = "Smoothness";
		private const string OcclusionStr = "Occlusion";
		private const string TransmissionStr = "Transmission";
		private const string TranslucencyStr = "Translucency";
		private const string AlphaStr = "Opacity";
		private const string AlphaDataStr = "Alpha";
		private const string DebugStr = "Debug";
		private const string SpecularStr = "Specular";
		private const string GlossStr = "Gloss";
		private GUIContent AlphaModeContent = new GUIContent( "Blend Mode", "Defines how the surface blends with the background\nDefault: Opaque" );
		private const string OpacityMaskClipValueStr = "Mask Clip Value";
		private GUIContent OpacityMaskClipValueContent = new GUIContent( "Mask Clip Value", "Default clip value to be compared with opacity alpha ( 0 = fully Opaque, 1 = fully Masked )\nDefault: 0.5" );
		private GUIContent CastShadowsContent = new GUIContent( "Cast Shadows", "Generates a shadow caster pass for vertex modifications and point lights in forward rendering\nDefault: ON" );
		private GUIContent ReceiveShadowsContent = new GUIContent( "Receive Shadows", "Untick it to disable shadow receiving, this includes self-shadowing\nDefault: ON" );
		private GUIContent KeepAlphaContent = new GUIContent( "Keep Alpha", "Using this option allows keeping lighting function's alpha value even for opaque surface shaders\nDefault: ON" );
		private GUIContent QueueIndexContent = new GUIContent( "Queue Index", "Value to offset the render queue, accepts both positive values to render later and negative values to render sooner\nDefault: 0" );
		private GUIContent RenderQueueContent = new GUIContent( "Render Queue", "Base rendering queue index\n(Background = 1000, Geometry = 2000, AlphaTest = 2450, Transparent = 3000, Overlay = 4000)\nDefault: Geometry" );
		private GUIContent RenderTypeContent = new GUIContent( "Render Type", "Categorizes shaders into several predefined groups, usually to be used with screen shader effects\nDefault: Opaque" );
		private GUIContent ColorMaskContent = new GUIContent( "Color Mask", "Sets color channel writing mask, turning all off makes the object completely invisible\nDefault: RGBA" );

		private GUIContent m_shaderNameContent;
		private const string DefaultShaderName = "MyNewShader";

		private const string ShaderInputOrderStr = "Shader Input Order";
		private const string PropertyOderFoldoutStr = "Available Properties";

		//private const string _codeGenerationTitle = "Code Generation Options";

		[SerializeField]
		private StencilBufferOpHelper m_stencilBufferHelper = new StencilBufferOpHelper();

		[SerializeField]
		private ZBufferOpHelper m_zBufferHelper = new ZBufferOpHelper();

		[SerializeField]
		private TessellationOpHelper m_tessOpHelper = new TessellationOpHelper();

		[SerializeField]
		private StandardShaderLightModel m_currentLightModel;

		[SerializeField]
		private StandardShaderLightModel m_lastLightModel;

		[SerializeField]
		private CullMode m_cullMode = CullMode.Back;

		[SerializeField]
		private AlphaMode m_alphaMode = AlphaMode.Opaque;

		[SerializeField]
		private RenderType m_renderType = RenderType.Opaque;

		[SerializeField]
		private RenderQueue m_renderQueue = RenderQueue.Geometry;

		[SerializeField]
		private RenderPath m_renderPath = RenderPath.All;

		[SerializeField]
		private bool m_customBlendMode = false;

		[SerializeField]
		private float m_opacityMaskClipValue = 0.5f;

		[SerializeField]
		private int m_discardPortId = -1;

		[SerializeField]
		private bool m_keepAlpha = true;

		[SerializeField]
		private bool m_castShadows = true;

		[SerializeField]
		private bool m_receiveShadows = true;

		[SerializeField]
		private int m_queueOrder = 0;

		[SerializeField]
		private List<CodeGenerationData> m_codeGenerationDataList;

		[SerializeField]
		private CacheNodeConnections m_cacheNodeConnections = new CacheNodeConnections();

		private ReorderableList m_propertyReordableList;
		private int m_lastCount = 0;

		[SerializeField]
		private bool[] m_renderingPlatformValues;

		[SerializeField]
		private bool m_renderingPlatformFoldout = false;

		[SerializeField]
		private bool[] m_colorMask = { true, true, true, true };

		private readonly char[] m_colorMaskChar = { 'R', 'G', 'B', 'A' };

		private GUIStyle m_inspectorFoldoutStyle;

		[SerializeField]
		private bool m_renderingShaderFoldout = false;

		//Tesselation


		//private GUIStyle _materialLabelStyle;
		private GUIStyle m_inspectorDefaultStyle;
		private GUIStyle m_propertyAdjustment;

		private GUIStyle m_leftToggleColorMask;
		private GUIStyle m_middleToggleColorMask;
		private GUIStyle m_rightToggleColorMask;

		protected override void CommonInit( int uniqueId )
		{
			m_renderingPlatformValues = new bool[ RenderingPlatformsLabels.Length ];
			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				m_renderingPlatformValues[ i ] = true;
			}
			_shaderTypeLabel += "Surface Shader";

			m_currentLightModel = m_lastLightModel = StandardShaderLightModel.Standard;
			m_codeGenerationDataList = new List<CodeGenerationData>();
			m_codeGenerationDataList.Add( new CodeGenerationData( "Exclude Deferred", "exclude_path:deferred" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Exclude Forward", "exclude_path:forward" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Exclude Legacy Deferred", "exclude_path:prepass" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable shadows", "noshadow" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable Ambient Light", "noambient" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable Per Vertex Light", "novertexlights " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable all lightmaps", "nolightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable dynamic global GI", "nodynlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable directional lightmaps", "nodirlightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable built-in fog", "nofog" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Don't generate meta", "nometa" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable Add Pass", "noforwardadd " ) );
			m_textLabelWidth = 97;
			base.CommonInit( uniqueId );
			m_shaderNameContent = new GUIContent( ShaderNameStr, string.Empty );
		}

		public override void AddMasterPorts()
		{
			base.AddMasterPorts();
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT, false, MetallicStr );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionStr );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT3, false, SpecularStr );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionStr );
				}
				break;
				case StandardShaderLightModel.Lambert:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr );
				}
				break;
			}

			AddInputPort( WirePortDataType.FLOAT3, false, TransmissionStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_currentLightModel == StandardShaderLightModel.Standard ) || ( m_currentLightModel == StandardShaderLightModel.StandardSpecular ) ? false : true;

			AddInputPort( WirePortDataType.FLOAT3, false, TranslucencyStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_currentLightModel == StandardShaderLightModel.Standard ) || ( m_currentLightModel == StandardShaderLightModel.StandardSpecular ) ? false : true;

			AddInputPort( WirePortDataType.FLOAT, false, AlphaStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].DataName = AlphaDataStr;

			AddInputPort( WirePortDataType.OBJECT, false, DiscardStr );
			m_discardPortId = m_inputPorts.Count - 1;

			AddInputPort( WirePortDataType.OBJECT, false, VertexDisplacementStr, -1, MasterNodePortCategory.Vertex );

			AddInputPort( WirePortDataType.OBJECT, false, PerVertexDataStr, -1, MasterNodePortCategory.Vertex );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;

			AddInputPort( WirePortDataType.OBJECT, false, CustomLightModelStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;

			AddInputPort( WirePortDataType.FLOAT3, false, DebugStr );

			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].CustomColor = Color.white;
			}
			m_sizeIsDirty = true;
		}

		public void ForcePortType()
		{
			int portId = 0;
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.Lambert:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
			}

			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
		}

		public override void SetName( string name )
		{
			ShaderName = name;
		}

		public void DrawInspectorProperty()
		{
			if ( m_inspectorDefaultStyle == null )
			{
				m_inspectorDefaultStyle = UIUtils.CustomStyle( CustomStyle.ResetToDefaultInspectorButton );
			}

			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			m_customInspectorName = EditorGUILayout.TextField( CustomInspectorStr, m_customInspectorName );
			if ( GUILayout.Button( string.Empty, UIUtils.CustomStyle( CustomStyle.ResetToDefaultInspectorButton ) ) )
			{
				GUIUtility.keyboardControl = 0;
				m_customInspectorName = Constants.DefaultCustomInspector;
			}
			EditorGUILayout.EndHorizontal();
		}

		public void DrawRenderingPlatforms()
		{
			EditorGUILayout.Separator();
			m_renderingPlatformFoldout = GUILayout.Toggle( m_renderingPlatformFoldout, RenderingPlatformsStr, m_inspectorFoldoutStyle );
			if ( m_renderingPlatformFoldout )
			{
				EditorGUI.indentLevel++;
				for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
				{
					GUILayout.Space( 5 );
					m_renderingPlatformValues[ i ] = EditorGUILayout.ToggleLeft( RenderingPlatformsLabels[ i ], m_renderingPlatformValues[ i ] );
				}
				EditorGUI.indentLevel--;
			}
		}

		public void DrawColorMask()
		{
			if ( m_leftToggleColorMask == null )
			{
				m_leftToggleColorMask = new GUIStyle( GUI.skin.GetStyle( "ButtonLeft" ) );
			}

			if ( m_middleToggleColorMask == null )
			{
				m_middleToggleColorMask = new GUIStyle( GUI.skin.GetStyle( "ButtonMid" ) );
			}

			if ( m_rightToggleColorMask == null )
			{
				m_rightToggleColorMask = new GUIStyle( GUI.skin.GetStyle( "ButtonRight" ) );
			}

			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( ColorMaskContent, GUILayout.Width( 90 ) );

			m_colorMask[ 0 ] = GUILayout.Toggle( m_colorMask[ 0 ], "R", m_leftToggleColorMask );
			m_colorMask[ 1 ] = GUILayout.Toggle( m_colorMask[ 1 ], "G", m_middleToggleColorMask );
			m_colorMask[ 2 ] = GUILayout.Toggle( m_colorMask[ 2 ], "B", m_middleToggleColorMask );
			m_colorMask[ 3 ] = GUILayout.Toggle( m_colorMask[ 3 ], "A", m_rightToggleColorMask );

			EditorGUILayout.EndHorizontal();
		}

		void Swap( ref List<PropertyNode> list, int indexA, int indexB )
		{
			PropertyNode tmp = list[ indexA ];
			list[ indexA ] = list[ indexB ];
			list[ indexB ] = tmp;
		}

		public void DrawMaterialInputs()
		{
			EditorGUILayout.Separator();
			m_renderingShaderFoldout = GUILayout.Toggle( m_renderingShaderFoldout, PropertyOderFoldoutStr, m_inspectorFoldoutStyle );
			if ( !m_renderingShaderFoldout )
				return;

			List<ParentNode> nodes = UIUtils.PropertyNodesList();
			if ( m_propertyReordableList == null || nodes.Count != m_lastCount )
			{
				List<PropertyNode> propertyNodes = new List<PropertyNode>();
				for ( int i = 0; i < nodes.Count; i++ )
				{
					PropertyNode node = nodes[ i ] as PropertyNode;
					if ( node != null )
					{
						propertyNodes.Add( node );
					}
				}

				propertyNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_propertyReordableList = new ReorderableList( propertyNodes, typeof( PropertyNode ), true, false, false, false );
				m_propertyReordableList.headerHeight = 0;
				m_propertyReordableList.footerHeight = 0;
				m_propertyReordableList.showDefaultBackground = false;

				m_propertyReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, propertyNodes[ index ].PropertyInspectorName );
				};

				//m_propertyReordableList.drawHeaderCallback = rect =>
				//{
				//	EditorGUI.LabelField( rect, ShaderInputOrderStr );
				//};

				m_propertyReordableList.onChangedCallback = ( list ) =>
				{
					for ( int i = 0; i < propertyNodes.Count; i++ )
					{
						propertyNodes[ i ].OrderIndex = i;
					}
				};

				m_lastCount = m_propertyReordableList.count;
			}

			if ( m_propertyReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_propertyReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void SetRenderingPlatforms( ref string ShaderBody )
		{
			int checkedPlatforms = 0;
			int uncheckedPlatforms = 0;

			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				if ( m_renderingPlatformValues[ i ] )
				{
					checkedPlatforms += 1;
				}
				else
				{
					uncheckedPlatforms += 1;
				}
			}

			if ( checkedPlatforms > 0 && checkedPlatforms < m_renderingPlatformValues.Length )
			{
				string result = string.Empty;
				if ( checkedPlatforms < uncheckedPlatforms )
				{
					result = "only_renderers ";
					for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
					{
						if ( m_renderingPlatformValues[ i ] )
						{
							result += ( RenderPlatforms ) i + " ";
						}
					}
				}
				else
				{
					result = "exclude_renderers ";
					for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
					{
						if ( !m_renderingPlatformValues[ i ] )
						{
							result += ( RenderPlatforms ) i + " ";
						}
					}
				}
				AddShaderPragma( ref ShaderBody, result );
			}
		}

		public override void DrawProperties()
		{
			if ( m_inspectorFoldoutStyle == null )
				m_inspectorFoldoutStyle = new GUIStyle( GUI.skin.GetStyle( "foldout" ) );

			base.DrawProperties();
			m_buttonStyle.fixedWidth = 200;
			m_buttonStyle.fixedHeight = 50;

			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Separator();

				//GUI.SetNextControlName( _shaderNameTextfieldControlName ); // Give a name to the textfield control so we can know when the player is typing on it
				EditorGUI.BeginChangeCheck();

				string newShaderName = EditorGUILayout.TextField( m_shaderNameContent, m_shaderName, m_textfieldStyle );
				//if ( Event.current.isKey && GUI.GetNameOfFocusedControl().Equals( _shaderNameTextfieldControlName ) ) //if player is typing on this specific textfield
				if ( EditorGUI.EndChangeCheck() )
				{
					if ( newShaderName.Length > 0 )
					{
						newShaderName = UIUtils.RemoveShaderInvalidCharacters( newShaderName );
					}
					else
					{
						newShaderName = DefaultShaderName;
					}
					ShaderName = newShaderName;
				}
				m_shaderNameContent.tooltip = m_shaderName;

				EditorGUILayout.Separator();

				m_currentLightModel = ( StandardShaderLightModel ) EditorGUILayout.EnumPopup( LightModelContent, m_currentLightModel );

				EditorGUILayout.Separator();
				m_shaderModelIdx = EditorGUILayout.Popup( ShaderModelStr, m_shaderModelIdx, ShaderModelTypeArr );

				EditorGUILayout.Separator();
				EditorGUI.BeginChangeCheck();
				DrawPrecisionProperty();
				if ( EditorGUI.EndChangeCheck() )
					UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision = m_currentPrecisionType;

				EditorGUILayout.Separator();

				m_cullMode = ( CullMode ) EditorGUILayout.EnumPopup( CullModeContent, m_cullMode );


				EditorGUILayout.Separator();
				m_renderPath = ( RenderPath ) EditorGUILayout.EnumPopup( RenderPathContent, m_renderPath );

				EditorGUILayout.Separator();
				EditorGUI.BeginChangeCheck();
				m_alphaMode = ( AlphaMode ) EditorGUILayout.EnumPopup( AlphaModeContent, m_alphaMode );
				if ( EditorGUI.EndChangeCheck() )
				{
					m_customBlendMode = false;
					UpdateFromBlendMode();
				}

				EditorGUILayout.Separator();

				EditorGUI.BeginChangeCheck();

				m_renderType = ( RenderType ) EditorGUILayout.EnumPopup( RenderTypeContent, m_renderType );

				EditorGUILayout.Separator();

				m_renderQueue = ( RenderQueue ) EditorGUILayout.EnumPopup( RenderQueueContent, m_renderQueue );

				if ( EditorGUI.EndChangeCheck() )
				{
					m_customBlendMode = true;
				}

				EditorGUILayout.Separator();
				m_queueOrder = EditorGUILayout.IntField( QueueIndexContent, m_queueOrder );

				bool bufferedEnabled = GUI.enabled;

				GUI.enabled = ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode );
				m_inputPorts[ m_discardPortId ].Locked = !GUI.enabled;
				EditorGUILayout.Separator();
				EditorGUI.BeginChangeCheck();
				m_opacityMaskClipValue = EditorGUILayout.FloatField( OpacityMaskClipValueContent, m_opacityMaskClipValue );
				if ( EditorGUI.EndChangeCheck() )
				{
					if ( m_currentMaterial.HasProperty( IOUtils.MaskClipValueName ) )
					{
						m_currentMaterial.SetFloat( IOUtils.MaskClipValueName, m_opacityMaskClipValue );
					}
				}

				GUI.enabled = bufferedEnabled;

				EditorGUILayout.Separator();
				m_keepAlpha = EditorGUILayout.Toggle( KeepAlphaContent, m_keepAlpha );

				EditorGUILayout.Separator();
				m_castShadows = EditorGUILayout.Toggle( CastShadowsContent, m_castShadows );

				EditorGUILayout.Separator();
				m_receiveShadows = EditorGUILayout.Toggle( ReceiveShadowsContent, m_receiveShadows );

				DrawColorMask();
				DrawInspectorProperty();
				m_stencilBufferHelper.Draw();
				m_tessOpHelper.Draw( m_currentMaterial );
				m_zBufferHelper.Draw();
				DrawRenderingPlatforms();
				DrawMaterialInputs();

				//DrawShaderKeywords();
				//EditorGUILayout.Separator();

				//for ( int i = 0; i < _codeGenerationDataList.Count; i++ )
				//{
				//	_codeGenerationDataList[ i ].IsActive = EditorGUILayout.ToggleLeft( _codeGenerationDataList[ i ].Name, _codeGenerationDataList[ i ].IsActive );
				//}

			}
			EditorGUILayout.EndVertical();

			if ( m_currentLightModel != m_lastLightModel )
			{
				CacheCurrentSettings();
				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				ConnectFromCache();
			}
		}

		private void CacheCurrentSettings()
		{
			m_cacheNodeConnections.Clear();
			for ( int portId = 0; portId < m_inputPorts.Count; portId++ )
			{
				if ( m_inputPorts[ portId ].IsConnected )
				{
					WireReference connection = m_inputPorts[ portId ].GetConnection();
					m_cacheNodeConnections.Add( m_inputPorts[ portId ].Name, new NodeCache( connection.NodeId, connection.PortId ) );
				}
			}
		}

		private void ConnectFromCache()
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				NodeCache cache = m_cacheNodeConnections.Get( m_inputPorts[ i ].Name );
				if ( cache != null )
				{
					UIUtils.SetConnection( m_uniqueId, i, cache.TargetNodeId, cache.TargetPortId );
				}
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode )
			{
				if ( mat.HasProperty( IOUtils.MaskClipValueName ) )
					mat.SetFloat( IOUtils.MaskClipValueName, m_opacityMaskClipValue );
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode )
			{
				if ( m_materialMode && mat.HasProperty( IOUtils.MaskClipValueName ) )
				{
					m_opacityMaskClipValue = mat.GetFloat( IOUtils.MaskClipValueName );
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			m_tessOpHelper.UpdateFromMaterial( material );

			if ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode )
			{
				if ( material.HasProperty( IOUtils.MaskClipValueName ) )
					m_opacityMaskClipValue = material.GetFloat( IOUtils.MaskClipValueName );
			}
		}

		public override void UpdateMasterNodeMaterial( Material material )
		{
			m_currentMaterial = material;
			UpdateMaterialEditor();
		}

		void UpdateMaterialEditor()
		{
			FireMaterialChangedEvt();
		}

		public void CreateInstructionsForPort( InputPort port, string portName, bool addCustomDelimiters = false, string customDelimiterIn = null, string customDelimiterOut = null, bool ignoreLocalVar = false )
		{
			WireReference connection = port.GetConnection();
			ParentNode node = UIUtils.GetNode( connection.NodeId );

			string newInstruction = node.GetValueFromOutputStr( connection.PortId, port.DataType, ref UIUtils.CurrentDataCollector, ignoreLocalVar );

			if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
			{
				UIUtils.CurrentDataCollector.AddInstructions( UIUtils.CurrentDataCollector.SpecialLocalVariables );
				UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
			}

			if ( UIUtils.CurrentDataCollector.DirtyVertexVariables )
			{
				UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.VertexLocalVariables, port.NodeId, false );
				UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
			}

			if ( UIUtils.CurrentDataCollector.ForceNormal )
			{
				UIUtils.CurrentDataCollector.AddToStartInstructions( "\t\t\t" + Constants.OutputVarStr + ".Normal = float3(0,0,1);\n" );
				UIUtils.CurrentDataCollector.ForceNormal = false;
			}

			UIUtils.CurrentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterIn : ( "\t\t\t" + portName + " = " ) );
			UIUtils.CurrentDataCollector.AddInstructions( newInstruction );
			UIUtils.CurrentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterOut : ";\n" );
		}

		public void AddLocalVarInstructions( bool dirtyNormal )
		{
			UIUtils.CurrentDataCollector.DirtyNormal = dirtyNormal;
			List<ParentNode> localVarNodes = UIUtils.CurrentWindow.CurrentGraph.LocalVarNodes.NodesList;
			int count = localVarNodes.Count;

			List<RegisterLocalVarNode> sortedList = new List<RegisterLocalVarNode>( count );
			for ( int i = 0; i < count; i++ )
			{
				RegisterLocalVarNode node = localVarNodes[ i ] as RegisterLocalVarNode;
				sortedList.Add( node );
			}
			sortedList.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

			for ( int i = 0; i < count; i++ )
			{
				bool isVertex = UIUtils.GetCategoryInBitArray( sortedList[ i ].Category, MasterNodePortCategory.Vertex );
				bool isFragment = UIUtils.GetCategoryInBitArray( sortedList[ i ].Category, MasterNodePortCategory.Fragment );

				if ( isFragment )
				{
					UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
					string newInstruction = sortedList[ i ].CreateLocalVariable( 0, WirePortDataType.FLOAT, ref UIUtils.CurrentDataCollector, false );
					if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
					{
						UIUtils.CurrentDataCollector.AddInstructions( UIUtils.CurrentDataCollector.SpecialLocalVariables );
						UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
					}
					UIUtils.CurrentDataCollector.AddInstructions( newInstruction );
				}

				if ( isVertex )
				{
					UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Vertex;
					string newInstruction = sortedList[ i ].CreateLocalVariable( 0, WirePortDataType.FLOAT, ref UIUtils.CurrentDataCollector, false );
					if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
					{
						UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.SpecialLocalVariables, sortedList[ i ].UniqueId, false );
						UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
					}
					UIUtils.CurrentDataCollector.AddVertexInstruction( newInstruction, sortedList[ i ].UniqueId, false );
				}
			}
		}

		public void GenerateTags()
		{
			bool hasVirtualTexture = UIUtils.HasVirtualTexture();

			string tags = "\"RenderType\" = \"{0}\"  \"Queue\" = \"{1}\"";
			tags = string.Format( tags, m_renderType, ( m_renderQueue + ( ( m_queueOrder >= 0 ) ? "+" : string.Empty ) + m_queueOrder ) );
			//if ( !m_customBlendMode )
			{
				if ( m_alphaMode == AlphaMode.Fade || m_alphaMode == AlphaMode.Transparent )
				{
					tags += " \"IgnoreProjector\" = \"True\"";
				}
			}

			//add virtual texture support
			if ( hasVirtualTexture )
			{
				tags += " \"Amplify\" = \"True\" ";
			}

			tags = "Tags{ " + tags + " }";
		}

		public override Shader Execute( string pathname, bool isFullPath )
		{
			ForcePortType();
			base.Execute( pathname, isFullPath );

			bool isInstancedShader = UIUtils.IsInstancedShader();
			bool hasVirtualTexture = UIUtils.HasVirtualTexture();
			bool hasTranslucency = false;
			bool hasTransmission = false;

			UIUtils.CurrentDataCollector = new MasterNodeDataCollector( this );
			UIUtils.CurrentDataCollector.TesselationActive = m_tessOpHelper.EnableTesselation;

			// See if each node is being used on frag and/or vert ports
			SetupNodeCategories();


			string tags = "\"RenderType\" = \"{0}\"  \"Queue\" = \"{1}\"";
			tags = string.Format( tags, m_renderType, ( m_renderQueue + ( ( m_queueOrder >= 0 ) ? "+" : string.Empty ) + m_queueOrder ) );
			//if ( !m_customBlendMode )
			{
				if ( m_alphaMode == AlphaMode.Fade || m_alphaMode == AlphaMode.Transparent )
				{
					tags += " \"IgnoreProjector\" = \"True\"";
				}
			}

			//add virtual texture support
			if ( hasVirtualTexture )
			{
				tags += " \"Amplify\" = \"True\" ";
			}

			tags = "Tags{ " + tags + " }";

			string outputStruct = "";
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard: outputStruct = "SurfaceOutputStandard"; break;
				case StandardShaderLightModel.StandardSpecular: outputStruct = "SurfaceOutputStandardSpecular"; break;
				case StandardShaderLightModel.Lambert:
				case StandardShaderLightModel.BlinnPhong: outputStruct = "SurfaceOutput"; break;
			}

			// Need to sort before creating local vars so they can inspect the normal port correctly
			SortedList<int, InputPort> sortedPorts = new SortedList<int, InputPort>();
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				sortedPorts.Add( m_inputPorts[ i ].OrderId, m_inputPorts[ i ] );
			}

			// Register Local variables
			AddLocalVarInstructions( sortedPorts[ 0 ].IsConnected );

			if ( m_inputPorts[ m_inputPorts.Count - 1 ].IsConnected )
			{
				//Debug Port active
				UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Debug;
				InputPort debugPort = m_inputPorts[ m_inputPorts.Count - 1 ];
				CreateInstructionsForPort( debugPort, Constants.OutputVarStr + ".Emission", false, null, null, UIUtils.IsNormalDependent() );
			}
			else
			{
				// Custom Light Model
				//TODO: Create Custom Light behaviour

				//Collect data from standard nodes
				for ( int i = 0; i < sortedPorts.Count; i++ )
				{
					if ( sortedPorts[ i ].IsConnected )
					{
						if ( i == 0 )// Normal Map is Connected
						{
							UIUtils.CurrentDataCollector.DirtyNormal = true;
						}
						if ( m_inputPorts[ i ].Name.Equals( TranslucencyStr ) )
						{
							hasTranslucency = true;
						}
						if ( m_inputPorts[ i ].Name.Equals( TransmissionStr ) )
						{
							hasTransmission = true;
						}

						if ( hasTranslucency || hasTransmission )
						{
							//Translucency and Transmission Generation

							//Add properties and uniforms
							UIUtils.CurrentDataCollector.AddToIncludes( m_uniqueId, Constants.UnityPBSLightingLib );

							if ( hasTranslucency )
							{
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "[Header(Translucency)]", 100 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_Translucency(\"Strength\", Range( 0 , 50)) = 1", 101 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransNormalDistortion(\"Normal Distortion\", Range( 0 , 1)) = 0.1", 102 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransScattering(\"Scaterring Falloff\", Range( 1 , 50)) = 2", 103 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransDirect(\"Direct\", Range( 0 , 1)) = 1", 104 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransAmbient(\"Ambient\", Range( 0 , 1)) = 0.2", 105 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransShadow(\"Shadow\", Range( 0 , 1)) = 0.9", 106 );

								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _Translucency;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransNormalDistortion;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransScattering;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransDirect;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransAmbient;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransShadow;" );
							}

							//Add custom struct
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								case StandardShaderLightModel.StandardSpecular:
								outputStruct = "SurfaceOutput" + m_currentLightModel.ToString() + Constants.CustomLightStructStr; break;
							}

							UIUtils.CurrentDataCollector.ChangeCustomInputHeader( m_currentLightModel.ToString() + Constants.CustomLightStructStr );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Albedo", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Normal", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half3 Emission", true );
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half Metallic", true );
								break;
								case StandardShaderLightModel.StandardSpecular:
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Specular", true );
								break;
							}
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half Smoothness", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half Occlusion", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed Alpha", true );
							if ( hasTranslucency )
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Translucency", true );

							if ( hasTransmission )
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Transmission", true );
						}

						if ( m_inputPorts[ i ].Name.Equals( DiscardStr ) )
						{
							//Discard Op Node
							UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
							string opacityValue = "0.0";
							switch ( m_inputPorts[ i ].ConnectionType() )
							{
								case WirePortDataType.INT:
								case WirePortDataType.FLOAT:
								{
									opacityValue = IOUtils.MaskClipValueName;//UIUtils.FloatToString( m_opacityMaskClipValue );
								}
								break;

								case WirePortDataType.FLOAT2:
								{
									opacityValue = string.Format( "( {0} ).xx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT3:
								{
									opacityValue = string.Format( "( {0} ).xxx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT4:
								{
									opacityValue = string.Format( "( {0} ).xxxx", IOUtils.MaskClipValueName );
								}
								break;
							}
							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, true, "\t\t\tclip( ", " - " + opacityValue + " );\n" );
						}
						else if ( m_inputPorts[ i ].Name.Equals( VertexDisplacementStr ) )
						{
							//Vertex displacement and per vertex custom data
							UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Vertex;
							WireReference connection = m_inputPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							string vertexInstructions = node.GetValueFromOutputStr( connection.PortId, m_inputPorts[ i ].DataType, ref UIUtils.CurrentDataCollector, true );

							if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
							{
								UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.SpecialLocalVariables, m_uniqueId, false );
								UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
							}

							if ( UIUtils.CurrentDataCollector.DirtyVertexVariables )
							{
								UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.VertexLocalVariables, m_uniqueId, false );
								UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
							}

							

							UIUtils.CurrentDataCollector.AddToVertexDisplacement( vertexInstructions );
						}
						else
						{
							// Surface shader instruccions
							UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
							// if working on normals and have normal dependent node then ignore local var generation
							bool ignoreLocalVar = ( i == 0 && UIUtils.IsNormalDependent() );
							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, false, null, null, ignoreLocalVar );
						}
					}
					else if ( m_keepAlpha && sortedPorts[ i ].Name.Equals( AlphaStr ) )
					{
						UIUtils.CurrentDataCollector.AddInstructions( string.Format( "\t\t\t{0}.{1} = 1;\n", Constants.OutputVarStr, sortedPorts[ i ].DataName ) );
					}
				}
			}

			for ( int i = 0; i < 4; i++ )
			{
				if ( UIUtils.CurrentDataCollector.GetChannelUsage( i ) == TextureChannelUsage.Required )
				{
					string channelName = UIUtils.GetChannelName( i );
					UIUtils.CurrentDataCollector.AddToProperties( -1, UIUtils.GetTex2DProperty( channelName, TexturePropertyValues.white ), -1 );
				}
			}

			UIUtils.CurrentDataCollector.AddToProperties( -1, IOUtils.DefaultASEDirtyCheckProperty, -1 );
			if ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode )
			{
				UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( IOUtils.MaskClipValueProperty, OpacityMaskClipValueStr, m_opacityMaskClipValue ), -1 );
				UIUtils.CurrentDataCollector.AddToUniforms( -1, string.Format( IOUtils.MaskClipValueUniform, m_opacityMaskClipValue ) );
			}

			if ( !UIUtils.CurrentDataCollector.DirtyInputs )
				UIUtils.CurrentDataCollector.AddToInput( m_uniqueId, "fixed filler", true );

			if ( m_currentLightModel == StandardShaderLightModel.BlinnPhong )
				UIUtils.CurrentDataCollector.AddToProperties( -1, "[HideInInspector]_SpecColor(\"SpecularColor\",Color)=(1,1,1,1)", -1 );


			//Tesselation
			if ( m_tessOpHelper.EnableTesselation )
			{
				m_tessOpHelper.AddToDataCollector( ref UIUtils.CurrentDataCollector );
			}


			UIUtils.CurrentDataCollector.CloseInputs();
			UIUtils.CurrentDataCollector.CloseCustomInputs();
			UIUtils.CurrentDataCollector.CloseProperties();
			UIUtils.CurrentDataCollector.ClosePerVertexHeader();

			//build Shader Body
			string ShaderBody = string.Empty;
			OpenShaderBody( ref ShaderBody, m_shaderName );
			{
				//set properties
				if ( UIUtils.CurrentDataCollector.DirtyProperties )
				{
					ShaderBody += UIUtils.CurrentDataCollector.BuildPropertiesString();
				}
				//set subshader
				OpenSubShaderBody( ref ShaderBody );
				{
					//Add SubShader tags
					AddRenderTags( ref ShaderBody, tags );
					AddRenderState( ref ShaderBody, "Cull", m_cullMode.ToString() );
					if ( m_zBufferHelper.IsActive )
					{
						ShaderBody += m_zBufferHelper.CreateDepthInfo();
					}
					if ( m_stencilBufferHelper.Active )
					{
						ShaderBody += m_stencilBufferHelper.CreateStencilOp();
					}
					// Build Color Mask
					int count = 0;
					string colorMask = string.Empty;
					for ( int i = 0; i < m_colorMask.Length; i++ )
					{
						if ( m_colorMask[ i ] )
						{
							count++;
							colorMask += m_colorMaskChar[ i ];
						}
					}

					if ( count != m_colorMask.Length )
					{
						AddRenderState( ref ShaderBody, "ColorMask", ( ( count == 0 ) ? "0" : colorMask ) );
					}

					//ShaderBody += "\t\tZWrite " + _zWriteMode + '\n';
					//ShaderBody += "\t\tZTest " + _zTestMode + '\n';

					//Add GrabPass
					if ( UIUtils.CurrentDataCollector.DirtyGrabPass )
					{
						ShaderBody += UIUtils.CurrentDataCollector.GrabPass;
					}

					//add cg program
					OpenCGProgram( ref ShaderBody );
					{
						//Add Includes
						if ( UIUtils.CurrentDataCollector.DirtyIncludes )
							ShaderBody += UIUtils.CurrentDataCollector.Includes;

						//define as surface shader and specify lighting model
						ShaderBody += string.Format( IOUtils.PragmaTargetHeader, ShaderModelTypeArr[ m_shaderModelIdx ] );

						if ( isInstancedShader )
						{
							ShaderBody += IOUtils.InstancedPropertiesHeader;
						}

						//Add pragmas
						if ( UIUtils.CurrentDataCollector.DirtyPragmas )
							ShaderBody += UIUtils.CurrentDataCollector.Pragmas;

						// build optional parameters
						string OptionalParameters = string.Empty;
						//if ( !m_customBlendMode )
						{
							switch ( m_alphaMode )
							{
								case AlphaMode.Opaque:
								case AlphaMode.Masked: break;
								case AlphaMode.Fade:
								{
									OptionalParameters += "alpha:fade" + Constants.OptionalParametersSep;
								}
								break;
								case AlphaMode.Transparent:
								{
									OptionalParameters += "alpha:premul" + Constants.OptionalParametersSep;
								}
								break;
							}
						}

						if ( m_keepAlpha )
						{
							OptionalParameters += "keepalpha" + Constants.OptionalParametersSep;
						}

						OptionalParameters += ( ( m_castShadows ) ? "addshadow" + Constants.OptionalParametersSep + "fullforwardshadows" : "" ) + Constants.OptionalParametersSep;

						OptionalParameters += m_receiveShadows ? "" : "noshadow" + Constants.OptionalParametersSep;

						switch ( m_renderPath )
						{
							case RenderPath.All: break;
							case RenderPath.DeferredOnly: OptionalParameters += "exclude_path:forward" + Constants.OptionalParametersSep; break;
							case RenderPath.ForwardOnly: OptionalParameters += "exclude_path:deferred" + Constants.OptionalParametersSep; break;
						}

						string customLightSurface = hasTranslucency || hasTransmission ? "Custom" : "";
						SetRenderingPlatforms( ref ShaderBody );


						//Check if Custom Vertex is being used and add tag
						if ( UIUtils.CurrentDataCollector.DirtyPerVertexData )
							OptionalParameters += "vertex:" + Constants.VertexDataFunc + Constants.OptionalParametersSep;

						if ( m_tessOpHelper.EnableTesselation )
						{
							OptionalParameters += TessellationOpHelper.TessSurfParam + Constants.OptionalParametersSep;
						}

						AddShaderPragma( ref ShaderBody, "surface surf " + m_currentLightModel.ToString() + customLightSurface + Constants.OptionalParametersSep + OptionalParameters );

						//Add code generation options
						for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
						{
							if ( m_codeGenerationDataList[ i ].IsActive )
							{
								ShaderBody += m_codeGenerationDataList[ i ].Value + Constants.OptionalParametersSep;
							}
						}


						// Add Input struct
						if ( UIUtils.CurrentDataCollector.DirtyInputs )
							ShaderBody += UIUtils.CurrentDataCollector.Inputs + "\n\n";

						if ( m_tessOpHelper.EnableTesselation )
						{
							ShaderBody += TessellationOpHelper.CustomAppData;
						}

						// Add Custom Lighting struct
						if ( UIUtils.CurrentDataCollector.DirtyCustomInput )
							ShaderBody += UIUtils.CurrentDataCollector.CustomInput + "\n\n";

						//Add Uniforms
						if ( UIUtils.CurrentDataCollector.DirtyUniforms )
							ShaderBody += UIUtils.CurrentDataCollector.Uniforms + "\n";


						//Add Instanced Properties
						if ( isInstancedShader && UIUtils.CurrentDataCollector.DirtyInstancedProperties )
						{
							UIUtils.CurrentDataCollector.SetupInstancePropertiesBlock( UIUtils.RemoveInvalidCharacters( ShaderName ) );
							ShaderBody += UIUtils.CurrentDataCollector.InstancedProperties + "\n";
						}

						if ( UIUtils.CurrentDataCollector.DirtyFunctions )
							ShaderBody += UIUtils.CurrentDataCollector.Functions + "\n";


						//Tesselation
						if ( m_tessOpHelper.EnableTesselation )
						{
							ShaderBody += m_tessOpHelper.GetCurrentTessellationFunction + "\n";
						}

						//Add Custom Vertex Data
						if ( UIUtils.CurrentDataCollector.DirtyPerVertexData )
						{
							ShaderBody += UIUtils.CurrentDataCollector.VertexData;
						}


						//Add custom lighting function
						if ( hasTranslucency || hasTransmission )
						{
							ShaderBody += "\t\tinline half4 Lighting" + m_currentLightModel.ToString() + Constants.CustomLightStructStr + "(" + outputStruct + " " + Constants.CustomLightOutputVarStr + ", half3 viewDir, UnityGI gi )\n\t\t{\n";
							if ( hasTranslucency )
							{
								ShaderBody += "\t\t\t#if !DIRECTIONAL\n";
								ShaderBody += "\t\t\tfloat3 lightAtten = gi.light.color;\n";
								ShaderBody += "\t\t\t#else\n";
								ShaderBody += "\t\t\tfloat3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );\n";
								ShaderBody += "\t\t\t#endif\n";
								ShaderBody += "\t\t\thalf3 lightDir = gi.light.dir + " + Constants.CustomLightOutputVarStr + ".Normal * _TransNormalDistortion;\n";
								ShaderBody += "\t\t\thalf transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );\n";
								ShaderBody += "\t\t\thalf3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * " + Constants.CustomLightOutputVarStr + ".Translucency;\n";
								ShaderBody += "\t\t\thalf4 c = half4( " + Constants.CustomLightOutputVarStr + ".Albedo * translucency * _Translucency, 0 );\n\n";
							}

							if ( hasTransmission )
							{
								ShaderBody += "\t\t\thalf3 transmission = max(0 , -dot(" + Constants.CustomLightOutputVarStr + ".Normal, gi.light.dir)) * gi.light.color * " + Constants.CustomLightOutputVarStr + ".Transmission;\n";
								ShaderBody += "\t\t\thalf4 d = half4(" + Constants.CustomLightOutputVarStr + ".Albedo * transmission , 0);\n\n";
							}

							ShaderBody += "\t\t\tSurfaceOutput" + m_currentLightModel.ToString() + " r;\n";
							ShaderBody += "\t\t\tr.Albedo = " + Constants.CustomLightOutputVarStr + ".Albedo;\n";
							ShaderBody += "\t\t\tr.Normal = " + Constants.CustomLightOutputVarStr + ".Normal;\n";
							ShaderBody += "\t\t\tr.Emission = " + Constants.CustomLightOutputVarStr + ".Emission;\n";
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								ShaderBody += "\t\t\tr.Metallic = " + Constants.CustomLightOutputVarStr + ".Metallic;\n";
								break;
								case StandardShaderLightModel.StandardSpecular:
								ShaderBody += "\t\t\tr.Specular = " + Constants.CustomLightOutputVarStr + ".Specular;\n";
								break;
							}
							ShaderBody += "\t\t\tr.Smoothness = " + Constants.CustomLightOutputVarStr + ".Smoothness;\n";
							ShaderBody += "\t\t\tr.Occlusion = " + Constants.CustomLightOutputVarStr + ".Occlusion;\n";
							ShaderBody += "\t\t\tr.Alpha = " + Constants.CustomLightOutputVarStr + ".Alpha;\n";
							ShaderBody += "\t\t\treturn Lighting" + m_currentLightModel.ToString() + " (r, viewDir, gi)" + ( hasTranslucency ? " + c" : "" ) + ( hasTransmission ? " + d" : "" ) + ";\n";
							ShaderBody += "\t\t}\n\n";

							//Add GI function
							ShaderBody += "\t\tinline void Lighting" + m_currentLightModel.ToString() + Constants.CustomLightStructStr + "_GI(" + outputStruct + " " + Constants.CustomLightOutputVarStr + ", UnityGIInput data, inout UnityGI gi )\n\t\t{\n";
							ShaderBody += "\t\t\tUNITY_GI(gi, " + Constants.CustomLightOutputVarStr + ", data);\n";
							ShaderBody += "\t\t}\n\n";
						}

						//Add Surface Shader body
						ShaderBody += "\t\tvoid surf( Input " + Constants.InputVarStr + " , inout " + outputStruct + " " + Constants.OutputVarStr + " )\n\t\t{\n";
						{
							//add local vars
							if ( UIUtils.CurrentDataCollector.DirtyLocalVariables )
								ShaderBody += UIUtils.CurrentDataCollector.LocalVariables;

							//add nodes ops
							ShaderBody += UIUtils.CurrentDataCollector.Instructions;
						}
						ShaderBody += "\t\t}\n";
					}
					CloseCGProgram( ref ShaderBody );
				}

				CloseSubShaderBody( ref ShaderBody );
				AddShaderProperty( ref ShaderBody, "Fallback", "Diffuse" );
				if ( !string.IsNullOrEmpty( m_customInspectorName ) )
				{
					AddShaderProperty( ref ShaderBody, "CustomEditor", m_customInspectorName );
				}
			}
			CloseShaderBody( ref ShaderBody );

			// Generate Graph info
			ShaderBody += UIUtils.CurrentWindow.GenerateGraphInfo();

			//TODO: Remove current SaveDebugShader and uncomment SaveToDisk as soon as pathname is editable
			if ( !String.IsNullOrEmpty( pathname ) )
			{
				IOUtils.StartSaveThread( ShaderBody, ( isFullPath ? pathname : ( IOUtils.dataPath + pathname ) ) );
			}
			else
			{
				IOUtils.StartSaveThread( ShaderBody, Application.dataPath + "/AmplifyShaderEditor/Samples/Shaders/" + m_shaderName + ".shader" );
			}

			// Load new shader into material

			if ( CurrentShader == null )
			{
				AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				CurrentShader = Shader.Find( ShaderName );
			}
			else
			{
				// need to always get asset datapath because a user can change and asset location from the project window 
				AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentShader ) );
			}

			if ( m_currentShader != null )
			{
				if ( m_currentMaterial != null )
				{
					m_currentMaterial.shader = m_currentShader;
					UIUtils.CurrentDataCollector.UpdateMaterialOnPropertyNodes( m_currentMaterial );
					UpdateMaterialEditor();
					// need to always get asset datapath because a user can change and asset location from the project window
					AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentMaterial ) );
				}

				UIUtils.CurrentDataCollector.UpdateShaderOnPropertyNodes( ref m_currentShader );
			}

			UIUtils.CurrentDataCollector.Destroy();
			UIUtils.CurrentDataCollector = null;

			return m_currentShader;
		}

		public override void UpdateFromShader( Shader newShader )
		{
			if ( m_currentMaterial != null )
			{
				m_currentMaterial.shader = newShader;
			}
			CurrentShader = newShader;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_codeGenerationDataList.Clear();
			m_codeGenerationDataList = null;
			m_inspectorDefaultStyle = null;
			m_inspectorFoldoutStyle = null;
			m_propertyAdjustment = null;
			m_zBufferHelper = null;
			m_stencilBufferHelper = null;
			m_tessOpHelper = null;
		}

		public override int VersionConvertInputPortId( int portId )
		{
			int newPort = portId;
			//added translucency input after occlusion
			if ( UIUtils.CurrentShaderVersion() < 2404 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 6 )
						newPort = newPort + 1;
					break;
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 5 )
						newPort = newPort + 1;
					break;
				}

			}

			//added transmission input after translucency
			if ( UIUtils.CurrentShaderVersion() < 2407 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 6 )
						newPort = newPort + 1;
					break;
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 5 )
						newPort = newPort + 1;
					break;
				}

			}
			return newPort;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			try
			{
				base.ReadFromString( ref nodeParams );
				m_currentLightModel = ( StandardShaderLightModel ) Enum.Parse( typeof( StandardShaderLightModel ), GetCurrentParam( ref nodeParams ) );
				//if ( _shaderCategory.Length > 0 )
				//	_shaderCategory = UIUtils.RemoveInvalidCharacters( _shaderCategory );
				ShaderName = GetCurrentParam( ref nodeParams );
				if ( m_shaderName.Length > 0 )
					ShaderName = UIUtils.RemoveShaderInvalidCharacters( ShaderName );

				for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
				{
					m_codeGenerationDataList[ i ].IsActive = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				}

				m_cullMode = ( CullMode ) Enum.Parse( typeof( CullMode ), GetCurrentParam( ref nodeParams ) );
				m_zBufferHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );

				m_alphaMode = ( AlphaMode ) Enum.Parse( typeof( AlphaMode ), GetCurrentParam( ref nodeParams ) );
				m_opacityMaskClipValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
				m_keepAlpha = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_castShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_queueOrder = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if ( UIUtils.CurrentShaderVersion() > 11 )
				{
					m_customBlendMode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_renderType = ( RenderType ) Enum.Parse( typeof( RenderType ), GetCurrentParam( ref nodeParams ) );
					m_renderQueue = ( RenderQueue ) Enum.Parse( typeof( RenderQueue ), GetCurrentParam( ref nodeParams ) );
				}
				if ( UIUtils.CurrentShaderVersion() > 2402 )
				{
					m_renderPath = ( RenderPath ) Enum.Parse( typeof( RenderPath ), GetCurrentParam( ref nodeParams ) );
				}
				if ( UIUtils.CurrentShaderVersion() > 2405 )
				{
					for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
					{
						m_renderingPlatformValues[ i ] = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					}
				}

				if ( UIUtils.CurrentShaderVersion() > 2500 )
				{
					for ( int i = 0; i < m_colorMask.Length; i++ )
					{
						m_colorMask[ i ] = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					}
				}


				if ( UIUtils.CurrentShaderVersion() > 2501 )
				{
					m_stencilBufferHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2504 )
				{
					m_tessOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
					m_receiveShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				}

				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				m_customBlendMode = TestCustomBlendMode();

				UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision = m_currentPrecisionType;
			}
			catch ( Exception e )
			{
				Debug.Log( e );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentLightModel );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderName );
			for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_codeGenerationDataList[ i ].IsActive );
			}

			IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
			m_zBufferHelper.WriteToString( ref nodeInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_alphaMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_opacityMaskClipValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_keepAlpha );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_castShadows );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_queueOrder );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customBlendMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderQueue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderPath );
			for ( int i = 0; i < m_renderingPlatformValues.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_renderingPlatformValues[ i ] );
			}

			for ( int i = 0; i < m_colorMask.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_colorMask[ i ] );
			}
			m_stencilBufferHelper.WriteToString( ref nodeInfo );
			m_tessOpHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_receiveShadows );
		}

		private bool TestCustomBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Geometry )
						return false;
				}
				break;
				case AlphaMode.Masked:
				{
					if ( m_renderType == RenderType.TransparentCutout && m_renderQueue == RenderQueue.AlphaTest )
						return false;
				}
				break;
				case AlphaMode.Fade:
				case AlphaMode.Transparent:
				{
					if ( m_renderType == RenderType.Transparent && m_renderQueue == RenderQueue.Transparent )
						return false;
				}
				break;
			}
			return true;
		}

		private void UpdateFromBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					m_renderType = RenderType.Opaque;
					m_renderQueue = RenderQueue.Geometry;
				}
				break;
				case AlphaMode.Masked:
				{
					m_renderType = RenderType.TransparentCutout;
					m_renderQueue = RenderQueue.AlphaTest;
				}
				break;
				case AlphaMode.Fade:
				case AlphaMode.Transparent:
				{
					m_renderType = RenderType.Transparent;
					m_renderQueue = RenderQueue.Transparent;
				}
				break;
			}
		}

		public StandardShaderLightModel CurrentLightingModel { get { return m_currentLightModel; } }
	}
}
