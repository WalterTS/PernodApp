using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using Occult.UI;
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
using System.Text.RegularExpressions;

public class NewProject : UIModule<NewProject>, IActivity
{
	GameObject pageType;
	GameObject parentProject;
	GameObject AssignmentGO;
	
	Text projectName;
	Text dateIni;
	Text dateFin;
	Text projectDesc;
	Text totalActiv;
	Text totalSale;
	Text impact;
	Text tasting;
	Text MaxActiv;
	GameObject Error;
	
	int region_id;
	int plaza_id;
    int cdc_id;
	float heightExpanded;
	float OheightExpanded;

	Button GenerateRegion;
	Button GeneratePlaza;
	Button GenerateCDC;
	Button SaveButton;

	Dropdown DropBrands;
	Dropdown DropAgency;
	MultiSelectDropdown DropTipo;
	MultiSelectDropdown DropRegiones;
	MultiSelectDropdown DropPlazas;
	MultiSelectDropdown DropCdc;
	Dropdown DropCancel;
	Dropdown DropTypeSale;

	LayoutElement AssignmentLayout;

	List<string> CancelList = new List<string>(){"-- Selecciona --","24 horas","48 horas","72 horas"};
	List<string> TipoVentaList = new List<string>(){"-- Selecciona --","Botella","Copa"};

	List<string> BrandList = new List<string>(){"-- Selecciona --"};
	List<string> BrandKeys = new List<string>(){"0"};

	List<string> AgencyList = new List<string>(){"-- Selecciona --"};
	List<string> AgencyKeys = new List<string>(){"0"};

    List<MultiSelectDropdown.OptionData> TipoActivList = new List<MultiSelectDropdown.OptionData>();
    List<int> TipoActivKeys = new List<int>() {0};

    List<MultiSelectDropdown.OptionData> RegionesList = new List<MultiSelectDropdown.OptionData>();
	List<int> RegionesKeys = new List<int>(){0};

    List<MultiSelectDropdown.OptionData> PlazasList = new List<MultiSelectDropdown.OptionData>();
    List<int> PlazasKeys = new List<int>(){0};

	List<MultiSelectDropdown.OptionData> CdcList = new List<MultiSelectDropdown.OptionData>();
    List<int> CdcKeys = new List<int>(){0};

    List<string> GralRegionesList = new List<string>() { "-- Selecciona --" };
    List<string> GralRegionesKeys = new List<string>() { "0" };

    List<string> GralPlazasList = new List<string>(){"-- Selecciona --"};
	List<string> GralPlazasKeys = new List<string>(){"0"};

	List<string> GralCdcList = new List<string>(){"-- Selecciona --"};
	List<string> GralCdcKeys = new List<string>(){"0"};

	List<GameObject> AssignRegList = new List<GameObject>();
	List<GameObject> AssignPlzList = new List<GameObject>();
	List<GameObject> AssignCdcList = new List<GameObject>();

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
			return DataPaths.ACTIVITY_NEWPROJECT_LAYOUT_PATH;
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

	DropBrands = this.FindAndResolveComponent<Dropdown> ("Brands<Drop>", DisplayObject);
	DropAgency = this.FindAndResolveComponent<Dropdown> ("Agency<Drop>", DisplayObject);
	DropTipo = this.FindAndResolveComponent<MultiSelectDropdown> ("TypeActiv<Drop>", DisplayObject);
	DropRegiones = this.FindAndResolveComponent<MultiSelectDropdown> ("Region<Drop>", DisplayObject);
	DropPlazas = this.FindAndResolveComponent<MultiSelectDropdown> ("Mall<Drop>", DisplayObject);
	DropCdc = this.FindAndResolveComponent<MultiSelectDropdown> ("CDC<Drop>", DisplayObject);
	DropCancel = this.FindAndResolveComponent<Dropdown> ("CancelActiv<Drop>", DisplayObject);
	DropTypeSale = this.FindAndResolveComponent<Dropdown> ("TypeSale<Drop>", DisplayObject);
	parentProject = this.Find("Assignment<Wrapper>", DisplayObject);
	AssignmentGO = this.Find("Item 4", DisplayObject);

	projectName = this.FindAndResolveComponent<Text>("ValueName<Text>", DisplayObject);
	dateIni = this.FindAndResolveComponent<Text>("ValueDateIni<Text>", DisplayObject);
	dateFin = this.FindAndResolveComponent<Text>("ValueDateEnd<Text>", DisplayObject);
	projectDesc = this.FindAndResolveComponent<Text>("ValueDesc<Text>", DisplayObject);
	totalActiv = this.FindAndResolveComponent<Text>("ValueTotalActiv<Text>", DisplayObject);
	totalSale = this.FindAndResolveComponent<Text>("ValueTotalSale<Text>", DisplayObject);
	impact = this.FindAndResolveComponent<Text>("ValueImpact<Text>", DisplayObject);
	tasting = this.FindAndResolveComponent<Text>("ValueTasting<Text>", DisplayObject);
	MaxActiv = this.FindAndResolveComponent<Text>("ValueMaxActiv<Text>", DisplayObject);
	Error = this.Find ("Error<Text>", DisplayObject); 
		Error.SetActive (false);

	GenerateRegion = this.FindAndResolveComponent<Button> ("GenerateRegion<Button>", DisplayObject);
	GeneratePlaza = this.FindAndResolveComponent<Button> ("GeneratePlaza<Button>", DisplayObject);
	GenerateCDC = this.FindAndResolveComponent<Button> ("GenerateCDC<Button>", DisplayObject);
	SaveButton = this.FindAndResolveComponent<Button> ("Save<Button>", DisplayObject);

	DropCancel.AddOptions(CancelList);
	DropTypeSale.AddOptions(TipoVentaList);

	AssignmentLayout = AssignmentGO.GetComponent<LayoutElement>();

	String api_url = DataPaths.WS_NEW_PROJECT_FORM_URL;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
					BrandList.Clear();
					BrandKeys.Clear();
					BrandList.Add("-- Selecciona --");
					BrandKeys.Add("0");
					 foreach(JsonData elem in rawData ["data"]["marcas"]){
					 	int brandId = (int)elem["id"];
						BrandList.Add((string)elem["nombre"]);
						BrandKeys.Add(brandId.ToString());
					 }
					DropBrands.ClearOptions();
					DropBrands.RefreshShownValue();
					DropBrands.AddOptions (BrandList);

					AgencyList.Clear();
					AgencyKeys.Clear();
					AgencyList.Add("-- Selecciona --");
					AgencyKeys.Add("0");
					foreach(JsonData elem in rawData ["data"]["agencias"]){
					 	int AgencyId = (int)elem["id"];
						AgencyList.Add((string)elem["nombre"]);
						AgencyKeys.Add(AgencyId.ToString());
					}
					DropAgency.ClearOptions();
					DropAgency.RefreshShownValue();
					DropAgency.AddOptions (AgencyList);

					TipoActivList.Clear();
					TipoActivKeys.Clear();
                    foreach (JsonData elem in rawData["data"]["tipo_activacion"])
                    {
                        int TipoId = (int)elem["id"];
                        TipoActivList.Add(new MultiSelectDropdown.OptionData((string)elem["nombre"]));
                        TipoActivKeys.Add(TipoId);
                    }
                    /*DropTipo.ClearOptions();
					DropTipo.RefreshShownValue();*/
                    DropTipo.options = (TipoActivList);

                    RegionesList.Clear();
					RegionesKeys.Clear();
					foreach(JsonData elem in rawData ["data"]["regiones"]){
					 	region_id = (int)elem["id"];
                        RegionesList.Add(new MultiSelectDropdown.OptionData((string)elem["nombre"]));
						RegionesKeys.Add(region_id);
					}
					/*DropRegiones.ClearOptions();
					DropRegiones.RefreshShownValue();*/
					DropRegiones.options = (RegionesList);

                    // lleno listas para campos dinámicos
                    foreach (JsonData elem in rawData["data"]["regiones"])
                    {
                        int id = (int)elem["id"];
                        GralRegionesList.Add((string)elem["nombre"]);
                        GralRegionesKeys.Add(id.ToString());
                    }

                    foreach (JsonData elem in rawData ["data"]["plazas"]){
					 	int id = (int)elem["id"];
						GralPlazasList.Add((string)elem["nombre"]);
						GralPlazasKeys.Add(id.ToString());
					}
					foreach(JsonData elem in rawData ["data"]["cdc"]){
					 	int id = (int)elem["id"];
						if(elem.Keys.Contains("plaza")){
							GralCdcList.Add((string)elem["nombre"]+" @ "+(string)elem["plaza"]["nombre"]);
						}else{
							GralCdcList.Add((string)elem["nombre"]);
						}
						GralCdcKeys.Add(id.ToString());
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

		DropRegiones.onValueChanged.AddListener(delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);

			Debug.Log("Mando ID de región: "+region_id);
			getPlazas(getRegionSelected());

		});

		//Botones para generar asignaciones

		GenerateRegion.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			generateType("region");
			getHeightExpanded(AssignRegList.Count < 1 ? true : false);
			AssignmentLayout.preferredHeight = heightExpanded + 200; 
		});
		GeneratePlaza.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			generateType("plaza");
			getHeightExpanded(AssignPlzList.Count < 1 ? true : false);
			AssignmentLayout.preferredHeight = heightExpanded + 200; 
		});
		GenerateCDC.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			generateType("cdc");
			getHeightExpanded(AssignCdcList.Count < 1 ? true : false);
			AssignmentLayout.preferredHeight = heightExpanded + 200; 
		});
		SaveButton.onClick.AddListener (delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
			sendFormWS();
		});

	}

	protected void getPlazas(string region_id){
		String api_url = DataPaths.WS_GET_MALL_CDC_BYREG_URL+"?region_id="+region_id;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				PlazasList.Clear();
				PlazasKeys.Clear();
                plaza_id = 0;
				JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
 
                    foreach (JsonData elem in rawData["data"])
                    {
                        plaza_id = (int)elem["id"];
                        PlazasList.Add(new MultiSelectDropdown.OptionData((string)elem["nombre"]));
                        PlazasKeys.Add(plaza_id);
                    }
                    /* DropPlazas.ClearOptions();
                     DropPlazas.RefreshShownValue();*/
                    DropPlazas.options = (PlazasList);

                }
                else{
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

		DropPlazas.onValueChanged.AddListener(delegate {
			NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);

			Debug.Log("Mando ID de plaza: OLD "+plaza_id);
			getCdc(getPlazaSelected());
		});


	}	

	protected void getCdc(string plaza_id){
		String api_url = DataPaths.WS_GET_MALL_CDC_BYREG_URL+"?plaza_id="+plaza_id;
		Debug.Log(api_url);
		NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
			NotificationSystem.Instance.Terminate();
			Debug.Log(webResponse);

			try
			{
				CdcList.Clear();
				CdcKeys.Clear();
                cdc_id = 0;
                JsonData rawData = JsonMapper.ToObject(webResponse);

				if((string)rawData["status"] == "ok"){
                    foreach (JsonData elem in rawData["data"])
                    {
                        cdc_id = (int)elem["id"];
                        CdcList.Add(new MultiSelectDropdown.OptionData((string)elem["nombre"]));
                        CdcKeys.Add(cdc_id);
                    }
                    /*DropCdc.ClearOptions();
					DropCdc.RefreshShownValue();*/
                    DropCdc.options = (CdcList);


                }
                else{
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

	protected void sendFormWS(){
		if (validateform ()) {
		String api_url = DataPaths.WS_SEND_PROJECT_FORM_URL;
		Debug.Log(api_url);

			NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
				NotificationSystem.Instance.Terminate ();
				Debug.Log (webResponse);

				try {
					JsonData rawData = JsonMapper.ToObject (webResponse);

					if ((string)rawData ["status"] == "ok") {
						SessionManager.Instance.CurrentSessionData.ProjectId = (int)rawData ["data"] ["id"];
						SessionManager.Instance.OpenActivity (ShowPlan.Instance);
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

		
			NameValueCollection parameters = new NameValueCollection ();

			parameters.Add ("user_id", PlayerPrefs.GetInt ("user_id").ToString ());
			parameters.Add ("project_name", projectName.text);
			parameters.Add ("project_dateIni", dateIni.text);
			parameters.Add ("project_dateFin", dateFin.text);
			parameters.Add ("project_desc", projectDesc.text);
			parameters.Add ("project_totalActiv", totalActiv.text);
			parameters.Add ("project_totalSale", totalSale.text);
			parameters.Add ("project_impact", impact.text);
			parameters.Add ("project_tasting", tasting.text);
			parameters.Add ("project_maxActiv", MaxActiv.text);
			parameters.Add ("project_region", getRegionSelected ());
			parameters.Add ("project_plaza", getPlazaSelected ());
			parameters.Add ("project_cdc", getCDCSelected ());
			parameters.Add ("project_cancel", getCancelSelected ());
			parameters.Add ("project_typeSale", getTypeSaleSelected ());
			parameters.Add ("project_typeActiv", getTipoActivSelected ());
			parameters.Add ("project_agency", getAgencySelected ());
			parameters.Add ("project_brand", getBrandSelected ());
		
			foreach (GameObject reg in AssignRegList) {
				Dropdown Drop = this.FindAndResolveComponent<Dropdown> ("Select<Drop>", reg);
				Text total = this.FindAndResolveComponent<Text> ("ValueTotal<Text>", reg);			
				parameters.Add ("assign_region", getAssignReg (Drop));
				parameters.Add ("assign_total_region", total.text);
			}
			foreach (GameObject plaza in AssignPlzList) {
				Dropdown Drop = this.FindAndResolveComponent<Dropdown> ("Select<Drop>", plaza);
				Text total = this.FindAndResolveComponent<Text> ("ValueTotal<Text>", plaza);			
				parameters.Add ("assign_plaza", getAssignPlz (Drop));
				parameters.Add ("assign_total_plaza", total.text);
			}
			foreach (GameObject cdc in AssignCdcList) {
				Dropdown Drop = this.FindAndResolveComponent<Dropdown> ("Select<Drop>", cdc);
				Text total = this.FindAndResolveComponent<Text> ("ValueTotal<Text>", cdc);			
				parameters.Add ("assign_cdc", getAssignCdc (Drop));
				parameters.Add ("assign_total_cdc", total.text);
			}
		
			StartCoroutine (NetClient.Instance.MakeWebRequest (api_url, parameters, onWebRequestDone, onWebRequestFail));
		} else {
			Error.SetActive (true);
			NotificationSystem.Instance.Terminate ();
		}
	} 

	protected void generateType(string typeform){
		List<string> items = null;
		String title = "";

		if(typeform == "region"){
			items = GralRegionesList;
			title = "Asignaciones de región";
		}else if(typeform == "plaza"){
			items = GralPlazasList;
			title = "Asignaciones de plaza";
		}else if(typeform == "cdc"){
			items = GralCdcList;
			title = "Asignaciones de centros de consumo";
		}

		pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_ASSIGNMENT_TYPE));
		Dropdown DropOptions = this.FindAndResolveComponent<Dropdown> ("Select<Drop>", pageType);
		DropOptions.AddOptions(items);

		Text Label = this.FindAndResolveComponent<Text> ("TitleSelect<Text>", pageType);
		Label.text = title;

		if(typeform == "region"){
			AssignRegList.Add(pageType);
			Button DeleteAssign = this.FindAndResolveComponent<Button> ("Close<Button>", pageType);
			DeleteAssign.onClick.AddListener (delegate {
				GameObject.Destroy(AssignRegList[AssignRegList.Count-1]);
				AssignRegList.RemoveAt(AssignRegList.Count-1);

				getHeightExpanded(AssignRegList.Count < 1 ? true : false);
				AssignmentLayout.preferredHeight = OheightExpanded - 200;
			});

		}else if(typeform == "plaza"){
			AssignPlzList.Add(pageType);

			Button DeleteAssign = this.FindAndResolveComponent<Button> ("Close<Button>", pageType);
			DeleteAssign.onClick.AddListener (delegate {
				GameObject.Destroy(AssignPlzList[AssignPlzList.Count-1]);
				AssignPlzList.RemoveAt(AssignPlzList.Count-1);

				getHeightExpanded(AssignPlzList.Count < 1 ? true : false);
				AssignmentLayout.preferredHeight = OheightExpanded - 200;
			});

		}else if(typeform == "cdc"){
			AssignCdcList.Add(pageType);

			Button DeleteAssign = this.FindAndResolveComponent<Button> ("Close<Button>", pageType);
			DeleteAssign.onClick.AddListener (delegate {
				GameObject.Destroy(AssignCdcList[AssignCdcList.Count-1]);
				AssignCdcList.RemoveAt(AssignCdcList.Count-1);

				getHeightExpanded(AssignCdcList.Count < 1 ? true : false);
				AssignmentLayout.preferredHeight = OheightExpanded - 200;
			});
		}
		
		pageType.transform.SetParent (parentProject.transform);
		pageType.transform.localScale = new Vector3 (1, 1, 1);
		NotificationSystem.Instance.Terminate();
	}

	public string getRegionSelected(){
        List<int> selectedIndices = DropRegiones.value;
        string region_list = "";
        string multiRegion = "";
        int indexToArray;
        foreach (int indice in selectedIndices) {
        	indexToArray = indice - 1;
           region_list = region_list + (string) indexToArray.ToString() + ",";
        }
        string[] dropPositions = Regex.Split(region_list, ",");
		foreach(string index in dropPositions){
			if(index != ""){
				multiRegion = multiRegion + (string) RegionesKeys[Int32.Parse(index)].ToString() + ",";
			}
		}

        return multiRegion;
	}

    public string getPlazaSelected()
    {
    	List<int> selectedIndices = DropPlazas.value;
        string plaza_list = "";
        string multiPlaza = "";
        int indexToArray;
        foreach (int indice in selectedIndices) {
        	indexToArray = indice - 1;
           plaza_list = plaza_list + (string) indexToArray.ToString() + ",";
        }
        string[] dropPositions = Regex.Split(plaza_list, ",");
		foreach(string index in dropPositions){
			if(index != ""){
				multiPlaza = multiPlaza + (string) PlazasKeys[Int32.Parse(index)].ToString() + ",";
			}
		}

        return multiPlaza;

    }

    public string getCDCSelected()
    {
        List<int> selectedIndices = DropCdc.value;
        string cdc_list = "";
        string multiCdc = "";
        int indexToArray;
        foreach (int indice in selectedIndices) {
        	indexToArray = indice - 1;
           cdc_list = cdc_list + (string) indexToArray.ToString() + ",";
        }
        string[] dropPositions = Regex.Split(cdc_list, ",");
		foreach(string index in dropPositions){
			if(index != ""){
				multiCdc = multiCdc + (string) CdcKeys[Int32.Parse(index)].ToString() + ",";
			}
		}

        return multiCdc;
    }

    public string getCancelSelected(){
		return (string)CancelList [DropCancel.value];
	}
	public string getTypeSaleSelected(){
		return (string)TipoVentaList [DropTypeSale.value];
	}
	public string getTipoActivSelected()
    {
       List<int> selectedIndices = DropTipo.value;
        string tipo_list = "";
        string multiTipo = "";
        int indexToArray;
        foreach (int indice in selectedIndices) {
        	indexToArray = indice - 1;
           tipo_list = tipo_list + (string) indexToArray.ToString() + ",";
        }
        string[] dropPositions = Regex.Split(tipo_list, ",");
		foreach(string index in dropPositions){
			if(index != ""){
				multiTipo = multiTipo + (string) TipoActivKeys[Int32.Parse(index)].ToString() + ",";
			}
		}

        return multiTipo;

    }

    public string getAgencySelected(){
		return (string)AgencyKeys [DropAgency.value];
	}
	public string getBrandSelected(){
		return (string)BrandKeys [DropBrands.value];
	}

	public string getAssignReg(Dropdown item){
		return (string) GralRegionesKeys [item.value];
	}
	public string getAssignPlz(Dropdown item){
		return (string)GralPlazasKeys [item.value];
	}
	public string getAssignCdc(Dropdown item){
		return (string)GralCdcKeys [item.value];
	}
	protected void getHeightExpanded(bool first){
		if(first){
			OheightExpanded = LayoutUtility.GetPreferredHeight(AssignmentGO.transform as RectTransform);
		}else{
			heightExpanded = LayoutUtility.GetPreferredHeight(AssignmentGO.transform as RectTransform);
		}
	}

	protected void ClearDropRegiones () {
		for (int i = DropRegiones.options.Count - 1; i >=0; i--)
		{
			DropRegiones.options.RemoveAt(i);
		}
	}
	protected void ClearDropPlazas () {
		Debug.Log("Entra ClearDropPlazas");
		for (int i = DropPlazas.options.Count - 1; i >=0; i--)
		{
			Debug.Log("iteracion: "+i);
			DropPlazas.options.RemoveAt(i);
		}
		//DropPlazas.RefreshShownValue();
	}

	public bool validateform(){

		if (projectName.text.Equals ("")) {
			Debug.Log (projectName.text);
			return false;
		}

		if ( dateIni.text.Equals ("")){
			Debug.Log (dateIni.text);
			return false;
		}

		if (dateFin.text.Equals ("")) {
			Debug.Log (dateFin.text);
			return false;
		}
		if (projectDesc.text.Equals ("")) {
			Debug.Log(projectDesc.text);
			return false;
		}
		if (totalActiv.text.Equals("")){
			Debug.Log(totalActiv.text);
			return false;
		}
		if (totalSale.text.Equals("")){
			Debug.Log (totalSale.text);
			return false;
		}
		if (impact.text.Equals ("")) {
			Debug.Log (impact.text);
			return false;
		}
		if (tasting.text.Equals ("")) {
			Debug.Log (tasting.text);
			return false;
		}
		if (MaxActiv.text.Equals ("")) {
			Debug.Log (MaxActiv.text);
			return false;
		}

		if (DropBrands.value.Equals (0)) {
			return false;
		}
		if (DropAgency.value.Equals (0)) {
			return false;
		}
		if (DropTipo.value.Equals (0)) {
			return false;
		}
		if (DropTypeSale.value.Equals (0)) {
			return false;
		}
		if (DropRegiones.value.Equals (0)) {
			return false;
		}
		if (DropPlazas.value.Equals (0)) {
			return false;
		}
		if (DropCdc.value.Equals (0)) {
			return false;
		}
		if (DropCancel.value.Equals (0)) {
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

