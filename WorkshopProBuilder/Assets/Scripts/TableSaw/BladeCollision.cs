using UnityEngine;
using System.Collections;

public class BladeCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Blade collided with : " + col.collider.name);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), col.collider);
    }

    //void OnCollisionStay(Collision col)
    //{
    //    Debug.Log("Blade is still colliding with : " + col.collider.name);
    //}
}
