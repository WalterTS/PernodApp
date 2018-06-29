using SQLite4Unity3d;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Caribeando.Models.Database.Tables{
	public class Notification
	{

		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public string title { get; set; }
		public string resume { get; set; }
		public string content { get; set; }
		public string link { get; set; }
		public string image { get; set; }
		public string image_filepath { get; set; }
		public string timestamp { get; set; }



	}
}