using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class OptionsWindow
	{
		[SerializeField]
		public bool ColoredPorts = false;

		public OptionsWindow()
		{
			//Load ();
		}

		public void Init (){
			Load ();
		}

		public void Destroy ()
		{
			Save ();
		}

		public void Save ()
		{
			EditorPrefs.SetBool ("ColoredPorts", ColoredPorts);
		}

		public void Load ()
		{
			ColoredPorts = EditorPrefs.GetBool ("ColoredPorts");
			UIUtils.CurrentWindow.ToggleDebug = ColoredPorts;
		}
	}
}