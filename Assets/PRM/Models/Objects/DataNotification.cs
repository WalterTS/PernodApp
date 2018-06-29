using SQLite4Unity3d;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace PRM.Models.Objects{
	public class DataNotification
	{
		public int id { get; set; }
		public string title { get; set; }
	    public string content { get; set; }
	    public string path { get; set; }
	}
}