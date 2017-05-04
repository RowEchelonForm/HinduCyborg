using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SaveLoad.Save("2C", transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveLoad.Load("2C");
        }
	}
}
