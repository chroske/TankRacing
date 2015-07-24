//#define SELECT_SERVER_WEB_API

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if SELECT_SERVER_WEB_API
using LitJson;
#endif

/***
 * Copyright MONOBIT Inc. All rights reserved.
 */

/**
 * クライアントクラス
 */
namespace mln {
public class Client : BaseClient {
	public enum PHASE {
        PHASE_NONE = 0,
#if ! SELECT_SERVER_WEB_API
        PHASE_MATCHING_SELECT_SERVER,
        PHASE_MATCHING_CONNECT_SERVER,
		PHASE_MATCHING_WAIT_CONNECT_SERVER,
		PHASE_MATCHING_CONNECT_RPC,
		PHASE_MATCHING_WAIT_CONNECT_RPC,
		PHASE_MATCHING_ENTER_ROOM,
		PHASE_MATCHING_CHECK_MATCHING_SUCCESS,
		PHASE_MATCHING_EXIT_ROOM,
		PHASE_BATTLE_PULIST_GET_ROOM_LIST,
#else
		PHASE_BATTLE_GET_ROOM_LIST,
#endif
		PHASE_BATTLE_ENTER_ROOM,
		PHASE_BATTLE_START,
		PHASE_BATTLE,
		PHASE_BATTLE_END,
		PHASE_BATTLE_EXIT_ROOM,
		PHASE_BATTLE_DISCONNECT,
		PHASE_COOL_DOWN,
	};
	
	private const UInt64			WAIT_TIME_CHANGE_PHASE      = 30;	        ///< フェーズ変更待ち時間
	private const UInt64			WAIT_TIME_PHASE_BATTLE_DISCONNECT = 5;      ///< BATTLE_DISCONNECTフェーズ実行待ち時間
	private const UInt64			WAIT_TIME_PHASE_COOL_DOWN   = 2;            ///< COOL_DOWNフェーズ実行待ち時間

    private PHASE                   m_Phase;                                    ///< フェーズ
	private UInt64					m_WaitTime;                                 ///< フェーズ変更待ち時間
    private UInt64                  m_WaitTime_MatchingState;                   ///< Send_GetMatchingState 呼び出し待ち時間
	
#if SELECT_SERVER_WEB_API
	/// ルーム情報
	protected class RoomInfo {
		public string     	ipAddr;         ///< IPアドレス
		public UInt16          port;           ///< ポート番号
		public UInt64          roomHubPuUid;   ///< RoomHubのPUUID
		public UInt64          battlePuUid;    ///< BattleのPUUID
		public byte             entered_num;    ///< 入室済み人数
		public byte             max_enter_num;  ///< 最大入室人数
		public byte             status;         ///< ステータス(BTL::BATTLESTATUS_XXX)
	};
#else
	/// サーバー情報
	public class ServerInfo {
		public string     ipAddr;         ///< IPアドレス
		public UInt16     port;           ///< ポート番号
		public UInt64     puUid;          ///< RoomHubのPUUID
	};
#endif
	
#if ! SELECT_SERVER_WEB_API
	public List<ServerInfo>		m_MatchingServerInfoList;	///< マッチングサーバー情報リスト
	public ServerInfo		    m_pMatchingServerInfo;		///< マッチングサーバー情報
#endif
	
    protected System.Random m_pRandom;
	
#if SELECT_SERVER_WEB_API
	static protected web.WebApiAgent m_WebApiManager = new web.WebApiAgent();
#endif

    public delegate void OnEvent(UInt64 nowTime);
    OnEvent onSelectServerEvent = null;
    OnEvent onMatchingEnterRoomEvent = null;
    OnEvent onBattleStartEvent = null;
    OnEvent onBattleEvent = null;
    OnEvent onBattleEndEvent = null;

    public void SetSelectServerEvent(OnEvent eventfunc) {
        onSelectServerEvent = eventfunc;
    }
    public void SetMatchingEnterRoomEvent(OnEvent eventfunc) {
        onMatchingEnterRoomEvent = eventfunc;
    }
    public void SetBattleStartEvent(OnEvent eventfunc) {
        onBattleStartEvent = eventfunc;
    }
    public void SetBattleEvent(OnEvent eventfunc) {
        onBattleEvent = eventfunc;
    }
    public void SetBattleEndEvent(OnEvent eventfunc) {
        onBattleEndEvent = eventfunc;
    }

    void SetPhase( PHASE phase, UInt64 waitTime ){
		m_Phase = phase;
		m_WaitTime = waitTime;
	}
	
	void ClearPhase(){
		SetPhase( PHASE.PHASE_NONE, 0 );
		
#if ! SELECT_SERVER_WEB_API
        if (null != m_MatchingServerInfoList)
        {
            m_MatchingServerInfoList.Clear();
        }
        m_pMatchingServerInfo = null;
#endif
	}

    public Client() {
#if ! SELECT_SERVER_WEB_API
        m_MatchingServerInfoList = new List<ServerInfo>();
#endif
    }

	~Client(){
#if ! SELECT_SERVER_WEB_API
		if (null != m_MatchingServerInfoList)
		{
			m_MatchingServerInfoList.Clear();
		}
#endif
	}

    public override void Start() {
        base.Start();
		
		ClearPhase();
		
        m_pRandom = new System.Random(Environment.TickCount);
	}

    // サーバ情報を登録する
    // @param ipAddr    IPアドレス
    // @param port      ポート番号
    // @param puUid     PUのユニークID
    public void EntryServerInfo(string ipAddr, UInt16 port, UInt64 puUid)
    {
#if SELECT_SERVER_WEB_API
		// TODO とりあえず、固定値を設定
		m_WebApiManager.SetRootUrl( "http://" + ipAddr + "/" );
#else
        ServerInfo info = new ServerInfo();
        info.ipAddr = ipAddr;
        info.port = port;
        info.puUid = puUid;
        m_MatchingServerInfoList.Add(info);
#endif
    }

    // サーバの選択
    // @param 選択したサーバの情報
    public bool SelectServer(ServerInfo info)
    {
        // サーバ情報を代入
        m_pMatchingServerInfo = info;

        // サーバへの接続へ
#if SELECT_SERVER_WEB_API
    	SetPhase( PHASE.PHASE_BATTLE_GET_ROOM_LIST, 0 );
#else
        SetPhase(PHASE.PHASE_MATCHING_CONNECT_SERVER, 0);
#endif
        return true;
    }

    // ルームへの入室
    // @param rule
    // @param value
    public void EnterMatchingRoom(uint rule, uint value)
    {
        ClientScene.Instance.Log("Matching Start");
        Logger.MLNLOG_INFO("Entry MatchingRoom");

        // 諸々を初期化しておく
        m_pPU.ClearMatchingData();

        UInt64 nowTime = Utility.GetSecond();
        m_WaitTime_MatchingState = nowTime + 1;

        m_pPU.GetInterface_Matching(m_pPU.GetRpcConnector().GetRPCID()).Send_EnterMatchingRoom(GetCharaId(), rule, value);

        SetPhase(PHASE.PHASE_MATCHING_CHECK_MATCHING_SUCCESS, nowTime + WAIT_TIME_CHANGE_PHASE);
    }

    // マッチング/バトルからの退出
    public void ExitGame()
    {
        if (m_pPU.GetRoomId() != 0)
        {
            m_pPU.GetInterface_Matching(m_pPU.GetRpcConnector().GetRPCID()).Send_ExitMatchingRoom(GetCharaId(), m_pPU.GetRoomId());
        }
        SetPhase(PHASE.PHASE_BATTLE_EXIT_ROOM, 0);
    }

    // 強制バトルスタート
    public void ForceBattleStart()
    {
        m_pPU.ForceStartBattle();
    }

    public override void Update()
    {
        base.Update();
		
	    UInt64 nowTime = Utility.GetSecond();
	    
	    switch ( m_Phase ){
	    case PHASE.PHASE_NONE:{
            if (null == m_pPU) break;

			RelayConnector pRelayConnector = m_pPU.GetRpcConnector().GetRelayConnector();
	        if ( null == pRelayConnector ) break;

			BTL.PU_Client pRpcClient = m_pPU.GetRpcConnector().GetRpcClient();
			if ( null == pRpcClient ) break;
	        
#if SELECT_SERVER_WEB_API
			SetPhase( PHASE.PHASE_BATTLE_GET_ROOM_LIST, 0 );
#else
			SetPhase(PHASE.PHASE_MATCHING_SELECT_SERVER, 0);
#endif
	    }break;
	    
#if ! SELECT_SERVER_WEB_API
        case PHASE.PHASE_MATCHING_SELECT_SERVER:{
            // サーバの選択
            if (onSelectServerEvent != null) {
                onSelectServerEvent(nowTime);
            }
        }break;
		
		// マッチングサーバへ接続
		case PHASE.PHASE_MATCHING_CONNECT_SERVER:{
			m_pPU.GetRpcConnector().ConnectServer(m_pMatchingServerInfo.ipAddr, m_pMatchingServerInfo.port, m_pMatchingServerInfo.puUid);

			SetPhase(PHASE.PHASE_MATCHING_WAIT_CONNECT_SERVER, nowTime + WAIT_TIME_CHANGE_PHASE);
		}break;

		// マッチングサーバ接続待ち
		case PHASE.PHASE_MATCHING_WAIT_CONNECT_SERVER:
			{
				if (m_pPU.GetRpcConnector().GetRelayConnector().IsEndSetup())
				{
					m_pPU.GetRpcConnector().GetRelayConnector().RegistRequestProcessUnit();

					SetPhase(PHASE.PHASE_MATCHING_CONNECT_RPC, 0);
					break;
				}

				if (m_WaitTime < nowTime)
				{
                    Debug.LogError("Error:マッチングサーバへの接続に失敗しました。IPアドレス、ポート番号について御確認ください。");
					Logger.MLNLOG_ERR("Phase Not Change Phase=" + (UInt32)m_Phase);

					SetPhase(PHASE.PHASE_BATTLE_EXIT_ROOM, 0);
				}
			} break;

		// マッチングサーバとPRC接続
		case PHASE.PHASE_MATCHING_CONNECT_RPC:
			{
				RelayConnector pRelayConnector = m_pPU.GetRpcConnector().GetRelayConnector();
				if (ProcessUnitManager.INVALID_PUUID == pRelayConnector.SendPUUID) break;

				// マッチングプロセスとRPC接続開始
				UInt64 rpcId = m_pPU.GetRpcConnector().ConnectRpc(BTL.INTFID.MATCHING, m_pPU.GetRpcConnector().GetPUUID(), pRelayConnector.SendPUUID, pRelayConnector.ClientID);
				Logger.MLNLOG_INFO("MatchingPU Connect RPCID=0x" + Utility.ToHex(rpcId) + ", dstPUUID=0x" + Utility.ToHex(m_pPU.GetRpcConnector().GetPUUID()) + ", srcPUUID=0x" + Utility.ToHex(pRelayConnector.SendPUUID) + ", clientId=0x" + Utility.ToHex(pRelayConnector.ClientID));

				SetPhase(PHASE.PHASE_MATCHING_WAIT_CONNECT_RPC, nowTime + WAIT_TIME_CHANGE_PHASE);
			} break;

		// RPC接続待ち
		case PHASE.PHASE_MATCHING_WAIT_CONNECT_RPC:
			{
				if (m_pPU.GetRpcConnector().IsConnectedRpc())
				{
					Logger.MLNLOG_INFO("Change Entry MatchingRoom");
					SetPhase(PHASE.PHASE_MATCHING_ENTER_ROOM, 0);
					break;
				}

				if (m_WaitTime < nowTime)
				{
					Logger.MLNLOG_ERR("Phase Not Change Phase=" + (UInt32)m_Phase);

					SetPhase(PHASE.PHASE_BATTLE_EXIT_ROOM, 0);
				}
			} break;

		// マッチングルームへ入室
		case PHASE.PHASE_MATCHING_ENTER_ROOM:
			{
                // ルームの選択
                if (onMatchingEnterRoomEvent != null)
                {
                    onMatchingEnterRoomEvent(nowTime);
                }
			} break;

		// マッチングが成立しているか問い合わせ
		case PHASE.PHASE_MATCHING_CHECK_MATCHING_SUCCESS:
			{
				// 入室したルームにマッチングが成立しているかを問い合わせる
				if (m_pPU.GetRoomId() != 0)
				{
					// まだバトル予約が完了していないなら、問い合わせ続ける
					if (BTL.PU_Client.PHASE.PHASE_MATCHING_WAIT_APPOINTMENT == m_pPU.GetPhase())
					{
                        if (m_WaitTime_MatchingState < nowTime)
                        {
                            m_WaitTime_MatchingState = nowTime + 1;
                            m_pPU.GetInterface_Matching(m_pPU.GetRpcConnector().GetRPCID()).Send_GetMatchingStatus(m_pPU.GetRoomId());
                        }
					}
					// バトル予約が完了した
					else{
						// 予約成功
						if (m_pPU.GetBattlePuInfo() != null){
                            ClientScene.Instance.Log("Appointment Battle Success");
							Logger.MLNLOG_INFO("Appointment Battle Success");
							SetPhase(PHASE.PHASE_MATCHING_EXIT_ROOM, nowTime + WAIT_TIME_CHANGE_PHASE);
						}
						// 空いているバトルが無く、予約に失敗していたら一旦終了
						else{
                            ClientScene.Instance.Log("Appointment Battle Failed");
							Logger.MLNLOG_INFO("Appointment Battle Failed");
                            SetPhase(PHASE.PHASE_BATTLE_EXIT_ROOM, 0);
                        }
						
					}
				}
				else if (m_WaitTime < nowTime)
				{
					Logger.MLNLOG_ERR("Phase Not Change Phase=" + (UInt32)m_Phase);

					SetPhase(PHASE.PHASE_BATTLE_END, 0);
				}
			} break;

		// マッチングが成立したら、バトルに移る前にマッチングルームから退室する
		case PHASE.PHASE_MATCHING_EXIT_ROOM:
			{
				Logger.MLNLOG_INFO("Exit MatchingRoom");
				// マッチングルーム退室要求を出す
				m_pPU.GetInterface_Matching(m_pPU.GetRpcConnector().GetRPCID()).Send_ExitMatchingRoom(GetCharaId(), m_pPU.GetRoomId());

				SetPhase(PHASE.PHASE_BATTLE_ENTER_ROOM, nowTime + WAIT_TIME_CHANGE_PHASE);
			} break;
#else
		case PHASE.PHASE_BATTLE_GET_ROOM_LIST:{
			if ( m_WaitTime < nowTime ){
				m_WaitTime = nowTime + 10;
				
				SendRequest_GetRoomList();
			}
		}break;
#endif
	    
		// バトルルーム入室
	    case PHASE.PHASE_BATTLE_ENTER_ROOM:{
	        if ( BTL.PU_Client.PHASE.PHASE_BATTLE == m_pPU.GetPhase() ){
	            m_pPU.EnterRoom();
	            
	            SetPhase( PHASE.PHASE_BATTLE_START, nowTime + WAIT_TIME_CHANGE_PHASE );
	        }else if ( m_WaitTime < nowTime ){
	            Logger.MLNLOG_ERR( "Phase Not Change Phase="+ (UInt32)m_Phase );
	            
	            SetPhase( PHASE.PHASE_BATTLE_EXIT_ROOM, 0 );
	        }
	    }break;
	    
		// バトル開始
	    case PHASE.PHASE_BATTLE_START:{
            if (onBattleStartEvent != null)
            {
                onBattleStartEvent(nowTime);
            }
            if (m_pPU.IsBattle())
			{
	            Logger.MLNLOG_INFO( "Battle Start" );
	            SetPhase( PHASE.PHASE_BATTLE, 0 );

                ClientScene.Instance.Log("Battle Start");
	        }else if ( m_WaitTime < nowTime ){
	            Logger.MLNLOG_ERR( "Phase Not Change Phase="+ (UInt32)m_Phase );
	            
	            SetPhase( PHASE.PHASE_BATTLE_EXIT_ROOM, 0 );
	        }
	    }break;
	    
		// バトル中
	    case PHASE.PHASE_BATTLE:{
			if (m_pPU.IsBattle())
			{
	            // バトル中に行いたい処理をコール
                if (onBattleEvent != null)
                {
                    onBattleEvent(nowTime);
                }
            }else{
	            SetPhase( PHASE.PHASE_BATTLE_END, 0 );
	        }
	    }break;
	    
		// バトル終了
	    case PHASE.PHASE_BATTLE_END:{
            if (onBattleEndEvent != null)
            {
                onBattleEndEvent(nowTime);
            }
            Logger.MLNLOG_INFO("Battle End");
	        SetPhase( PHASE.PHASE_BATTLE_EXIT_ROOM, 0 );

            ClientScene.Instance.Log("Battle End");
	    }break;
	    
		// バトルルーム退室
	    case PHASE.PHASE_BATTLE_EXIT_ROOM:{
            m_pPU.ExitRoom();
	        
	        SetPhase( PHASE.PHASE_BATTLE_DISCONNECT, nowTime + WAIT_TIME_PHASE_BATTLE_DISCONNECT );
	    }break;
	    
		// バトル切断
	    case PHASE.PHASE_BATTLE_DISCONNECT:{
	        if ( m_WaitTime < nowTime ){
	        	Logger.MLNLOG_INFO( "Battle Disconnect" );
	        	m_pPU.DisconnectAll();
	        	
	        	SetPhase( PHASE.PHASE_COOL_DOWN, nowTime + WAIT_TIME_PHASE_COOL_DOWN );
	        }
	    }break;
	    
	    case PHASE.PHASE_COOL_DOWN:{
	        if ( m_WaitTime < nowTime ){
	            ClearPhase();

				m_pPU.GetRpcConnector().RenewRelayConnector();
	            m_pPU.ClearPhase();
	        }
	    }break;
	    }
	    
#if SELECT_SERVER_WEB_API
		m_WebApiManager.Update ();
#endif
	}

    public string GetStatus() {
        Logger.MLNLOG_DEBUG("GetStatus");
        m_pPU.GetStatus();

        string str = "CharaId=0x" + Utility.ToHex(GetCharaId()) + " ClientId=0x" + Utility.ToHex(m_pPU.GetRpcConnector().GetRelayConnector().ClientID);
        Logger.MLNLOG_DEBUG("SendChat Str=" + str);

        return str;
    }

    public void SendChat(string str) {
        m_pPU.SendChat(str);
    }

#if SELECT_SERVER_WEB_API
	/**
     * ルームリスト取得リクエストの送信
     */
	protected void SendRequest_GetRoomList(){
		Hashtable param = new Hashtable();
		
		m_WebApiManager.SendRequest( web.WebApiAgent.RequestType.GET, "app/api/battle/battle/get_room_list", param, RecvResponse_GetRoomList );
	}
	
	/**
     * ルームリスト取得レスポンスの受信
     * 
     * @param   pResponseInfo       レスポンス情報
     */
	protected void RecvResponse_GetRoomList( web.WebApiAgent.HttpResponseInfo pResponseInfo ){
		Logger.MLNLOG_DEBUG( "code="+ pResponseInfo.code +" str="+ pResponseInfo.str );
		
		if ( 200 != pResponseInfo.code ) return;
		
		LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject( pResponseInfo.str );
		do{
			Logger.MLNLOG_DEBUG( "pu_num="+ jsonData[ "data" ][ "pu_num" ] );
			Logger.MLNLOG_DEBUG( "room_hub_pu_high_list="+ jsonData[ "data" ][ "room_hub_pu_high_list" ] );
			Logger.MLNLOG_DEBUG( "room_hub_pu_low_list="+ jsonData[ "data" ][ "room_hub_pu_low_list" ] );
			Logger.MLNLOG_DEBUG( "battle_pu_high_list="+ jsonData[ "data" ][ "battle_pu_high_list" ] );
			Logger.MLNLOG_DEBUG( "battle_pu_low_list="+ jsonData[ "data" ][ "battle_pu_low_list" ] );
			Logger.MLNLOG_DEBUG( "ip_addr_list="+ jsonData[ "data" ][ "ip_addr_list" ] );
			Logger.MLNLOG_DEBUG( "port_list="+ jsonData[ "data" ][ "port_list" ] );
			Logger.MLNLOG_DEBUG( "entered_num_list="+ jsonData[ "data" ][ "entered_num_list" ] );
			Logger.MLNLOG_DEBUG( "max_enter_num_list="+ jsonData[ "data" ][ "max_enter_num_list" ] );
			Logger.MLNLOG_DEBUG( "status_list="+ jsonData[ "data" ][ "status_list" ] );
			
			Int32 pu_num = Convert.ToInt32( (string)jsonData[ "data" ][ "pu_num" ] );
			if ( 0 == pu_num ) break;
			
			char delimiter = ',';
			string[]	roomHubPuHighList = ((string)jsonData[ "data" ][ "room_hub_pu_high_list" ]).Split ( delimiter );
			string[]	roomHubPuLowList = ((string)jsonData[ "data" ][ "room_hub_pu_low_list" ]).Split ( delimiter );
			string[]	battlePuHighList = ((string)jsonData[ "data" ][ "battle_pu_high_list" ]).Split ( delimiter );
			string[]	battlePuLowList = ((string)jsonData[ "data" ][ "battle_pu_low_list" ]).Split ( delimiter );
			string[]	ipAddrList = ((string)jsonData[ "data" ][ "ip_addr_list" ]).Split ( delimiter );
			string[]	portList = ((string)jsonData[ "data" ][ "port_list" ]).Split ( delimiter );
			string[]	enteredNumList = ((string)jsonData[ "data" ][ "entered_num_list" ]).Split ( delimiter );
			string[]	maxEnterNumList = ((string)jsonData[ "data" ][ "max_enter_num_list" ]).Split ( delimiter );
			string[]	statusList = ((string)jsonData[ "data" ][ "status_list" ]).Split ( delimiter );
			
			List<RoomInfo> roomInfoList = new List<RoomInfo>();
			for ( Int32 i = 0; i < pu_num; ++i ){
				UInt64 puUid;
				RoomInfo info = new RoomInfo();
				
				puUid =		Convert.ToUInt64( roomHubPuHighList[ i ] ) << 32;
				puUid +=	Convert.ToUInt32( roomHubPuLowList[ i ] );
				info.roomHubPuUid = puUid;
				
				puUid =		Convert.ToUInt64( battlePuHighList[ i ] ) << 32;
				puUid +=	Convert.ToUInt32( battlePuLowList[ i ] );
				info.battlePuUid = puUid;
				
				info.ipAddr         = ipAddrList[ i ];
				info.port           = Convert.ToUInt16( portList[ i ] );
				info.entered_num    = Convert.ToByte( enteredNumList[ i ] );
				info.max_enter_num  = Convert.ToByte( maxEnterNumList[ i ] );
				info.status         = Convert.ToByte( statusList[ i ] );
				
				roomInfoList.Add( info );
				
				DebugManager.Instance.Log( "["+ i +"] "+ info.ipAddr +":"+ info.port +" 0x"+ Utility.ToHex( info.battlePuUid ) +" "+ info.entered_num +"/"+ info.max_enter_num +" "+ info.status );
			}
				
			DebugManager.Instance.Log ( "RoomListNum="+ pu_num );
			
			// TODO 本来はユーザーが任意のルームを選択する必要があるが、
			// ひとまずランダムで選択したルームに入室しておく
			Int32 roomIndex = (Int32)m_pRandom.Next( pu_num );
			m_pPU.SetRoomHubPU_PUUID( roomInfoList[ roomIndex ].roomHubPuUid );
			if ( m_pPU.ConnectServer( roomInfoList[ roomIndex ].ipAddr, roomInfoList[ roomIndex ].port, roomInfoList[ roomIndex ].battlePuUid ) ){
				SetPhase( PHASE.PHASE_BATTLE_ENTER_ROOM, Utility.GetSecond() + WAIT_TIME_CHANGE_PHASE );
				
				DebugManager.Instance.Log ( "m_CharaId=0x"+ mln.Utility.ToHex( m_CharaId ) );
				DebugManager.Instance.Log( "Choice ["+ roomIndex +"] "+ roomInfoList[ roomIndex ].ipAddr +":"+ roomInfoList[ roomIndex ].port +" 0x"+ Utility.ToHex( roomInfoList[ roomIndex ].battlePuUid ) + " "+ roomInfoList[ roomIndex ].entered_num +"/"+ roomInfoList[ roomIndex ].max_enter_num +" "+ roomInfoList[ roomIndex ].status );
			}
		}while ( false );
	}
#endif
}
}
