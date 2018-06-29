using UnityEngine;
using System.Collections;
using System.Reflection;

using GLIB.Core;
using GLIB.Interface;
using GLIB.Utils;
using KiiCorp.Cloud.Storage;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class SessionManager : BackModule<SessionManager> {
 

    List<IActivity> _activityViewHistory = new List<IActivity>();
    
    IActivity _lastActivity {
        get {
            if (_activityViewHistory.Count > 0)
                return _activityViewHistory[_activityViewHistory.Count - 1];
            else
                return null;
        }
    }

    SessionData _currentSessionData;
    public SessionData CurrentSessionData {
        get {

            if (_currentSessionData == null)
                _currentSessionData = new SessionData();

            return _currentSessionData;

        }
    }

    public void OpenActivity(IActivity activity) {

        if (!isRunning)
        {
            Debug.LogWarning("SessionManager wasn't running. Initializing.");
            Initialize();
        }

        activity.StartActivity();
        _activityViewHistory.Add(activity);
        Debug.Log("Opening Activity: " + activity.GetType().Name);


    }

    public void TerminateActivity(IActivity activity) {

        if (!isRunning)
        {
            Debug.LogWarning("SessionManager wasn't running. Initializing.");
            Initialize();
        }

        activity.EndActivity();
        _activityViewHistory.Remove(activity);
        Debug.Log("Terminating Activity: " + activity.GetType().Name);

    }

    protected override void ProcessInitialization()
    {
        Debug.Log("SessionManager => Started");

        // Set kii push notification handlers
        if (Application.isMobilePlatform) {
            KiiPushService.Instance.OnPushMessageReceived = delegate(ReceivedMessage receivedMessage) {
                string kiiKey= "";
                if(receivedMessage.GetString("MsgBody") != null){
                    //Android
                    kiiKey= receivedMessage.GetString("MsgBody");
                }else if(receivedMessage.GetString("body") != null){
                    //iOS
                    kiiKey= receivedMessage.GetString("body");
                }
                
                // NotificationSystem.Instance.NotifyMessage(kiiKey); 
                selectActivity(receivedMessage.GetString("path"), kiiKey);

            //fin de instancia    
            };

            KiiPushService.Instance.OnBackgroundPushMessageReceived = delegate (ReceivedMessage receivedMessage)
            {
                string kiiKey= "";
                if(receivedMessage.GetString("MsgBody") != null){
                    //Android
                    kiiKey= receivedMessage.GetString("MsgBody");
                }else if(receivedMessage.GetString("body") != null){
                    //iOS
                    kiiKey= receivedMessage.GetString("body");
                }
                
               
                selectActivity(receivedMessage.GetString("path"), kiiKey);
            };
        }
    }

    protected void selectActivity(string path, string message){
        Debug.Log("Path sin separar: "+path);

        string[] lines = Regex.Split(path, "/");
        lines = lines.Skip(1).ToArray();  

        if(lines.Length > 1){
            if(lines[0] == "activacion"){
                NotificationSystem.Instance.PromptAction(message, delegate { 
                    SessionManager.Instance.CurrentSessionData.ProjectPending = Int32.Parse(lines[1]);
                    Debug.Log("ProjectPending "+SessionManager.Instance.CurrentSessionData.ProjectPending);
                    SessionManager.Instance.TryShuttingDownAllOpenedActivities();
                    SessionManager.Instance.OpenActivity(ShowProject.Instance);
                }, delegate { NotificationSystem.Instance.Terminate(); });
            }
        }else{
            //En caso de que no lleve a una pantalla interna con id
        }
    }

    protected override void ProcessTermination()
    {
        Debug.Log("SessionManager => Terminated");
    }

    protected override void ProcessUpdate()
    {
        
    }

    public void GoToPreviousActivity() {

        if (_activityViewHistory.Count > 1 && _lastActivity.isActivityReady)
        {
            TerminateActivity(_lastActivity);

            // Now that the last activity has been deleted, the previous becomes the last one so that we can reference it.
            IActivity previousActivity = _lastActivity;

            previousActivity.StartActivity();

            Debug.Log("SessionManager => Starting Activity: " + previousActivity.GetType().Name);
        }

    }    

	public bool IsCurrentlyOpened(String activity){
		IActivity currentActivity = _activityViewHistory [_activityViewHistory.Count-1];
		return  activity == currentActivity.GetType ().Name ? true:false;
	}

    public void ClearSession( bool preserveFirstActivityInViewHistory = true ) {

        IActivity firstActivity = null;

        if (preserveFirstActivityInViewHistory && _activityViewHistory.Count > 0) {
            firstActivity = _activityViewHistory[0];
        }  

        _currentSessionData = new SessionData();
        _activityViewHistory.Clear();

        if (firstActivity != null) {
            _activityViewHistory.Add(firstActivity);
        }
                
    }

	public void SetHomeAsRoot(bool logout = false){
		_currentSessionData = new SessionData();
		_activityViewHistory.Clear();

        if(logout == false){
            Debug.Log("logout false");
            _activityViewHistory.Add (ExecutiveHome.Instance);
        }
	}

    public void TryShuttingDownAllOpenedActivities() {

        foreach (IActivity activity in _activityViewHistory)
            activity.EndActivity();

    }
    
}
