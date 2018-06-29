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

public class Plans : UIModule<Plans>, IActivity
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
		String api_url = DataPaths.WS_PLANS_URL;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					foreach(JsonData elem in rawData ["data"]["proyectos"]){
						DateTime dtinit = DateTime.Parse ((string)elem["fecha_inicio"]);
						DateTime dtend = DateTime.Parse ((string)elem["fecha_fin"]);
						GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_ITEM_LIST_H_PATH));

						Text ProjectName = this.FindAndResolveComponent<Text> ("Hour<Text>", pageType);
						ProjectName.text = (string)elem["nombre"];

						Text projectDescription = this.FindAndResolveComponent<Text> ("Name<Text>", pageType);
						projectDescription.text = "\n"+"Fecha de inicio: "+ dtinit.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("es-Mx")) +"\n"+"Fecha de fin: "+ dtend.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("es-Mx")) +"\n"+"Marca: "+(string)elem["marca"]["nombre"]+"\n"+"Agencia: "+(string)elem["agencia"]["nombre"]+"\n";


						pageType.transform.SetParent (parentProject.transform);
						pageType.transform.localScale = new Vector3 (1, 1, 1);

						Button notificationButton = pageType.gameObject.GetComponent<Button>() as Button;
						notificationButton.onClick.AddListener (delegate {
							SessionManager.Instance.CurrentSessionData.ProjectName = (string)elem["nombre"];
                            SessionManager.Instance.CurrentSessionData.ProjectDate = (string)elem["fecha_inicio"];
                            SessionManager.Instance.CurrentSessionData.ProjectPlace = (string)elem["agencia"]["nombre"];
                            SessionManager.Instance.CurrentSessionData.ProjectStatus = "";
                            SessionManager.Instance.CurrentSessionData.ProjectId = (int)elem["id"];
							SessionManager.Instance.OpenActivity(ShowPlan.Instance);
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

