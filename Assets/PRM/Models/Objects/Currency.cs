using SQLite4Unity3d;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Caribeando.Models.Objects{
public class Currency
{
    public string pais { get; set; }
	public string nombre_moneda { get; set; }
	public string rate { get; set; }
	public string image { get; set; }
}
}