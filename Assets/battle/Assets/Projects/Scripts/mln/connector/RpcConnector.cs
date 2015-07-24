using UnityEngine;
using System;

/*******************************************************************************
 * @file	RpcConnector.cpp
 * @brief	ユーザー記述
 *******************************************************************************/

using mln;

/**
 * RPCコネクター
 */
namespace mln {
using PUUID = System.UInt64;
public class RpcConnector {
    protected RelayConnector				m_pRelayConnector;                  ///< リレーコネクター
    
	protected SocketManager				m_pSocketManager;					///< 参照用ソケットマネージャー
    protected ProcessUnitBank            m_pProcessUnitBank;                 ///< 参照用プロセスユニットバンク

    protected ProcessUnit m_pProcessUnit;                     ///< 参照用プロセスユニット
    protected BTL.PU_Client m_pRpcClient;                       ///< 参照用RPCクライアント

    protected PUUID m_PUUID;                            ///< PUUID
    protected UInt64 m_RPCID;                            ///< RPCID
	
    public RelayConnector GetRelayConnector(){ return m_pRelayConnector; }
	
	public void SetSocketManager( SocketManager pManager ){ m_pSocketManager = pManager; }
	public SocketManager GetSocketManager(){ return m_pSocketManager; }
	
    public void SetProcessUnitBank( ProcessUnitBank pBank ){ m_pProcessUnitBank = pBank; }
	public ProcessUnitBank GetProcessUnitBank(){ return m_pProcessUnitBank; }

    public void SetProcessUnit(ProcessUnit pProcessUnit) { m_pProcessUnit = pProcessUnit; }
    public ProcessUnit GetProcessUnit() { return m_pProcessUnit; }

    public void SetRpcClient(BTL.PU_Client pRpcClient) { m_pRpcClient = pRpcClient; }
    public BTL.PU_Client GetRpcClient() { return m_pRpcClient; }

    public void SetPUUID(PUUID puUid) { m_PUUID = puUid; }
    public PUUID GetPUUID() { return m_PUUID; }

    public void SetRPCID(UInt64 rpcId) { m_RPCID = rpcId; }
    public UInt64 GetRPCID() { return m_RPCID; }

    public virtual bool IsConnectedRpc() { return m_pRpcClient.IsConfirmedRPC(m_RPCID); }

    void Start() {
        m_pRelayConnector = null;
        m_pSocketManager = null;
        m_pProcessUnit = null;
        m_pProcessUnitBank = null;
    
        m_PUUID = 0;
        m_RPCID = 0;
    }

    ~RpcConnector() {
	    DeleteRelayConnector();
    }

    public void NewRelayConnector() {
	    if ( null != m_pRelayConnector ) return;
	
	    m_pRelayConnector = new RelayConnector( true );
	    m_pRelayConnector.SetSocketManager( m_pSocketManager );
	    m_pRelayConnector.SetProcessUnit( m_pProcessUnit );
    }

    public void DeleteRelayConnector() {
	    if ( null == this.m_pRelayConnector ) return;
	
        if ( null != m_pRpcClient ){
            m_pRpcClient.DeleteConnectionBySrcPUUID( m_pRelayConnector.SendPUUID );
        }
        
        if ( null != m_pProcessUnitBank ){
            m_pProcessUnitBank.RemoveProcessUnitReal( m_PUUID );
        }
	    
	    if ( null != m_pSocketManager ){
            UInt32 socketId = this.m_pRelayConnector.SocketID;
            BaseSocket pSocket = m_pSocketManager.GetSocketFromID(socketId);
		    if ( null != pSocket ){
			    m_pSocketManager.DeleteSocket( pSocket );
                pSocket = null;
		    }
	    }
	    
        m_pProcessUnit.UnsetRelayConnector( this.m_pRelayConnector );
    
	    this.m_pRelayConnector.SetProcessUnit( null );
	    this.m_pRelayConnector.SetSocketManager( null );
	    m_pRelayConnector = null;
    }

    public void RenewRelayConnector(){
	    DeleteRelayConnector();
	
	    NewRelayConnector();
    }

    public bool ConnectServer( string ipAddr, UInt16 port, PUUID puUid ){
        Logger.MLNLOG_INFO( "ipAddr="+ ipAddr +", port="+ port +", puUid=0x"+ Utility.ToHex( puUid ) );
    
        m_pRelayConnector.SetDirectMode( true ); // バトルサーバへは、リレーサーバを介さず直接繋げる
        Connector.CONNECTOR_RET_CODE ret = m_pRelayConnector.Setup(port, ipAddr);
        if (Connector.CONNECTOR_RET_CODE.OK != ret){
            Logger.MLNLOG_WARNING( "Fail Setup ret="+ (Int32)ret );
            return false;
        }
    
        m_PUUID = puUid;
        return true;
    }

    public UInt64 ConnectRpc( BTL.INTFID interfaceId, PUUID dstPuUid, PUUID srcPuUid, UInt64 anyKey ) {
        if ( null == m_pRpcClient ) return 0;
        
        m_pProcessUnit.SetRelayConnector( m_pRelayConnector );
        
        UInt64 rpcId = m_pRpcClient.Connect( (UInt32)interfaceId, dstPuUid, srcPuUid, anyKey );
        if ( 0 == rpcId ){
            Logger.MLNLOG_WARNING( "Fail Connect interfaceId="+ interfaceId
                +" dstPuUid=0x"+ Utility.ToHex( dstPuUid ) +" srcPuUid=0x"+ Utility.ToHex( srcPuUid ) +" anyKey=0x"+ Utility.ToHex( anyKey ) );
            return 0;
        }
    
        m_RPCID = rpcId;
        return rpcId;
    }
}
}