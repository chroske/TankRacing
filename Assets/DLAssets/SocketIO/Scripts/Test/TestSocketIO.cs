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
	
	
	private GameObject m_Car;
	private GameObject m_Shadow;
	private GameManager m_GameManager;
	private SocketIOComponent socket;
	private float gameStartTime;
	private float emitInterval = 0.3f;//0.016f;
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
		
		SettingGameModeByParam();
		
		
		gameStartTime = Time.timeSinceLevelLoad;
		
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		myRigidbody = GetComponent<Rigidbody>();
		
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
		
	}
	
	private bool moveSim;
	private Rigidbody myRigidbody;
	
	void FixedUpdate(){
		if(moveSim){
			moveSim = false;
			
			// データは1/Network.sendRate間隔で送信されてくる。このうちの経過時間分が内分する値
			float t = emitInterval;
			// 移動先から速度を逆算
			Vector3 move = (Vector3.Lerp(m_Shadow.transform.position, position, t) - m_Shadow.transform.position) / Time.fixedDeltaTime;
			// 速度を設定
			myRigidbody.velocity = move;
			
			// 回転
			//m_Shadow.transform.rotation = Quaternion.Slerp(m_Shadow.transform.rotation, rotation, t);
		}
	}
	
	// 追加関数
	public void S_to_C_message( SocketIOEvent e ){
		Debug.Log("[SocketIO] C_to_S_message received: " + e.name + " " + e.data);
	}
	
	public void response_log( SocketIOEvent e ){
		Debug.Log("[SocketIO] response received: " + e.name + " " + e.data);
		
		JSONObject json = new JSONObject(e.data.ToString());
		
		SetReceiveTankParam (json);
		
		moveSim = true;
	}
	
	private Vector3 position;
	
	
	private void SetReceiveTankParam(JSONObject json) {
		string posX = json.GetField("position_x").str;
		string posY = json.GetField("position_y").str;
		string posZ = json.GetField("position_z").str;
		
		string rotX = json.GetField("rotate_x").str;
		string rotY = json.GetField("rotate_y").str;
		string rotZ = json.GetField("rotate_z").str;
		
		var diff = Time.timeSinceLevelLoad - gameStartTime;
		var rate = diff / emitInterval;
		
		//m_Shadow.transform.position = Vector3.Lerp (m_Shadow.transform.position, new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ)), rate);
		//m_Shadow.transform.rotation = Quaternion.Slerp(m_Shadow.transform.rotation, Quaternion.Euler(new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ))), rate);
		
		position = new Vector3 (float.Parse (posX), float.Parse (posY), float.Parse (posZ));
		
		m_Shadow.transform.position = new Vector3( float.Parse(posX), float.Parse(posY), float.Parse(posZ));
		m_Shadow.transform.eulerAngles = new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ));
	}
	
	
	
	private IEnumerator EmitRoop() {
		for (;;) {
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
