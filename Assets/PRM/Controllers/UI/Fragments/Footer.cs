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

public class MainFooter : UIModule<MainFooter> {

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
            return DataPaths.FRAGMENT_FOOTER_LAYOUT_PATH;
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
        Debug.Log("Footer => Started");
    }

    protected override void ProcessUpdate()
    {

    }

    protected override void ProcessTermination()
    {
		Debug.Log("Footer => Terminated");
    }

}
