using UnityEngine;
using System.Collections;

public class TpsCameraController : MonoBehaviour {

	public float smooth = 3f;		// カメラモーションのスムーズ化用変数
	Transform standardPos;			// the usual position for the camera, specified by a transform in the game

	// Use this for initialization
	void Start () {
		// 各参照の初期化
		standardPos = GameObject.Find ("CamPos").transform;

		//カメラをスタートする
		transform.position = standardPos.position;	
		transform.forward = standardPos.forward;	
	}
	
	// Update is called once per frame
	void Update () {
//		if(transform.position.y <= 0.5f){
//
//			transform.position = new Vector3(transform.position.x,0.5f,transform.position.z);
//		}
	}

	void FixedUpdate ()	// このカメラ切り替えはFixedUpdate()内でないと正常に動かない
	{
		setCameraPositionNormalView();
	}

	void setCameraPositionNormalView()
	{
		// the camera to standard position and direction
		transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);	
		transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
	}
}
