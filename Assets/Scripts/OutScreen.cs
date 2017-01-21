using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutScreen : MonoBehaviour {
    private Transform myTransform;
	// Use this for initialization
	void Start () {
        myTransform = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(myTransform.position);
        Debug.Log(viewportPos);
        if (viewportPos.x < 0.01f)
        {
            myTransform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.99f, viewportPos.y, viewportPos.z));
        }
        if (viewportPos.x > 0.99f)
        {
            myTransform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.01f, viewportPos.y, viewportPos.z));
        }
        if (viewportPos.z < 28.29f)
        {
            myTransform.position = Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 54.79f));
        }
        if (viewportPos.z > 54.79f)
        {
            myTransform.position = Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 28.29f));
        }
    }
}
