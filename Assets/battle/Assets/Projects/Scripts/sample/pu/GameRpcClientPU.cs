using UnityEngine;
using System;

/*******************************************************************************
 * @file	GameRpcClientPU.cpp
 * @brief	ユーザー記述
 *******************************************************************************/

using mln;

namespace BTL {
public partial class PU_Client {
	protected System.Random m_pRandom;

	protected Int32 m_Result;					///< 戻り値

	public virtual void ClearPhase(){
		ClearPhaseRpcClientPU();
		ClearResult();
	}

    public virtual void ClearResult() { m_Result = 0; }
    public virtual Int32 GetResult() { return m_Result; }

	public virtual void Start() {
        StartRpcClientPU();

		m_pRandom = new System.Random( Environment.TickCount );
	}
	
	/**
	 * RPCの切断検出
	 * 
	 * @param	id		ID
	 * @param	state	切断状態
	 * 
	 * 切断状態により、第2引数の state に以下のいずれかの値が指定される
	 * DISCONNECT_STATE_SOCKET                           ソケットレベルでの切断
	 * DISCONNECT_STATE_RPC                              RPC接続時に Disconnect() を実行したことによる切断
	 * DISCONNECT_STATE_RPC_TIMEOUT                      RPCヘルスチェックでのタイムアウト
	 * 
	 * 第1引数の id が state の値により、意味合いが変わる
	 * ソケットレベルでの切断                            自分（クライアント）のPUUID
	 * RPC接続時に Disconnect() を実行したことによる切断 接続先（クライアント、サーバー）のPUUID
	 * RPCヘルスチェックでのタイムアウト                 接続先（クライアント、サーバー）のPUUID
	 */
	protected override void OnDisconnectedRPC( UInt64 id, DISCONNECT_STATE state ){
    	Logger.MLNLOG_DEBUG( "OnDisconnectedRPC id=0x"+ Utility.ToHex( id ) +", state="+ state );
		
		switch ( state ){
		case DISCONNECT_STATE.DISCONNECT_STATE_SOCKET:{			// ソケットレベルでの切断
			m_IsBattle = false;
		}break;
		
		case DISCONNECT_STATE.DISCONNECT_STATE_RPC:{				// RPC接続時に Disconnect() を実行したことによる切断
			// 何もしない
		}break;
		
		case RPCBasePU.DISCONNECT_STATE.DISCONNECT_STATE_RPC_TIMEOUT:{		// RPCヘルスチェックでのタイムアウト
			// 何もしない
		}break;
		}
		
		base.OnDisconnectedRPC( id, state );
    }
	
	public void EnterRoom() {
		GetInterface_Battle(m_RpcConnector.GetRPCID()).Send_EnterRoom(m_CharaId, m_RpcConnector.GetRelayConnector().ClientID);
	}

    public void ExitRoom() {
		if (0 == m_RpcConnector.GetRPCID()) return;

		GetInterface_Battle(m_RpcConnector.GetRPCID()).Send_ExitRoom(m_CharaId);
	}

    public void ForceStartBattle() {
        GetInterface_Battle(m_RpcConnector.GetRPCID()).Send_ForceStartBattle();
    }

    public void GetStatus() {
		GetInterface_Battle(m_RpcConnector.GetRPCID()).Send_GetStatus();
	}
	
	public void SendChat( string str ) {
		GetInterface_Battle(m_RpcConnector.GetRPCID()).Send_Chat(str);
	}

    public void Recv_QueryConnectBattlePuList(UInt64 conid, UInt64 any_key)
    {
    }

    public void Recv_QueryConnectBattlePuListResult(UInt64 conid, byte num)
    {
    }
	
    public void Recv_GetRoomListResult(UInt64 conid, BTL.BattleRoomInfo[] room_info, UInt32 room_info_len) {
        ClientScene.Instance.Log("RoomListNum=" + room_info_len);
		
		if ( 0 == room_info_len ) return;

		if (PHASE.PHASE_WAIT_CONNECT_SERVER != m_Phase) return;
		
		if ( 0 == room_info_len ) return;
		
		for ( UInt32 i = 0; i < room_info_len; ++i ){
			BTL.BattleRoomInfo pInfo = room_info[ i ];
			
			Logger.MLNLOG_DEBUG( "["+ i +"] ipAddr="+ pInfo.pu_info.ip_addr
                +" port="+ pInfo.pu_info.port +" roomHubPuUid=0x"+ Utility.ToHex( pInfo.pu_info.room_hub_pu_uid )
                +" battlePuUid=0x"+ Utility.ToHex( pInfo.pu_info.battle_pu_uid )
                +" enteredNum="+ pInfo.status_info.entered_num +" maxEnterNum="+ pInfo.status_info.max_enter_num +" status="+ pInfo.status_info.status );
            ClientScene.Instance.Log("[" + i + "] " + pInfo.pu_info.ip_addr + ":" + pInfo.pu_info.port + " 0x" + Utility.ToHex(pInfo.pu_info.battle_pu_uid) + " " + pInfo.status_info.entered_num + "/" + pInfo.status_info.max_enter_num + " " + pInfo.status_info.status);
		}
		
		// 接続済PUとの通信を切断
		m_RpcConnector.RenewRelayConnector();
		
		// TODO 本来はユーザーが任意のルームを選択する必要があるが、
		// ひとまずランダムで選択したルームに入室しておく
		Int32 roomIndex = m_pRandom.Next( (Int32)room_info_len );
		BTL.BattleRoomInfo pRoomInfo = room_info[ roomIndex ];
		m_RoomHubPU_PUUID = pRoomInfo.pu_info.room_hub_pu_uid;
		if (ConnectServer(pRoomInfo.pu_info.ip_addr, pRoomInfo.pu_info.port, pRoomInfo.pu_info.battle_pu_uid))
		{
            ClientScene.Instance.Log("m_CharaId=0x" + mln.Utility.ToHex(m_CharaId));
            ClientScene.Instance.Log("Choice [" + roomIndex + "] " + pRoomInfo.pu_info.ip_addr + ":" + pRoomInfo.pu_info.port + " 0x" + Utility.ToHex(pRoomInfo.pu_info.battle_pu_uid) + " " + pRoomInfo.status_info.entered_num + "/" + pRoomInfo.status_info.max_enter_num + " " + pRoomInfo.status_info.status);
		}
	}

    public void Recv_UpdateRoomInfo(UInt64 conid) {
	}

    public void Recv_DeleteRoomInfo(UInt64 conid) {
	}

	public void Recv_QueryConnectBattle( UInt64 conid, UInt64 any_key ) {
	}
	
	public void Recv_QueryConnectBattleResult( UInt64 conid, byte num ) {
		SetPhase( PHASE.PHASE_BATTLE_START, Utility.GetSecond() + WAIT_TIME_CHANGE_PHASE );
	}
	
	public void Recv_EnterRoomResult( UInt64 conid, Int32 result ) {
		if ( result < (Int32)BTL.BattleResult.BATTLERESULT_SUCCESS ){
			Logger.MLNLOG_DEBUG( "Recv_EnterRoomResult CharaId=0x"+ Utility.ToHex( m_CharaId ) +" result="+ result );

			m_Result = result;
		}
	}
	
	public void Recv_ExitRoomResult( UInt64 conid, Int32 result ) {
		if ( result < (Int32)BTL.BattleResult.BATTLERESULT_SUCCESS ){
			Logger.MLNLOG_DEBUG( "Recv_ExitRoomResult CharaId=0x"+ Utility.ToHex( m_CharaId ) +" result="+ result );
			
			m_Result = result;
		}
	}

    public void Recv_StartBattle(UInt64 conid) {
		Logger.MLNLOG_DEBUG( "Recv_StartBattle" );
		m_IsBattle = true;
	}

    public void Recv_EndBattle(UInt64 conid) {
		Logger.MLNLOG_DEBUG( "Recv_EndBattle" );
		m_IsBattle = false;
	}

    public void Recv_GetStatusResult(UInt64 conid, BTL.BattleStatusInfo status_info)
    {
		Logger.MLNLOG_DEBUG( "Recv_GetStatusResult conid=0x"+ Utility.ToHex( conid )
            +", entered_num="+ status_info.entered_num +", max_enter_num="+ status_info.max_enter_num +", status="+ status_info.status );
	}

    public void Recv_ChatResult(UInt64 conid, string str)
    {
		Logger.MLNLOG_DEBUG( "Recv_ChatResult conid=0x"+ Utility.ToHex( conid ) +" str="+ str );

        ClientScene.Instance.Log("Chat : " + str);
	}

	public void Recv_QueryConnectMatching(UInt64 conid, UInt64 any_key)
	{
	}

	public void Recv_QueryConnectMatchingResult(UInt64 conid, Byte num)
	{
	}

	/**
	 * マッチングルーム入室結果
	 */
	public void Recv_EnterMatchingRoomResult(UInt64 conid, UInt64 matching_room_id, Int32 result)
	{
		Logger.MLNLOG_DEBUG("Recv_EnterMatchingRoomResult result = " + result + ", matching_room_id = " + matching_room_id);

		if( result != 0 ){
			// バトル予約待ち状態に切り替える
			m_Phase = PHASE.PHASE_MATCHING_WAIT_APPOINTMENT;
		
			// ルームIDを格納
			m_RoomId = matching_room_id;
            ClientScene.Instance.Log("Entry MatchingRoom Success roomId = " + matching_room_id);
		}
		// 入室失敗
		else{
            ClientScene.Instance.Log("Entry MatchingRoom Failed");
			Logger.MLNLOG_DEBUG("Entry MatchingRoom Failed");
		}
	}

	/**
	 * マッチング状態取得結果
	 */
	public void Recv_GetMatchingStatusResult(UInt64 conid, UInt64 matching_room_id, Byte status, BTL.BattlePuInfo pu_info)
	{
		Logger.MLNLOG_DEBUG("Recv_GetMatchingStatusResult : matching_room_id = " + matching_room_id + ", status = " + status +
			", ip_addr = " + pu_info.ip_addr + ", port = " + pu_info.port + ", room_hub_pu_uid = " + pu_info.room_hub_pu_uid + ", battle_pu_uid = " + pu_info.battle_pu_uid);

		// そもそもバトル予約が完了していなければ終了
		if ((Int32)status != (Int32)BTL.MatchingStatus.MATCHINGSTATUS_BATTLE_APPOINTMENT_SUCCESS &&
			(Int32)status != (Int32)BTL.MatchingStatus.MATCHINGSTATUS_BATTLE_APPOINTMENT_FAILURE)
		{
			return;
		}

		// バトル予約成功
		if ((Int32)status == (Int32)BTL.MatchingStatus.MATCHINGSTATUS_BATTLE_APPOINTMENT_SUCCESS)
		{
			// バトル子プロセス情報を保存しておく
			m_BattlePuInfo = pu_info;
		}

		// 退室待ち状態に切り替える
		m_Phase = PHASE.PHASE_MATCHING_WAIT_EXIT_ROOM;
	}

	/**
	 * マッチングルーム退室結果
	 */
	public void Recv_ExitMatchingRoomResult(UInt64 conid, Int32 result)
	{
		if (result != 0){
            ClientScene.Instance.Log("Exit MatchingRoom Success");

			// 接続済PUとの通信を切断
			m_RpcConnector.RenewRelayConnector();

            ClientScene.Instance.Log("Matching End");

			// バトルサーバとの接続を開始する
			m_RoomHubPU_PUUID = m_BattlePuInfo.room_hub_pu_uid;
			if (ConnectServer(m_BattlePuInfo.ip_addr, m_BattlePuInfo.port, m_BattlePuInfo.battle_pu_uid))
			{
                ClientScene.Instance.Log("m_CharaId=0x" + mln.Utility.ToHex(m_CharaId));
                ClientScene.Instance.Log(m_BattlePuInfo.ip_addr + ":" + m_BattlePuInfo.port + " 0x" + Utility.ToHex(m_BattlePuInfo.battle_pu_uid));
                ClientScene.Instance.Log("Battle Waiting");
			}
		}
		// 退室失敗
		else{
            ClientScene.Instance.Log("Exit MatchingRoom Failed");
			Logger.MLNLOG_DEBUG("Exit MatchingRoom Failed");
		}
	}

	public void Recv_AppointmentBattleResult( UInt64 conid, UInt64 matching_room_id ,BTL.BattlePuInfo battle_pu_info ,UInt64 appointment_battle_pu_time, Int32 result )
	{
	}

	public void Recv_UpdateStatusAppointmentResult(UInt64 addr, Int32 result, UInt64 battle_pu_list_pu_uid, Int32 pid, BattleStatusInfo status_info, UInt64 matching_room_id)
	{
	}
}
}
