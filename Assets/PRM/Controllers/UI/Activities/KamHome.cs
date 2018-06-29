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

public class KamHome : UIModule<KamHome>, IActivity {
	Text notificationsLegend;
	public bool isActivityReady
	{
		get
		{
			if (UIModuleCurrentStatus == UIModule<KamHome>.UIModuleStatusModes.STAND_BY)
				return true;
			else
				return false;
		}
	}

	protected override Transform DisplayObjectParent
	{
		get
		{
			return null;
		}
	}

	protected override string DisplayObjectPath
	{
		get
		{
			return DataPaths.ACTIVITY_KAMHOME_LAYOUT_PATH;
		}
	}

	protected override Vector2? DisplayObjectPosition
	{
		get
		{
			return null;
		}
	}

	protected override int DisplayObjectZIndex
	{
		get
		{
			return (int)ZIndexPlacement.MIDDLE;
		}
	}

	protected override Transition InOutTransition
	{
		get
		{
			return new Transition(Transition.InOutAnimations.SCALE);
		}
	}

	public void EndActivity(bool force = false)
	{
		Terminate(force);
	}

	public void StartActivity()
	{
		Initialize();
	}

	protected override void ProcessInitialization()
	{
		String api_url = DataPaths.WS_USERS_URL+"?username="+PlayerPrefs.GetString("username")+"&password="+PlayerPrefs.GetString("password");
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					foreach(JsonData elem in rawData ["data"]["usuario"]["notificaciones_recibidas"]){
						Debug.Log((string)elem["titulo"]);

						GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_NOTIFICATION_PATH));

						Text promotionSubtitle = this.FindAndResolveComponent<Text> ("Title<Text>", pageType);
						promotionSubtitle.text = (string)elem["titulo"];

						Text promotionDescription = this.FindAndResolveComponent<Text> ("Content<Text>", pageType);
						promotionDescription.text = (string)elem["contenido"];

						GameObject parent_notification = this.Find ("Notifications<Wrapper>", DisplayObject);

						pageType.transform.SetParent (parent_notification.transform);
						pageType.transform.localScale = new Vector3 (1, 1, 1);

						Button notificationButton = pageType.gameObject.GetComponent<Button>() as Button;
						notificationButton.onClick.AddListener (delegate {
							SessionManager.Instance.OpenActivity(Report.Instance);
							Terminate();
						});

					}

				}else{

					NotificationSystem.Instance.NotifyMessage(DataMessages.SERVER_LOGIN_FAIL);
				}
			}catch (JsonException e) {
				Debug.LogError("JSON Exception: " + e.Message + "\n" + e.StackTrace);
				throw e;
			}
		};

		NetClient.OnWebRequestFailDelegate onWebRequestFail = delegate
		{
			NotificationSystem.Instance.PromptAction(DataMessages.SERVER_RESPONSE_FAIL, delegate {  }, delegate { NotificationSystem.Instance.Terminate(); });
		};

		//REVISA WEBSERVICE;

		StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));


		Debug.Log ("init home");

		Button calendarButton = this.FindAndResolveComponent<Button> ("Calendar<Button>", DisplayObject);
		calendarButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(BuildProject.Instance);
			Terminate();
		});

		Button buildButton = this.FindAndResolveComponent<Button> ("BuildProject<Button>", DisplayObject);
		buildButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(BuildProject.Instance);
			Terminate();
		});

	}

	protected override void ProcessTermination()
	{
		Debug.Log("KamHomeActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}
}
