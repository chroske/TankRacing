using UnityEngine;
using System;

/*******************************************************************************
 * @file	RpcClientPU.cpp
 * @brief	ユーザー記述
 *******************************************************************************/

using mln;

/**
 * RPC継承PU・クライアントサイド建設予定地
 */
using PUUID = System.UInt64;
namespace BTL
{
	public partial class PU_Client : mln.RPCBasePU
	{
		// フェーズ
		public enum PHASE
		{
			PHASE_NONE = 0,
			PHASE_WAIT_CONNECT_SERVER,
			PHASE_MATCHING_WAIT_APPOINTMENT,
			PHASE_MATCHING_WAIT_EXIT_ROOM,
			PHASE_BATTLE_PASS,
			PHASE_BATTLE_WAIT_PASS,
			PHASE_BATTLE_CONNECT_RPC,
			PHASE_BATTLE_WAIT_CONNECT_RPC,
			PHASE_BATTLE_START,
			PHASE_BATTLE,
			PHASE_BATTLE_END,
		};

		public const UInt64 WAIT_TIME_CHANGE_PHASE = 60;	///< フェーズ変更待ち時間

		protected PHASE m_Phase;							///< フェーズ
		protected UInt64 m_WaitTime;							///< フェーズ変更待ち時間

		protected UInt64 m_CharaId;							///< キャラID

		protected bool m_IsBattle;							///< バトル中かどうか

		protected bool m_IsAppointmentSuccess;				///< バトル予約が完了したかどうか

		protected RpcConnector m_RpcConnector;				///< RPCコネクター

		protected PUUID m_RoomHubPU_PUUID;					///< RoomHubPU用PUUID

		protected UInt64 m_RoomId;							///< ルームID

		protected BTL.BattlePuInfo m_BattlePuInfo;			///< バトル子プロセス情報

		public void SetCharaId(UInt64 charaId) { m_CharaId = charaId; }

		public void SetPhase(PHASE phase, UInt64 waitTime)
		{
			m_Phase = phase;
			m_WaitTime = waitTime;
		}

		public PHASE GetPhase() { return m_Phase; }

		public void SetRoomId( UInt64 roomId ){ m_RoomId = roomId; }
		public UInt64 GetRoomId(){ return m_RoomId; }

		public void ClearPhaseRpcClientPU()
		{
			SetPhase(PHASE.PHASE_NONE, 0);

			m_IsBattle = false;

			m_RoomHubPU_PUUID = 0;
		}

		public bool IsBattle() { return m_IsBattle; }
		public void SetIsBattle(bool isBattle) { m_IsBattle = isBattle; }

		public RpcConnector GetRpcConnector() { return m_RpcConnector; }

		public void SetRoomHubPU_PUUID(PUUID puUid) { m_RoomHubPU_PUUID = puUid; }

		public void SetBattlePuInfo( BTL.BattlePuInfo info ){ m_BattlePuInfo = info; }
		public BTL.BattlePuInfo GetBattlePuInfo(){ return m_BattlePuInfo; }

		public bool IsAppointmentSuccess() { return m_IsAppointmentSuccess; }

		public void ClearMatchingData(){
			m_RoomId = 0;
			m_BattlePuInfo = null;
			m_IsAppointmentSuccess = false;
		}

		void StartRpcClientPU()
		{
			m_RpcConnector = new RpcConnector();

			m_CharaId = 0;

			m_RpcConnector.SetProcessUnit(this);
			m_RpcConnector.SetRpcClient(this);

			SetHealthCheckUpdateTime(Constants.RPC_HEALTH_CHECK_UPDATE_TIME);
		}

		/**
		 * ProcessUnit 向けメッセージを処理する仮想関数
		 * @param	i_pMeta		メッセージメタデータ
		 * @retval	ProcessUnit.MSGPROC_RET_CODE のいずれか
		 */
		public override ProcessUnit.MSGPROC_RET_CODE ProcProcessUnitMessage(MessageMeta i_pMeta)
		{
			MSGPROC_RET_CODE ret = ProcessUnit.MSGPROC_RET_CODE.MSGPROC_OK;
			/*
			Logger.MLNLOG_DEBUG( "type=0x"+ Utility.ToHex( i_pMeta.type )
				+" id=0x"+ Utility.ToHex( i_pMeta.id )
				+" sadr=0x"+ Utility.ToHex( i_pMeta.sadr )
				+" dadr=0x"+ Utility.ToHex( i_pMeta.dadr ) );
			*/
			switch ((PACKET_TYPE_LIB)i_pMeta.id)
			{
				case PACKET_TYPE_LIB.PKTTYPE_CLC_DIRECT_PASSING_NOTIFY:
					{
						if (PHASE.PHASE_BATTLE_WAIT_PASS == m_Phase)
						{
							// PUIDをサーバーに登録
							m_RpcConnector.GetRelayConnector().RegistRequestProcessUnit();

							SetPhase(PHASE.PHASE_BATTLE_CONNECT_RPC, Utility.GetSecond() + WAIT_TIME_CHANGE_PHASE);
						}
					} break;

				default:
					{
						// 基底クラスの処理を呼ぶ 
						ret = base.ProcProcessUnitMessage(i_pMeta);
					} break;
			}

			return ret;
		}

		protected override Int32 ProcUpdate()
		{
			Int32 ret = base.ProcUpdate();
			if (0 != ret) return ret;

			UInt64 nowTime = Utility.GetSecond();

			switch (m_Phase)
			{
				case PHASE.PHASE_NONE:
					{
						if (null != m_RpcConnector.GetRelayConnector())
						{
							SetPhase(PHASE.PHASE_WAIT_CONNECT_SERVER, 0);
						}
					} break;

				case PHASE.PHASE_WAIT_CONNECT_SERVER:
					{
						// サーバーとの接続待ち
					} break;

				case PHASE.PHASE_MATCHING_WAIT_APPOINTMENT:
					{
						// バトル予約待ち
					} break;

				case PHASE.PHASE_MATCHING_WAIT_EXIT_ROOM:
					{
						// ルーム退室待ち
					} break;

				case PHASE.PHASE_BATTLE_PASS:
					{
						if (m_RpcConnector.GetRelayConnector().IsEndSetup())
						{
							PassBattle();

							SetPhase(PHASE.PHASE_BATTLE_WAIT_PASS, nowTime + WAIT_TIME_CHANGE_PHASE);
						}
						else if (m_WaitTime < nowTime)
						{
							Logger.MLNLOG_ERR("Phase Not Change Phase=" + (UInt32)m_Phase + ", CharaId=0x" + Utility.ToHex(m_CharaId));

							SetPhase(PHASE.PHASE_BATTLE_END, 0);
						}
					} break;

				case PHASE.PHASE_BATTLE_WAIT_PASS:
					{
						if (m_WaitTime < nowTime)
						{
							Logger.MLNLOG_ERR("Phase Not Change Phase=" + (UInt32)m_Phase + ", CharaId=0x" + Utility.ToHex(m_CharaId));

							SetPhase(PHASE.PHASE_BATTLE_END, 0);
						}
					} break;

				case PHASE.PHASE_BATTLE_CONNECT_RPC:
					{
						RelayConnector pRelayConnector = m_RpcConnector.GetRelayConnector();
						if (ProcessUnitManager.INVALID_PUUID == pRelayConnector.SendPUUID) break;

						// バトル子プロセスとRPC接続開始
						UInt64 rpcId = m_RpcConnector.ConnectRpc(BTL.INTFID.BATTLE, m_RpcConnector.GetPUUID(), pRelayConnector.SendPUUID, pRelayConnector.ClientID);
						Logger.MLNLOG_INFO("BattlePU Connect CharaId=0x" + Utility.ToHex(m_CharaId)
							+ ", RPCID=0x" + Utility.ToHex(rpcId) + ", dstPUUID=0x" + Utility.ToHex(m_RpcConnector.GetPUUID())
							+ ", srcPUUID=0x" + Utility.ToHex(pRelayConnector.SendPUUID) + ", ClientId=0x" + Utility.ToHex(pRelayConnector.ClientID));

						SetPhase(PHASE.PHASE_BATTLE_WAIT_CONNECT_RPC, nowTime + WAIT_TIME_CHANGE_PHASE);
					} break;

				case PHASE.PHASE_BATTLE_WAIT_CONNECT_RPC:
					{
						if (m_WaitTime < nowTime)
						{
							Logger.MLNLOG_ERR("Phase Not Change Phase=" + (UInt32)m_Phase + ", CharaId=0x" + Utility.ToHex(m_CharaId));

							SetPhase(PHASE.PHASE_BATTLE_END, 0);
						}
					} break;

				case PHASE.PHASE_BATTLE_START:
					{
						SetPhase(PHASE.PHASE_BATTLE, 0);
					} break;

				case PHASE.PHASE_BATTLE:
					{
						// 何もせず、状態が切り替わるのを待つ
					} break;

				case PHASE.PHASE_BATTLE_END:
					{
						ClearPhase();

						m_RpcConnector.RenewRelayConnector();
					} break;
			}

			// RPCのヘルスチェック
			RPC_Update();

			return 0;
		}

		protected override void ProcEvent(EVENT i_pEvent)
		{
			switch ((PU_EVENT)i_pEvent.type)
			{
				case PU_EVENT.PU_EVENT_NOTIFY_DEREG_GPU:
					{			// 接続先の PU の切断通知
						EndianStream es = new EndianStream();
						es.write(i_pEvent.msg.data(), i_pEvent.msg.size());

						PACKET_PNW_GPUINFO data = new PACKET_PNW_GPUINFO();
						data.get(es);

						OnDisconnectedRPC(data.gpuId, RPCBasePU.DISCONNECT_STATE.DISCONNECT_STATE_RPC);
					} break;

				case PU_EVENT.PU_EVENT_NOTIFY_DISCONNECTED_SOCKET:
					{	// クライアントの切断通知
						EndianStream es = new EndianStream();
						es.write(i_pEvent.msg.data(), i_pEvent.msg.size());

						PACKET_CLC_CONDISCONNECTED data = new PACKET_CLC_CONDISCONNECTED();
						data.get(es);

						OnDisconnectedRPC(i_pEvent.msg.sadr, RPCBasePU.DISCONNECT_STATE.DISCONNECT_STATE_SOCKET);
					} break;
			}
		}

		public void PassBattle()
		{
			RelayConnector pRelayConnector = m_RpcConnector.GetRelayConnector();

			MessageMeta meta = new MessageMeta(
				(UInt32)MSGMETA_TYPE_LIB.MSGMETA_TYPE_LIB_PU_TOGLOBAL,
				(UInt32)mln.ROOM_HUB.MSG_ID.MSG_ID_PASS_BATTLE,
				m_RoomHubPU_PUUID,						// dadr
				pRelayConnector.ClientID	// sadr
			);

			// パケット内部情報セット
			EndianStream es = new EndianStream();
			es.put(m_RpcConnector.GetPUUID());
			meta.SetData(es.size(), es.getBuffer());

			// 送信
			pRelayConnector.Post(meta);
		}

		public bool ConnectServer(string ipAddr, UInt16 port, PUUID puUid)
		{
			if (!m_RpcConnector.ConnectServer(ipAddr, port, puUid)) return false;

			SetPhase(PHASE.PHASE_BATTLE_PASS, Utility.GetSecond() + WAIT_TIME_CHANGE_PHASE);
			return true;
		}
	}
}
