using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class SceneViewTest : EditorWindow
{

	// method
	[MenuItem("Window/SceneViewTest")]
	static void Open()
	{
		EditorWindow.GetWindow<SceneViewTest>( "SceneViewTest" );
	}

	void OnGUI()
	{
		if( GUILayout.Button("start") )
			letgo();
	}


	private void letgo(){
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
	
//	static SceneViewTest ()
//	{
//
//	}
		
	private void OnSceneGUI (SceneView sceneView)
	{
		Handles.BeginGUI ();
		GUILayout.Window (1, new Rect (10, 30, 100, 100), Func, "Window");
		Handles.EndGUI ();
	}
	
	private void Func (int id)
	{
		//SceneView.lastActiveSceneView.camera.transform.position.z = new Vector3(0, 5, 0);
		if (GUILayout.Button ("Hogeeeeee"))
			test ();
	}
	
	private void test(){
		//SceneView.LookAt(new Vector3(0,0,0));
		SceneView.lastActiveSceneView.LookAt(new Vector3(0,50,0));
		//SceneView.lastActiveSceneView.LookAtDirect (new Vector3(0,150,0), new Quaternion(0,0,0,0));
	}

}