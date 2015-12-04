using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


public class SaikyoUnityExtetion : EditorWindow 
{
	// 変数
	private string searchText = string.Empty;
	
	private List<List<List<Dictionary<string,string>>>> objList = null;
	
	private List<bool> foldOutFlagList = new List<bool>();
	private List<bool> foldOutFlagList2 = new List<bool>();
	
	private Vector2 scrollPosition = Vector2.zero;
	
	private static bool IsRegex = true;
	
	
	// method
	[MenuItem("Window/SaikyoUnityExtetion")]
	static void Open()
	{
		EditorWindow.GetWindow<SaikyoUnityExtetion>( "SaikyoUnityExtetion" );
	}
	
	
	
	void OnGUI()
	{
		GUILayout.Label( "SearchText" );
		searchText = GUILayout.TextArea( searchText );
		
		IsRegex = EditorGUILayout.Toggle( "大文字小文字チェック", IsRegex );
		
		
		if( GUILayout.Button("Scan") )
			EditorCoroutine.Start(Find());
			//Find();
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		//DrawResults(); // 結果一覧
		
		GUILayout.EndScrollView();	// スクロールバー終了
		
	}

	private IEnumerator Find (){
		string url = "https://api.twitter.com/oauth2/token";

		// header
		var headers = new Dictionary<string, string> ();
		//Hashtable headers = new Hashtable();

		string inputText = "71981472-aUOUq61ZxI72XoWT1Sw8KSpEwOG4dZwCtrAhaH3w9:LQ7b99yShWbAoP27746W64vzsTOhDxuy9ZWzRQ3NvJiBZ";
		byte[] bytesToEncode = Encoding.UTF8.GetBytes (inputText);
		string credential = Convert.ToBase64String (bytesToEncode);
		
//		headers.Add("Authorization", "Basic "+credential);
//		headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");

		// post param
		WWWForm form = new WWWForm();

		headers ["Authorization"] = "Basic " + credential;
		headers ["Content-Type"] = "application/x-www-form-urlencoded;charset=UTF-8";
//		form.AddField("Authorization", "Basic "+credential);
//		form.AddField("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
//		form.AddField("BODY", "grant_type=client_credentials");

		//headers = form.headers;

		byte[] data = System.Text.Encoding.UTF8.GetBytes("grant_type=client_credentials");



		//string dataStr = "{\"grant_type\":\"client_credentials\"}";
		//byte[] data = Encoding.Default.GetBytes (dataStr);
		//byte[] data = form.data;

		//WWW www = new WWW(url, form.data, headers);
		WWW www = new WWW(url, data, headers);
		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
		} else {
			Debug.Log (www.text);
		}
	}

	
//	/* 文字列検索 */
//	private void Find (){
//		if(searchText != "" && searchText != null){
//			objList = new List<List<List<Dictionary<string,string>>>>();
//			
//			//foldOutFlag format
//			foldOutFlagList = new List<bool>();
//			foldOutFlagList2 = new List<bool>();
//			
//			foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
//			{
//				// シーン上に存在するオブジェクトならば処理
//				if(obj.activeInHierarchy){
//					// アセットからパスを取得.シーン上に存在するオブジェクトの場合,シーンファイル（.unity）のパスを取得.
//					string path = AssetDatabase.GetAssetOrScenePath(obj);
//					// シーン上に存在するオブジェクトかどうか文字列で判定.
//					bool isScene = path.Contains(".unity");
//					// シーン上に存在するオブジェクトならば処理.
//					if (isScene)
//					{
//						List<List<Dictionary<string,string>>> componentList = new List<List<Dictionary<string,string>>> ();
//						
//						// コンポーネント一覧を配列で取得
//						var components = obj.GetComponents<MonoBehaviour> ();
//						if (components != null) {
//							foreach(var componentData in components){
//								if(componentData != null){
//									
//									// コンポーネントのテキストを取得
//									var monoscript = MonoScript.FromMonoBehaviour (componentData);
//									List<Dictionary<string,string>> lines = new List<Dictionary<string,string>> ();
//									
//									// 行数カウント用
//									int lineCounter = 0;
//									
//									foreach (var line in monoscript.text.Split( new string[]{ Environment.NewLine }, StringSplitOptions.None )) {
//										
//										Dictionary<string,string> lineParams = new Dictionary<string,string> ();
//										
//										lineCounter++;
//										if (!Match (line, searchText))
//											continue;
//										
//										// コンポーネント名
//										lineParams.Add ("compornent",componentData.GetType().ToString());
//										
//										// オブジェクト名
//										lineParams.Add ("objectName",componentData.gameObject.ToString());
//										
//										// 何行目か
//										lineParams.Add ("lineNum",lineCounter.ToString());
//										
//										// ヒットした行の文字列
//										lineParams.Add ("line",line);
//										
//										lines.Add (lineParams);
//									}
//									if(lines.Count != 0)
//										componentList.Add (lines);
//								}
//							}
//						}
//						
//						objList.Add(componentList);
//					}
//				}
//			}
//		}
//		
//	}
//	
//	
//	/* 文字列がマッチしたらtrueを返す */
//	public static bool Match(string input, string search)
//	{
//		if (string.IsNullOrEmpty (input))
//			return false;
//		
//		if (!IsRegex) {
//			var match = Regex.Match (input, search, RegexOptions.IgnoreCase);
//			return match.Success;
//		} else {
//			var result = input.IndexOf (search);
//			return (result != 0 && result != -1);
//		}
//	}
//	
//	/* 結果一覧を描画 */
//	private void DrawResults()
//	{
//		int i = 0;
//		int j = 0;
//		if (objList != null) {
//			foreach(List<List<Dictionary<string,string>>> componentList in objList){
//				if(componentList.Count != 0 && componentList[0].Count != 0){
//					
//					// インデント初期化
//					EditorGUI.indentLevel = 0;
//					
//					// オブジェクト名Foldout
//					foldOutFlagList.Add(false);
//					foldOutFlagList[i] = EditorGUILayout.Foldout( foldOutFlagList[i],componentList[0][0]["objectName"]);
//					
//					if (foldOutFlagList[i]){
//						
//						foreach(List<Dictionary<string,string>> lines in componentList){
//							foreach(Dictionary<string,string> lineParams in lines){
//								// インデント追加
//								EditorGUI.indentLevel = 1;
//								
//								// コンポーネント名+行数
//								foldOutFlagList2.Add(false);
//								foldOutFlagList2[j] =  EditorGUILayout.Foldout(foldOutFlagList2[j],  "    ↳" + lineParams["compornent"] + "    " + lineParams["lineNum"] );
//								
//								if (foldOutFlagList2[j]){
//									// インデント追加
//									EditorGUI.indentLevel = 2;
//									
//									// ヒットした行の文字列
//									EditorGUILayout.SelectableLabel(lineParams["line"]);
//								}
//								j++;
//							}
//						}
//					}
//					i++;
//				}
//			}
//		}
//	}
//}

}