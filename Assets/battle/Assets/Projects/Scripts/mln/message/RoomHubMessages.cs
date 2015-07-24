/**
 * @file	RoomHubMessages.hpp
 * @brief	
 * 
 * 
 * 
 * RoomHubPU通信用パケット
 * 
 * 
 * Copyright MONOBIT Inc. All rights reserved.
 */

namespace mln {
namespace ROOM_HUB {
using PUUID = System.UInt64;

	/**
	 * ローカル通信パケット
	 */
	public enum MSG_ID : uint {
		MSG_ID_START			= 0x10000000,		// ライブラリやFrameworkをかぶらない十分でかい値 (念のため)
		MSG_ID_BLOCK_SIZE__		= 0x00000100,

		// PU->Client, Client->PU
		MSG_ID_CLIENT_START		= MSG_ID_START,
		
		MSG_ID_PASS_BATTLE		= MSG_ID_CLIENT_START,	// バトルのパッシング要求

		// PU->Server, Server->PU
		MSG_ID_SERVER_START		= MSG_ID_START + MSG_ID_BLOCK_SIZE__,
		
		
	};

	/* メッセージ用構造体 */

	/**
	 * パッシング要求 (MSG_ID_PASS_BATTLE)
	 */
	public struct MSG_DATA_REQ_PASS_BATTLE {
		PUUID	puUid;			// パッシング先のPUUID
		
		/// EndianStream に出力する
		public void put(EndianStream es)
		{
			es.put(puUid);
		}
		
		/// EndianStream から入力する
		public void get(EndianStream es)
		{
			es.get(out puUid );
		}
	};
}
}
