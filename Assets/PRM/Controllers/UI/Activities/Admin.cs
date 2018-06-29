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

public class Admin : UIModule<Admin>, IActivity
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
			return DataPaths.ACTIVITY_ADMIN_LAYOUT_PATH;
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
		Debug.Log("AdminActivity => Initializing");
	}

	protected override void ProcessInitialization()
	{
		Button usersButton = this.FindAndResolveComponent<Button> ("Users<Button>", DisplayObject);
		

		usersButton.onClick.AddListener (delegate {			
			SessionManager.Instance.OpenActivity(Users.Instance);
			Terminate();
		});
		
	}

	protected override void ProcessTermination()
	{
		Debug.Log("AdminActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}


}

