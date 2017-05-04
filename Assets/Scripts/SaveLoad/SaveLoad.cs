using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour {

    public static SaveLoad instance = null; // singleton

    public static void Save(string levelName, GameObject root)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + levelName + ".gd");
        bf.Serialize(file, root);
        file.Close();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    public static void Load(string levelName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + levelName + ".gd", FileMode.Open);
        Instantiate((GameObject)bf.Deserialize(file));
        file.Close();
    }
}
