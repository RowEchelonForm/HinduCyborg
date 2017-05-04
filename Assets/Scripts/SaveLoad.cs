using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

    public static GameObject sceneRoot;

	public static void Save(string levelName, GameObject root)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + levelName + ".gd");
        bf.Serialize(file, root);
        file.Close();
    }

    public static void Load(string levelName, GameObject root)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + levelName + ".gd", FileMode.Open);
        sceneRoot = (GameObject)bf.Deserialize(file);
        file.Close();
        Instantiate(sceneRoot);
    }
}
