using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using GLIB.Interface;
using GLIB.Extended;
using GLIB.Utils;
using System;
using System.IO;
using GLIB.Core;
using GLIB.Net;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using PRM.Models.Objects;
using System.Threading;

public class NotificationAlert : BackModule<NotificationAlert> {

	public List<DataNotification> data_notification;

	public delegate void NotificationEventHandler(object source,EventArgs args);
	public event NotificationEventHandler NotificationLoaded;
	String newData;

	protected virtual void onNotificationLoaded(){
		if (NotificationLoaded != null) {
			NotificationLoaded (this, EventArgs.Empty);
		}
	}

	// Use this for initialization
	protected override void ProcessInitialization(){ 
		Debug.Log("NotificationAlert => Started");
		VerifyWs();
		StartCoroutine (CheckNotification());
	}

	IEnumerator CheckNotification()
	{
		yield return new WaitForSeconds(30);
		Debug.Log("Consulting new Notifications...");
		VerifyWs();
		StartCoroutine (CheckNotification());
	}

	void VerifyWs(){
		data_notification = new List<DataNotification> ();

		String api_url = DataPaths.WS_NOTIFICATION_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
		Debug.Log("====== URL");
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			Debug.Log(webResponse);
			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);
				if((string)rawData["status"] == "ok"){
					foreach(JsonData elem in rawData ["data"]["notification"]){ 
						if((bool)elem["status"] == false){
							DataNotification data = new DataNotification ();
							data.id = (int)elem["id"];
							data.title = (string)elem["titulo"];
							data.content = (string)elem["contenido"];
							data.path = (string)elem["path"];
							data_notification.Add(data);
						}
					}
					onNotificationLoaded();
				}else{
					Debug.Log("Sin notificaciones");
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


	protected override void ProcessTermination()
	{

	}
	
	// Update is called once per frame
	protected override void ProcessUpdate () {
		
	}
}
