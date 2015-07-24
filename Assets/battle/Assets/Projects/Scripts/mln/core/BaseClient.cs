using UnityEngine;
using System;

/***
 * project : 
 * package : 
 * 
 * Copyright MONOBIT Inc. All rights reserved.
 */

/**
 * ベースクライアントクラス
 */
namespace mln {
using PUUID = UInt64;
public class BaseClient {
	private const UInt64			PHASE_WAIT_CHANGE_TIME	= 10;	///< フェーズ変更待ち時間
    
	protected PUUID                       m_PUUID;
	
	protected bool                        m_IsUpdate;

	protected BTL.PU_Client				m_pPU;

	protected ProcessUnit                m_pProcessUnit;

	protected SocketManager              m_pSocketManager;

	protected ProcessUnitBank            m_pProcessUnitBank;

    protected UInt64 m_CharaId;							///< キャラID
    
    public void SetCharaId(UInt64 charaId) {
        m_CharaId = charaId;
        if (null != m_pPU){
            m_pPU.SetCharaId(charaId);
        }
    }
    
    public UInt64 GetCharaId() {
    	return m_CharaId;
    }

	/**
	 * PU生成処理
	 * 
	 * @return PUUID
	 */
	protected virtual PUUID CreatePU() {
        return m_pProcessUnitBank.CreateProcessUnit(new BTL.ProcessUnitFactory_Client(), false, 0, 0, 0, 0);
    }
    
	public virtual void Start() {
        m_PUUID = 0;
        m_IsUpdate = false;
        m_pPU = null;
        m_pProcessUnit = null;
		m_pProcessUnitBank = new ProcessUnitBank();
        
        do{
            m_pSocketManager = new SocketManager();
            if ( null == m_pSocketManager ){
                Logger.MLNLOG_ERR("Error SocketManager");
                break;
            }
            
            if ( SocketManager.SOCKETMANAGER_RET_CODE.ERROR == m_pSocketManager.Initialize() ){
                Logger.MLNLOG_ERR( "Error SocketManager Initialize" );
                break;
            }

            m_PUUID = CreatePU();
            if (ProcessUnitManager.INVALID_PUUID == m_PUUID)
            {
                Logger.MLNLOG_ERR("Error CreatePU");
                m_IsUpdate = false;
                break;
            }

            m_pProcessUnit = m_pProcessUnitBank.GetProcessUnitFromId(m_PUUID);

			m_pPU = (BTL.PU_Client)m_pProcessUnit as BTL.PU_Client;
			m_pPU.Start();
			m_pPU.SetCharaId( m_CharaId );
			m_pPU.GetRpcConnector().SetSocketManager(m_pSocketManager);
			m_pPU.GetRpcConnector().SetProcessUnit(m_pProcessUnitBank.GetProcessUnitFromId(m_PUUID));
			m_pPU.GetRpcConnector().SetProcessUnitBank(m_pProcessUnitBank);
			m_pPU.GetRpcConnector().NewRelayConnector();

            m_IsUpdate = true;
        }while ( false );
    }
	
	~BaseClient()
    {
		if ( null != m_pProcessUnitBank ){
			m_pProcessUnitBank = null;
		}

		if ( null != m_pPU ){
			m_pPU = null;
		}

        if ( null != m_pProcessUnit ){
            m_pProcessUnit = null;
        }

        if ( null != m_pSocketManager ){
            m_pSocketManager.Terminate();
            m_pSocketManager.Update();
            m_pSocketManager.Destroy();
            
            m_pSocketManager = null;
        }
    }
	
	/**
	 * 更新処理
	 */
	public virtual void Update(){
        if ( ! m_IsUpdate ) return;
        
        m_pProcessUnitBank.Update();

        if (null != m_pSocketManager){
            m_pSocketManager.Update();
        }

        if (null != m_pPU){
            m_pPU.Update();
        }
    }
}
}
