// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[AttributeUsage( AttributeTargets.Class )]
	public class NodeAttributes : Attribute
	{
		public string Name;
		public string Description;
		public string Category;
		public KeyCode ShortcutKey;
		public bool Available;
		public Type[] CastType; // Type that will be converted to AttribType if dropped on the canvas ... p.e. dropping a texture2d on the canvas will generate a sampler2d node 
		public bool Deprecated;
		public string DeprecatedAlternative;
		public Type DeprecatedAlternativeType;
		public bool FromCommunity;
		public string CustomCategoryColor; // Color created via a string containing its hexadecimal representation
		public NodeAttributes( string name, string category, string description, Type castType = null, KeyCode shortcutKey = KeyCode.None, bool available = true, bool deprecated = false, string deprecatedAlternative = null, Type deprecatedAlternativeType = null, bool fromCommunity = false, string customCategoryColor = null  )
		{
			Name = name;
			Description = description;
			Category = category;
			if ( castType != null )
				CastType = new Type[] { castType };

			ShortcutKey = shortcutKey;
			Available = available;
			Deprecated = deprecated;
			DeprecatedAlternative = deprecatedAlternative;
			FromCommunity = fromCommunity;
			CustomCategoryColor = customCategoryColor;
			DeprecatedAlternativeType = deprecatedAlternativeType;
		}

		public NodeAttributes( string name, string category, string description, KeyCode shortcutKey, bool available, params Type[] castTypes )
		{
			Name = name;
			Description = description;
			Category = category;
			if ( castTypes != null && castTypes.Length > 0 )
			{
				CastType = castTypes;
			}

			ShortcutKey = shortcutKey;
			Available = available;
		}
	}
}
