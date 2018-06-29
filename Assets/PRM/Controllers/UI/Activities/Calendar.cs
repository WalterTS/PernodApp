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

public class Calendar : UIModule<Calendar>, IActivity
{
    String currentMonth;
    String loopMonth;
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
			return DataPaths.ACTIVITY_CALENDAR_LAYOUT_PATH;
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
        NotificationSystem.Instance.NotifyProgress(DataMessages.ACTIVITY_LOADING);
        currentMonth = "";
		GameObject parentProject = this.Find("Events<Wrapper>", DisplayObject);
        String api_url = DataPaths.WS_CALENDAR_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
        Debug.Log(api_url);
        NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
            NotificationSystem.Instance.Terminate();
            Debug.Log(webResponse);

            try
            {
                JsonData rawData = JsonMapper.ToObject(webResponse);

                if((string)rawData["status"] == "ok"){
                    foreach(JsonData elem in rawData ["data"]["activaciones"]){
                        DateTime dt = DateTime.Parse((string)elem["fecha"]);
						loopMonth = dt.ToString("M", CultureInfo.CreateSpecificCulture("es-Mx"));

                        if(loopMonth != currentMonth){
                            currentMonth = loopMonth;

                            GameObject pageTypeMonth = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_MONTH_PATH));

                            Text MonthTitle = this.FindAndResolveComponent<Text> ("Title<Text>", pageTypeMonth);
                            MonthTitle.text = currentMonth;

                            pageTypeMonth.transform.SetParent (parentProject.transform);
                            pageTypeMonth.transform.localScale = new Vector3 (1, 1, 1);
                        }

                        GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_ITEM_LIST_H_PATH));

                        Text Hour = this.FindAndResolveComponent<Text> ("Hour<Text>", pageType);
						Hour.text = dt.ToString("dddd dd MMMM yyyy hh:mm tt", CultureInfo.CreateSpecificCulture("es-Mx"));

                        Text NameProject = this.FindAndResolveComponent<Text> ("Name<Text>", pageType);
                        NameProject.text = "@"+(string)elem["cdc"]["nombre"];

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
        Debug.Log("CalendarACTIVITY => Terminated");
    }

    protected override void ProcessUpdate()
    {
        
    }

}

