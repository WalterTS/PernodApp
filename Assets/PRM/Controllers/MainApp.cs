using UnityEngine;
using System.Collections;

using GLIB.Utils;



public class MainApp : MonoBehaviour {

	// Use this for initialization
	void Start () { 	

		NotificationSystem.Instance.TemplatePath = DataPaths.FRAGMENT_GRIMOIRECUSTOMNOTIFICATION_LAYOUT_PATH;

		Screen.fullScreen = false;
		Application.targetFrameRate = 60;

		int first_time = PlayerPrefs.GetInt("first_time", 0);
		if (first_time == 1) {

			// NotificationAlert.Instance.Initialize ();

			SessionManager.Instance.Initialize ();
			MainMenu.Instance.Initialize ();
			// MainFooter.Instance.Initialize (); 
			SessionManager.Instance.OpenActivity(ExecutiveHome.Instance);

		} else {
			SessionManager.Instance.Initialize ();
			SessionManager.Instance.OpenActivity (PassCode.Instance);
		}


	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.C))
		{
			PlayerPrefs.DeleteAll();
			Debug.LogError("PLAYER PREFS CLEARED");
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SessionManager.Instance.GoToPreviousActivity();
		}

	}
}
