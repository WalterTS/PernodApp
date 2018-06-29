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
using System.Collections.Specialized;
using System.Collections.Generic;


public class NewActivacion : UIModule<NewActivacion>, IActivity
{
	Dropdown DropName;
	Dropdown DropTipo;
	Dropdown DropCdc;
	ProjectObj projectData = new ProjectObj();
	int project_id;
	int user_id;
	string json;

	Text date;
	GameObject Error;

	List<string> projectsList = new List<string>(){"-- Selecciona --"};
	List<string> tiposList = new List<string>(){"-- Selecciona --"};
	List<string> cdcList = new List<string>(){"-- Selecciona --"};

	List<string> projectsKeys = new List<string>(){"0"};
	List<string> tiposKeys = new List<string>(){"0"};
	List<string> cdcKeys = new List<string>(){"0"};


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
			return DataPaths.ACTIVITY_NEWACTIVACION_LAYOUT_PATH;
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
		Debug.Log("NewProjectActivity => Initializing");
	}

	protected override void ProcessInitialization()
	{
		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);

		DropName = this.FindAndResolveComponent<Dropdown> ("Name<Drop>", DisplayObject);
		DropTipo = this.FindAndResolveComponent<Dropdown> ("Tipo<Drop>", DisplayObject);
		DropCdc = this.FindAndResolveComponent<Dropdown> ("Cdc<Drop>", DisplayObject);
		date = this.FindAndResolveComponent<Text> ("ValueDate<Text>",DisplayObject);
		Error = this.Find ("Error<Text>",DisplayObject);
		Error.SetActive (false);
		Button saveButton = this.FindAndResolveComponent<Button> ("Save<Button>", DisplayObject);



        String api_url = DataPaths.WS_FORM_ACTIVACION_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
        Debug.Log(api_url);
        NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
            NotificationSystem.Instance.Terminate();
            Debug.Log(webResponse);

            try
            {
                JsonData rawData = JsonMapper.ToObject(webResponse);
 
                if((string)rawData["status"] == "ok"){
					projectsList.Clear();
					projectsKeys.Clear();
					projectsList.Add("-- Selecciona --");
					projectsKeys.Add("0");
                    foreach(JsonData elem in rawData ["data"]["formulario"]){
            
                    	// Debug.Log("=> "+(string)elem["nombre"]);

						project_id = (int)elem["id"];
						int projectId = (int)elem["id"];

						projectsList.Add((string)elem["nombre"]);
						projectsKeys.Add(projectId.ToString());
                }
                ClearDropNameOptions();
                DropName.AddOptions (projectsList);

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

		DropName.onValueChanged.AddListener(delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			Debug.Log("Mando ID del proyecto: "+project_id);
			CheckTipoWS(project_id);
		});

		saveButton.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			SendFormWS();
		});

	}

	protected void CheckTipoWS(int project_id){
		String api_url = DataPaths.WS_FORM_ACTIVACIONES_TIPO_URL+"?project_id="+project_id;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					tiposList.Clear();
					tiposKeys.Clear();
					tiposList.Add("-- Selecciona --");
					tiposKeys.Add("0");
					foreach(JsonData elem in rawData ["data"]["tipo"]){

						Debug.Log("=> "+(string)elem["nombre"]);

						int tipoId = (int)elem["id"];

						tiposList.Add((string)elem["nombre"]);
						tiposKeys.Add(tipoId.ToString());
						
					}
					ClearDropTipoOptions();
					DropTipo.AddOptions (tiposList);

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

		DropTipo.onValueChanged.AddListener(delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			Debug.Log("Mando ID del proyecto (Tipo): "+project_id);
			CheckCdcWS(project_id);
		});

	}

	protected void CheckCdcWS(int project_id){
		String api_url = DataPaths.WS_FORM_CDC_URL+"?project_id="+project_id+"&user_id="+PlayerPrefs.GetInt("user_id");
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					cdcList.Clear();
					cdcKeys.Clear();
					cdcList.Add("-- Selecciona --");
					cdcKeys.Add("0");
					foreach(JsonData elem in rawData ["data"]["cdcs"]){

						Debug.Log("=> "+(string)elem["nombre"]);

						int cdcId = (int)elem["id"];

						cdcList.Add((string)elem["nombre"]);
						cdcKeys.Add(cdcId.ToString());
						
					}
					ClearDropCDCOptions();
					DropCdc.AddOptions (cdcList);

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

	protected void SendFormWS(){
		if (validateform ()) {
			String api_url = DataPaths.WS_SEND_FORM_ACTIVACION_URL;
			Debug.Log (api_url);
			NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
				Debug.Log (webResponse);

				try {
					JsonData rawData = JsonMapper.ToObject (webResponse);

					if ((string)rawData ["status"] == "ok") { 
						SessionManager.Instance.CurrentSessionData.ProjectPending = (int)rawData ["data"] ["activacion"] ["id"];
						SessionManager.Instance.OpenActivity (ShowProject.Instance);
						Terminate ();
						NotificationSystem.Instance.Terminate ();
					} else {

						NotificationSystem.Instance.NotifyMessage (DataMessages.SERVER_LOGIN_FAIL);
					}
				} catch (JsonException e) {
					Debug.LogError ("JSON Exception: " + e.Message + "\n" + e.StackTrace);
					throw e;
				}
			};

			NetClient.OnWebRequestFailDelegate onWebRequestFail = delegate {
				NotificationSystem.Instance.PromptAction (DataMessages.SERVER_RESPONSE_FAIL, delegate {
				}, delegate {
					NotificationSystem.Instance.Terminate ();
				});
			};

			// Serializacion
			NameValueCollection parameters = new NameValueCollection ();

			parameters.Add ("project_id", getProjectSelected ());
			parameters.Add ("project_date", date.text);
			parameters.Add ("tipo_id", getTipoSelected ());
			parameters.Add ("cdc_id", getCDCSelected ());
			parameters.Add ("user_id", PlayerPrefs.GetInt ("user_id").ToString ());

			StartCoroutine (NetClient.Instance.MakeWebRequest (api_url, parameters, onWebRequestDone, onWebRequestFail));
		} else {
			Error.SetActive (true);
			NotificationSystem.Instance.Terminate ();
		}
	}

	public string getProjectSelected(){
		return (string)projectsKeys [DropName.value];
	}

	public string getTipoSelected(){
		return (string)tiposKeys [DropTipo.value];
	}

	public string getCDCSelected(){
		return (string)cdcKeys [DropCdc.value];
	}

	protected void ClearDropNameOptions () {
		for (int i = DropName.options.Count - 1; i >=0; i--)
		{
			DropName.options.RemoveAt(i);
		}
	}

	protected void ClearDropTipoOptions () {
		for (int i = DropTipo.options.Count - 1; i >=0; i--)
		{
			DropTipo.options.RemoveAt(i);
		}
	}

	protected void ClearDropCDCOptions () {
		for (int i = DropCdc.options.Count - 1; i >=0; i--)
		{
			DropCdc.options.RemoveAt(i);
		}
	}

	public bool validateform(){

		if (date.text.Equals ("")) {
			return false;
		}
		if (DropName.value.Equals (0)) {
			return false;
		}
		if (DropTipo.value.Equals (0)) {
			return false;
		}
		if (DropCdc.value.Equals (0)) {
			return false;
		}
		return true;
	}


	protected override void ProcessTermination()
	{
		Debug.Log("NewProjectActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}


}

