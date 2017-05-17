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
    public string sceneName;

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
	public List<string> enabledAbilities = new List<string>();
    public int health;

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
    
    // The name of the scene where a save was previously loaded.
    public string previouslyLoadedSaveScene = "";
    
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
                if (tr.gameObject.GetComponent<ShouldSave>())
                {
                    result.Add(tr.gameObject);
                    //Debug.Log(tr.name);
                }
                /*
                 * older implementation
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
                */
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
            //Debug.Log("saving space");
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
                //Debug.Log(o.name + " ignored");
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
            stats.sceneName = LevelManager.currentLevelName;

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
                if (o.tag == "Player")
                {
                    PlayerStats playerStats = SavePlayerStats(stats, o, scene);
                    game.sceneObjects.Add(playerStats);
                }
                else
                {
                    game.sceneObjects.Add(stats);
                }
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
        game.previouslyLoadedSaveScene = game.scene;
        game.scene = LevelManager.currentLevelName;
        Time.timeScale = 0;
        List<ObjectStats> objs;
        if (LevelManager.currentLevelName == "Space")
        {
            //Debug.Log("loading space");
            objs = game.spaceObjects;
        }
        else
        {
            objs = game.sceneObjects;
        }
        Debug.Log("Loading: " + game.scene + ". Number of objects to load: " + objs.Count);

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
                if (real.tag != "Player" && obj.sceneName != game.scene)
                {
                    // Don't load object if it wasn't in the same scene.
                    // Player is an exception.
                    continue;
                }
                if (real.tag != "Player" || (real.tag == "Player" && game.scene == "Space")) // non-space player loaded differently
                {
                    real.SetActive(obj.active);
                    real.transform.position = new Vector3(obj.posX, obj.posY, obj.posZ);
                    real.transform.rotation = new Quaternion(obj.rotX, obj.rotY, obj.rotZ, obj.rotW);
                }
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
                    if (rb && real.tag != "Player")
                    {
                        rb.position = new Vector3(obj.posX, obj.posY, obj.posZ);
                        rb.rotation = obj.rotZ;
                    }
                    else if (rb && real.tag == "Player")
                    {
                    	LoadPlayerStats((PlayerStats)obj, real, rb);
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
    private static PlayerStats SavePlayerStats(ObjectStats stats, GameObject player, string scene)
    {
		PlayerStats plStats = new PlayerStats(stats);
    	PlayerAbilityManager abilityManager = player.GetComponent<PlayerAbilityManager>();
		if (abilityManager != null)
    	{
    		plStats.enabledAbilities = abilityManager.getEnabledAbilities();
    	}
        PlayerHealth plHealth = player.GetComponent<PlayerHealth>();
        if (plHealth != null)
        {
            plStats.health = plHealth.getHealth();
        }
        plStats.sceneName = scene;
    	return plStats;
    }

	// Returns a PlayerStats object containing the extra stats that the player has. 
    // The regular ObjectStats should be in 'stats' already.
    private static void LoadPlayerStats(PlayerStats stats, GameObject player, Rigidbody2D rb)
    {
    	PlayerAbilityManager abilityManager = player.GetComponent<PlayerAbilityManager>();
		if (abilityManager != null)
    	{
    		for (int i = 0; i < stats.enabledAbilities.Count; ++i)
    		{
				abilityManager.enableAbility( stats.enabledAbilities[i] );
                Debug.Log("Player loading enabled abilities");
    		}
    	}
        PlayerHealth plHealth = player.GetComponent<PlayerHealth>();
        
        // Only load position, rotation and health if the scene of the same was same as the current one and
        // the scene where a save was previously loaded was the current one.
        if (stats.sceneName == game.scene && game.previouslyLoadedSaveScene == game.scene)
        {
            // Only load position and health if same level.
            rb.position = new Vector3(stats.posX, stats.posY, stats.posZ);
            rb.rotation = stats.rotZ;
            if (plHealth != null && stats.health > 0) // safety check
            {
                plHealth.setHealth(stats.health);
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
