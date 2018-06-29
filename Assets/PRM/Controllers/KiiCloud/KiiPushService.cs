using UnityEngine;
using System;
using System.Collections;
using KiiCorp.Cloud.Unity;
using KiiCorp.Cloud.Storage;
using GLIB.Core;

public class KiiPushService : BackModule<KiiPushService>
{

    KiiPushPlugin.KiiPushMessageReceivedCallback _onPushMessageReceived;
    public KiiPushPlugin.KiiPushMessageReceivedCallback OnPushMessageReceived
    {
        get { return _onPushMessageReceived; }
        set { _onPushMessageReceived += value; }
    }

    KiiPushPlugin.KiiPushMessageReceivedCallback _onBackgroundPushMessageReceived;
    public KiiPushPlugin.KiiPushMessageReceivedCallback OnBackgroundPushMessageReceived
    {
        get { return _onBackgroundPushMessageReceived; }
        set { _onBackgroundPushMessageReceived += value; }
    }
    
    string user
    {
        get
        {
			string username = "user_"+PlayerPrefs.GetString("user_code");

            if (string.IsNullOrEmpty(username))
            {
                // retrieve new user
                username = Guid.NewGuid().ToString().Replace("-", "");
                PlayerPrefs.SetString("kiiUser", username);
            }

            Debug.Log("####Username: " + username);

            return username;
        }
    }

    string _lastBackgroundPushMessageIdTriggered {
        get {
            return PlayerPrefs.GetString("lastPushMessageId", "");
        }
        set {
            PlayerPrefs.SetString("lastPushMessageId", value);
        }
    }

    string pass = "Pa$$word";
    //string topicName = "message_box";
    string m_bucketName = "app_bucket";
    string m_appTopicName = "app_topic";

    private PushSetting pushSetting = null;
    private APNSSetting apnsSetting = null;
    private GCMSetting gcmSetting = null;
    
    KiiPushPlugin kiiPushPlugin;
        
    protected override void ProcessInitialization()
    {
        Debug.Log("#####Main.Start");
        kiiPushPlugin = GameObject.Find("KiiPushPlugin").GetComponent<KiiPushPlugin>();

        string lastMessage = kiiPushPlugin.GetLastMessage();
        if (lastMessage != null)
        {
            //Check if the last message id was triggered from notification.
            ReceivedMessage parsedLastMessage = ReceivedMessage.Parse(lastMessage);

            string BackgroundMsg= "";
            if(parsedLastMessage.GetString("MsgBody") != null){
                //Android
                BackgroundMsg= parsedLastMessage.GetString("MsgBody");
            }else if(parsedLastMessage.GetString("body") != null){
                //iOS
                BackgroundMsg= parsedLastMessage.GetString("body");
            }
            string parsedLastMessageId = BackgroundMsg;

            _lastBackgroundPushMessageIdTriggered = parsedLastMessageId;
            if (_onBackgroundPushMessageReceived != null)
            {
                _onBackgroundPushMessageReceived(parsedLastMessage);
            }
            else {
                Debug.LogWarning("No function bound to OnBackgroundPushMessageReceived.");
            }
                       
        }
        
        kiiPushPlugin.OnPushMessageReceived += _onPushMessageReceived;
        
        pushSetting = new PushSetting();
        apnsSetting = new APNSSetting();
        gcmSetting = new GCMSetting();

        LogIn();
    }

    protected override void ProcessUpdate()
    {

    }

    protected override void ProcessTermination()
    {

    }

   

    void LogIn()
    {

        if (KiiUser.CurrentUser != null)
        {
            // User is logged in already
            RegisterPush();
            return;
        }

        KiiUser.LogIn(user, pass, (KiiUser u1, System.Exception e1) =>
        {
            if (e1 != null)
            {
                // User doesn't exist, create one
                KiiUser newUser = KiiUser.BuilderWithName(user).Build();
                Debug.Log("#####Register");

                newUser.Register(pass, (KiiUser u2, System.Exception e2) =>
                {

                    Debug.Log("#####callback Register");

                    if (e2 != null)
                    {
                        // Error registering user
                        Debug.Log("#####failed to Register");
                        this.ShowException("Failed to register user.", e2);
                        return;
                    }
                    else
                    {
                        //Invoke ("registerPush", 0);
                        RegisterPush();
                    }
                });
            }
            else
            {
                //Invoke ("registerPush", 0);
                RegisterPush();
            }
        });

    }

    void RegisterPush()
    {
#if UNITY_IPHONE
		KiiPushInstallation.DeviceType deviceType = KiiPushInstallation.DeviceType.IOS;
#elif UNITY_ANDROID
        KiiPushInstallation.DeviceType deviceType = KiiPushInstallation.DeviceType.ANDROID;
#else
		KiiPushInstallation.DeviceType deviceType = KiiPushInstallation.DeviceType.ANDROID;
#endif

        if (this.kiiPushPlugin == null)
        {
            Debug.Log("#####failed to find KiiPushPlugin");
            return;
        }
        this.kiiPushPlugin.RegisterPush((string pushToken, System.Exception e0) =>
        {
            if (e0 != null)
            {
                Debug.Log("#####failed to RegisterPush: " + pushToken);
                return;
            }

            Debug.Log("#####RegistrationId= Token:" + pushToken);
            Debug.Log("#####Install");

            KiiUser.PushInstallation(false).Install(pushToken, deviceType, (System.Exception e3) =>
            {
                if (e3 != null)
                {
                    Debug.Log("#####failed to Install");
                    this.ShowException("Failed to install PushNotification -- pushToken=" + pushToken, e3);
                    return;
                }

						suscribeTopic("MainTopic");
						String country_topic = PlayerPrefs.GetString("pais_locale");
						if(country_topic != null && country_topic != ""){
							suscribeTopic(country_topic);
						}
            });
        });
    }

	private void suscribeTopic(String topicName){

		KiiTopic topic = Kii.Topic(topicName);

		Debug.Log("#####Subscribe");
		KiiUser.CurrentUser.PushSubscription.Subscribe(topic, (KiiSubscribable subscribable, Exception e6) => {
			Debug.Log("#####callback Subscribe");
			if (e6 != null)
			{
				if (e6 is ConflictException)
				{
					Debug.Log("Topic is already subscribed" + "\n");
					Debug.Log("Push is ready");
					Debug.Log("#####all setup success!!!!!!");
					return;
				}
				Debug.Log("#####failed to Subscribe");
				this.ShowException("Failed to subscribe bucket", e6);
				return;
			}
			else
			{
				Debug.Log("Push is ready");
				Debug.Log("#####all setup success!!!!!!");
			}
		});
	}

   

    private void ShowException(string msg, System.Exception e)
    {
        Debug.Log("#####" + e.Message);
        Debug.Log("#####" + e.StackTrace);
        //this.message = "#####ERROR: " + msg + "   type=" + e.GetType() + "\n";
        if (e.InnerException != null)
        {
            Debug.Log("#####InnerExcepton=" + e.InnerException.GetType() + "\n");
            Debug.Log("#####InnerExcepton.Message=" + e.InnerException.Message + "\n");
            Debug.Log("#####InnerExcepton.Stacktrace=" + e.InnerException.StackTrace + "\n");
        }
        //this.message += "#####" + e.Message + "\n" + "#####" + e.StackTrace;
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
#if UNITY_IPHONE
            UnityEngine.iOS.LocalNotification setCountNotif = new UnityEngine.iOS.LocalNotification();
			setCountNotif.fireDate = System.DateTime.Now;
			setCountNotif.applicationIconBadgeNumber = -1;
			setCountNotif.hasAction = false;
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(setCountNotif);
#endif
        }
    }

    class PushSetting
    {
        public bool EnableAPNS { get; set; }

        public bool EnableGCM { get; set; }

        public bool SendAppID { get; set; }

        public bool SendObjectScope { get; set; }

        public bool SendOrigin { get; set; }

        public bool SendSender { get; set; }

        public bool SendToDevelopment { get; set; }

        public bool SendTopicId { get; set; }

        public bool SendToProduction { get; set; }

        public bool SendWhen { get; set; }

        public PushSetting()
        {
            EnableAPNS = true;
            EnableGCM = true;
            SendAppID = true;
            SendObjectScope = true;
            SendOrigin = true;
            SendSender = true;
            SendToDevelopment = true;
            SendTopicId = true;
            SendToProduction = true;
            SendWhen = true;
        }

        public KiiPushMessage GetKiiPushMessage(KiiPushMessageData data, APNSSetting apns, GCMSetting gcm)
        {
            return KiiPushMessage.BuildWith(data)
                .EnableAPNS(EnableAPNS)
                    .EnableGCM(EnableGCM)
                    .SendAppID(SendAppID)
                    .SendObjectScope(SendObjectScope)
                    .SendOrigin(SendOrigin)
                    .SendSender(SendSender)
                    .SendToDevelopment(SendToDevelopment)
                    .SendTopicId(SendTopicId)
                    .SendToProduction(SendToProduction)
                    .SendWhen(SendWhen)
                    .WithAPNSMessage(apns.GetAPNSMessage())
                    .WithGCMMessage(gcm.GetGCMMessage())
                    .Build();
        }
    }

    class APNSSetting
    {
        public bool Enable { get; set; }

        public int ContentAvailable { get; set; }

        public string AlertBody { get; set; }

        public APNSSetting()
        {
            Enable = true;
            ContentAvailable = 0;
            AlertBody = "";
        }

        public APNSMessage GetAPNSMessage()
        {
            if (Enable == true)
            {
                APNSMessage.Builder builder = APNSMessage.CreateBuilder()
                    .Enable(Enable)
                        .WithContentAvailable(ContentAvailable);
                if (AlertBody != "" && AlertBody != null)
                {
                    builder = builder.WithAlertBody(AlertBody);
                }
                return builder.Build();
            }
            else
            {
                return APNSMessage.CreateBuilder()
                    .Enable(Enable)
                        .Build();
            }
        }
    }

    class GCMSetting
    {
        public bool Enable { get; set; }

        public GCMSetting()
        {
            Enable = true;
        }

        public GCMMessage GetGCMMessage()
        {
            return GCMMessage.CreateBuilder()
                .Enable(Enable)
                    .Build();
        }
    }

}