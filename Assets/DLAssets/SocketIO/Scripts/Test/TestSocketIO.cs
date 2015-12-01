#region License
/*
 * TestSocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 Fabio Panettieri
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System.Collections;
using UnityEngine;
using SocketIO;

public class TestSocketIO : MonoBehaviour
{
	[SerializeField] private GameObject gameManager;
	[SerializeField] private GameObject Car;
	[SerializeField] private GameObject Shadow;
	[SerializeField] private GameObject DummySteer;
	[SerializeField] private GameObject SteerController;
	
	
	private GameObject m_Car;
	private GameObject m_Shadow;
	private GameManager m_GameManager;
	private DummySteer m_DummySteer;
	private SteerController m_SteerController;
	private SocketIOComponent socket;
	private float gameStartTime;
	private float emitInterval = 0.3f;//0.016f;
	private float rate;
	private float diff;
	private Vector3 prePosition;
	private Vector3 preRotation;

	
	public void SettingGameModeByParam(){
		if(m_GameManager.battleMode == "Car"){
			m_Car = Car;
			m_Shadow = Shadow;
		} else if(m_GameManager.battleMode == "Canon"){
			m_Car = Shadow;
			m_Shadow = Car;
		}
	}
	
	public void Start() 
	{
		
		m_GameManager = gameManager.GetComponent<GameManager>();
		m_DummySteer = DummySteer.GetComponent<DummySteer>();
		m_SteerController = SteerController.GetComponent<SteerController>();
		
		SettingGameModeByParam();
		
		

		
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", TestOpen);
		socket.On("boop", TestBoop);
		socket.On("error", TestError);
		socket.On("close", TestClose);
		
		// メッセージ受信を追加
		//socket.On ("S_to_C_message", S_to_C_message);
		socket.On ("response"+m_GameManager.playerId, response_log);

		StartCoroutine("EmitRoop");
	}
	
	void Update(){
		diff = Time.timeSinceLevelLoad - gameStartTime;
		rate = diff; /* / emitInterval*/
		//Debug.Log (rate);
	}
	
	// 追加関数
	public void S_to_C_message( SocketIOEvent e ){
		//Debug.Log("[SocketIO] C_to_S_message received: " + e.name + " " + e.data);
	}
	
	public void response_log( SocketIOEvent e ){
		//Debug.Log("[SocketIO] response received: " + e.name + " " + e.data);

		JSONObject json = new JSONObject(e.data.ToString());

		gameStartTime = Time.timeSinceLevelLoad;
		SetReceiveTankParam (json);
	}

	bool firstGetParam = false;
	private void SetReceiveTankParam(JSONObject json) {
		string posX = json.GetField("position_x").str;
		string posY = json.GetField("position_y").str;
		string posZ = json.GetField("position_z").str;
		
		string rotX = json.GetField("rotate_x").str;
		string rotY = json.GetField("rotate_y").str;
		string rotZ = json.GetField("rotate_z").str;

		string steerAngle = json.GetField("steerAngle").str;
		string valConDistance = json.GetField("valConDistance").str;
		
		//float diff = ;
		//var rate = diff / emitInterval;

//		float CarPositionLagDistance = Vector3.Distance (m_Shadow.transform.position, new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ)));
//		m_Shadow.transform.position = Vector3.Lerp (m_Shadow.transform.position, new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ)), 0.0002f);
		//m_Shadow.transform.rotation = Quaternion.Slerp(m_Shadow.transform.rotation, Quaternion.Euler(new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ))), rate);

		
		
		if (firstGetParam == false) {
			m_Shadow.transform.position = new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ));
			//m_Shadow.transform.eulerAngles = new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ));

			iTween.LookTo(m_Shadow, new Vector3 (90, 90, 90), 3);
			firstGetParam = true;
		} else {
			float CarPositionLagDistance = Vector3.Distance (m_Shadow.transform.position, new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ)));
			float moveTime = 5.0f-(CarPositionLagDistance/2);
			if(moveTime < 3f){
				moveTime = 3f;
			}

			iTween.MoveTo(m_Shadow,  iTween.Hash(
				"position", new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ)),
				"time", moveTime+moveTime*rate
			));
			//iTween.LookTo(m_Shadow, new Vector3 (m_Shadow.transform.eulerAngles.x, float.Parse(rotY), m_Shadow.transform.eulerAngles.z), 0);
			//m_Shadow.transform.eulerAngles = new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ));
			//Debug.Log (moveTime*rate);
		}

		//m_Shadow.transform.eulerAngles = new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ));

		m_DummySteer.steerAngle = float.Parse(steerAngle);
		m_DummySteer.valConDistance = float.Parse(valConDistance);

	}
	
	
	
	private IEnumerator EmitRoop() {
		for (;;) {
			// 移動していない限り送らない
			if(m_Car.transform.position != prePosition || m_Car.transform.eulerAngles != preRotation){
				prePosition = m_Car.transform.position;
				preRotation = m_Car.transform.eulerAngles;
				
				JSONObject jsonobj = new JSONObject(JSONObject.Type.OBJECT);
				
				jsonobj.AddField("player_id", m_GameManager.playerId);
				jsonobj.AddField("position_x", m_Car.transform.localPosition.x.ToString());
				jsonobj.AddField("position_y", m_Car.transform.localPosition.y.ToString());
				jsonobj.AddField("position_z", m_Car.transform.localPosition.z.ToString());
				
				jsonobj.AddField("rotate_x", m_Car.transform.eulerAngles.x.ToString());
				jsonobj.AddField("rotate_y", m_Car.transform.eulerAngles.y.ToString());
				jsonobj.AddField("rotate_z", m_Car.transform.eulerAngles.z.ToString());

				jsonobj.AddField("steerAngle", m_SteerController.steerAngle.ToString());
				jsonobj.AddField("valConDistance", m_SteerController.valConDistance.ToString());
				
				socket.Emit("position"+m_GameManager.playerId,jsonobj);
			}
			
			
			yield return new WaitForSeconds(emitInterval);
		}
	}
	
	private IEnumerator BeepBoop()
	{
		// wait 1 seconds and continue
		yield return new WaitForSeconds(1);
		
		socket.Emit("beep");
		
		// wait 3 seconds and continue
		yield return new WaitForSeconds(3);
		
		socket.Emit("beep");
		
		// wait 2 seconds and continue
		yield return new WaitForSeconds(2);
		
		socket.Emit("beep");
		
		// wait ONE FRAME and continue
		yield return null;
		
		socket.Emit("beep");
		socket.Emit("beep");
	}
	
	public void TestOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}
	
	public void TestBoop(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Boop received: " + e.name + " " + e.data);
		
		if (e.data == null) { return; }
		
		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
			);
	}
	
	public void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}
	
	public void TestClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}
}
