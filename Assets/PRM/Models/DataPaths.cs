using UnityEngine;
using System.Collections;
using System.IO;



public static class DataPaths {
    public const string DB_NAME = "prm_db";
	public const string WS_URL = "http://prm.flagship.com.mx";
	public const string WS_IMAGES_URL = "/uploads/images/";
	public const string WS_API_URL = WS_URL + "/api";
	public const string WS_NOTIFICATIONS_URL = WS_API_URL + "/noticias";
	public const string WS_USERS_URL = WS_API_URL + "/usuario";
	public const string WS_NOTIFICATION_URL = WS_API_URL + "/user-notification";
	public const string WS_READ_NOTIFICATION_URL = WS_API_URL + "/read-notification";
	public const string WS_PROJECTS_URL = WS_API_URL + "/activacion";
	public const string WS_GET_USERS_URL = WS_API_URL + "/all-users";
	public const string WS_CALENDAR_URL = WS_API_URL + "/calendario";
	public const string WS_PLANS_URL = WS_API_URL + "/proyecto";
	public const string WS_GET_ACTIVACION_URL = WS_API_URL + "/get-activacion";
	public const string WS_SET_STATUS_URL = WS_API_URL + "/set-status";
	public const string WS_FORM_ACTIVACION_URL = WS_API_URL + "/get-form-activacion";
	public const string WS_FORM_ACTIVACIONES_TIPO_URL = WS_API_URL + "/get-activaciones-tipo";
	public const string WS_FORM_CDC_URL = WS_API_URL + "/get-cdc-scope";
	public const string WS_SEND_FORM_ACTIVACION_URL = WS_API_URL + "/send-form-activacion";
	public const string WS_GET_PROD_LIST_URL = WS_API_URL + "/get-productores-list";
	public const string WS_GET_SUPERV_LIST_URL = WS_API_URL + "/get-supervisores-list";
	public const string WS_SET_PROD_ACTIVACION_URL = WS_API_URL + "/set-productor-activacion";
	public const string WS_SET_SUPERV_ACTIVACION_URL = WS_API_URL + "/set-supervisor-activacion";
	public const string WS_PRE_REPORT_FORM_URL = WS_API_URL + "/pre-capture-form";
	public const string WS_BUILD_REPORT_FORM_URL = WS_API_URL + "/capture-form";
	public const string WS_SEND_REPORT_FORM_URL = WS_API_URL + "/save-capture-form";
	public const string WS_NEW_PROJECT_FORM_URL = WS_API_URL + "/new-project-form";
	public const string WS_GET_MALL_CDC_BYREG_URL = WS_API_URL + "/get-mall-cdc-byreg";
	public const string WS_SEND_PROJECT_FORM_URL = WS_API_URL + "/send-project-form";
	public const string WS_SHOW_PROJECT_URL = WS_API_URL + "/show-project-plan";
	public const string WS_SAVE_CHECK_IN_URL = WS_API_URL + "/save-pre-reportform";
	public const string WS_GET_DATA_CAPTURE_FORM_URL = WS_API_URL + "/get-data-capture-form";
	public const string WS_GET_STATUS_ACTIVACION_URL = WS_API_URL + "/get-status-activacion";


    public static string DatabasePath
    {

        get
        {


#if UNITY_EDITOR

            var dbPath = string.Format(@"Assets/StreamingAssets/localdata/{0}", DB_NAME);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DB_NAME);
		
        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DB_NAME);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
             var loadDb = Application.dataPath + "/Raw/" + DB_NAME;  // this is the path to your StreamingAssets in iOS
            // then save to Application.persistentDataPath
			if(File.Exists(loadDb))
            	File.Copy(loadDb, filepath);
			// Set no backup flag to the database.
			UnityEngine.iOS.Device.SetNoBackupFlag (filepath);
#elif UNITY_WP8
            var loadDb = Application.dataPath + "/StreamingAssets/" + DB_NAME;  // this is the path to your StreamingAssets in iOS
            // then save to Application.persistentDataPath
            File.Copy(loadDb, filepath);
#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DB_NAME;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DB_NAME;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }


            var dbPath = filepath;
#endif
            return dbPath;
        }

    }

    public static string DataPath
    {
        get { return "/localdata/caribeando/assets"; }
    }


    public static string NotificationsAssetsPath
    {
        get { return DataPath + "/notifications"; }
    }
	
//	public const string ACTIVITY_LOGIN_LAYOUT_PATH = "UI/Layouts/Activities/Login";
    public const string ACTIVITY_KAMHOME_LAYOUT_PATH = "UI/Layouts/Activities/KamHome";
	public const string ACTIVITY_MANAGERHOME_LAYOUT_PATH = "UI/Layouts/Activities/ManagerHome";
	public const string ACTIVITY_BUILD_LAYOUT_PATH = "UI/Layouts/Activities/BuildProject";
	public const string ACTIVITY_PASSCODE_LAYOUT_PATH = "UI/Layouts/Activities/PassCode";
	public const string ACTIVITY_EXECUTIVEHOME_LAYOUT_PATH = "UI/Layouts/Activities/ExecutiveHome";
	public const string ACTIVITY_PROJECTS_LAYOUT_PATH = "UI/Layouts/Activities/Projects";
	public const string ACTIVITY_CALENDAR_LAYOUT_PATH = "UI/Layouts/Activities/Calendar";
	public const string ACTIVITY_USERS_LAYOUT_PATH = "UI/Layouts/Activities/Users";
	public const string ACTIVITY_REPORTMANAGER_LAYOUT_PATH = "UI/Layouts/Activities/ReportManager";
	public const string ACTIVITY_PRODUCERHOME_LAYOUT_PATH = "UI/Layouts/Activities/ProducerHome";
	public const string ACTIVITY_SHOWPROJECT_LAYOUT_PATH = "UI/Layouts/Activities/ShowProject";
	public const string ACTIVITY_SHOW_PLAN_LAYOUT_PATH = "UI/Layouts/Activities/ShowPlan";
	public const string ACTIVITY_NEWPROJECT_LAYOUT_PATH = "UI/Layouts/Activities/NewProject";
	public const string ACTIVITY_NEWACTIVACION_LAYOUT_PATH = "UI/Layouts/Activities/NewActivacion";
	public const string ACTIVITY_ADMIN_LAYOUT_PATH = "UI/Layouts/Activities/Admin";
	public const string ACTIVITY_INFO_LAYOUT_PATH = "UI/Layouts/Activities/Info";
	public const string ACTIVITY_REPORT_LAYOUT_PATH = "UI/Layouts/Activities/Report";
	public const string FRAGMENT_SIDEMENU_LAYOUT_PATH = "UI/Layouts/Fragments/SideMenu";
	public const string FRAGMENT_PROFILEMENU_LAYOUT_PATH = "UI/Layouts/Fragments/ProfileMenu";
	public const string FRAGMENT_GRIMOIRECUSTOMNOTIFICATION_LAYOUT_PATH = "UI/Layouts/Fragments/GrimoireCustomNotification";
	public const string FRAGMENT_MAINMENU_LAYOUT_PATH = "UI/Layouts/Fragments/MainMenu";
	public const string FRAGMENT_FOOTER_LAYOUT_PATH = "UI/Layouts/Fragments/Footer";
	public const string	FRAGMENT_PROMOTIONS_PAGE_ROW_PATH = "UI/Layouts/Fragments/PageRow";
	public const string	FRAGMENT_CONTACTO_ROW_PATH = "UI/Layouts/Fragments/ContactoRow";
	public const string	FRAGMENT_LADA_ROW_PATH = "UI/Layouts/Fragments/LadaRow";
	public const string	FRAGMENT_TYPE_PATH = "UI/Layouts/Fragments/Type";
	public const string	FRAGMENT_FORM_PATH = "UI/Layouts/Fragments/Form";
	public const string	FRAGMENT_CENTER_PATH = "UI/Layouts/Fragments/Center";
	public const string	FRAGMENT_DATE_PATH =  "UI/Layouts/Fragments/Date";
	public const string	FRAGMENT_PROJEC_PATH =  "UI/Layouts/Fragments/Projec";
	public const string	FRAGMENT_PROJ_PATH =  "UI/Layouts/Fragments/Proj";
	public const string	FRAGMENT_NOTIFICATION_PATH =  "UI/Layouts/Fragments/Notification";
	public const string	FRAGMENT_PROJECT_PATH =  "UI/Layouts/Fragments/ChildProject";
	public const string	FRAGMENT_ITEM_LIST_H_PATH =  "UI/Layouts/Fragments/ItemListH";
	public const string	FRAGMENT_MONTH_PATH =  "UI/Layouts/Fragments/Month";
	public const string	FRAGMENT_TYPE_OPCIONES =  "UI/Layouts/Fragments/Form/Opciones";
	public const string	FRAGMENT_TYPE_TEXTO =  "UI/Layouts/Fragments/Form/Texto";
	public const string	FRAGMENT_TYPE_ENTERO =  "UI/Layouts/Fragments/Form/Entero";
	public const string	FRAGMENT_TYPE_DECIMAL =  "UI/Layouts/Fragments/Form/Decimal";
	public const string	FRAGMENT_TYPE_CIRCLE =  "UI/Layouts/Fragments/Form/Circles";
	public const string	FRAGMENT_GROUP_1 =  "UI/Layouts/Fragments/InfoGral";
	public const string	FRAGMENT_ASSIGNMENT_TYPE =  "UI/Layouts/Fragments/Assignment";

}
