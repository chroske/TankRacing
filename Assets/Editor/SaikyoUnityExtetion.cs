using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Net;
using System.Globalization;
using System.Security.Cryptography;
using MiniJSON;
using System.Xml;
using System.Reflection;



public class SaikyoUnityExtetion : EditorWindow 
{
	// 変数
	private string tweetText = string.Empty;
	
	private List<List<List<Dictionary<string,string>>>> objList = null;
	
	private List<bool> foldOutFlagList = new List<bool>();
	private List<bool> foldOutFlagList2 = new List<bool>();
	
	private Vector2 scrollPosition = Vector2.zero;
	
	private static bool IsRegex = true;

	private string since_id = "0";

	private List<Dictionary<string,string>> HomeTimeLineList = new List<Dictionary<string,string>>();

	private string oauth_consumer_key = "xhRaULqKMOdbYIgtYUyHwVXjn";
	private string oauth_consumer_secret  = "T712U3kcBMNndx6VLpbBx5Vkb2mh5vyOMYfmzUGUDkYeNUJEzh";
	private string oauth_token = "71981472-aUOUq61ZxI72XoWT1Sw8KSpEwOG4dZwCtrAhaH3w9";
	private string oauth_token_secret = "LQ7b99yShWbAoP27746W64vzsTOhDxuy9ZWzRQ3NvJiBZ";
	private string oauth_signature_method = "HMAC-SHA1";
	private string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
	private string oauth_version = "1.0";
	private string count = "15";


	// method
	[MenuItem("Window/SaikyoUnityExtetion")]
	static void Open()
	{
		EditorWindow.GetWindow<SaikyoUnityExtetion>( "SaikyoUnityExtetion" );
	}

	void OnGUI()
	{
		GUILayout.Label( "TweetText" );
		tweetText = GUILayout.TextArea( tweetText );

		if (GUILayout.Button ("Tweet")) {
			EditorCoroutine.Start(Tweet());
			//ついーとしたら更新もする
			EditorCoroutine.Start(Reload());
		}

		
		if (GUILayout.Button ("Reload")) {
			EditorCoroutine.Start(Reload());
		}

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		DrawTimeLine();
		
		GUILayout.EndScrollView();	// スクロールバー終了
		
	}

	private IEnumerator Reload (){
		string urlstr = "https://api.twitter.com/1.1/statuses/home_timeline.json";

		Dictionary<string, string> parameters = new Dictionary<string, string>();

		parameters.Add("oauth_version", oauth_version);
		parameters.Add("oauth_nonce", oauth_nonce);
		parameters.Add("oauth_timestamp", GenerateTimeStamp());
		parameters.Add("oauth_signature_method", oauth_signature_method);
		parameters.Add("oauth_consumer_key", oauth_consumer_key);
		parameters.Add("oauth_consumer_secret", oauth_consumer_secret);
		parameters.Add("oauth_token", oauth_token);
		parameters.Add("oauth_token_secret", oauth_token_secret);
		parameters.Add("count", count);
		if(since_id != "0"){
			parameters.Add("since_id", since_id);
		}

		string oauth_signature = GenerateSignature("GET", urlstr, parameters);

		parameters.Add("oauth_signature", oauth_signature);

		var sortedParameters = from p in parameters
			where OAuthParametersToIncludeInHeader.Contains(p.Key)
				orderby p.Key, UrlEncode(p.Value)
				select p;

		StringBuilder authHeaderBuilder = new StringBuilder("OAuth ");

		int i = 0;
		string kanma = "";
		foreach (var item in sortedParameters)
		{
			if(i != 0){
				kanma = ",";
			}
			authHeaderBuilder.AppendFormat(kanma+"{0}=\"{1}\"", UrlEncode(item.Key), UrlEncode(item.Value));
			i++;
		}
		
		authHeaderBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(parameters["oauth_signature"]));

		WWWForm form = new WWWForm();
		Dictionary<string, string> headers = form.headers;
		headers ["Authorization"] = authHeaderBuilder.ToString ();

		string url = urlstr + "?" + "count=" + count;
		if(since_id != "0"){
			url += "&since_id=" + since_id;
		}

		WWW www = new WWW(url, null, headers);
		yield return www;
		if (www.error == null) {
			IList TimeLineList = (IList)Json.Deserialize(www.text);
			List<Dictionary<string,string>> HomeTimeLineListBlock = new List<Dictionary<string,string>>();

			int j = 0;

			foreach(IDictionary person in TimeLineList){
				Dictionary<string,string> TimeLineListDatas = new Dictionary<string,string>();
				if(j == 0){
					since_id = person["id"].ToString();
				}

				IDictionary datas = (IDictionary)person["user"];

				if(datas["screen_name"] != null){
					TimeLineListDatas.Add ("user_id",datas["screen_name"].ToString());
				}

				TimeLineListDatas.Add ("tweet_text",person["text"].ToString().Replace("\n"," "));

				HomeTimeLineListBlock.Add (TimeLineListDatas);

				j++;

				Debug.Log(datas["screen_name"]);
				Debug.Log(person["text"]);
			}

			HomeTimeLineListBlock.AddRange(HomeTimeLineList);

			HomeTimeLineList = HomeTimeLineListBlock;

			Debug.Log (www.text);
		
		} else {
			Debug.Log (www.text);
		}



		// header
		//var headers = new Dictionary<string, string> ();
		//Hashtable headers = new Hashtable();



//		string inputText = "71981472-aUOUq61ZxI72XoWT1Sw8KSpEwOG4dZwCtrAhaH3w9:LQ7b99yShWbAoP27746W64vzsTOhDxuy9ZWzRQ3NvJiBZ";
//
//		byte[] bytesToEncode = Encoding.UTF8.GetBytes (inputText);
//		string credential = Convert.ToBase64String (bytesToEncode);
//		
////		headers.Add("Authorization", "Basic "+credential);
////		headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
//
//		// post param
//		WWWForm form = new WWWForm();
//		//form.AddField("BODY", "grant_type=client_credentials");
//		//form.AddField("grant_type", "client_credentials");
//
//		Dictionary<string, string> headers = form.headers;
//		//byte[] data = form.data;
//
//		//headers["Authorization"] = "Basic " + credential;
//		headers ["Authorization"] = string.Format ("Basic {0}", credential);
//		Debug.Log (headers ["Authorization"]);
//		//headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("71981472-aUOUq61ZxI72XoWT1Sw8KSpEwOG4dZwCtrAhaH3w9:LQ7b99yShWbAoP27746W64vzsTOhDxuy9ZWzRQ3NvJiBZ"));
//		headers["Content-Type"] = "application/x-www-form-urlencoded;charset=UTF-8";
////		form.AddField("Authorization", "Basic "+credential);
////		form.AddField("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
////		form.AddField("BODY", "grant_type=client_credentials");
//
//		//byte[] data = System.Text.Encoding.UTF8.GetBytes("grant_type=client_credentials");
//		byte[] data = System.Text.Encoding.UTF8.GetBytes("grant_type=client_credentials");
//
//
//		//string dataStr = "{\"grant_type\":\"client_credentials\"}";
//		//byte[] data = Encoding.Default.GetBytes (dataStr);
//		//byte[] data = form.data;
//
//		//WWW www = new WWW(url, form.data, headers);
//		WWW www = new WWW(url, data, headers);
//		yield return www;
//		if (www.error == null) {
//			Debug.Log (www.text);
//		} else {
//			Debug.Log (www.text);
//		}
	}

	private IEnumerator Tweet (){
		string urlstr = "https://api.twitter.com/1.1/statuses/update.json";
		
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		
		parameters.Add("oauth_version", oauth_version);
		parameters.Add("oauth_nonce", oauth_nonce);
		parameters.Add("oauth_timestamp", GenerateTimeStamp());
		parameters.Add("oauth_signature_method", oauth_signature_method);
		parameters.Add("oauth_consumer_key", oauth_consumer_key);
		parameters.Add("oauth_consumer_secret", oauth_consumer_secret);
		parameters.Add("oauth_token", oauth_token);
		parameters.Add("oauth_token_secret", oauth_token_secret);
		parameters.Add("status", tweetText);

		string oauth_signature = GenerateSignature("POST", urlstr, parameters);
		
		parameters.Add("oauth_signature", oauth_signature);
		
		var sortedParameters = from p in parameters
			where OAuthParametersToIncludeInHeader.Contains(p.Key)
				orderby p.Key, UrlEncode(p.Value)
				select p;
		
		StringBuilder authHeaderBuilder = new StringBuilder("OAuth ");
		
		int i = 0;
		string kanma = "";
		foreach (var item in sortedParameters)
		{
			if(i != 0){
				kanma = ",";
			}
			authHeaderBuilder.AppendFormat(kanma+"{0}=\"{1}\"", UrlEncode(item.Key), UrlEncode(item.Value));
			i++;
		}
		
		authHeaderBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(parameters["oauth_signature"]));
		
		WWWForm form = new WWWForm();
		form.AddField("status", tweetText);

		Dictionary<string, string> headers = form.headers;
		headers ["Authorization"] = authHeaderBuilder.ToString ();
		
		WWW www = new WWW(urlstr, form.data, headers);
		yield return www;

		if (www.error == null) {
			Debug.Log (www.text);
			tweetText = "";

		} else {
			Debug.Log (www.text);
		}
	}

	private static string GenerateTimeStamp()
	{
		// Default implementation of UNIX time of the current UTC time
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return Convert.ToInt64(ts.TotalSeconds, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);
	}

	private static readonly string[] SecretParameters = new[]
	{
		"oauth_consumer_secret",
		"oauth_token_secret",
		"oauth_signature"
	};

	private static readonly string[] OAuthParametersToIncludeInHeader = new[]
	{
		"oauth_version",
		"oauth_nonce",
		"oauth_timestamp",
		"oauth_signature_method",
		"oauth_consumer_key",
		"oauth_token",
		"oauth_verifier",
		"screen_name",
		"count"
	};

	private static string GenerateSignature(string httpMethod, string url, Dictionary<string, string> parameters)
	{
		var nonSecretParameters = (from p in parameters
		                           where !SecretParameters.Contains(p.Key)
		                           select p);
		
		// Create the base string. This is the string that will be hashed for the signature.
		string signatureBaseString = string.Format(CultureInfo.InvariantCulture,
		                                           "{0}&{1}&{2}",
		                                           httpMethod,
		                                           UrlEncode(NormalizeUrl(new Uri(url))),
		                                           UrlEncode(nonSecretParameters));


		// Create our hash key (you might say this is a password)
		string key = string.Format(CultureInfo.InvariantCulture,
		                           "{0}&{1}",
		                           UrlEncode(parameters["oauth_consumer_secret"]),
		                           parameters.ContainsKey("oauth_token_secret") ? UrlEncode(parameters["oauth_token_secret"]) : string.Empty);
		
		
		// Generate the hash
		HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(key));
		byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
		return Convert.ToBase64String(signatureBytes);
	}

	private static string UrlEncode(IEnumerable<KeyValuePair<string, string>> parameters)
	{
		StringBuilder parameterString = new StringBuilder();
		
		var paramsSorted = from p in parameters
			orderby p.Key, p.Value
				select p;
		
		foreach (var item in paramsSorted)
		{
			if (parameterString.Length > 0)
			{
				parameterString.Append("&");
			}
			
			parameterString.Append(
				string.Format(
				CultureInfo.InvariantCulture,
				"{0}={1}",
				UrlEncode(item.Key),
				UrlEncode(item.Value)));
		}
		
		return UrlEncode(parameterString.ToString());
	}

	private static string UrlEncode(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		
		value = Uri.EscapeDataString(value);
		
		// UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
		value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
		
		// these characters are not escaped by UrlEncode() but needed to be escaped
		value = value
			.Replace("(", "%28")
				.Replace(")", "%29")
				.Replace("$", "%24")
				.Replace("!", "%21")
				.Replace("*", "%2A")
				.Replace("'", "%27");
		
		// these characters are escaped by UrlEncode() but will fail if unescaped!
		value = value.Replace("%7E", "~");
		
		return value;
	}

	private static string NormalizeUrl(Uri url)
	{
		string normalizedUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", url.Scheme, url.Host);
		if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
		{
			normalizedUrl += ":" + url.Port;
		}
		
		normalizedUrl += url.AbsolutePath;
		return normalizedUrl;
	}

	/* 結果一覧を描画 */
	private void DrawTimeLine()
	{
		int i = 0;
		//int j = 0;
		if (HomeTimeLineList != null) {
			foreach(Dictionary<string,string> HomeTimeLineData in HomeTimeLineList){
				// インデント初期化
				EditorGUI.indentLevel = 0;
				
				foldOutFlagList.Add(false);
				foldOutFlagList[i] = EditorGUILayout.Foldout( foldOutFlagList[i],HomeTimeLineData["user_id"]+" "+HomeTimeLineData["tweet_text"]);

				i++;
			}
		}
	}
	


}