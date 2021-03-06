﻿/*******************************************************************************
 * @file    RPC_BTL_Battle_intf_cl.cs
 * @brief   Auto generated by mbrpcgen.rb
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
namespace BTL
{
    /*
     * function ID
     */
    public enum Battle_FuncID
    {
        ID_QUERYCONNECTBATTLE = 0,
        ID_QUERYCONNECTBATTLERESULT = 1,
        ID_QUERYHEALTHCHECKBATTLE = 2,
        ID_QUERYHEALTHCHECKBATTLERESULT = 3,
        ID_ENTERROOM = 4,
        ID_ENTERROOMRESULT = 5,
        ID_EXITROOM = 6,
        ID_EXITROOMRESULT = 7,
        ID_FORCESTARTBATTLE = 8,
        ID_STARTBATTLE = 9,
        ID_ENDBATTLE = 10,
        ID_GETSTATUS = 11,
        ID_GETSTATUSRESULT = 12,
        ID_CHAT = 13,
        ID_CHATRESULT = 14,
        ID_UPDATESTATUSAPPOINTMENT = 15,
        ID_UPDATESTATUSAPPOINTMENTRESULT = 16,
        ID_QUERYDISCONNECTBATTLE = 17,
	    ID_MAX,
    }
	/*
     * max values
     */
    public enum BattleMaxValues
    {
    }
	/*
     * RPC Stub Interface
     */
    public interface IBattleClient
    {
        /*
         * Send Funcs
         */
        UInt32 Send_QueryConnectBattle( UInt64 any_key );
        UInt32 Send_QueryConnectBattleResult( Byte num );
        UInt32 Send_QueryHealthCheckBattle(  );
        UInt32 Send_QueryHealthCheckBattleResult(  );
        UInt32 Send_EnterRoom( UInt64 chara_id, UInt64 client_id );
        UInt32 Send_ExitRoom( UInt64 chara_id );
        UInt32 Send_ForceStartBattle(  );
        UInt32 Send_GetStatus(  );
        UInt32 Send_Chat( string  str );
        UInt32 Send_UpdateStatusAppointment( UInt64 battle_pu_list_pu_uid, UInt64 matching_room_id );
        UInt32 Send_QueryDisconnectBattle(  );
        /*
         * Receive Funcs
         */
        void Recv_QueryConnectBattle( UInt64 addr, UInt64 any_key );
        void Recv_QueryConnectBattleResult( UInt64 addr, Byte num );
        void Recv_QueryHealthCheckBattle( UInt64 addr );
        void Recv_QueryHealthCheckBattleResult( UInt64 addr );
        void Recv_EnterRoomResult( UInt64 addr, Int32 result );
        void Recv_ExitRoomResult( UInt64 addr, Int32 result );
        void Recv_StartBattle( UInt64 addr );
        void Recv_EndBattle( UInt64 addr );
        void Recv_GetStatusResult( UInt64 addr, BattleStatusInfo status_info );
        void Recv_ChatResult( UInt64 addr, string  str );
        void Recv_UpdateStatusAppointmentResult( UInt64 addr, Int32 result, UInt64 battle_pu_list_pu_uid, Int32 pid, BattleStatusInfo status_info, UInt64 matching_room_id );
        void Recv_QueryDisconnectBattle( UInt64 addr );
    }
}
