//#define SELECT_SERVER_WEB_API

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class ClientScene : SingletonBehaviour<ClientScene> {

    private enum PHASE
    {
        PHASE_NONE = 0,
#if ! SELECT_SERVER_WEB_API
        PHASE_MATCHING_SELECT_SERVER,
        PHASE_MATCHING_CONNECT_SERVER,
        PHASE_MATCHING_ENTER_ROOM,
#else
		PHASE_BATTLE_GET_ROOM_LIST,
#endif
        PHASE_BATTLE_ENTER_ROOM,
        PHASE_BATTLE_START,
        PHASE_BATTLE,
        PHASE_BATTLE_END,
    };
    private PHASE m_Phase;
    
    
    // マッチングサーバ設定
    private string m_TargetIpAddr = "192.168.189.128";   ///< IPアドレス
    private string m_TargetPort = "8501";               ///< ポート
    private string m_TargetPUUID_High = "0x00024000";   ///< PUUID（上位）
    private string m_TargetPUUID_Low = "0x00000001";   ///< PUUID（下位）

    public static string m_CharaId = "0";   ///< キャラクタID
    private string m_MatchingRule = "1";    ///< マッチングルール
    private string m_MatchingValue = "0";   ///< マッチング値？

    private string m_ChatString = "";   ///< 会話文の設定

    // GUIまわりの記述
    void OnGUI() {
        switch (m_Phase)
        {
            case PHASE.PHASE_NONE:
                {
                } break;
#if ! SELECT_SERVER_WEB_API
            case PHASE.PHASE_MATCHING_SELECT_SERVER:
                {
                    // 初期化
                    m_WaitTime = 0;

                    // TODO 本来はクライアント固有のキャラIDを設定すべきだが、
                    // ランダムな値を設定しておく
                    if ("0" == m_CharaId)
                    {
                        System.Random rand = new System.Random();
                        m_CharaId = "0x" + rand.Next(Int32.MaxValue).ToString("x");
                    }

                    // マッチングサーバ情報＆キャラクタIDの入力欄
                    GUI.Label(new Rect(10, 0, 620, 20), "IPアドレス　　　　 ポート番号　PUUID(上位16進)　PUUID(下位16進)　キャラクタID");
                    m_TargetIpAddr = GUI.TextField(new Rect(10, 20, 110, 20), m_TargetIpAddr);
                    m_TargetPort = GUI.TextField(new Rect(135, 20, 60, 20), m_TargetPort);
                    m_TargetPUUID_High = GUI.TextField(new Rect(210, 20, 100, 20), m_TargetPUUID_High);
                    m_TargetPUUID_Low = GUI.TextField(new Rect(330, 20, 100, 20), m_TargetPUUID_Low);
                    m_CharaId = GUI.TextField(new Rect(445, 20, 100, 20), m_CharaId);

                    // 「マッチングリスト登録」ボタン押下で、マッチングサーバリストに追加
                    if (GUI.Button(new Rect(550, 0, 80, 40), "マッチング\nリスト登録"))
                    {
                        // マッチングサーバの設定                    
                        m_Client.EntryServerInfo(m_TargetIpAddr, Convert.ToUInt16(m_TargetPort, 10), (Convert.ToUInt64(m_TargetPUUID_High, 16) << 32) + Convert.ToUInt64(m_TargetPUUID_Low, 16));
                    }

                    // マッチングサーバリストの表示
                    if ((int)m_Client.m_MatchingServerInfoList.Count > 0)
                    {
                        GUI.Label(new Rect(10, 50, 620, 20), "マッチングサーバリスト");
                    }

                    // リストから選択
                    for (int i = 0; i < (int)m_Client.m_MatchingServerInfoList.Count; i++)
                    {
                        // 生成されたマッチングサーバのリストボタン押下で、コネクションスタート
                        mln.Client.ServerInfo m_pMatchingServerInfo = m_Client.m_MatchingServerInfoList[i];
                        if (GUI.Button(new Rect(10, 70 + i * 20, 620, 20), "IPAddr = " + m_pMatchingServerInfo.ipAddr +
                                                                           ", Port = " + m_pMatchingServerInfo.port +
                                                                           ", PUUID = 0x" + mln.Utility.ToHex(m_pMatchingServerInfo.puUid)))
                        {
                            // キャラクタIDの設定
                            m_Client.SetCharaId(Convert.ToUInt64(m_CharaId, 16));

                            // マッチングサーバの選択
                            m_Client.m_MatchingServerInfoList.Clear();
                            m_Client.SelectServer(m_pMatchingServerInfo);

                            m_Phase = PHASE.PHASE_MATCHING_CONNECT_SERVER;
                        }
                    }
                }
                break;
            case PHASE.PHASE_MATCHING_CONNECT_SERVER:
                {
                    // 現在選択中のルーム情報を表示
                    if (m_Client.m_pMatchingServerInfo != null)
                    {
                        GUI.Button(new Rect(10, 0, 620, 20), "IPAddr = " + m_Client.m_pMatchingServerInfo.ipAddr +
                                                             ", Port = " + m_Client.m_pMatchingServerInfo.port +
                                                             ", PUUID = 0x" + mln.Utility.ToHex(m_Client.m_pMatchingServerInfo.puUid) +
                                                             ", CharaId = " + m_CharaId);
                    }
                    // ログを表示
                    GUI.TextArea(new Rect(10, 20, 620, 460), m_LogString);
                }
                break;
            case PHASE.PHASE_MATCHING_ENTER_ROOM:
                {
                    // 現在選択中のルーム情報を表示
                    GUI.Button(new Rect(10, 0, 620, 20), "IPAddr = " + m_Client.m_pMatchingServerInfo.ipAddr +
                                                         ", Port = " + m_Client.m_pMatchingServerInfo.port +
                                                         ", PUUID = 0x" + mln.Utility.ToHex(m_Client.m_pMatchingServerInfo.puUid) +
                                                         ", CharaId = " + m_CharaId);
                    // マッチングルールの設定
                    GUI.Label(new Rect(10, 20, 620, 20), "マッチングルール　　マッチング値");
                    m_MatchingRule = GUI.TextField(new Rect(10, 40, 110, 20), m_MatchingRule);	    // TODO: ひとまず仮の値
                    m_MatchingValue = GUI.TextField(new Rect(140, 40, 60, 20), m_MatchingValue);	// TODO: ひとまず仮の値

                    if (GUI.Button(new Rect(240, 20, 120, 40), "マッチング\nスタート") )
                    {
                        m_Client.EnterMatchingRoom(Convert.ToUInt32(m_MatchingRule, 10), Convert.ToUInt32(m_MatchingValue, 10));
                        m_Phase = PHASE.PHASE_BATTLE_ENTER_ROOM;
                    }
                    if (GUI.Button(new Rect(360, 20, 120, 40), "マッチング\n退出" ) )
                    {
                        m_Client.ExitGame();
                        m_Phase = PHASE.PHASE_BATTLE_END;
                    }

                    // ログを表示
                    GUI.TextArea(new Rect(10, 60, 620, 420), m_LogString);
                }
                break;
            case PHASE.PHASE_BATTLE_ENTER_ROOM:
                {
                    // 現在選択中のルーム情報を表示
                    GUI.Button(new Rect(10, 0, 620, 20), "IPAddr = " + m_Client.m_pMatchingServerInfo.ipAddr +
                                                         ", Port = " + m_Client.m_pMatchingServerInfo.port +
                                                         ", PUUID = 0x" + mln.Utility.ToHex(m_Client.m_pMatchingServerInfo.puUid) +
                                                         ", CharaId = " + m_CharaId);
                    // マッチングルールの情報を表示
                    GUI.Button(new Rect(10, 20, 620, 20), "マッチングルール = " + m_MatchingRule + ", マッチング値 = " + m_MatchingValue);

                    // ログを表示
                    GUI.TextArea(new Rect(10, 40, 620, 440), m_LogString);
                }
                break;
            case PHASE.PHASE_BATTLE_START:
                {
                    // 現在選択中のルーム情報を表示
                    GUI.Button(new Rect(10, 0, 620, 20), "IPAddr = " + m_Client.m_pMatchingServerInfo.ipAddr +
                                                         ", Port = " + m_Client.m_pMatchingServerInfo.port +
                                                         ", PUUID = 0x" + mln.Utility.ToHex(m_Client.m_pMatchingServerInfo.puUid) +
                                                         ", CharaId = " + m_CharaId);
                    // マッチングルールの情報を表示
                    GUI.Button(new Rect(10, 20, 620, 20), "マッチングルール = " + m_MatchingRule + ", マッチング値 = " + m_MatchingValue);

                    if (GUI.Button(new Rect(10, 40, 120, 40), "強制バトル\nスタート"))
                    {
                        m_Client.ForceBattleStart();
                        m_Phase = PHASE.PHASE_BATTLE;
                    }
                    if (GUI.Button(new Rect(130, 40, 120, 40), "バトル\n退出"))
                    {
                        m_Client.ExitGame();
                        m_Phase = PHASE.PHASE_BATTLE_END;
                    }

                    // ログを表示
                    GUI.TextArea(new Rect(10, 80, 620, 400), m_LogString);
                }
                break;
            case PHASE.PHASE_BATTLE:
                {
                    // 現在選択中のルーム情報を表示
                    GUI.Button(new Rect(10, 0, 620, 20), "IPAddr = " + m_Client.m_pMatchingServerInfo.ipAddr +
                                                         ", Port = " + m_Client.m_pMatchingServerInfo.port +
                                                         ", PUUID = 0x" + mln.Utility.ToHex(m_Client.m_pMatchingServerInfo.puUid) +
                                                         ", CharaId = " + m_CharaId);
                    // マッチングルールの情報を表示
                    GUI.Button(new Rect(10, 20, 620, 20), "マッチングルール = " + m_MatchingRule + ", マッチング値 = " + m_MatchingValue);

                    if (GUI.Button(new Rect(450, 40, 180, 20), "バトルから退出"))
                    {
                        m_Client.ExitGame();
                        m_Phase = PHASE.PHASE_BATTLE_END;
                    }

                    // TODO とりあえずチャット発言するための機能を設定
                    {
                        m_ChatString = GUI.TextField(new Rect(10, 40, 320, 20), m_ChatString );
                        if( GUI.Button(new Rect(330, 40, 120, 20 ), "発言する" ) )
                        {
                            if( m_ChatString.Length > 0 )
                            {
                                m_Client.SendChat(m_Client.GetStatus() + " : " + m_ChatString);
                                m_ChatString = "";
                            }
                        }
                    }

                    // ログを表示
                    GUI.TextArea(new Rect(10, 60, 620, 400), m_LogString);
                }
                break;
            case PHASE.PHASE_BATTLE_END:
            default:
                {
                    // ログを表示
                    GUI.TextArea(new Rect(10, 0, 620, 480), m_LogString);
                }
                break;
#else
		    case mln.Client.PHASE.PHASE_BATTLE_GET_ROOM_LIST:
                break;
#endif
        }
    }



























}
