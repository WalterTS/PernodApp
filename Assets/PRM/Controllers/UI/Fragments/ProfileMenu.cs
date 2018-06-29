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

public class ProfileMenu : UIModule<ProfileMenu>
{

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
			return DataPaths.FRAGMENT_PROFILEMENU_LAYOUT_PATH;
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
			return new Transition(Transition.InOutAnimations.NONE);
		}
	}

	protected override void ProcessInitialization()
	{
		Button exitTrigger = this.FindAndResolveComponent<Button>("ExitTrigger<Button>", DisplayObject);
		exitTrigger.onClick.AddListener(delegate { Terminate(); });

		/*Button settingsButton = this.FindAndResolveComponent<Button> ("Settings<Button>", DisplayObject);
		settingsButton.onClick.AddListener (delegate {
			SessionManager.Instance.TryShuttingDownAllOpenedActivities();
			SessionManager.Instance.SetHomeAsRoot();					
			SessionManager.Instance.OpenActivity(ExecutiveHome.Instance);
			Terminate();
		});*/
			
		Button logoutButton = this.FindAndResolveComponent<Button> ("Logout<Button>", DisplayObject);
		logoutButton.onClick.AddListener (delegate {
			SessionManager.Instance.TryShuttingDownAllOpenedActivities();
			PlayerPrefs.SetInt("user_id", 0);
			PlayerPrefs.SetString("UserName", null);
			PlayerPrefs.DeleteAll();
			SessionManager.Instance.SetHomeAsRoot(true);			
			SessionManager.Instance.OpenActivity(PassCode.Instance);
			Terminate(); 
		});
	}

	protected override void ProcessTermination()
	{
		Debug.Log ("ProfileMenuActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}

}
