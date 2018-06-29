using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PRM.Models.Objects;
using GLIB.Interface;
using GLIB.Extended;
using GLIB.Utils;
using System;
using System.IO;
using GLIB.Core;
using GLIB.Net;
using LitJson;
using System.Text.RegularExpressions;
using System.Linq;

public class PrintNotification : MonoBehaviour {

	// Use this for initialization
	void Start(){
		NotificationAlert.Instance.NotificationLoaded += addNotifications;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected void selectActivity(string path){
		Debug.Log("Path sin separar: "+path);

		string[] lines = Regex.Split(path, "/");
		lines = lines.Skip(1).ToArray();  

		if(lines.Length > 1){
			if(lines[0] == "activacion"){
				//Activar un session para el tema del id lines[1]
				SessionManager.Instance.CurrentSessionData.ProjectPending = Int32.Parse(lines[1]);
				Debug.Log("ProjectPending "+SessionManager.Instance.CurrentSessionData.ProjectPending);
				SessionManager.Instance.TryShuttingDownAllOpenedActivities();
				SessionManager.Instance.OpenActivity(ShowProject.Instance);
			}
		}else{
			//En caso de que no lleve a una pantalla interna con id
		}
	}

	protected void readNotification(int n_id, string path){
		String api_url = DataPaths.WS_READ_NOTIFICATION_URL+"?notification_id="+n_id;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			Debug.Log(webResponse);
			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					Debug.Log("Notificacion eliminada correctamente");
					selectActivity(path);
				}else{
					Debug.Log("Error al intentar eliminar notificacion");
				}
			}catch (JsonException e) {
				Debug.LogError("JSON Exception: " + e.Message + "\n" + e.StackTrace);
				throw e;
			}
		};

		NetClient.OnWebRequestFailDelegate onWebRequestFail = delegate
		{
			// NotificationSystem.Instance.PromptAction(DataMessages.SERVER_RESPONSE_FAIL, delegate {  }, delegate { NotificationSystem.Instance.Terminate(); });
			Debug.Log("Error al llamar el WS");
		};

		//REVISA WEBSERVICE;

		StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));
	}
		

	protected void cleanNotifications(){
		int childs = gameObject.transform.childCount;

		for (int i = childs - 1; i >= 0; i--)
		{
			GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
		}
	}

	void addNotifications(object source,EventArgs e){

		cleanNotifications ();

		foreach (DataNotification data in NotificationAlert.Instance.data_notification) {
			GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_NOTIFICATION_PATH));

			Text promotionSubtitle = this.FindAndResolveComponent<Text> ("Title<Text>", pageType);
			promotionSubtitle.text = data.title;

			Text promotionDescription = this.FindAndResolveComponent<Text> ("Content<Text>", pageType);
			promotionDescription.text = data.content;

			pageType.transform.SetParent (gameObject.transform);
			pageType.transform.localScale = new Vector3 (1, 1, 1);

			Button notificationButton = pageType.gameObject.GetComponent<Button> () as Button;
			notificationButton.onClick.AddListener (delegate {
				readNotification(data.id, data.path);
				//selectActivity(data.path);
			});
		}
	}
}
