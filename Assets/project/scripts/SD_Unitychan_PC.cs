using UnityEngine;
using System;
using System.Collections;

public class SD_Unitychan_PC : MonoBehaviour {

	private Animator animator;                  // アニメータコントローラ
	private int animId = 0;                     // 再生中のアニメーションID
	private UInt64 charaId = 0;                 // キャラクタID
	private Vector3 charaIdDisp;                // キャラクタIDの表示位置
	private static Camera mainCamera = null;    // メインカメラ


	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		// キャラクタID表示位置の算出
		if (mainCamera != null)
		{
			charaIdDisp = mainCamera.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 1.25f, 0));
		}
		
		// キャラクタの移動＆アニメーション切り替え
		if (Input.GetKey("up") || Input.GetKey("w"))
		{
			animator.SetBool("Run", true);
			animator.SetBool("Rest", false);
		} else if(Input.GetKey("down") || Input.GetKey("s")){
			animator.SetBool("Back", true);
			animator.SetBool("Rest", false);
		}
		else
		{
			animator.SetBool("Run", false);
			animator.SetBool("Back", false);
			animator.SetBool("Rest", true);
		}
	}

	void OnGUI()
	{
		// キャラクタIDの表示
		if (charaIdDisp != null)
		{
			//GUI.Label(new Rect(charaIdDisp.x - 60, Screen.height - charaIdDisp.y - 20, 120, 40), "CharaId:0x" + mln.Utility.ToHex(charaId));
		}
	}
	
	public void SetCharaID(UInt64 charaId)
	{
		this.charaId = charaId;
	}
	
	public void SetMainCamera(Camera camera)
	{
		mainCamera = camera;
	}
	
	public static Camera GetMainCamera()
	{
		return  mainCamera;
	}
}
