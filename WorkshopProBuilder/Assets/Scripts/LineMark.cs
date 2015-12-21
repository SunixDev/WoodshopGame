﻿using UnityEngine;
using System.Collections;

public class LineMark : MonoBehaviour 
{
    public LineRenderer line;
    public Transform StartPoint;
    public Transform EndPoint;
    public bool GoodLineMark { get; set; }
	
	void Update () 
    {
        if (line != null)
        {
            line.SetPosition(0, StartPoint.position);
            line.SetPosition(1, EndPoint.position);
        }
	}
}
