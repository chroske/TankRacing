using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class ClientScene : SingletonBehaviour<ClientScene> {
	const int MAX_LOG_LINES	= 20;
	
	protected List<string> logList = new List<string>();

    protected string m_LogString = "";
	
	public void Log( object msgObject ){
		logList.Add ( ( null == msgObject ) ? "Null" : msgObject.ToString() );
		if ( MAX_LOG_LINES <= logList.Count ){
			logList.RemoveAt( 0 );
		}

        m_LogString = string.Join("\n", logList.ToArray());
	}
}
