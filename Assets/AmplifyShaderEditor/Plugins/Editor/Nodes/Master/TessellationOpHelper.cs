using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TessellationOpHelper
	{
		public const string TessSurfParam = "tessellate:tessFunction nolightmap";
		public const string TessInclude = "Tessellation.cginc";
		public const string CustomAppData = "\t\tstruct appdata\n" +
											"\t\t{\n" +
											"\t\t\tfloat4 vertex : POSITION;\n" +
											"\t\t\tfloat4 tangent : TANGENT;\n" +
											"\t\t\tfloat3 normal : NORMAL;\n" +
											"\t\t\tfloat2 texcoord : TEXCOORD0;\n" +
											"\t\t\tfloat4 texcoord1 : TEXCOORD1;\n" +
											"\t\t\tfloat4 texcoord2 : TEXCOORD2;\n" +
											"\t\t\tfloat4 texcoord3 : TEXCOORD3;\n" +
											"\t\t\tfixed4 color : COLOR;\n" +
#if UNITY_5_5_OR_NEWER
											"\t\t\tUNITY_VERTEX_INPUT_INSTANCE_ID\n" +
#else
											"\t\t\tUNITY_INSTANCE_ID\n" +
#endif
											"\t\t};\n\n";



		private const string TessUniformName = "_TessValue";
		private const string TessMinUniformName = "_TessMin";
		private const string TessMaxUniformName = "_TessMax";

		private GUIContent EnableTessContent = new GUIContent( "Tessellation", "Activates the use of tessellation which subdivides polygons to increase geometry detail using a set of rules\nDefault: OFF" );
		private GUIContent TessFactorContent = new GUIContent( "Tess", "Tessellation factor\nDefault: 4" );
		private GUIContent TessMinDistanceContent = new GUIContent( "Min", "Minimum tessellation distance\nDefault: 10" );
		private GUIContent TessMaxDistanceContent = new GUIContent( "Max", "Maximum tessellation distance\nDefault: 25" );


		private readonly int[] TesselationTypeValues = { 0 , 1 , 2 };
		private readonly string[] TesselationTypeLabels = { "Distance-based" , "Fixed","Edge Length"  };
		private readonly string TesselationTypeStr = "Type";
		//private GUIContent TesselationTypeContent = new GUIContent( "Type", "Tessellation rule" );

		private const string TessProperty = "_TessValue( \"Tessellation\", Range( 1, 32 ) ) = {0}";
		private const string TessMinProperty = "_TessMin( \"Tess Min Distance\", Float ) = {0}";
		private const string TessMaxProperty = "_TessMax( \"Tess Max Distance\", Float ) = {0}";

		// Distance based function
		private const string DistBasedTessFunctionOpen = "\t\tfloat4 tessFunction( appdata v0, appdata v1, appdata v2 )\n\t\t{\n";
		private const string DistBasedTessFunctionBody = "\t\t\treturn UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );\n";
		private const string DistBasedTessFunctionClose = "\t\t}\n";

		// Fixed amount function
		private const string FixedAmountTessFunctionOpen = "\t\tfloat4 tessFunction( )\n\t\t{\n";
		private const string FixedAmountTessFunctionBody = "\t\t\treturn _TessValue;\n";
		private const string FixedAmountTessFunctionClose = "\t\t}\n";

		// Edge Length
		private GUIContent	EdgeLengthContent = new GUIContent( "Edge Length", "Tessellation levels ccomputed based on triangle edge length on the screen\nDefault: 4" );
		private const string EdgeLengthTessProperty = "_EdgeLength ( \"Edge length\", Range( 2, 50 ) ) = {0}";
		private const string EdgeLengthTessUniformName = "_EdgeLength";
		
		private const string EdgeLengthTessFunctionOpen = "\t\tfloat4 tessFunction(appdata v0, appdata v1, appdata v2)\n\t\t{\n";
		private const string EdgeLengthTessFunctionBody = "\t\t\treturn UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);\n";
		private const string EdgeLengthTessFunctionClose = "\t\t}\n";


		private const string EdgeLengthTessCullFunctionOpen = "\t\tfloat4 tessFunction(appdata v0, appdata v1, appdata v2)\n\t\t{\n";
		private const string EdgeLengthTessCullFunctionBody = "\t\t\treturn UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);\n";
		private const string EdgeLengthTessCullFunctionClose = "\t\t}\n";


		// Phong
		public const string TessPhongParam = "tessphong:_Phong";
		private const string TessPhongProperty = "_TessPhong( \"Phong Strength\", Range( 0, 1 ) ) = {0}";

		[SerializeField]
		private bool m_enabled = false;

		[SerializeField]
		private int m_tessType = 0;

		[SerializeField]
		private float m_tessMinDistance = 10f;

		[SerializeField]
		private float m_tessMaxDistance = 25f;

		[SerializeField]
		private float m_tessFactor = 4f;

		public void Draw( Material mat )
		{
			EditorGUILayout.Separator();
			m_enabled = EditorGUILayout.Toggle( EnableTessContent, m_enabled );
			if ( m_enabled )
			{
				EditorGUI.indentLevel += 1;
				m_tessType = EditorGUILayout.IntPopup( TesselationTypeStr, m_tessType, TesselationTypeLabels, TesselationTypeValues );
				switch ( m_tessType ) // already did a switch-case to further extend to other methods
				{
					case 0:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = EditorGUILayout.Slider( TessFactorContent, m_tessFactor, 1, 32 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessUniformName ) )
								mat.SetFloat( TessUniformName, m_tessFactor );
						}

						EditorGUI.BeginChangeCheck();
						m_tessMinDistance = EditorGUILayout.FloatField( TessMinDistanceContent, m_tessMinDistance );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessMinUniformName ) )
								mat.SetFloat( TessMinUniformName, m_tessMinDistance );
						}

						EditorGUI.BeginChangeCheck();
						m_tessMaxDistance = EditorGUILayout.FloatField( TessMaxDistanceContent, m_tessMaxDistance );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessMaxUniformName ) )
								mat.SetFloat( TessMaxUniformName, m_tessMaxDistance );
						}

					}
					break;
					case 1:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = EditorGUILayout.Slider( TessFactorContent, m_tessFactor, 1, 32 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( TessUniformName ) )
								mat.SetFloat( TessUniformName, m_tessFactor );
						}
					}
					break;
					case 2:
					case 3:
					{
						EditorGUI.BeginChangeCheck();
						m_tessFactor = EditorGUILayout.Slider( EdgeLengthContent, m_tessFactor, 2, 50 );
						if ( EditorGUI.EndChangeCheck() && mat != null )
						{
							if ( mat.HasProperty( EdgeLengthTessUniformName ) )
								mat.SetFloat( EdgeLengthTessUniformName, m_tessFactor );
						}
					}
					break;
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_enabled = Convert.ToBoolean( nodeParams[ index++ ] );
			m_tessType = Convert.ToInt32( nodeParams[ index++ ] );
			m_tessFactor = Convert.ToSingle( nodeParams[ index++ ] );
			m_tessMinDistance = Convert.ToSingle( nodeParams[ index++ ] );
			m_tessMaxDistance = Convert.ToSingle( nodeParams[ index++ ] );
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_enabled );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessFactor );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessMinDistance );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_tessMaxDistance );
		}

		public void AddToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			switch ( m_tessType )
			{
				case 0:
				{
					UIUtils.CurrentDataCollector.AddToIncludes( -1, TessellationOpHelper.TessInclude );

					//Tess
					UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( TessProperty, m_tessFactor ), -1000 );
					UIUtils.CurrentDataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessUniformName + ";" );

					//Min
					UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( TessMinProperty, m_tessMinDistance ), -999 );
					UIUtils.CurrentDataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessMinUniformName + ";" );

					//Max
					UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( TessMaxProperty, m_tessMaxDistance ), -998 );
					UIUtils.CurrentDataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessMaxUniformName + ";" );
				}break;
				case 1:
				{
					//Tess
					UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( TessProperty, m_tessFactor ), -1000 );
					UIUtils.CurrentDataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + TessUniformName + ";" );
				}
				break;
				case 2:
				case 3:
				{
					UIUtils.CurrentDataCollector.AddToIncludes( -1, TessellationOpHelper.TessInclude );
					
					//Tess
					UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( EdgeLengthTessProperty, m_tessFactor ), -1000 );
					UIUtils.CurrentDataCollector.AddToUniforms( -1, "uniform " + UIUtils.FinalPrecisionWirePortToCgType( PrecisionType.Float, WirePortDataType.FLOAT ) + " " + EdgeLengthTessUniformName + ";" );
				}
				break;
			}
		}

		//ToDo: Optimize material property fetches to use Id instead of string 
		public void UpdateFromMaterial( Material mat )
		{
			if ( m_enabled )
			{
				switch ( m_tessType )
				{
					case 0:
					{
						if ( mat.HasProperty( TessUniformName ) )
							m_tessFactor = mat.GetFloat( TessUniformName );

						if ( mat.HasProperty( TessMinUniformName ) )
							m_tessMinDistance = mat.GetFloat( TessMinUniformName );

						if ( mat.HasProperty( TessMaxUniformName ) )
							m_tessMaxDistance = mat.GetFloat( TessMaxUniformName );
					}
					break;
					case 1:
					{
						if ( mat.HasProperty( TessUniformName ) )
							m_tessFactor = mat.GetFloat( TessUniformName );
					}
					break;
					case 2:
					case 3:
					{
						if ( mat.HasProperty( EdgeLengthTessUniformName ) )
							m_tessFactor = mat.GetFloat( EdgeLengthTessUniformName );
					}
					break;
				}
			}
		}

		public string GetCurrentTessellationFunction
		{
			get
			{
				string tessFunction = string.Empty;
				switch ( m_tessType )
				{
					case 0:
					{
						tessFunction = DistBasedTessFunctionOpen +
										DistBasedTessFunctionBody +
										DistBasedTessFunctionClose;
					}break;
					case 1:
					{
						tessFunction =  FixedAmountTessFunctionOpen +
										FixedAmountTessFunctionBody +
										FixedAmountTessFunctionClose;
					}
					break;
					case 2:
					{
						tessFunction =  EdgeLengthTessFunctionOpen +
										EdgeLengthTessFunctionBody +
										EdgeLengthTessFunctionClose;
					}
					break;
					case 3:
					{
						tessFunction =  EdgeLengthTessCullFunctionOpen +
										EdgeLengthTessCullFunctionBody +
										EdgeLengthTessCullFunctionClose;
					}
					break;
				}
				return tessFunction;
			}
		}
		public bool EnableTesselation { get { return m_enabled; } }
	}
}
