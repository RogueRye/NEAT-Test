using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {


    Transform point1;
    Transform point2;
    LineRenderer lineR;

	// Use this for initialization
	void Start () {
        point1 = transform.Find("Point1");
        point2 = transform.Find("Point2");
        lineR = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        lineR.SetPosition(0, point1.position);
        lineR.SetPosition(1, point2.position);
	}
}
