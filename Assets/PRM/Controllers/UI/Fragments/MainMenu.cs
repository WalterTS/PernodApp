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


public class MainMenu : UIModule<MainMenu> {
	
	GameObject backbutton;

	public int readNotifications = 0;

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
			return DataPaths.FRAGMENT_MAINMENU_LAYOUT_PATH;
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
			return new Transition(Transition.InOutAnimations.NONE);
		}
	}

	protected override void ProcessInitialization()
	{
		Debug.Log("MainMenu => Started");

		backbutton = this.Find ("Back<Button>", DisplayObject);
		backbutton.SetActive (true);

		Button profileButton = this.FindAndResolveComponent<Button>("Profile<Wrapper>", DisplayObject);
		profileButton.onClick.AddListener(delegate { 
			ProfileMenu.Instance.Initialize();
		});
			
		Button backButton = this.FindAndResolveComponent<Button> ("Back<Button>", DisplayObject);
		backButton.onClick.AddListener (delegate { 
			if (ExecutiveHome.Instance.isRunning) {

			}else{
				SessionManager.Instance.GoToPreviousActivity ();
				SideMenu.Instance.Terminate ();
				ProfileMenu.Instance.Terminate ();
			}
			});
		
			
		Button menuButton = this.FindAndResolveComponent<Button> ("Menu<Button>", DisplayObject);
		menuButton.onClick.AddListener(delegate {
			SideMenu.Instance.Initialize();
		});
		Debug.Log (menuButton);

		Text titleText = this.FindAndResolveComponent<Text> ("Title<Text>", DisplayObject);
		titleText.text = PlayerPrefs.GetString("UserName");
		
	}

	public void setTitle(String title, bool first_time = false){
		if(first_time){
			Text titleText = this.FindAndResolveComponent<Text> ("Title<Text>", DisplayObject);
			titleText.text = title;
		}
	}



	public void setIcon(String iconName){

	}


	protected override void ProcessUpdate()
	{

	}

	protected override void ProcessTermination()
	{
		Debug.Log("MainMenuActicity => Terminated");
	}

}
