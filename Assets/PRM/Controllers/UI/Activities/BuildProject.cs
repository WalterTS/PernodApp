using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using GLIB.Interface;
using GLIB.Extended;
using GLIB.Utils;
using System.IO;
using GLIB.Core;
using GLIB.Net;
using LitJson;

public class BuildProject : UIModule<BuildProject>, IActivity{
	public bool isActivityReady
	{
		get
		{
			if (UIModuleCurrentStatus == UIModuleStatusModes.STAND_BY)
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
			return DataPaths.ACTIVITY_BUILD_LAYOUT_PATH;
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


		/*String api_url = DataPaths.WS_USERS_URL+"?username="+PlayerPrefs.GetString("username")+"&password="+PlayerPrefs.GetString("password");
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

		StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));*/

		List<Date> date = new List<Date> ();
		 
		date.Add( new Date ("Fecha", new DateTime(2018,02,03)));
		date.Add( new Date ("Fecha", new DateTime(2018,02,03)));
		date.Add( new Date ("Fecha", new DateTime(2018,02,03)));
		date.Add( new Date ("Fecha", new DateTime(2018,02,03)));
		date.Add( new Date ("Fecha", new DateTime(2018,02,03)));

		foreach(Date dates in date){
			GameObject pagetipo = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_DATE_PATH));
			Text Name = this.FindAndResolveComponent<Text> ("Text<text>", pagetipo) as Text;
			Name.text = (dates.nombre);
			Text Fecha = this.FindAndResolveComponent<Text> ("Prub<text>", pagetipo) as Text;
			Fecha.text = (dates.fecha.ToString("yyyyMMdd"));
			GameObject parent_date = this.Find("Date<Wrapper>", DisplayObject);
			pagetipo.transform.SetParent (parent_date.transform);
			pagetipo.transform.localScale = new Vector3 (1, 1, 1);
			// Text dateText = pagetipo.gameObject.GetComponent<Text> () as Text;
		}
		date.Clear();

		MainMenu.Instance.setTitle ("Crear Proyecto");

		/*GameObject pagetipo = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_DATE_PATH));
		GameObject parent_date = this.Find("Date<Wrapper>", DisplayObject);
		pagetipo.transform.SetParent (parent_date.transform);
		pagetipo.transform.localScale = new Vector3 (1, 1, 1);

		Text dateText = pagetipo.gameObject.GetComponent<Text> () as Text;*/

		//GameObject pagetyp = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_CENTER_PATH));
		//GameObject parent_center = this.Find("Center<Wrapper>", DisplayObject);
		//pagetyp.transform.SetParent (parent_center.transform);
		//pagetyp.transform.localScale = new Vector3 (1, 1, 1);

		//Text centerText = pagetyp.gameObject.GetComponent<Text> () as Text;

		//GameObject pagetipo = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_PATH));
		//GameObject parent_type = this.Find("Type<Wrapper>", DisplayObject);
		//pagetipo.transform.SetParent (parent_type.transform);
		//pagetipo.transform.localScale = new Vector3 (1, 1, 1);

		//Text typeText = pagetipo.gameObject.GetComponent<Text> () as Text;
	}

	protected override void ProcessTermination()
	{
		Debug.Log("BuildProjectActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}

}
