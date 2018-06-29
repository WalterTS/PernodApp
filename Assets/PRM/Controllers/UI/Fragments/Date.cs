using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using GLIB.Interface;
using GLIB.Extended;
using GLIB.Utils;
using System.IO;
using GLIB.Core;
using GLIB.Net;
using LitJson;

/*public class DateImage{
 * public string type {get;set;}
 * public string normal {get; set;}
 * public string hover {get; set;}
 * }*/

public class Date : MonoBehaviour {
	
	public string nombre;
	public DateTime fecha;

	public Date(string newnombre, DateTime newfecha )
	{
		nombre = newnombre;
		fecha = newfecha; 
	}

	/*[PrimaryKey, AutoIncrement]
	public int ID { get; set;}
	public string date_id { get; set;}
	public string date_name { get; set;}
	public string normal_image_url { get; set;}
	public string normal_image_fielpath { get; set;}
	public string hover_image_url { get; set;}
	public string hover_image_filepath { get; set;}
	public bool visible { get; set;}*/
}