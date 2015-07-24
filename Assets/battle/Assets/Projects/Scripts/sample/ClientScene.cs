//#define SELECT_SERVER_WEB_API

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class ClientScene : SingletonBehaviour<ClientScene> {
    protected mln.Client m_Client;
    protected UInt64 m_WaitTime = 0;

	public GameObject unityChan_PC;     //< プレイヤープレハブ（プレハブ受け取り口）
	public GameObject unityChan_NPC;    //< 非プレイヤープレハブ（プレハブ受け取り口）
	public static GameObject g_unityChan_PC;    //< プレイヤープレハブ（グローバルオブジェクト）
	public static GameObject g_unityChan_NPC;   //< 非プレイヤープレハブ（グローバルオブジェクト）
	public static Dictionary<UInt64, GameObject> g_CharaList = new Dictionary<UInt64, GameObject>();    //< キャラクタリスト
	public static readonly object g_mutex = new object();    //< g_CharaList の排他制御用
	
	// Use this for initialization
	void Start () {
		// グローバル変数として利用可能に
		g_unityChan_PC = unityChan_PC;
		g_unityChan_NPC = unityChan_NPC;

        // クライアント生成
        m_Client = new mln.Client();

        // コールバック定義
        m_Client.SetSelectServerEvent(new mln.Client.OnEvent(this.OnSelectServerEvent));
        m_Client.SetMatchingEnterRoomEvent(new mln.Client.OnEvent(this.OnMatchingEnterRoomEvent));
        m_Client.SetBattleStartEvent(new mln.Client.OnEvent(this.OnBattleStartEvent));
        m_Client.SetBattleEvent(new mln.Client.OnEvent(this.OnBattleEvent));
        m_Client.SetBattleEndEvent(new mln.Client.OnEvent(this.OnBattleEndEvent));

        // クライアント起動
        m_Client.Start();
    }
        
	// Update is called once per frame
	void Update () {
        // クライアントのアップデート
        m_Client.Update();
	}

    // マッチングサーバ接続前に呼び出される処理
    // @param nowTime 現在の時間（秒）
    public void OnSelectServerEvent(UInt64 nowTime) {
        m_Phase = PHASE.PHASE_MATCHING_SELECT_SERVER;
    }

    // マッチングルーム入室前に呼び出される処理
    // @param nowTime 現在の時間（秒）
    public void OnMatchingEnterRoomEvent(UInt64 nowTime)
    {
        m_Phase = PHASE.PHASE_MATCHING_ENTER_ROOM;
    }

    // バトル開始前に呼び出される処理
    // @param nowTime 現在の時間（秒）
    public void OnBattleStartEvent(UInt64 nowTime)
    {
        m_Phase = PHASE.PHASE_BATTLE_START;
    }

    // バトル中に呼び出される処理
    // @param nowTime 現在の時間（秒）
    public void OnBattleEvent(UInt64 nowTime)
    {
        m_Phase = PHASE.PHASE_BATTLE;
    }

    // バトル終了時に呼び出される処理
    // @param nowTime 現在の時間（秒）
    public void OnBattleEndEvent(UInt64 nowTime)
    {
        m_Phase = PHASE.PHASE_BATTLE_END;
    }
}
