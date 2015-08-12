using UnityEngine;
using System.Collections;

public class RenderingSteerLine : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		DrawLine(new Vector2(1f,2f),new Vector2(8f,7f),2);
	}

	public Texture aTexture;

	private void DrawLine(Vector2 start, Vector2 end, int width)
	{
		Vector2 d = end - start;
		float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
		if (d.x < 0)
			a += 180;
		
		int width2 = (int) Mathf.Ceil(width / 2);
		
		GUIUtility.RotateAroundPivot(a, start);
		GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), aTexture);
		GUIUtility.RotateAroundPivot(-a, start);
	}
}
