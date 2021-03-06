﻿/*******************************************************************************
 * @file	RPC_BTL_Battle_intf_cl.cs
 * @brief	Auto generated by mbrpcgen.rb
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
namespace BTL {
	public partial class PU_Client : mln.RPCBasePU
            , IBattleClient
            , IBattlePuListClient
            , IMatchingClient
    {
        // return 0 if error or next
        public UInt32 Receive_Battle( UInt64 conid, UInt32 type, UInt16 totalSize, mln.EndianStream rStream )
        {
		    UInt32 ofs = 0;
            switch (type)
            {
                case (Int32)Battle_FuncID.ID_QUERYCONNECTBATTLE: // 0 
                    {
						// ARGUMENTS
						// ARG 0 : UInt64 any_key
						UInt64 any_key;
						// BASIC TYPE 
						rStream.get(out any_key);
						ofs += (8);
						// RECEIVE CALL 
						Recv_QueryConnectBattle( conid, 
							any_key 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_QUERYCONNECTBATTLERESULT: // 1 
                    {
						// ARGUMENTS
						// ARG 0 : Byte num
						Byte num;
						// BASIC TYPE 
						rStream.get(out num);
						ofs += (1);
						// RECEIVE CALL 
						Recv_QueryConnectBattleResult( conid, 
							num 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_QUERYHEALTHCHECKBATTLE: // 2 
                    {
						// ARGUMENTS
						// RECEIVE CALL 
						Recv_QueryHealthCheckBattle( conid
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_QUERYHEALTHCHECKBATTLERESULT: // 3 
                    {
						// ARGUMENTS
						// RECEIVE CALL 
						Recv_QueryHealthCheckBattleResult( conid
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_ENTERROOMRESULT: // 5 
                    {
						// ARGUMENTS
						// ARG 0 : Int32 result
						Int32 result;
						// BASIC TYPE 
						rStream.get(out result);
						ofs += (4);
						// RECEIVE CALL 
						Recv_EnterRoomResult( conid, 
							result 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_EXITROOMRESULT: // 7 
                    {
						// ARGUMENTS
						// ARG 0 : Int32 result
						Int32 result;
						// BASIC TYPE 
						rStream.get(out result);
						ofs += (4);
						// RECEIVE CALL 
						Recv_ExitRoomResult( conid, 
							result 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_STARTBATTLE: // 9 
                    {
						// ARGUMENTS
						// RECEIVE CALL 
						Recv_StartBattle( conid
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_ENDBATTLE: // 10 
                    {
						// ARGUMENTS
						// RECEIVE CALL 
						Recv_EndBattle( conid
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_GETSTATUSRESULT: // 12 
                    {
						// ARGUMENTS
						// ARG 0 : BattleStatusInfo status_info
						BattleStatusInfo status_info = new BattleStatusInfo();
						// STRUCT TYPE 
							ofs += BattleStatusInfo.get( rStream, status_info );
						// RECEIVE CALL 
						Recv_GetStatusResult( conid, 
							status_info 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_CHATRESULT: // 14 
                    {
						// ARGUMENTS
						// ARG 0 : const char* str
						// string buffer 
						//char str[RPC_STRLEN_SIZE+8];
						string str;
						// STRING TYPE
						UInt32 str_length = 0U;
                        UInt32 str_str_ofs = mln.cRPC_ID.GetStringFromStream(out str, rStream, out str_length);
						ofs += str_str_ofs;
						// RECEIVE CALL 
						Recv_ChatResult( conid, 
							str 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_UPDATESTATUSAPPOINTMENTRESULT: // 16 
                    {
						// ARGUMENTS
						// ARG 0 : Int32 result
						Int32 result;
						// BASIC TYPE 
						rStream.get(out result);
						ofs += (4);
						// ARG 1 : UInt64 battle_pu_list_pu_uid
						UInt64 battle_pu_list_pu_uid;
						// BASIC TYPE 
						rStream.get(out battle_pu_list_pu_uid);
						ofs += (8);
						// ARG 2 : Int32 pid
						Int32 pid;
						// BASIC TYPE 
						rStream.get(out pid);
						ofs += (4);
						// ARG 3 : BattleStatusInfo status_info
						BattleStatusInfo status_info = new BattleStatusInfo();
						// STRUCT TYPE 
							ofs += BattleStatusInfo.get( rStream, status_info );
						// ARG 4 : UInt64 matching_room_id
						UInt64 matching_room_id;
						// BASIC TYPE 
						rStream.get(out matching_room_id);
						ofs += (8);
						// RECEIVE CALL 
						Recv_UpdateStatusAppointmentResult( conid, 
							result ,
							battle_pu_list_pu_uid ,
							pid ,
							status_info ,
							matching_room_id 
						);
						break;
					}
                case (Int32)Battle_FuncID.ID_QUERYDISCONNECTBATTLE: // 17 
                    {
						// ARGUMENTS
						// RECEIVE CALL 
						Recv_QueryDisconnectBattle( conid
						);
						break;
					}
			}
			return ofs;
        }
		// Send Funcs
		public UInt32 Send_QueryConnectBattle( UInt64 any_key )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_QueryConnectBattle: start") );
		// check send size 
			// BASIC TYPE 
			totalSize += ( 8 );
			//if( totalSize > left ) ERROR_RETURN(0);
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_QUERYCONNECTBATTLE;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			sendEs.put(any_key);
			//RPCPRINT( "Send_QueryConnectBattle: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_QueryConnectBattleResult( Byte num )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_QueryConnectBattleResult: start") );
		// check send size 
			// BASIC TYPE 
			totalSize += ( 1 );
			//if( totalSize > left ) ERROR_RETURN(0);
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_QUERYCONNECTBATTLERESULT;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			sendEs.put(num);
			//RPCPRINT( "Send_QueryConnectBattleResult: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_QueryHealthCheckBattle(  )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_QueryHealthCheckBattle: start") );
		// check send size 
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_QUERYHEALTHCHECKBATTLE;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			//RPCPRINT( "Send_QueryHealthCheckBattle: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_QueryHealthCheckBattleResult(  )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_QueryHealthCheckBattleResult: start") );
		// check send size 
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_QUERYHEALTHCHECKBATTLERESULT;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			//RPCPRINT( "Send_QueryHealthCheckBattleResult: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_EnterRoom( UInt64 chara_id, UInt64 client_id )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_EnterRoom: start") );
		// check send size 
			// BASIC TYPE 
			totalSize += ( 8 );
			//if( totalSize > left ) ERROR_RETURN(0);
			// BASIC TYPE 
			totalSize += ( 8 );
			//if( totalSize > left ) ERROR_RETURN(0);
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_ENTERROOM;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			sendEs.put(chara_id);
			sendEs.put(client_id);
			//RPCPRINT( "Send_EnterRoom: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_ExitRoom( UInt64 chara_id )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_ExitRoom: start") );
		// check send size 
			// BASIC TYPE 
			totalSize += ( 8 );
			//if( totalSize > left ) ERROR_RETURN(0);
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_EXITROOM;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			sendEs.put(chara_id);
			//RPCPRINT( "Send_ExitRoom: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_ForceStartBattle(  )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_ForceStartBattle: start") );
		// check send size 
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_FORCESTARTBATTLE;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			//RPCPRINT( "Send_ForceStartBattle: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_GetStatus(  )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_GetStatus: start") );
		// check send size 
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_GETSTATUS;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			//RPCPRINT( "Send_GetStatus: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_Chat( string  str )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_Chat: start") );
		// check send size 
			UInt32 str_length = (UInt32)System.Text.Encoding.UTF8.GetBytes(str).Length;
			totalSize += 4; // 4:input length 
			totalSize += str_length;
			//if( totalSize > left ) ERROR_RETURN(0);
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_CHAT;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			mln.cRPC_ID.SetStringToStream( str, sendEs, str_length );
			//RPCPRINT( "Send_Chat: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_UpdateStatusAppointment( UInt64 battle_pu_list_pu_uid, UInt64 matching_room_id )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_UpdateStatusAppointment: start") );
		// check send size 
			// BASIC TYPE 
			totalSize += ( 8 );
			//if( totalSize > left ) ERROR_RETURN(0);
			// BASIC TYPE 
			totalSize += ( 8 );
			//if( totalSize > left ) ERROR_RETURN(0);
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_UPDATESTATUSAPPOINTMENT;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			sendEs.put(battle_pu_list_pu_uid);
			sendEs.put(matching_room_id);
			//RPCPRINT( "Send_UpdateStatusAppointment: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public UInt32 Send_QueryDisconnectBattle(  )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID; // PUUID 
			UInt64 conid = 0;
			GetSendID(ref destID, ref srcID, ref conid);
			if( destID == mln.ProcessUnitManager.INVALID_PUUID )
			{
				mln.Utility.MLN_TRACE_LOG("Fail : GetSendID");
				return 0;
			}
				// if( srcID == ProcessUnitManager::INVALID_PUUID ) return 0;
			UInt32 totalSize = 0;
			//RPCPRINT( fprintf(stderr, "Send_QueryDisconnectBattle: start") );
		// check send size 
		// Check size OK  
			mln.EndianStream sendEs = new mln.EndianStream( mln.EndianStream.STREAM_ENDIAN.STREAM_ENDIAN_LITTLE );	// little endian 
			mln.PACKET_HEADER sendHead;
			sendHead.type = (UInt32)Battle_FuncID.ID_QUERYDISCONNECTBATTLE;
			sendHead.dadr = (UInt64)( (UInt64)0<<32 | (UInt64)INTFID.BATTLE );
			sendHead.sadr = conid;
			sendHead.size = (UInt16)totalSize;
		// serialize 
			sendHead.put(sendEs); // serialize head 
			//RPCPRINT( "Send_QueryDisconnectBattle: size %d done", totalSize );
		// send done 
			RPCSendPUBase( sendEs, destID, srcID );
		// finish 
			ClearSendID();
			return totalSize;
		}
		public void Recv_QueryHealthCheckBattle( UInt64 rpc_id )
		{
			EndHealthCheck( rpc_id );
			ResolveSendID( rpc_id );
			Send_QueryHealthCheckBattleResult();
		}
		public void Recv_QueryHealthCheckBattleResult( UInt64 rpc_id )
		{
			EndHealthCheck( rpc_id );
		}
		public void Recv_QueryDisconnectBattle( UInt64 rpc_id )
		{
			UInt64 destID = mln.ProcessUnitManager.INVALID_PUUID;
			UInt64 srcID = mln.ProcessUnitManager.INVALID_PUUID;
			UInt64 conid = 0;
			ResolveSendID( rpc_id );
			GetSendID( ref destID, ref srcID, ref conid );
			if ( destID != mln.ProcessUnitManager.INVALID_PUUID ){
				OnDisconnectedRPC( destID, DISCONNECT_STATE.DISCONNECT_STATE_RPC );
			}
		}
	}
} // namespace
