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

public class ReportManager : UIModule<ReportManager>, IActivity {
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
			return DataPaths.ACTIVITY_REPORTMANAGER_LAYOUT_PATH;
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
		Debug.Log("ReportManager");

	}

	protected override void ProcessTermination()
	{
		Debug.Log("HomeActivity => Terminated");
	}

	protected override void ProcessUpdate()
	{

	}
}
