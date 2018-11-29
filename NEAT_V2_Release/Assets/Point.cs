using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {


    SpriteRenderer myRenderer;
	// Use this for initialization
	void Start () {
        myRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        myRenderer.color = Color.red;
    }

    private void OnMouseDrag()
    {
        var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        transform.position = mouseWorld;

    }

    private void OnMouseExit()
    {
        myRenderer.color = Color.white;
    }
}
