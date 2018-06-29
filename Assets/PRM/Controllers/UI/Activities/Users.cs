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

public class Users : UIModule<Users>, IActivity
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
			return DataPaths.ACTIVITY_USERS_LAYOUT_PATH;
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
        GameObject parentProject = this.Find("UsersContainer<Wrapper>", DisplayObject);
        String api_url = DataPaths.WS_GET_USERS_URL+"?user_id="+PlayerPrefs.GetInt("user_id");
        Debug.Log(api_url);
        NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
            NotificationSystem.Instance.Terminate();
            Debug.Log(webResponse);

            try
            {
                JsonData rawData = JsonMapper.ToObject(webResponse);

                if((string)rawData["status"] == "ok"){
                    foreach(JsonData elem in rawData ["data"]["usuarios"]){


                        GameObject pageType = Instantiate<GameObject> (Resources.Load<GameObject> (DataPaths.FRAGMENT_NOTIFICATION_PATH));

                        Text promotionSubtitle = this.FindAndResolveComponent<Text> ("Title<Text>", pageType);

                        promotionSubtitle.text = elem.Keys.Contains("nombre") ? (string)elem["nombre"] : (string)elem["username"];

                        Text promotionDescription = this.FindAndResolveComponent<Text> ("Content<Text>", pageType);
                        promotionDescription.text = (string)elem["email"];

                        pageType.transform.SetParent (parentProject.transform);
                        pageType.transform.localScale = new Vector3 (1, 1, 1);

                        // Button notificationButton = pageType.gameObject.GetComponent<Button>() as Button;
                        // notificationButton.onClick.AddListener (delegate {
                        //  SessionManager.Instance.OpenActivity(ExecutiveHome.Instance);
                        //  Terminate();
                        // });

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
        Debug.Log("UsersACTIVITY => Terminated");
    }

    protected override void ProcessUpdate()
    {
        
    }

}

