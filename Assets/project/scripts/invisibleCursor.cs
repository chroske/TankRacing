using UnityEngine;
using System.Collections;

public class invisibleCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// マウスカーソルを削除する
		Cursor.visible = false;
		// マウスカーソルを画面内にロックする
		Screen.lockCursor = true;
	}
}
