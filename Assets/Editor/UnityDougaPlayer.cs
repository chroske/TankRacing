using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;

public class UnityDougaPlayer : EditorWindow {

	static BindingFlags Flags = BindingFlags.Public | BindingFlags.Static;
	
	[MenuItem("Window/UnityDougaPlayer")]
	static void Open ()
	{
		var type = Types.GetType ("UnityEditor.Web.WebViewEditorWindow", "UnityEditor.dll");
		var methodInfo = type.GetMethod ("Create", Flags);
		methodInfo = methodInfo.MakeGenericMethod (typeof(UnityDougaPlayer));
		methodInfo.Invoke (null, new object[]{
			"UnityDougaPlayer",
			"file:///M:/products/unity_projects/GYRO-FPS/Assets/Editor/DougaPlayer.html",
			150, 180, 400, 600
		});
	}
}