using UnityEngine;
using System.Collections;

public class DebugLogScript : MonoBehaviour 
{
    public string DebugString;

    void Start () 
    {
        Debug.Log(DebugString);
	}
}
