using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class ObjectStats
{
    public string name;

    //vector3
    public float posX;
    public float posY;
    public float posZ;
    
    //quad
    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;
}

[System.Serializable]
public class Game
{
    public string scene = "Space";
    
    public List<ObjectStats> sceneObjects = new List<ObjectStats>();

    public List<ObjectStats> spaceObjects = new List<ObjectStats>();
}

public class SaveLoad : MonoBehaviour {

    public static SaveLoad instance = null; // singleton

    public static Game game = null;

    public static bool load = false;

    //save scene
    public static void Save(string scene)
    {
        Time.timeScale = 0;
        bool space = (scene == "Space");
        game.scene = scene;
        if (space)
        {
            game.spaceObjects.Clear();
        }
        else
        {
            game.sceneObjects.Clear();
        }
        GameObject[] obj = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject o in obj)
        {
            if (o.transform.root.name == "Player" && o.name != "Player")
            {
                continue;
            }
            ObjectStats stats = new ObjectStats();

            stats.name = o.name;

            stats.posX = o.transform.position.x;
            stats.posY = o.transform.position.y;
            stats.posZ = o.transform.position.z;

            stats.rotX = o.transform.rotation.x;
            stats.rotY = o.transform.rotation.y;
            stats.rotZ = o.transform.rotation.z;
            stats.rotW = o.transform.rotation.w;

            if (space)
            {
                Rigidbody rb = o.GetComponent<Rigidbody>();
                if (rb)
                {
                    stats.posX = rb.position.x;
                    stats.posY = rb.position.y;
                    stats.posZ = rb.position.z;

                    stats.rotX = rb.rotation.x;
                    stats.rotY = rb.rotation.y;
                    stats.rotZ = rb.rotation.z;
                    stats.rotW = rb.rotation.w;
                }
                game.spaceObjects.Add(stats);
            }
            else
            {
                Rigidbody2D rb = o.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    stats.posX = rb.position.x;
                    stats.posY = rb.position.y;
                    
                    stats.rotZ = rb.rotation;
                }
                game.sceneObjects.Add(stats);
            }
        }
        Time.timeScale = 1;
    }


    //load scene and then set objects
    public static void Load()
    {
        Time.timeScale = 0;
        LevelManager.loadLevel(game.scene);
        load = true;
    }

    //only set objects
    public static void Reload()
    {
        List<ObjectStats> objs;
        if (game.scene == "Space")
        {
            objs = game.spaceObjects;
        }
        else
        {
            objs = game.sceneObjects;
        }
        foreach (ObjectStats obj in objs)
        {
            GameObject real = GameObject.Find(obj.name);
            if (real)
            {
                real.transform.position = new Vector3(obj.posX, obj.posY, obj.posZ);
                real.transform.rotation = new Quaternion(obj.rotX, obj.rotY, obj.rotZ, obj.rotW);
                if (game.scene == "Space")
                {
                    Rigidbody rb = real.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.position = new Vector3(obj.posX, obj.posY, obj.posZ);
                        rb.rotation = new Quaternion(obj.rotX, obj.rotY, obj.rotZ, obj.rotW);
                        Ship ship = real.GetComponent<Ship>();
                        if (ship)
                        {
                            ship.Reload();
                        }
                    }
                }
                else
                {
                    Rigidbody2D rb = real.GetComponent<Rigidbody2D>();
                    if (rb)
                    {
                        rb.position = new Vector3(obj.posX, obj.posY, obj.posZ);
                        rb.rotation = obj.rotZ;
                    }
                }
            }
        }
        Time.timeScale = 1;
    }

    public static void SaveToFile(string filename)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + filename + ".gd");
        bf.Serialize(file, game);
        file.Close();
    }

    public static void LoadFromFile(string filename)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + filename + ".gd", FileMode.Open);
        game = (Game)bf.Deserialize(file);
        file.Close();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
            game = new Game();
        }
        else if (instance != null)
        {
            Destroy(gameObject);
            if (load)
            {
                Reload();
                load = false;
            }
        }
    }
    
}
