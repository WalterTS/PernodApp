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

public class ShowProject : UIModule<ShowProject>, IActivity
{
    float swipeSpeed = 0.05F;
    float inputX;
    float inputY;
    GameObject Image1;
    GameObject Image2;
    GameObject Image3;
    GameObject detailsProject;
	GameObject approveProject;
	GameObject assignProducer;
	GameObject reportForm;
	GameObject checkIn;
	GameObject pageType;
	GameObject Error;
	GameObject writepreform;
	GameObject showCheckIn;
	GameObject showPreform;
	GameObject showReportFormData;
	GameObject showPreFormData;
	GameObject parentPreForm;
    GameObject circulo;

	Button saveButton;
	Button checkInSaveButton;
	Button saveFormButton;

	Text StatusProject;
	Text TitleAssign;
	Text TitleAssignList;
	Text StatusProjectProd;
	Text data;
	Text totalData;
	Text copeoData;
	Text botellasData;
	String UserRole;

	Text TitleForm;
	Text InstructForm;
	Text ImageName;
	Text checkInNameImg;

	Dropdown DropProductores;
	Dropdown DropOptions;
	Dropdown DropBoolean;
	Dropdown Droptest;

	bool preForm;

    RectTransform Image1Rect;
    RectTransform Image2Rect;
    RectTransform Image3Rect;

    int _currentPage;
    int _maxPages = 3;

    List<GameObject> pages;
    List<string> producerList = new List<string>(){"-- Selecciona --"};
	List<string> producerKeys = new List<string>(){"0"};
	List<string> OptionItemList = new List<string>(){"-- Selecciona --"};
	List<string> OptionKeyList = new List<string>(){"0"};
	List<string> BooleanItemList = new List<string>(){"-- Selecciona --","Si","No"};

	List<GameObject> ColumnList = new List<GameObject>();
	List<string> TypeColumnList = new List<string>();
	
	List<int> ColumnId = new List<int>();
	List<int> ItemId = new List<int>();
	List<string> SelectedValue = new List<string>();
	List<string> LabelPreForm = new List<string>();

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

    bool _doingTransition
    {
        get
        {

            bool rval = false;
            if (pages != null)
            {
                foreach (GameObject page in pages)
                {
                    if (page != null)
                    {
                        AnimateComponent animation = page.ResolveComponent<AnimateComponent>();
                        if (animation.isTranslating)
                            rval = true;
                    }
                }
            }

            return rval;
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
			return DataPaths.ACTIVITY_SHOWPROJECT_LAYOUT_PATH;
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
		Debug.Log("ShowProjectActivity => Initializing");
	}

	protected override void ProcessInitialization()
	{
		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		detailsProject = this.Find ("DetailsProject<Wrapper>", DisplayObject);
		checkIn = this.Find ("CheckIn<Wrapper>", DisplayObject);
		approveProject = this.Find ("Approve<Wrapper>", DisplayObject);
		assignProducer = this.Find ("AssignProd<Wrapper>", DisplayObject);
		reportForm = this.Find ("ReportForm<Wrapper>", DisplayObject);
		Error = this.Find ("Error<Wrapper>", DisplayObject);
		writepreform = this.Find ("PreForm<Wrapper>", DisplayObject);
		showCheckIn = this.Find ("ShowCheckIn<Wrapper>", DisplayObject);
		showPreform = this.Find ("PreFormData<Wrapper>", DisplayObject);
		showReportFormData = this.Find ("ShowForm<Wrapper>", DisplayObject);
		showPreFormData = this.Find ("SeePre<Wrapp>", DisplayObject);

		StatusProject = this.FindAndResolveComponent<Text> ("Status<Text>", DisplayObject);
		TitleAssign = this.FindAndResolveComponent<Text> ("Title_assign<Text>", DisplayObject);
		TitleAssignList = this.FindAndResolveComponent<Text> ("TitleAssignList<Text>", DisplayObject);
		ImageName = this.FindAndResolveComponent<Text> ("ImageNameValue<Text>", DisplayObject);

		StatusProjectProd = this.FindAndResolveComponent<Text> ("Status_assign<Text>", DisplayObject);
		UserRole = PlayerPrefs.GetString("UserRole");
		DropProductores = this.FindAndResolveComponent<Dropdown> ("Productores<Drop>", DisplayObject);
		saveButton = this.FindAndResolveComponent<Button> ("Save_assign<Button>", DisplayObject);
		saveFormButton = this.FindAndResolveComponent<Button> ("Save_form<Button>", DisplayObject);

		TitleForm = this.FindAndResolveComponent<Text> ("Title_form<Text>", DisplayObject);
		InstructForm = this.FindAndResolveComponent<Text> ("Instrucciones_form<Text>", DisplayObject);

		totalData = this.FindAndResolveComponent<Text>("TotalDataValue<Text>",DisplayObject);
		copeoData = this.FindAndResolveComponent<Text>("CopeoDataValue<Text>",DisplayObject);
		botellasData = this.FindAndResolveComponent<Text>("BotellasDataValue<Text>",DisplayObject);
		checkInNameImg = this.FindAndResolveComponent<Text>("CheckIn<Text>",DisplayObject);

		//Seteo todos los contenedores de acción en falso
		detailsProject.SetActive(false);
		approveProject.SetActive(false);
		assignProducer.SetActive(false);
		reportForm.SetActive(false);
		checkIn.SetActive(false);
		Error.SetActive (false);
		writepreform.SetActive (false);
		showPreform.SetActive (false);

		Debug.Log("IDD: "+SessionManager.Instance.CurrentSessionData.ProjectId);


		// INGRESA A TRAVES DE LAS BURBUJAS DE NOTIFICACIÓN
		if(SessionManager.Instance.CurrentSessionData.ProjectPending != 0){
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			Debug.Log("Proyecto atraves de notificación");
			String api_url = DataPaths.WS_GET_STATUS_ACTIVACION_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending;

			Debug.Log(api_url);
			NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {			
				Debug.Log(webResponse);

				try
				{
					JsonData rawData = JsonMapper.ToObject(webResponse);

					if((string)rawData["status"] == "ok"){
						SessionManager.Instance.CurrentSessionData.ProjectStatus = (string)rawData["data"]["status"];
						SessionManager.Instance.CurrentSessionData.ProjectId = SessionManager.Instance.CurrentSessionData.ProjectPending;
						WorkFlow();
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
			StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));
				
		// INGRESA A TRAVES DEL FLUJO NORMAL (LISTADO DE PROYECTOS)
		}else{
			WorkFlow();
		}
	}

	protected void WorkFlow(){
		//Llenado de datos en el contenedor principal
		if(SessionManager.Instance.CurrentSessionData.ProjectPending == 0){
			DateTime dt = DateTime.Parse(SessionManager.Instance.CurrentSessionData.ProjectDate);

			Text NameProject = this.FindAndResolveComponent<Text> ("Title<Text>", DisplayObject);
			NameProject.text = SessionManager.Instance.CurrentSessionData.ProjectName;

			Text PlaceProject = this.FindAndResolveComponent<Text> ("Place<Text>", DisplayObject);
			PlaceProject.text = SessionManager.Instance.CurrentSessionData.ProjectPlace;

			Text DateProject = this.FindAndResolveComponent<Text> ("Date<Text>", DisplayObject);
			DateProject.text = dt.ToString("d MMMM yyyy hh:mm tt", CultureInfo.CreateSpecificCulture("es-Mx"));
		}

		StatusProject.text = "Status: "+SessionManager.Instance.CurrentSessionData.ProjectStatus;

		// (flujo) CUENTA DE AGENCIA => ASIGNAR PRODUCTOR A LA ACTIVACIÓN
		if(UserRole == "ROLE_USER_CUENTA" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Aprobada"){
			assignProducer.SetActive(true);
			Error.SetActive (false);
			StatusProjectProd.text = "Status: "+SessionManager.Instance.CurrentSessionData.ProjectStatus;
			getProdList();
		// (flujo) CUENTA DE PRODUCTOR => ASIGNAR SUPERVISOR A LA ACTIVACIÓN
		}else if(UserRole == "ROLE_USER_PRODUCTOR" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Por asignar supervisores"){
			assignProducer.SetActive(true);
			StatusProjectProd.text = "Status: "+SessionManager.Instance.CurrentSessionData.ProjectStatus;
			TitleAssign.text = "Asigna tus supervisores";
			TitleAssignList.text = "Supervisores";
			getSupervList();
			Error.SetActive (false);
		// (flujo) CUENTA DE GERENTE => APROBAR O RECHAZAR ACTIVACIÓN
		}else if(UserRole == "ROLE_USER_GERENTE" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "En espera de aprobación"){
			approveProject.SetActive(true);
			NotificationSystem.Instance.Terminate();
			getActivacionWS();
		// (flujo) CUENTA DE SUPERVISOR => FORMULARIO DE REPORTE 
		}else if(UserRole == "ROLE_USER_SUPERVISOR" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Listo para reporte"){
			reportForm.SetActive(true);
			NotificationSystem.Instance.Terminate();
			ClearBuildReportList();
			BuildReportFormWS();
		// (flujo) CUENTA DE SUPERVISOR => FORMULARIO DE CHECK IN 
		}else if(UserRole == "ROLE_USER_SUPERVISOR" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Listo para checkin"){
			checkIn.SetActive(true);
			NotificationSystem.Instance.Terminate();
			Error.SetActive (false);
			checkInSaveButton = this.FindAndResolveComponent<Button> ("CheckIn_Save<Button>", DisplayObject);

			checkInSaveButton.onClick.AddListener (delegate {			
				NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
				Debug.Log(ImageName.text);
				SendChekIn (ImageName.text);
			});
		// (flujo) CUENTA DE PRODUCTOR => APROBAR O RECHAZAR CHECK IN
		}else if(UserRole == "ROLE_USER_PRODUCTOR" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Revision de checkin"){
			approveProject.SetActive(true);
			NotificationSystem.Instance.Terminate();
			getActivacionWS(false, true);
		// (flujo) CUENTA DE AGENCIA => APROBAR O RECHAZAR CHECK IN ***genero otro if, en el caso de que cambie la función
		}else if(UserRole == "ROLE_USER_CUENTA" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Revision de checkin por cuentas"){
			approveProject.SetActive(true);
			NotificationSystem.Instance.Terminate();
			getActivacionWS(false, true);
		// (flujo) CUENTA DE PRODUCTOR => APROBAR O RECHAZAR FORMULARIO DE REPORTE
		}else if(UserRole == "ROLE_USER_PRODUCTOR" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "En espera de aprobación de productor"){
			approveProject.SetActive(true);
			NotificationSystem.Instance.Terminate();
			getActivacionWS(false, false, true);
		// (flujo) CUENTA DE AGENCIA => APROBAR O RECHAZAR FORMULARIO DE REPORTE (PARA FINALIZAR FLUJO NORMAL)
		}else if(UserRole == "ROLE_USER_CUENTA" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "En espera de aprobación de ejecutivo de cuentas"){
			approveProject.SetActive(true);
			NotificationSystem.Instance.Terminate();
			getActivacionWS(false, false, true);
		// (RECHAZADO) CUENTA DE SUPERVISOR => EDICIÓN DE PRE-FORMULARIO DE REPORTE
		}else if(UserRole == "ROLE_USER_SUPERVISOR" && SessionManager.Instance.CurrentSessionData.ProjectStatus == "Reporte Rechazado"){
			reportForm.SetActive(true);
			NotificationSystem.Instance.Terminate();
			BuildReportFormWS(1);
		}else{
			NotificationSystem.Instance.Terminate();
			detailsProject.SetActive(true);
			if(UserRole == "ROLE_USER_SUPERVISOR" || UserRole == "ROLE_USER_PRODUCTOR" || UserRole == "ROLE_USER_CUENTA"){
				if(SessionManager.Instance.CurrentSessionData.ProjectStatus == "En espera de aprobación de productor" || 
					SessionManager.Instance.CurrentSessionData.ProjectStatus == "Finalizada" || 
					SessionManager.Instance.CurrentSessionData.ProjectStatus == "Listo para reporte" || 
					SessionManager.Instance.CurrentSessionData.ProjectStatus == "Revision de checkin" || 
					SessionManager.Instance.CurrentSessionData.ProjectStatus == "Revision de checkin por cuentas" || 
					SessionManager.Instance.CurrentSessionData.ProjectStatus == "Reporte Rechazado" || 
					SessionManager.Instance.CurrentSessionData.ProjectStatus == "Reporte rechazado por cuentas"){
						getDataFromCF();
				}
			}
		}
	}

	protected void getActivacionWS(bool refresh = false, bool operateCheckIn = false, bool operateReportForm = false){
		String api_url= "";
		if(refresh){
			api_url = DataPaths.WS_GET_ACTIVACION_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectId;
		}else{
			api_url = DataPaths.WS_GET_ACTIVACION_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending;
		}
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					DateTime dt = DateTime.Parse((string)rawData ["data"]["activacion"]["fecha"]);
					//Llenado de datos en el contenedor principal
					Text NameProject = this.FindAndResolveComponent<Text> ("Title<Text>", DisplayObject);
					NameProject.text = (string)rawData ["data"]["activacion"]["cdc"]["nombre"];

					Text PlaceProject = this.FindAndResolveComponent<Text> ("Place<Text>", DisplayObject);
					PlaceProject.text = (string)rawData ["data"]["activacion"]["cdc"]["plaza"]["nombre"];

					Text DateProject = this.FindAndResolveComponent<Text> ("Date<Text>", DisplayObject);
					DateProject.text = dt.ToString("d MMMM yyyy hh:mm tt",CultureInfo.CreateSpecificCulture("es-Mx"));
					Debug.Log("=> Status: "+(string)rawData ["data"]["activacion"]["status"]["nombre"]);
					// if(!refresh){
						StatusProject.text ="Status: "+(string)rawData ["data"]["activacion"]["status"]["nombre"];	
					// }

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

		if(SessionManager.Instance.CurrentSessionData.ProjectId == 0 && SessionManager.Instance.CurrentSessionData.ProjectPending != 0 || refresh){
			StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));
		}

		//Validación por roles, añadir solo los que puedan aprobar
		if(UserRole == "ROLE_USER_GERENTE" || UserRole == "ROLE_USER_EJECUTIVO" && operateCheckIn == false && operateReportForm == false){
			//Validación si es la primera o segunda vez que se ejecuta el método
			if(!refresh){
				Button okButton = this.FindAndResolveComponent<Button> ("Accept<Button>", DisplayObject);
				okButton.onClick.AddListener (delegate {			
					NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
					operateAnswer(2);
				});

				Button refuseButton = this.FindAndResolveComponent<Button> ("Refuse<Button>", DisplayObject);
				refuseButton.onClick.AddListener (delegate {			
					NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
					operateAnswer(3);
				});			
			}
		}

		// Aprobación / Rechazo del Check in por Productor y Cuentas
		if(operateCheckIn){
			if (UserRole == "ROLE_USER_PRODUCTOR") {
				getDataFromCF();
				if(!refresh){
					Button okButton = this.FindAndResolveComponent<Button> ("Accept<Button>", DisplayObject);
					okButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(12);
					});

					Button refuseButton = this.FindAndResolveComponent<Button> ("Refuse<Button>", DisplayObject);
					refuseButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(6);
					});			
				}
			}
			if (UserRole == "ROLE_USER_CUENTA") {
				getDataFromCF();
				if(!refresh){
					Button okButton = this.FindAndResolveComponent<Button> ("Accept<Button>", DisplayObject);
					okButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(7);
					});

					Button refuseButton = this.FindAndResolveComponent<Button> ("Refuse<Button>", DisplayObject);
					refuseButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(6);
					});			
				}
			}
		}

		// Aprobación / Rechazo del formulario de reporte por Productor y Cuentas
		if(operateReportForm){
			if (UserRole == "ROLE_USER_PRODUCTOR") {
				getDataFromCF();
				if(!refresh){
					Button okButton = this.FindAndResolveComponent<Button> ("Accept<Button>", DisplayObject);
					okButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(10);
					});

					Button refuseButton = this.FindAndResolveComponent<Button> ("Refuse<Button>", DisplayObject);
					refuseButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(13);
					});			
				}
			}
			if (UserRole == "ROLE_USER_CUENTA") {
				getDataFromCF();
				if(!refresh){
					Button okButton = this.FindAndResolveComponent<Button> ("Accept<Button>", DisplayObject);
					okButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
						operateAnswer(5);
					});

					Button refuseButton = this.FindAndResolveComponent<Button> ("Refuse<Button>", DisplayObject);
					refuseButton.onClick.AddListener (delegate {			
						NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);

						BuildReportFormWS();

						operateAnswer(14);
					});			
				}
			}
		}

	}

	protected void operateAnswer(int answer){
		String api_url = "";
		if(SessionManager.Instance.CurrentSessionData.ProjectId != 0){
			api_url = DataPaths.WS_SET_STATUS_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectId+"&status_id="+answer;
		}else{
			api_url = DataPaths.WS_SET_STATUS_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending+"&status_id="+answer;
		}

		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {			
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					/*if(answer == 2){
						NotificationSystem.Instance.NotifyMessage("Se ha aceptado este proyecto correctamente");	
					}else{
						NotificationSystem.Instance.NotifyMessage("Se ha rechazado este proyecto correctamente");	
					}*/
					refreshActivity();
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

	protected void getProdList(){
		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		String api_url = DataPaths.WS_GET_PROD_LIST_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				producerList.Clear();
				producerKeys.Clear();
				producerList.Add("-- Selecciona --");
				producerKeys.Add("0");
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					 foreach(JsonData elem in rawData ["data"]["productores"]){

					 	int prodId = (int)elem["id"];

						producerList.Add((string)elem["nombre"]);
						producerKeys.Add(prodId.ToString());
					 }
					DropProductores.ClearOptions();
					DropProductores.RefreshShownValue();
					DropProductores.AddOptions (producerList);

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

		saveButton.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			SendProdDataWS();

		});

	}

	protected void getSupervList(){
		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		String api_url = DataPaths.WS_GET_SUPERV_LIST_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				producerList.Clear();
				producerKeys.Clear();
				producerList.Add("-- Selecciona --");
				producerKeys.Add("0");
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					 foreach(JsonData elem in rawData ["data"]["supervisores"]){

					 	int prodId = (int)elem["id"];

						producerList.Add((string)elem["nombre"]);
						producerKeys.Add(prodId.ToString());
					 }
					DropProductores.ClearOptions();
					DropProductores.RefreshShownValue();
					DropProductores.AddOptions (producerList);

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

		saveButton.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			SendProdDataWS(true);
		});

	}

	protected void SendProdDataWS(bool superv = false){
		String api_url ="";
		if(superv){
			api_url = DataPaths.WS_SET_SUPERV_ACTIVACION_URL;
		}else{
			api_url = DataPaths.WS_SET_PROD_ACTIVACION_URL;
		}
		
		Debug.Log(api_url);
		if (validateProd ()) {
			NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
				Debug.Log (webResponse);

				try {
					JsonData rawData = JsonMapper.ToObject (webResponse);

					if ((string)rawData ["status"] == "ok") {
						bool resp = (bool)rawData ["data"] ["asignacion"] ["done"];
						if (resp) {
							refreshActivity ();
						}

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

			parameters.Add ("producer_id", getProdSelected ());
			parameters.Add ("activacion_id", SessionManager.Instance.CurrentSessionData.ProjectId.ToString ());

			StartCoroutine (NetClient.Instance.MakeWebRequest (api_url, parameters, onWebRequestDone, onWebRequestFail));
		} else {
			NotificationSystem.Instance.Terminate ();
			Error.SetActive (true);
		}
	}

	public bool validateProd(){

		if (DropProductores.value.Equals (0)) {
			return false;
		}
		return true;
	}

	protected void ClearBuildReportList(){
		OptionItemList.Clear();
		OptionItemList.Add("-- Selecciona --");
		OptionKeyList.Clear();
		OptionKeyList.Add("0");
		BooleanItemList.Clear();
		BooleanItemList.Add("-- Selecciona --");
		BooleanItemList.Add("Si");
		BooleanItemList.Add("No");
		ColumnList.Clear();
		TypeColumnList.Clear();
		ColumnId.Clear();
		ItemId.Clear();
		SelectedValue.Clear();
		LabelPreForm.Clear();
		Debug.Log(":: Listas vaciadas");
	}	

	protected void BuildReportFormWS(int editForm = 0){
		NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		GameObject parentProject = this.Find("Fields<Wrapper>", DisplayObject);
		if(editForm == 2){
			writepreform.SetActive (false);
		}

		String api_url = "";
		if(SessionManager.Instance.CurrentSessionData.ProjectId != 0){
			if(editForm == 1){
				api_url = DataPaths.WS_BUILD_REPORT_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectId+"&edit_type=1";
			}else if(editForm == 2){
				api_url = DataPaths.WS_BUILD_REPORT_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectId+"&edit_type=2";
			}else{
				api_url = DataPaths.WS_BUILD_REPORT_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectId;
			}
			
		}else{
			if(editForm == 1){
				api_url = DataPaths.WS_BUILD_REPORT_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending+"&edit_type=1";
			}if(editForm == 2){
				api_url = DataPaths.WS_BUILD_REPORT_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending+"&edit_type=2";
			}else{
				api_url = DataPaths.WS_BUILD_REPORT_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending;
			}
		}

		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					TitleForm.text = (string)rawData ["headers"]["Titulo"];
					InstructForm.text = (string)rawData ["headers"]["Instrucciones"];
					 foreach(JsonData elem in rawData ["formulario"]["columnas"]){

					 	if((string)elem["Tipo"] == "Opciones"){
					 		pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_OPCIONES));
					 		DropOptions = this.FindAndResolveComponent<Dropdown> ("Opciones<Drop>", pageType);
					 		Text Label = this.FindAndResolveComponent<Text> ("Label<Text>", pageType);
					 		Label.text = (string)elem["Etiqueta"];
					 	}else if((string)elem["Tipo"] == "Booleano"){
					 		pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_OPCIONES));
					 		DropBoolean = this.FindAndResolveComponent<Dropdown> ("Opciones<Drop>", pageType);
					 		Text Label = this.FindAndResolveComponent<Text> ("Label<Text>", pageType);
					 		Label.text = (string)elem["Etiqueta"];
					 	}else if((string)elem["Tipo"] == "Texto"){
					 		pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_TEXTO));
					 		Text Label = this.FindAndResolveComponent<Text> ("Label<Text>", pageType);
					 		Label.text = (string)elem["Etiqueta"];
					 	}else if((string)elem["Tipo"] == "Entero"){
							if ((string) elem ["Etiqueta"] == "Total" || (string) elem ["Etiqueta"] == "Copeo" || (string) elem ["Etiqueta"] == "Botellas"){
								writepreform.SetActive(true);
								parentPreForm = this.Find("PreformContainer<Wrapp>", DisplayObject);

								pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_CIRCLE));
						 		Text Label = this.FindAndResolveComponent<Text> ("LabelPreform<Text>", pageType);
                                InputField Valor = this.FindAndResolveComponent<InputField>("Circle<image>", pageType);
						 		LabelPreForm.Add((string)elem["Etiqueta"]);
						 		
						 		if(elem.Keys.Contains("Valor")){
						 			Label.text = (string)elem["Etiqueta"];
                                    Valor.text = elem["Valor"].ToString();
                                 }
                                else{
						 			Label.text = (string)elem["Etiqueta"];
						 		}
							}else{
						 		pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_ENTERO));
						 		Text Label = this.FindAndResolveComponent<Text> ("Label<Text>", pageType);
						 		LabelPreForm.Add((string)elem["Etiqueta"]);
						 		Label.text = (string)elem["Etiqueta"];
							}
					 	}else if((string)elem["Tipo"] == "Porcentaje" || (string)elem["Tipo"] == "Precio"){
					 		pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_TYPE_DECIMAL));
					 		Text Label = this.FindAndResolveComponent<Text> ("Label<Text>", pageType);
					 		Label.text = (string)elem["Etiqueta"];
					 	}

						if (editForm == 1){
							TitleForm.text = (string)rawData ["headers"]["Titulo"];
							InstructForm.text = (string)rawData ["headers"]["Instrucciones"];
						}

					 	ColumnList.Add(pageType);
					 	TypeColumnList.Add((string)elem["Tipo"]);
					 	
					 	if(elem.Keys.Contains("Id")){
					 		ColumnId.Add((int)elem["Id"]);
					 	}else{
					 		preForm= true;
					 	}

					 	if((string)elem["Tipo"] == "Opciones"){
							OptionItemList.Clear();
							OptionKeyList.Clear();
							OptionItemList.Add("--Selecciona--");
							OptionKeyList.Add("0");
					 		foreach(JsonData item in elem["item"]){
					 			// Debug.Log(item["Id"]);
					 			OptionItemList.Add((string)item["Valor"]);


					 			int optionId = (int)item["Id"];
					 			OptionKeyList.Add(optionId.ToString());
					 		}
					 		DropOptions.AddOptions (OptionItemList);
					 	}else if((string)elem["Tipo"] == "Booleano"){
					 		DropBoolean.AddOptions (BooleanItemList);
					 	}

						if ((string) elem ["Etiqueta"] == "Total" || (string) elem ["Etiqueta"] == "Copeo" || (string) elem ["Etiqueta"] == "Botellas" && (string)elem["Tipo"] == "Entero"){
							pageType.transform.SetParent (parentPreForm.transform);	
						}else{
							pageType.transform.SetParent (parentProject.transform);	
						}
						pageType.transform.localScale = new Vector3 (1, 1, 1);
						if(editForm == 2){
							editReportForm();
						}
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

		saveFormButton.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			if(preForm){
				getFormValue(preForm);
			}else{
				getFormValue();
			}
		});
	}

	protected void SendReportFormWS(bool preFormBool = false){
		String api_url="";
		if(preFormBool){ 
			api_url = DataPaths.WS_PRE_REPORT_FORM_URL;
		}else{
			api_url = DataPaths.WS_SEND_REPORT_FORM_URL;
		}	
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					if(preFormBool){
						BuildReportFormWS(2);
					}else{
						refreshActivity();
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

		// Serializacion
		NameValueCollection parameters = new NameValueCollection();

		int key= 0;
		if(SessionManager.Instance.CurrentSessionData.ProjectId != 0){
			parameters.Add ("activacion_id", SessionManager.Instance.CurrentSessionData.ProjectId.ToString());
		}else{
			parameters.Add ("activacion_id", SessionManager.Instance.CurrentSessionData.ProjectPending.ToString());
		}
		parameters.Add("user_role", PlayerPrefs.GetString("UserRole"));
		if(preFormBool){
			parameters.Add("edit_preform", "1");
		}
		foreach(String item in SelectedValue){
			if(preFormBool){
				Debug.Log (LabelPreForm[key]+"->"+item);
				parameters.Add(LabelPreForm[key], item);
			}else{
				Debug.Log(ColumnId[key].ToString()+" , "+item);
				parameters.Add (ColumnId[key].ToString(), item);
			}

			key++;
		}

		StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, parameters, onWebRequestDone, onWebRequestFail));
	}

	protected void SendChekIn(String ImgName){
		String api_url = DataPaths.WS_SAVE_CHECK_IN_URL;
		 Debug.Log(api_url);
		if (validateCheckin ()) {
			NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
				Debug.Log (webResponse);

				try {
					JsonData rawData = JsonMapper.ToObject (webResponse);

					if ((string)rawData ["status"] == "ok") {
						NotificationSystem.Instance.Terminate ();
						refreshActivity ();
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

			parameters.Add ("imageText", ImgName);
			parameters.Add ("activacion_id", SessionManager.Instance.CurrentSessionData.ProjectId.ToString ());

			StartCoroutine (NetClient.Instance.MakeWebRequest (api_url, parameters, onWebRequestDone, onWebRequestFail));
		} else {
			NotificationSystem.Instance.Terminate ();
			Error.SetActive (true);
		}
	}

	protected void getDataFromCF(){
		// NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
		String api_url;
		if(SessionManager.Instance.CurrentSessionData.ProjectId != 0){
			api_url = DataPaths.WS_GET_DATA_CAPTURE_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectId;
		}else{
			api_url = DataPaths.WS_GET_DATA_CAPTURE_FORM_URL+"?activacion_id="+SessionManager.Instance.CurrentSessionData.ProjectPending;
		}
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

                if ((string)rawData["status"] == "ok"){
					showPreform.SetActive(true);

                    Image1 = this.Find("Picture1", DisplayObject);
                    Image2 = this.Find("Picture2", DisplayObject);
                    Image3 = this.Find("Picture3", DisplayObject);
                    /*Image2.SetActive(false);
                    Image3.SetActive(false);*/
                    _currentPage = 0;

                    pages = new List<GameObject> { Image1, Image2, Image3 };

                    //Muestro datos de la imagen e imágenes
                    if (rawData ["data"].Keys.Contains("Imagenes")){
						foreach(JsonData elem in rawData ["data"]["Imagenes"]){
							checkInNameImg.text = (string)elem["name"];
					 	}
					}

					 //Muestro datos de los circulos (Total, Copeo , Botellas)
					 if(rawData ["data"].Keys.Contains("preformulario")){
					 	totalData.text = rawData ["data"]["preformulario"]["Total"].ToString();
					 	copeoData.text = rawData ["data"]["preformulario"]["Copeo"].ToString();
					 	botellasData.text = rawData ["data"]["preformulario"]["Botellas"].ToString();
					 }else{
					 	showPreFormData.SetActive(false);
					 }

					 //Muestro datos del reporte completo
					 if(rawData ["data"].Keys.Contains("reporte")){
					 	GameObject parentProject = this.Find("Form<Wrapper>", DisplayObject);
					 	foreach(JsonData elem in rawData ["data"]["reporte"]){
					 		GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_FORM_PATH));
						 	Text ReportLabel = this.FindAndResolveComponent<Text> ("Label<Text>", pageType);
							ReportLabel.text = (string)elem["Label"];

							Text ReportValue = this.FindAndResolveComponent<Text> ("Content<Text>", pageType);
							ReportValue.text = (string)elem["Valor"];

							pageType.transform.SetParent (parentProject.transform);
							pageType.transform.localScale = new Vector3 (1, 1, 1);
					 	}
					 }else{
					 	showReportFormData.SetActive(false);
					 }

				}else{

					NotificationSystem.Instance.NotifyMessage(DataMessages.SERVER_LOGIN_FAIL);
				}
			}catch (JsonException e) {
				Debug.LogError("JSON Exception: " + e.Message + "\n" + e.StackTrace);
				throw e;
			}

            StartCoroutine("FixSlider");
        };

		NetClient.OnWebRequestFailDelegate onWebRequestFail = delegate
		{
			NotificationSystem.Instance.PromptAction(DataMessages.SERVER_RESPONSE_FAIL, delegate {  }, delegate { NotificationSystem.Instance.Terminate(); });
		};

		//REVISA WEBSERVICE;

		StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));
	}

	public bool validateCheckin(){

		if (ImageName.text.Equals ("")) {
			return false;
		}
		return true;
	}

	protected void refreshActivity(){
		assignProducer.SetActive(false);
		approveProject.SetActive(false);
		reportForm.SetActive(false);
		checkIn.SetActive(false);
		detailsProject.SetActive(true);

		NotificationSystem.Instance.Terminate();
		getActivacionWS(true);
	}

	protected void ClearDropOptions () {
		for (int i = DropProductores.options.Count - 1; i >=0; i--)
		{
			DropProductores.options.RemoveAt(i);
		}
	}

	protected void editReportForm() {
		int key = 0;
		foreach(GameObject item in ColumnList){
			if (TypeColumnList [key] == "Opciones") {
				// codigo aqui para drops de tipo opciones
			}else if (TypeColumnList [key] == "Booleano") {
				// codigo aqui para drops de tipo booleano
			}else{
				// codigo aqui para drops de tipo entero
			}
		}
	}

	protected void getFormValue (bool preFormBool = false) {

		int key = 0;
		
		foreach (GameObject column in ColumnList) {
			if (TypeColumnList [key] == "Opciones" || TypeColumnList [key] == "Opciones") {
				Droptest = this.FindAndResolveComponent<Dropdown> ("Opciones<Drop>", column);
				SelectedValue.Add (getOptionSelected (Droptest));
			} else if (TypeColumnList [key] == "Booleano") {
				Droptest = this.FindAndResolveComponent<Dropdown> ("Opciones<Drop>", column);
				SelectedValue.Add (getBooleanSelected (Droptest));
			} else {
				data = this.FindAndResolveComponent<Text> ("ValueTexto<Text>", column);
				SelectedValue.Add (data.text);
			}

			key++;	

		}
		SendReportFormWS (preFormBool);
	}


	public string getProdSelected(){
		return (string)producerKeys [DropProductores.value];
	}

	public string getOptionSelected(Dropdown item){
		return (string)OptionItemList [item.value];
	}
	public string getBooleanSelected(Dropdown item){
		return (string)BooleanItemList [item.value];
	}

	protected override void ProcessUpdate()
    {
        {

            if (!_doingTransition)
            {

                if (Input.GetMouseButton(0))
                {

                    inputX = Input.GetAxis("Mouse X");

                }

                if (Input.GetKeyDown(KeyCode.D) || inputX < -0.5f)
                    NextPage();
                else if (Input.GetKeyDown(KeyCode.A) || inputX > 0.5f)
                    PrevPage();


                //Debug.Log(inputX);


                inputX = 0;

                if (Input.GetKeyDown(KeyCode.D))
                    NextPage();
                else if (Input.GetKeyDown(KeyCode.A))
                    PrevPage();
            }

        }
    }

    protected override void ProcessTermination()
    {
        Debug.Log("ShowProjectActivity => Terminated");
    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            inputX += touchDeltaPosition.x * swipeSpeed;
            inputY += touchDeltaPosition.y * swipeSpeed;
            //Debug.Log("X, Y: " + touchDeltaPosition.x + ", " + touchDeltaPosition.y);

            /*Text inputXDebug = this.FindAndResolveComponent<Text>("inputXDebug<Text>", DisplayObject);
            inputXDebug.text = inputX.ToString();
            */

            if (inputX < -1)
                NextPage();
            else if (inputX > 1)
                PrevPage();


            //Debug.Log(inputX);


            inputX = 0;
        }

    }

    void NextPage()
    {

        if (_doingTransition)
            return;

        // Check which is the leftmost page
        float leftMostX = 0;
        GameObject leftMostPage = null;

        foreach (GameObject page in pages)
        {

            RectTransform rect = page.ResolveComponent<RectTransform>();
            

            if (rect.anchoredPosition.x < leftMostX)
            {
                leftMostPage = page;
                leftMostX = rect.anchoredPosition.x;
                Debug.Log(page);
            }
            
        }

        foreach (GameObject page in pages)
        {

            // chek the leftmost page and change its position to the rightmost
            RectTransform rect = page.ResolveComponent<RectTransform>();

            

            if (page == leftMostPage)
            {
                // Place it on the rightmost position
                Vector2 nPos = rect.anchoredPosition;
                nPos.x = rect.rect.width * (_maxPages - 1);
                rect.anchoredPosition = nPos;
            }

            Vector3 tPos = rect.anchoredPosition;
            tPos.x -= rect.rect.width;

            page.ResolveComponent<AnimateComponent>().TranslateObject(tPos, 0.5f, null);


        }

        _currentPage++;

        _currentPage %= _maxPages;

    }

    void PrevPage()
    {
        /*
                if (_doingTransition)
                    return;

                // Check which is the righmost page
                float rightMostX = 0;
                GameObject rightMostPage = null;

                foreach (GameObject page in pages) {

                    RectTransform rect = page.ResolveComponent<RectTransform>();

                    if (rect.anchoredPosition.x > rightMostX) {
                        rightMostPage = page;
                        rightMostX = rect.anchoredPosition.x;
                    }

                }

                foreach (GameObject page in pages)
                {

                    // chek the rightmost page and change its position to the leftmost
                    RectTransform rect = page.ResolveComponent<RectTransform>();

                    if (page == rightMostPage)
                    {
                        // Place it on the leftmost position
                        Vector2 nPos = rect.anchoredPosition;
                        nPos.x = -rect.rect.width * (_maxPages - 1);
                        rect.anchoredPosition = nPos;
                    }

                    Vector3 tPos = rect.anchoredPosition;
                    tPos.x += rect.rect.width;

                    page.ResolveComponent<AnimateComponent>().TranslateObject(tPos, 0.5f, null);


                }

                _currentPage--;

                if (_currentPage < 0)
                    _currentPage = _maxPages - 1;
        */
    }

    IEnumerator FixSlider()
    {
        yield return new WaitForSeconds(.1f);
        Image2Rect = Image2.GetComponent<RectTransform>();
        Vector2 Image2Position = Image2Rect.anchoredPosition;
        Image2Position.x = Image2Rect.rect.width * 1;
        Image2Rect.anchoredPosition = Image2Position;

        Image3Rect = Image3.GetComponent<RectTransform>();
        Vector2 Image3Position = Image3Rect.anchoredPosition;
        Image3Position.x = Image3Rect.rect.width * 2;
        Image3Rect.anchoredPosition = Image3Position;
    }
}

