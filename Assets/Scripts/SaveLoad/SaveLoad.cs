using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Anima2D;

[System.Serializable]
public class ObjectStats
{
    public string name;

    public bool active;

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
public class PlayerStats : ObjectStats
{
	public List<string> enabledAbilities;

	public PlayerStats(ObjectStats objStats)
	{
		name = objStats.name;
		active = objStats.active;
		posX = objStats.posX;
		posY = objStats.posY;
		posZ = objStats.posZ;
		rotX = objStats.rotX;
		rotY = objStats.rotY;
		rotZ = objStats.rotZ;
		rotW = objStats.rotW;
	}
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


    public static List<GameObject> FindAllObjects()
    {
        List<GameObject> result = new List<GameObject>();
        List<Transform> transforms = new List<Transform>();
        var s = SceneManager.GetActiveScene();
        if (s.isLoaded)
        {
            var allGameObjects = s.GetRootGameObjects();
            foreach (GameObject go in allGameObjects)
            {
                transforms.AddRange(go.GetComponentsInChildren<Transform>(true));
            }
            foreach (Transform tr in transforms)
            {
                if (tr.parent != null)
                {
                    if (!tr.name.Contains("Bone") && !tr.name.Contains("bone") &&
                        !tr.parent.name.Contains("Bone") && !tr.parent.name.Contains("bone") &&
                        !tr.name.Contains("Sprite") && !tr.name.Contains("sprite") &&
                        !tr.parent.name.Contains("Sprite") && !tr.parent.name.Contains("sprite"))
                    {
                        bool amIGettingReallyAnnoyedWithThisShit =
                            tr.parent || tr.GetComponentInChildren<Sprite>() || (Anima2D.Bone2D)null;
                        bool yesReallyAnnoyed = tr.name.Contains("srpite");
                        //Debug.Log(tr.name);
                        if (!tr.gameObject.GetComponent<Bone2D>() &&
                            !tr.gameObject.GetComponent<IkLimb2D>() &&
                            !tr.gameObject.GetComponent<IkCCD2D>() &&
                            !tr.gameObject.GetComponent<Animator>() &&
                            !tr.gameObject.GetComponent<SpriteMeshInstance>() &&
                            !tr.gameObject.GetComponent<SkinnedMeshRenderer>())
                        {
                            result.Add(tr.gameObject);
                            //if (yesReallyAnnoyed); Debug.LogError("amIGettingReallyAnnoyedWithThisShit = " + amIGettingReallyAnnoyedWithThisShit);
                        }
                        //else; Debug.LogWarning("only 3h extra work");
                    }
                }
                else
                {
                    result.Add(tr.gameObject);
                }
            }
        }
        return result;
    }

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
        List<GameObject> obj = FindAllObjects();
        //GameObject[] obj = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject o in obj)
        {
            //and even with that excessive object filtering this is needed
            if (o.transform.root.name == "Player" && o.name != "Player")
            {
                continue;
            }
            ObjectStats stats = new ObjectStats();
            stats.name = o.name;

            stats.active = o.activeSelf;

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
            }

			if (o.tag == "Player")
            {
            	PlayerStats playerStats = SavePlayerStats(stats, o);
				game.sceneObjects.Add(playerStats);
            }
            else
            {
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
    }

    //only set objects
    public static void Reload()
    {
        Time.timeScale = 0;
        List<ObjectStats> objs;
        if (game.scene == "Space")
        {
            objs = game.spaceObjects;
        }
        else
        {
            objs = game.sceneObjects;
        }

		List<GameObject> allObjs = FindAllObjects();
        foreach (ObjectStats obj in objs)
        {
            //GameObject real = GameObject.Find(obj.name);
            GameObject real = null;
            foreach(GameObject go in allObjs) // This will ensure the same order for the objects
            {
            	if (go.name == obj.name)
            	{
            		real = go;
            		allObjs.Remove(go);
            		break;
            	}
            }
            if (real)
            {
                real.SetActive(obj.active);
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
                    if (real.tag == "Player")
                    {
                    	LoadPlayerStats((PlayerStats)obj, real);
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



    // Returns a PlayerStats object containing the extra stats that the player has. 
    // The regular ObjectStats should be in 'stats' already.
    private static PlayerStats SavePlayerStats(ObjectStats stats, GameObject player)
    {
		PlayerStats plStats = new PlayerStats(stats);
		plStats.enabledAbilities = new List<string>();
    	PlayerAbilityManager abilityManager = player.GetComponent<PlayerAbilityManager>();
		if (abilityManager != null)
    	{
    		plStats.enabledAbilities = abilityManager.getEnabledAbilities();
    	}
    	return plStats;
    }

	// Returns a PlayerStats object containing the extra stats that the player has. 
    // The regular ObjectStats should be in 'stats' already.
    private static void LoadPlayerStats(PlayerStats stats, GameObject player)
    {
    	PlayerAbilityManager abilityManager = player.GetComponent<PlayerAbilityManager>();
		if (abilityManager != null)
    	{
    		for (int i = 0; i < stats.enabledAbilities.Count; ++i)
    		{
				abilityManager.enableAbility( stats.enabledAbilities[i] );
    		}
    	}
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
        }
    }

    void sceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
    	Reload();
    }

    void OnEnable()
    {
		SceneManager.sceneLoaded += sceneWasLoaded;
    }

    void OnDisable()
    {
		SceneManager.sceneLoaded -= sceneWasLoaded;
    }
    
}
