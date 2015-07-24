using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/**
 * WebAPI仲介
 */
namespace web {
public class WebApiAgent {
	/// リクエストタイプ
	public enum RequestType {
		GET = 0,
	};
	
	/// HTTPレスポンス情報
	public class HttpResponseInfo {
		public Int32		code;					///< レスポンスコード
		public string		str;					///< レスポンス文字列
		public WWW			www;					///< HTTPデータ
	};
	
	public delegate void CallBackResponse( HttpResponseInfo pResponseInfo );   ///< コールバックレスポンスデリゲート
	
	/// HTTPリクエスト情報
	protected class HttpRequestInfo {
		CallBackResponse			pCallBackResponse;	  ///< コールバックレスポンス
	};
	
	protected String	 m_RootUrl;			  ///< ルートURL
	
	public void SetRootUrl( string rootUrl ){ m_RootUrl = rootUrl; }
	
	protected IEnumerator m_Coroutine;	///< コルーチン
	
	public WebApiAgent(){
		m_RootUrl = "";
		
		m_Coroutine = null;
	}
	
	~WebApiAgent(){
	}
	
	/**
	 * リクエストの送信
	 *
	 * @param   requestType リクエストタイプ
	 * @param	apiUrl		APIのURL
	 * @param	paramMap	パラメーターマップ
	 * @param	pCallBack	コールバック
	 */
	public void SendRequest( RequestType requestType, string apiUrl, Hashtable paramMap, CallBackResponse pCallBack )
	{
		string url;
		url = m_RootUrl + apiUrl;
		
		string paramStr;
		List<string> list = new List<string>();
		foreach( DictionaryEntry param in paramMap ){
			list.Add( param.Key + "=" + param.Value );
		}
		if ( 0 < list.Count ){
			paramStr = string.Join( "&", list.ToArray() );
			
			mln.Logger.MLNLOG_DEBUG( "ParamStr="+ paramStr );
			
			if ( RequestType.GET == requestType ){
				url +=  "?" + paramStr;
			}
		}
		
		mln.Logger.MLNLOG_DEBUG( "Type="+ requestType +" URL="+ url );
		
		m_Coroutine = Fetch( url, pCallBack );
	}
	
	/**
	 *  リクエストを送信し、レスポンスを受信する
	 * 
	 * @param	url				URL
	 * @param	pCallBack	コールバック
	 */
	public IEnumerator Fetch( string url, CallBackResponse pCallBack ){
		WWW www = new WWW( url );
		
		// レスポンスが返ってくるまで待つ
		while ( ! www.isDone ){
			yield return www;
		}
		
		HttpResponseInfo info = new HttpResponseInfo();
		MatchCollection mc = Regex.Matches( www.responseHeaders[ "STATUS" ], "[0-9]{3}" );
		info.code	= ( 0 == mc.Count ) ? 0 : Convert.ToInt32( mc[ 0 ].Value );
		info.str		= String.IsNullOrEmpty( www.error ) ? www.text : www.error;
		info.www	= www;
		pCallBack( info );
	 }
	 
	 public void Update(){
		if ( null != m_Coroutine ){
			bool isContinue = m_Coroutine.MoveNext();
			if ( ! isContinue ){
				m_Coroutine = null;
			}
		}
	 }
}
}
