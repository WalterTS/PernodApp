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
using System.Collections.Specialized;
using System.Collections.Generic;

public class ShowPlan : UIModule<ShowPlan>, IActivity
{
	Text dataText;
	String regions;
	String activetype;
	String cdcs;
	String places;

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
			return DataPaths.ACTIVITY_SHOW_PLAN_LAYOUT_PATH;
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
		Debug.Log("ShowPlanActivity => Initializing");
	}

	protected override void ProcessInitialization()
	{

        Button endButton = this.FindAndResolveComponent<Button>("End<Button>", DisplayObject);
        endButton.onClick.AddListener(delegate {
            SessionManager.Instance.TryShuttingDownAllOpenedActivities();
            SessionManager.Instance.SetHomeAsRoot();
            SessionManager.Instance.OpenActivity(ExecutiveHome.Instance);
            Terminate();
        });

		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		String api_url = DataPaths.WS_SHOW_PROJECT_URL+"?project_id="+SessionManager.Instance.CurrentSessionData.ProjectId;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);
				if((string)rawData["status"] == "ok"){
					DateTime dtinit = DateTime.Parse((string) rawData ["data"]["fecha_inicio"]);
					DateTime dtend = DateTime.Parse((string) rawData ["data"]["fecha_fin"]);
					Text title = this.FindAndResolveComponent<Text>("Title<Text>", DisplayObject);
					title.text = "Proyecto: " + (string) rawData ["data"]["nombre"];

					Text InitDate = this.FindAndResolveComponent<Text> ("InitDate<Text>", DisplayObject);
					InitDate.text = dtinit.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("es-Mx"));

					Text EndDate = this.FindAndResolveComponent<Text> ("EndDate<Text>", DisplayObject);
					EndDate.text = dtend.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("es-Mx"));

					Text Brand = this.FindAndResolveComponent<Text> ("Brand<Text>", DisplayObject);
					Brand.text = (string) rawData ["data"]["marca"]["nombre"];

					foreach(JsonData elem in rawData ["data"]["regiones"]){
					regions = regions + (string) elem ["nombre"] + ", ";
					}

					Debug.Log (rawData ["data"]["regiones"].Count);

					Text Regions = this.FindAndResolveComponent<Text> ("Regions<Text>", DisplayObject);
					Regions.text = regions;

					Text KpiType = this.FindAndResolveComponent<Text> ("KpiType<Text>", DisplayObject);
					int kpitype = (int) rawData ["data"]["kpi_tipo"];
					KpiType.text = kpitype.ToString();

					Text KpiTotal = this.FindAndResolveComponent<Text> ("KpiTotal<Text>", DisplayObject);
					int kpitotal = (int) rawData ["data"]["kpi_total"];
					KpiTotal.text = kpitotal.ToString();

					Text MaxPlace = this.FindAndResolveComponent<Text> ("MaxPlace<Text>", DisplayObject);
					int maxplace = (int) rawData ["data"]["maximo_plaza"];
					MaxPlace.text = maxplace.ToString();

					Text CancelType = this.FindAndResolveComponent<Text> ("CancelType<Text>", DisplayObject);
					int canceltype = (int) rawData ["data"]["tiempo_cancelacion"];
					CancelType.text = canceltype.ToString();

					Text Agency = this.FindAndResolveComponent<Text> ("Agency<Text>", DisplayObject);
					Agency.text = (string) rawData ["data"]["agencia"]["nombre"];

					foreach(JsonData elem in rawData ["data"]["activaciones_tipo"]){
						activetype = activetype + (string) elem ["nombre"] + ", ";
					}

					Text ActiveType = this.FindAndResolveComponent<Text> ("ActiveType<Text>", DisplayObject);
					ActiveType.text = activetype;

					foreach(JsonData elem in rawData ["data"]["cdcs"]){
						cdcs = cdcs + (string) elem ["nombre"] + ", ";
					}

					Text CDC = this.FindAndResolveComponent<Text> ("CDC<Text>", DisplayObject);
					CDC.text = cdcs;

					Text TotalActive = this.FindAndResolveComponent<Text> ("TotalActive<Text>", DisplayObject);
					int totalactive = (int) rawData ["data"]["total_activaciones"];
					TotalActive.text = totalactive.ToString();

					foreach(JsonData elem in rawData ["data"]["plazas"]){
						places = places + (string) elem ["nombre"] + ", ";
					}

					Text Places = this.FindAndResolveComponent<Text> ("Places<Text>", DisplayObject);
					Places.text = places;

					Text Description = this.FindAndResolveComponent<Text> ("Description<Text>", DisplayObject);
					Description.text = (string) rawData ["data"]["descripcion"];

					Debug.Log(title.text);
					 Debug.Log("OK");


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
		Debug.Log("ShowProjectActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}


}

