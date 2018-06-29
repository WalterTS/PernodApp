using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Globalization;
using GLIB.Interface;
using GLIB.Extended;
using GLIB.Utils;
using System;
using System.IO;
using GLIB.Core;
using GLIB.Net;
using LitJson;

public class Projects : UIModule<Projects>, IActivity
{
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
			return DataPaths.ACTIVITY_PROJECTS_LAYOUT_PATH;
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
		Debug.Log("Projects");
		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		GameObject parentProject = this.Find("ProjectContainer<Wrapper>", DisplayObject);
		String api_url = DataPaths.WS_PROJECTS_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					foreach(JsonData elem in rawData ["data"]["activaciones"]){
						DateTime dt = DateTime.Parse ((string)elem["fecha"]);
						GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_PROJECT_PATH));

						Text DateProject = this.FindAndResolveComponent<Text> ("Date<Text>", pageType);
						DateProject.text = dt.ToString("MMMM d yyyy hh:mm tt", CultureInfo.CreateSpecificCulture("es-Mx"));

						Text projectDescription = this.FindAndResolveComponent<Text> ("Data<Text>", pageType);
						projectDescription.text = (string)elem["cdc"]["nombre"]+"\n"+(string)elem["status"]["nombre"];

						pageType.transform.SetParent (parentProject.transform);
						pageType.transform.localScale = new Vector3 (1, 1, 1);

						Button notificationButton = pageType.gameObject.GetComponent<Button>() as Button;
						notificationButton.onClick.AddListener (delegate {
							SessionManager.Instance.CurrentSessionData.ProjectName = (string)elem["cdc"]["nombre"];
							SessionManager.Instance.CurrentSessionData.ProjectDate = (string)elem["fecha"];
							SessionManager.Instance.CurrentSessionData.ProjectPlace = (string)elem["cdc"]["plaza"]["nombre"];
							SessionManager.Instance.CurrentSessionData.ProjectStatus = (string)elem["status"]["nombre"];
							SessionManager.Instance.CurrentSessionData.ProjectId = (int)elem["id"];
							SessionManager.Instance.OpenActivity(ShowProject.Instance);
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

	

	}

	

	protected override void ProcessTermination()
	{
		Debug.Log("ProjectActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}


}

