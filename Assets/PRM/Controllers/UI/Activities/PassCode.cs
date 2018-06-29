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


public class PassCode : UIModule<PassCode>, IActivity {
	Text textCode;
	Text passCode;
	Text errorLabel;
	Text textButton;
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
			return DataPaths.ACTIVITY_PASSCODE_LAYOUT_PATH;
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
			return (int)ZIndexPlacement.TOP;
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
		String user;
		PlayerPrefs.DeleteAll();
		textCode = this.FindAndResolveComponent<Text>("ValueUs<Text>", DisplayObject);
		textCode.text = "";
		user = textCode.text;
		user.ToLower ();
		passCode = this.FindAndResolveComponent<Text>("ValuePass<Text>", DisplayObject);
		passCode.text = "";
		errorLabel = this.FindAndResolveComponent<Text>("Error<Text>", DisplayObject);
		textButton = this.FindAndResolveComponent<Text>("TextButton<Text>", DisplayObject);
		textButton.text = "ENTRAR";
		
		Button continueButton = this.FindAndResolveComponent<Button>("Login<Button>", DisplayObject);
		continueButton.onClick.AddListener (delegate { 
			// NotificationSystem.Instance.NotifyProgress("Ingresando...");
			textButton.text = "Ingresando...";
			String api_url = DataPaths.WS_USERS_URL+"?username="+textCode.text+"&password="+passCode.text;
			Debug.Log(api_url);
			NetClient.OnWebRequestDoneDelegate onWebRequestDone = delegate (string webResponse) {
				Debug.Log(webResponse);

				try
				{
					JsonData rawData = JsonMapper.ToObject(webResponse);

					if((string)rawData["status"] == "ok"){
						// NotificationSystem.Instance.Terminate();
						PlayerPrefs.SetInt("first_time", 1);
						MainMenu.Instance.setTitle (rawData["data"]["usuario"].Keys.Contains("nombre") ? (string) rawData ["data"]["usuario"]["nombre"] : (string) rawData ["data"]["usuario"]["username"], true);
						PlayerPrefs.SetString("username",textCode.text);
						PlayerPrefs.SetString("password",passCode.text);
						PlayerPrefs.SetInt("user_id",(int) rawData ["data"]["usuario"]["id"]);
						String code = (string) rawData ["data"]["usuario"]["id"].ToString();
						PlayerPrefs.SetString("user_code", code);
						PlayerPrefs.SetString("UserRole",(string) rawData ["data"]["usuario"]["roles"][0]); 

						PlayerPrefs.SetString("UserName", rawData["data"]["usuario"].Keys.Contains("nombre") ? (string) rawData ["data"]["usuario"]["nombre"] : (string) rawData ["data"]["usuario"]["username"]);

							SessionManager.Instance.Initialize ();
							// NotificationAlert.Instance.Initialize ();
							MainMenu.Instance.Initialize ();
							MainFooter.Instance.Initialize ();
							Terminate();

						ExecutiveHome.Instance.Initialize ();
						SessionManager.Instance.OpenActivity(ExecutiveHome.Instance);
					
					}else{
						// NotificationSystem.Instance.NotifyMessage(DataMessages.SERVER_LOGIN_FAIL);
						textButton.text = "ENTRAR";
						errorLabel.text = DataMessages.SERVER_LOGIN_FAIL;
					}

					}catch (JsonException e) {
					Debug.LogError("JSON Exception: " + e.Message + "\n" + e.StackTrace);
					throw e;
				}
			};



			NetClient.OnWebRequestFailDelegate onWebRequestFail = delegate
			{
				// NotificationSystem.Instance.PromptAction(DataMessages.SERVER_RESPONSE_FAIL, delegate {  }, delegate { NotificationSystem.Instance.Terminate(); });
				NotificationSystem.Instance.NotifyMessage("El usuario con el que intenta ingresar, no contiene un nombre de usuario");

			};

			//REVISA WEBSERVICE;

			StartCoroutine(NetClient.Instance.MakeWebRequest(api_url, null, onWebRequestDone, onWebRequestFail));

		});


	}

	protected override void ProcessTermination()
	{
		Debug.Log("PassCodeActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}

}
