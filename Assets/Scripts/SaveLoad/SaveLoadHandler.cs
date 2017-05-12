using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//SaveLoad.Save(LevelManager.currentLevelName);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveLoad.Load();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
			SaveLoad.Save(LevelManager.currentLevelName);
            SaveLoad.SaveToFile("test");
        }


        if (Input.GetKeyDown(KeyCode.Y))
        {
            SaveLoad.LoadFromFile("test");
            SaveLoad.Load();
        }
    }
}
