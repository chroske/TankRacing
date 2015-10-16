﻿#region License
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
	private SocketIOComponent socket;

	public GameObject Car;
	public GameObject Shadow;
	public int player_id = 1;

	public void Start() 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		
		socket.On("open", TestOpen);
		socket.On("boop", TestBoop);
		socket.On("error", TestError);
		socket.On("close", TestClose);
		
		// メッセージ受信を追加
		//socket.On ("S_to_C_message", S_to_C_message);
		socket.On ("response"+player_id, response_log);
		
		StartCoroutine("EmitRoop");
	}

	void Update(){
		
	}

	// 追加関数
	public void S_to_C_message( SocketIOEvent e ){
		Debug.Log("[SocketIO] C_to_S_message received: " + e.name + " " + e.data);
	}

	public void response_log( SocketIOEvent e ){
		Debug.Log("[SocketIO] response received: " + e.name + " " + e.data);

		JSONObject json = new JSONObject(e.data.ToString());
		string posX = json.GetField("position_x").str;
		string posY = json.GetField("position_y").str;
		string posZ = json.GetField("position_z").str;

		string rotX = json.GetField("rotate_x").str;
		string rotY = json.GetField("rotate_y").str;
		string rotZ = json.GetField("rotate_z").str;

		Shadow.transform.position = new Vector3( float.Parse(posX), float.Parse(posY), float.Parse(posZ));
		Shadow.transform.eulerAngles = new Vector3 (float.Parse(rotX), float.Parse(rotY), float.Parse(rotZ));
	}

	private IEnumerator EmitRoop() {
		for (;;) {
			JSONObject jsonobj = new JSONObject(JSONObject.Type.OBJECT);

			jsonobj.AddField("player_id", player_id);
			jsonobj.AddField("position_x", Car.transform.localPosition.x.ToString());
			jsonobj.AddField("position_y", Car.transform.localPosition.y.ToString());
			jsonobj.AddField("position_z", Car.transform.localPosition.z.ToString());

			jsonobj.AddField("rotate_x", Car.transform.eulerAngles.x.ToString());
			jsonobj.AddField("rotate_y", Car.transform.eulerAngles.y.ToString());
			jsonobj.AddField("rotate_z", Car.transform.eulerAngles.z.ToString());


			socket.Emit("position"+player_id,jsonobj);
			
			yield return new WaitForSeconds(0.016f);
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
