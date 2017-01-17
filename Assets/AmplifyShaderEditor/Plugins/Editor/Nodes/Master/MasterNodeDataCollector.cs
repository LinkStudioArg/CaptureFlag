// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class PropertyDataCollector
	{
		public int NodeId;
		public int OrderIndex;
		public string PropertyName;

		public PropertyDataCollector( int nodeId, string propertyName, int orderIndex = -1 )
		{
			NodeId = nodeId;
			PropertyName = propertyName;
			OrderIndex = orderIndex;
		}
	}

	public class TextureDefaultsDataColector
	{
		private List<string> m_names = new List<string>();
		private List<Texture> m_values = new List<Texture>();
		public void AddValue( string newName, Texture newValue )
		{
			m_names.Add( newName );
			m_values.Add( newValue );
		}

		public void Destroy()
		{
			m_names.Clear();
			m_names = null;

			m_values.Clear();
			m_values = null;
		}

		public string[] NamesArr { get { return m_names.ToArray(); } }
		public Texture[] ValuesArr { get { return m_values.ToArray(); } }
	}

	public enum TextureChannelUsage
	{
		Not_Used,
		Used,
		Required
	}

	public class MasterNodeDataCollector
	{
		private bool ShowDebugMessages = false;
		private string m_input;
		private string m_customInput;
		private string m_properties;
		private string m_instancedProperties;
		private string m_uniforms;
		private string m_includes;
		private string m_pragmas;
		private string m_instructions;
		private string m_localVariables;
		private string m_vertexLocalVariables;
		private string m_specialLocalVariables;
		private string m_vertexData;
		private string m_functions;
		private string m_grabPass;

		private Dictionary<string, PropertyDataCollector> m_inputDict;
		private Dictionary<string, PropertyDataCollector> m_customInputDict;
		private Dictionary<string, PropertyDataCollector> m_propertiesDict;
		private Dictionary<string, PropertyDataCollector> m_instancedPropertiesDict;
		private Dictionary<string, PropertyDataCollector> m_uniformsDict;
		private Dictionary<string, PropertyDataCollector> m_includesDict;
		private Dictionary<string, PropertyDataCollector> m_pragmasDict;
		private Dictionary<string, int> m_virtualCoordinatesDict;
		private Dictionary<string, PropertyDataCollector> m_localVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_vertexLocalVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_specialLocalVariablesDict;
		private Dictionary<string, PropertyDataCollector> m_vertexDataDict;
		private Dictionary<string, string> m_localFunctions;
		private TextureChannelUsage[] m_requireTextureProperty = { TextureChannelUsage.Not_Used, TextureChannelUsage.Not_Used, TextureChannelUsage.Not_Used, TextureChannelUsage.Not_Used };

		private bool m_dirtyInputs;
		private bool m_dirtyCustomInputs;
		private bool m_dirtyFunctions;
		private bool m_dirtyProperties;
		private bool m_dirtyInstancedProperties;
		private bool m_dirtyUniforms;
		private bool m_dirtyIncludes;
		private bool m_dirtyPragmas;
		private bool m_dirtyInstructions;
		private bool m_dirtyLocalVariables;
		private bool m_dirtyVertexLocalVariables;
		private bool m_dirtySpecialLocalVariables;
		private bool m_dirtyPerVertexData;
		private bool m_dirtyNormal;
		private bool m_forceNormal;
		private bool m_forceNormalIsDirty;
		private bool m_grabPassIsDirty;
		private bool m_tesselationActive;

		private List<PropertyNode> m_propertyNodes;
		private MasterNode m_masterNode;

		private int m_availableUvInd = 0;

		private MasterNodePortCategory m_portCategory;

		public MasterNodeDataCollector( MasterNode masterNode )
		{
			m_masterNode = masterNode;
			m_input = "\t\tstruct Input\n\t\t{\n";
			m_customInput = "\t\tstruct SurfaceOutput{0}\n\t\t{\n";
			m_properties = IOUtils.PropertiesBegin;//"\tProperties\n\t{\n";
			m_uniforms = "";
			m_instructions = "";
			m_includes = "";
			m_pragmas = "";
			m_localVariables = "";
			m_specialLocalVariables = "";

			m_inputDict = new Dictionary<string, PropertyDataCollector>();
			m_customInputDict = new Dictionary<string, PropertyDataCollector>();

			m_propertiesDict = new Dictionary<string, PropertyDataCollector>();
			m_instancedPropertiesDict = new Dictionary<string, PropertyDataCollector>();
			m_uniformsDict = new Dictionary<string, PropertyDataCollector>();
			m_includesDict = new Dictionary<string, PropertyDataCollector>();
			m_pragmasDict = new Dictionary<string, PropertyDataCollector>();
			m_virtualCoordinatesDict = new Dictionary<string, int>();
			m_localVariablesDict = new Dictionary<string, PropertyDataCollector>();
			m_specialLocalVariablesDict = new Dictionary<string, PropertyDataCollector>();
			m_vertexLocalVariablesDict = new Dictionary<string, PropertyDataCollector>();
			m_localFunctions = new Dictionary<string, string>();
			m_vertexDataDict = new Dictionary<string, PropertyDataCollector>();

			m_dirtyInputs = false;
			m_dirtyCustomInputs = false;
			m_dirtyProperties = false;
			m_dirtyInstancedProperties = false;
			m_dirtyUniforms = false;
			m_dirtyInstructions = false;
			m_dirtyIncludes = false;
			m_dirtyPragmas = false;
			m_dirtyLocalVariables = false;
			m_dirtySpecialLocalVariables = false;
			m_grabPassIsDirty = false;

			m_portCategory = MasterNodePortCategory.Fragment;
			m_propertyNodes = new List<PropertyNode>();
		}

		public void SetChannelUsage( int channelId, TextureChannelUsage usage )
		{
			if ( channelId > -1 && channelId < 4 )
				m_requireTextureProperty[ channelId ] = usage;
		}

		public TextureChannelUsage GetChannelUsage( int channelId )
		{
			if ( channelId > -1 && channelId < 4 )
				return m_requireTextureProperty[ channelId ];

			return TextureChannelUsage.Not_Used;
		}

		public void OpenPerVertexHeader( bool includeCustomData )
		{
			if ( m_dirtyPerVertexData )
				return;

			m_dirtyPerVertexData = true;
			if ( m_tesselationActive )
			{
				m_vertexData = "\t\tvoid " + Constants.VertexDataFunc + "( inout appdata " + Constants.VertexShaderInputStr+ " )\n\t\t{\n";
			}
			else
			{
				m_vertexData = "\t\tvoid " + Constants.VertexDataFunc + "( inout appdata_full " + Constants.VertexShaderInputStr + ( includeCustomData ? ( string.Format( ", out Input {0}", Constants.VertexShaderOutputStr ) ) : string.Empty ) + " )\n\t\t{\n";
				if ( includeCustomData )
					m_vertexData += string.Format( "\t\t\tUNITY_INITIALIZE_OUTPUT( Input, {0} );\n", Constants.VertexShaderOutputStr );
			}
		}

		public void ClosePerVertexHeader()
		{
			if ( m_dirtyPerVertexData )
				m_vertexData += "\t\t}\n\n";
		}

		public void AddToVertexDisplacement( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_dirtyPerVertexData )
			{
				OpenPerVertexHeader( true );
			}

			m_vertexData += "\t\t\t" + Constants.VertexShaderInputStr + ".vertex.xyz += " + value + ";\n";
		}

		public void AddVertexInstruction( string value, int nodeId, bool addDelimiters = true )
		{
			if ( !m_dirtyPerVertexData )
			{
				OpenPerVertexHeader( true );
			}
			if ( !m_vertexDataDict.ContainsKey( value ) )
			{
				m_vertexDataDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_vertexData += ( addDelimiters ? ( "\t\t\t" + value + ";\n" ) : value );
			}
		}

		public bool ContainsInput( string value )
		{
			return m_inputDict.ContainsKey( value );
		}

		public void AddToInput( int nodeId, string value, bool addSemiColon )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_inputDict.ContainsKey( value ) )
			{
				m_inputDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_input += "\t\t\t" + value + ( ( addSemiColon ) ? ( ";\n" ) : "\n" );
				m_dirtyInputs = true;
			}
		}

		public void CloseInputs()
		{
			m_input += "\t\t};";
		}

		public void ChangeCustomInputHeader( string value )
		{
			m_customInput = m_customInput.Replace( "{0}", value );
		}

		public void AddToCustomInput( int nodeId, string value, bool addSemiColon )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_customInputDict.ContainsKey( value ) )
			{
				m_customInputDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_customInput += "\t\t\t" + value + ( ( addSemiColon ) ? ( ";\n" ) : "\n" );
				m_dirtyCustomInputs = true;
			}
		}

		public void CloseCustomInputs()
		{
			m_customInput += "\t\t};";
		}

		// Instanced properties
		public void SetupInstancePropertiesBlock( string blockName )
		{
			if ( m_dirtyInstancedProperties )
			{
				m_instancedProperties = string.Format( IOUtils.InstancedPropertiesBegin, blockName ) + m_instancedProperties + IOUtils.InstancedPropertiesEnd;
			}
		}

		public void AddToInstancedProperties( int nodeId, string value, int orderIndex )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_instancedPropertiesDict.ContainsKey( value ) )
			{
				m_instancedPropertiesDict.Add( value, new PropertyDataCollector( nodeId, value, orderIndex ) );
				m_instancedProperties += value;
				m_dirtyInstancedProperties = true;
			}
		}

		public void CloseInstancedProperties()
		{
			if ( m_dirtyInstancedProperties )
			{
				m_instancedProperties += IOUtils.InstancedPropertiesEnd;
			}
		}

		// Properties

		public void AddToProperties( int nodeId, string value, int orderIndex )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_propertiesDict.ContainsKey( value ) )
			{
				m_propertiesDict.Add( value, new PropertyDataCollector( nodeId, value, orderIndex ) );
				m_properties += string.Format( IOUtils.PropertiesElement, value );
				m_dirtyProperties = true;
			}
		}

		public string BuildPropertiesString()
		{
			List<PropertyDataCollector> list = new List<PropertyDataCollector>( m_propertiesDict.Values );
			list.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
			m_properties = IOUtils.PropertiesBegin;
			for ( int i = 0; i < list.Count; i++ )
			{
				m_properties += string.Format( IOUtils.PropertiesElement, list[ i ].PropertyName );
			}
			m_properties += IOUtils.PropertiesEnd;
			return m_properties;
		}

		public void CloseProperties()
		{
			if ( m_dirtyProperties )
			{
				m_properties += IOUtils.PropertiesEnd;
			}
		}

		public void AddGrabPass( string value )
		{
			m_grabPassIsDirty = true;
			if ( string.IsNullOrEmpty( value ) )
			{
				m_grabPass = IOUtils.GrabPassEmpty;
			}
			else
			{
				m_grabPass = IOUtils.GrabPassBegin + value + IOUtils.GrabPassEnd;
			}
		}

		public void AddToUniforms( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_uniformsDict.ContainsKey( value ) )
			{
				m_uniforms += "\t\t" + value + '\n';
				m_uniformsDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_dirtyUniforms = true;
			}
			else if ( m_uniformsDict[ value ].NodeId != nodeId )
			{
				if ( ShowDebugMessages ) UIUtils.ShowMessage( "AddToUniforms:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void AddToIncludes( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_includesDict.ContainsKey( value ) )
			{
				m_includesDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_includes += "\t\t#include \"" + value + "\"\n";
				m_dirtyIncludes = true;
			}
			else
			{
				if ( ShowDebugMessages ) UIUtils.ShowMessage( "AddToIncludes:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void AddToPragmas( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_pragmasDict.ContainsKey( value ) )
			{
				m_pragmasDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_pragmas += "\t\t#pragma " + value + "\n";
				m_dirtyPragmas = true;
			}
			else
			{
				if ( ShowDebugMessages ) UIUtils.ShowMessage( "AddToPragmas:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public int GetVirtualCoordinatesId( int nodeId, string coord, string lodBias )
		{
			if ( !m_virtualCoordinatesDict.ContainsKey( coord ) )
			{
				m_virtualCoordinatesDict.Add( coord, nodeId );
				AddInstructions( "\t\t\tVirtualCoord " + Constants.VirtualCoordNameStr + nodeId + " = VTComputeVirtualCoord" + lodBias + "(" + coord + ");\n" );
				return nodeId;
			}
			else
			{
				int fetchedId = 0;
				m_virtualCoordinatesDict.TryGetValue( coord, out fetchedId );
				return fetchedId;
			}
		}

		public bool AddToLocalVariables( int nodeId, PrecisionType precisionType, WirePortDataType type, string varName, string varValue )
		{
			if ( string.IsNullOrEmpty( varName ) || string.IsNullOrEmpty( varValue ) )
				return false;

			string value = UIUtils.PrecisionWirePortToCgType( precisionType, type ) + " " + varName + " = " + varValue + ";";
			return AddToLocalVariables( nodeId, value );
		}

		public bool AddToLocalVariables( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return false;

			if ( !m_localVariablesDict.ContainsKey( value ) )
			{
				m_localVariablesDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				AddToSpecialLocalVariables( nodeId, value );
				return true;
			}
			else
			{
				if ( ShowDebugMessages ) UIUtils.ShowMessage( "AddToLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
			return false;
		}

		public void AddToSpecialLocalVariables( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_specialLocalVariablesDict.ContainsKey( value ) )
			{
				m_specialLocalVariablesDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_specialLocalVariables += "\t\t\t" + value + '\n';
				m_dirtySpecialLocalVariables = true;
			}
			else
			{
				if ( ShowDebugMessages ) UIUtils.ShowMessage( "AddToSpecialLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void ClearSpecialLocalVariables()
		{
			m_specialLocalVariablesDict.Clear();
			m_specialLocalVariables = string.Empty;
			m_dirtySpecialLocalVariables = false;
		}


		public void AddToVertexLocalVariables( int nodeId, string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			if ( !m_vertexLocalVariablesDict.ContainsKey( value ) )
			{
				m_vertexLocalVariablesDict.Add( value, new PropertyDataCollector( nodeId, value ) );
				m_vertexLocalVariables += "\t\t\t" + value + '\n';
				m_dirtyVertexLocalVariables = true;
			}
			else
			{
				if ( ShowDebugMessages ) UIUtils.ShowMessage( "AddToVertexLocalVariables:Attempting to add duplicate " + value, MessageSeverity.Warning );
			}
		}

		public void ClearVertexLocalVariables()
		{
			m_vertexLocalVariablesDict.Clear();
			m_vertexLocalVariables = string.Empty;
			m_dirtyVertexLocalVariables = false;
		}


		public bool CheckFunction( string header )
		{
			return m_localFunctions.ContainsKey( header );
		}

		public string AddFunctions( string header, string body, params object[] inParams )
		{
			if ( !m_localFunctions.ContainsKey( header ) )
			{
				m_localFunctions.Add( header, body );
				m_functions += "\n" + body + "\n";
				m_dirtyFunctions = true;
			}

			return String.Format( header, inParams );
		}

		public void AddInstructions( string value )
		{
			m_instructions += value;
			m_dirtyInstructions = true;
		}

		public void AddToStartInstructions( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;

			m_instructions = value + m_instructions;
			m_dirtyInstructions = true;
		}

		public void AddPropertyNode( PropertyNode node )
		{
			m_propertyNodes.Add( node );
		}

		public void UpdateMaterialOnPropertyNodes( Material material )
		{
			m_masterNode.UpdateMaterial( material );
			for ( int i = 0; i < m_propertyNodes.Count; i++ )
				m_propertyNodes[ i ].UpdateMaterial( material );
		}

		public void UpdateShaderOnPropertyNodes( ref Shader shader )
		{
			if ( m_propertyNodes.Count == 0 )
				return;

			try
			{
				bool hasContents = false;
				//string metaNewcontents = IOUtils.LINE_TERMINATOR.ToString();
				TextureDefaultsDataColector defaultCol = new TextureDefaultsDataColector();
				for ( int i = 0; i < m_propertyNodes.Count; i++ )
				{
					hasContents = m_propertyNodes[ i ].UpdateShaderDefaults( ref shader, ref defaultCol ) || hasContents;
				}

				if ( hasContents )
				{
					ShaderImporter importer = ( ShaderImporter ) ShaderImporter.GetAtPath( AssetDatabase.GetAssetPath( shader ) );
					importer.SetDefaultTextures( defaultCol.NamesArr, defaultCol.ValuesArr );
					defaultCol.Destroy();
					defaultCol = null;
					//string metaFilepath = AssetDatabase.GetTextMetaFilePathFromAssetPath( AssetDatabase.GetAssetPath( shader ) );
					//string metaContents = IOUtils.LoadTextFileFromDisk( metaFilepath );

					//int startIndex = metaContents.IndexOf( IOUtils.MetaBegin );
					//int endIndex = metaContents.IndexOf( IOUtils.MetaEnd );

					//if ( startIndex > 0 && endIndex > 0 )
					//{
					//	startIndex += IOUtils.MetaBegin.Length;
					//	string replace = metaContents.Substring( startIndex, ( endIndex - startIndex ) );
					//	if ( hasContents )
					//	{
					//		metaContents = metaContents.Replace( replace, metaNewcontents );
					//	}
					//}
					//IOUtils.SaveTextfileToDisk( metaContents, metaFilepath, false );
				}
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}
		}

		public void Destroy()
		{
			m_masterNode = null;

			m_propertyNodes.Clear();
			m_propertyNodes = null;

			m_inputDict.Clear();
			m_inputDict = null;

			m_customInputDict.Clear();
			m_customInputDict = null;

			m_propertiesDict.Clear();
			m_propertiesDict = null;

			m_instancedPropertiesDict.Clear();
			m_instancedPropertiesDict = null;

			m_uniformsDict.Clear();
			m_uniformsDict = null;

			m_includesDict.Clear();
			m_includesDict = null;

			m_pragmasDict.Clear();
			m_pragmasDict = null;

			m_virtualCoordinatesDict.Clear();
			m_virtualCoordinatesDict = null;

			m_localVariablesDict.Clear();
			m_localVariablesDict = null;

			m_specialLocalVariablesDict.Clear();
			m_specialLocalVariablesDict = null;

			m_vertexLocalVariablesDict.Clear();
			m_vertexLocalVariablesDict = null;

			m_localFunctions.Clear();
			m_localFunctions = null;

			m_vertexDataDict.Clear();
			m_vertexDataDict = null;
		}

		public string Inputs { get { return m_input; } }
		public string CustomInput { get { return m_customInput; } }
		public string Properties { get { return m_properties; } }
		public string InstancedProperties { get { return m_instancedProperties; } }
		public string Uniforms { get { return m_uniforms; } }
		public string Instructions { get { return m_instructions; } }
		public string Includes { get { return m_includes; } }
		public string Pragmas { get { return m_pragmas; } }
		public string LocalVariables { get { return m_localVariables; } }
		public string SpecialLocalVariables { get { return m_specialLocalVariables; } }
		public string VertexLocalVariables { get { return m_vertexLocalVariables; } }
		public string VertexData { get { return m_vertexData; } }
		public string Functions { get { return m_functions; } }
		public string GrabPass { get { return m_grabPass; } }
		public bool DirtyInstructions { get { return m_dirtyInstructions; } }
		public bool DirtyUniforms { get { return m_dirtyUniforms; } }
		public bool DirtyProperties { get { return m_dirtyProperties; } }
		public bool DirtyInstancedProperties { get { return m_dirtyInstancedProperties; } }
		public bool DirtyInputs { get { return m_dirtyInputs; } }
		public bool DirtyCustomInput { get { return m_dirtyCustomInputs; } }
		public bool DirtyIncludes { get { return m_dirtyIncludes; } }
		public bool DirtyPragmas { get { return m_dirtyPragmas; } }
		public bool DirtyLocalVariables { get { return m_dirtyLocalVariables; } }
		public bool DirtyVertexVariables { get { return m_dirtyVertexLocalVariables; } }
		public bool DirtySpecialLocalVariables { get { return m_dirtySpecialLocalVariables; } }
		public bool DirtyPerVertexData { get { return m_dirtyPerVertexData; } }
		public bool DirtyFunctions { get { return m_dirtyFunctions; } }
		public bool DirtyGrabPass { get { return m_grabPassIsDirty; } }
		public int LocalVariablesAmount { get { return m_localVariablesDict.Count; } }
		public int SpecialLocalVariablesAmount { get { return m_specialLocalVariablesDict.Count; } }
		public int VertexLocalVariablesAmount { get { return m_vertexLocalVariablesDict.Count; } }
		public int AvailableUvIndex { get { return m_availableUvInd++; } }
		public bool TesselationActive { set { m_tesselationActive = value; } }

		public bool DirtyNormal
		{
			get { return m_dirtyNormal; }
			set { m_dirtyNormal = value; }
		}

		public MasterNodePortCategory PortCategory
		{
			get { return m_portCategory; }
			set { m_portCategory = value; }
		}

		public bool ForceNormal
		{
			get { return m_forceNormal; }
			set
			{
				if ( value )
				{
					if ( !m_forceNormalIsDirty )
					{
						m_forceNormal = value;
						m_forceNormalIsDirty = value;
					}
				}
				else
				{
					m_forceNormal = value;
				}
			}
		}
	}
}
