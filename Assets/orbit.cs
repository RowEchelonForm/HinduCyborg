using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbit : MonoBehaviour {

    [SerializeField]
    private float rotation = 5;
    [SerializeField]
    private float selfRotation = 20;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position = new Vector3(transform.position.x+Time.deltaTime, 0, 0);
        transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 0, 1), rotation * Time.deltaTime);
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * selfRotation));
	}
}
