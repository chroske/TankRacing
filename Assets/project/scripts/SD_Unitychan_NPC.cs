using UnityEngine;
using System;
using System.Collections;

public class SD_Unitychan_NPC : MonoBehaviour {

	private Animator animator;          // アニメータコントローラ
	private int animId = 0;             // 再生中のアニメーションID
	private UInt64 charaId = 0;         // キャラクタID
	private Vector3 charaIdDisp;        // キャラクタIDの表示位置

	private bool Run = false;
	private bool Back = false;
	private bool Rest = false;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
		animId = Animator.StringToHash("animId");
	}
	
	// Update is called once per frame
	void Update () {
		// キャラクタID表示位置の算出
		if (SD_Unitychan_PC.GetMainCamera() != null)
		{
			charaIdDisp = SD_Unitychan_PC.GetMainCamera().WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 1.25f, 0));
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
	
	public void SetAnimId(Int32 animId)
	{
		if(animator != null )
		{
			animator.SetInteger(this.animId, animId);
		}
	}

	public void SetRun(bool Run)
	{
		if(animator != null )
		{
			animator.SetBool("Run", Run);
		}
	}

	public void SetBack(bool Back)
	{
		if(animator != null )
		{
			animator.SetBool("Back", Back);
		}
	}

	public void SetRest(bool Rest)
	{
		if(animator != null )
		{
			animator.SetBool("Rest", Rest);
		}
	}
}
