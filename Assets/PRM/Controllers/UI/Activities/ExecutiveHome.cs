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

public class ExecutiveHome : UIModule<ExecutiveHome>, IActivity {
	Text notificationsLegend;
	String UserRole;
	
	public bool isActivityReady
	{
		get
		{
			if (UIModuleCurrentStatus == UIModule<ExecutiveHome>.UIModuleStatusModes.STAND_BY)
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
			return DataPaths.ACTIVITY_EXECUTIVEHOME_LAYOUT_PATH;
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

		Debug.Log ("Init HomeActivity");
		SessionManager.Instance.SetHomeAsRoot();				

		if (Application.isMobilePlatform)
			KiiPushService.Instance.Initialize();

		//Desactivo todos los botones
		UserRole = PlayerPrefs.GetString("UserRole");
		
		Button newPlanButton = this.FindAndResolveComponent<Button> ("NewPlan<Button>", DisplayObject);
		newPlanButton.gameObject.SetActive (false);

		Button planButton = this.FindAndResolveComponent<Button> ("Plan<Button>", DisplayObject);
		planButton.gameObject.SetActive (false);

		Button projectButton = this.FindAndResolveComponent<Button> ("Project<Button>", DisplayObject);
		projectButton.gameObject.SetActive (false);

		Button newProjectButton = this.FindAndResolveComponent<Button> ("NewProject<Button>", DisplayObject);
		newProjectButton.gameObject.SetActive (false);

		Button reportButton = this.FindAndResolveComponent<Button> ("Report<Button>", DisplayObject);
		reportButton.gameObject.SetActive (false);

		Button adminButton = this.FindAndResolveComponent<Button> ("Admin<Button>", DisplayObject);
		adminButton.gameObject.SetActive (false);

		Button usersButton = this.FindAndResolveComponent<Button> ("Users<Button>", DisplayObject);
		usersButton.gameObject.SetActive (false);

		
		//Administración de botones por roles
		switch(UserRole){
		case "ROLE_SUPER_ADMIN":
			newPlanButton.gameObject.SetActive (true);
			planButton.gameObject.SetActive (true);
			projectButton.gameObject.SetActive (true);
			newProjectButton.gameObject.SetActive (true);
			usersButton.gameObject.SetActive (true);
			break;
		case "ROLE_USER_GERENTE":
			newPlanButton.gameObject.SetActive (true);
			planButton.gameObject.SetActive (true);
			break;

		case "ROLE_USER_KAM":
			newProjectButton.gameObject.SetActive (true);
			break;

		case "ROLE_USER_EJECUTIVO":
			newProjectButton.gameObject.SetActive (true);
			break;

		case "ROLE_USER_PRODUCTOR":
			projectButton.gameObject.SetActive (true);
			break;

		case "ROLE_USER_CUENTA":
			projectButton.gameObject.SetActive (true);
			break;

		case "ROLE_USER_SUPERVISOR":
			projectButton.gameObject.SetActive (true);
			break;

		}

		
		newProjectButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(NewActivacion.Instance);
			Terminate();
		});

		newPlanButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(NewProject.Instance);
			Terminate();
		});

		planButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(Plans.Instance);
			Terminate();
		});

		projectButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(Projects.Instance);
			Terminate();
		});

		Button calendarButton = this.FindAndResolveComponent<Button> ("Calendar<Button>", DisplayObject);
		calendarButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(Calendar.Instance);
			Terminate();
		});

		adminButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(Admin.Instance);
			Terminate();
		});

		usersButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(Users.Instance);
			Terminate();
		});



		
		

	}

	protected override void ProcessTermination()
	{
		Debug.Log("ExecutiveHomeActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}
}
