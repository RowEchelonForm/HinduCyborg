using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [System.Serializable]
    public struct gravityTransform
    {
        public float gravity;
        public Transform other;
        public float maxDistance;
    };

    [System.Serializable]
    struct gravityRigid
    {
        public float gravity;
        public Rigidbody other;
        public float maxDistance;
    };


    [SerializeField]
    private gravityTransform[] staticGravity;
    [SerializeField]
    private gravityRigid[] nonStaticGravity;
    [SerializeField]
    private bool divByDistance;
    [SerializeField]
    private bool divByDistance2;

    [SerializeField]
    private double bufferTime = 0.01;
    [SerializeField]
    private int bufferParts = 40;

    [SerializeField]
    private Text planetName;
    [SerializeField]
    private Text planetDesc;
    [SerializeField]
    private Text landText;
    [SerializeField]
    private GameObject planetPanel;
    [SerializeField]
    private GameObject landPanel;
    [SerializeField]
    private Button yes;
    [SerializeField]
    private Button no;
    [SerializeField]
    private AudioClip thrusterSound;
    [SerializeField] [Range(0f, 1f)]
    private float thrusterVolume = 0.3f;

    [SerializeField]
    private float landingSpeed = 0.66f;
    [SerializeField]
    [TextArea(3, 10)]
    private string landingFailmsg = "\nYou landed too hard.\nNow you are stuck here with the broken ship";
    [SerializeField]
    [TextArea(3, 10)]
    private string meteorCrash = "Asteroid field filled with rubble...\nand the wreck of your spaceship.\nHopefully you eventually land somewhere ending the eternal floating in space.";
    [SerializeField]
    private GameObject arrow;

    private Rigidbody rb;
    private bool onPlanet;
    private string level;
    private bool inGui;
    private int shownGui;
    private AudioSource thrusterAudioSource;
    private bool justLoaded; // true for a couple frames after loading the scene


    private double[] speedParts;
    private double[] speedTimes;
    private double speedBuffer;
    private double timeBuffer;
    private Vector3 lastPosition;

    // Use this for initialization
    void Start()
    {
        yes.onClick.AddListener(yesClick);
        no.onClick.AddListener(exitGui);
        justLoaded = true;
        StartCoroutine(afterStartBuffer());
    }

    void yesClick()
    {
        exitGui();
        SaveLoad.Save(LevelManager.currentLevelName);
        SaveLoad.SaveToFile("space");
        if (shownGui == 5) // quit gui
        {
            LevelManager.loadStart();
        }
        else
        {
            LevelManager.loadLevel(level);
        }
    }

    void exitGui()
    {
        planetPanel.SetActive(false);
        landPanel.SetActive(false);
        arrow.SetActive(true);
        inGui = false;
    }

    public void Reload()
    {
        Awake();
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        onPlanet = false;
        inGui = false;
        speedParts = new double[bufferParts];
        speedTimes = new double[bufferParts];
        for (int i = 0; i < speedParts.Length; i++)
        {
            speedParts[i] = 0;
            speedTimes[i] = bufferTime;
        }
        lastPosition = rb.position;
        speedBuffer = 0;
        timeBuffer = 0;
        planetPanel.SetActive(false);
        landPanel.SetActive(false);
    }

    public float avgSpeed()
    {
        double avgSpeed = 0;
        foreach (double i in speedParts)
        {
            avgSpeed += i;
        }
        double div = 0;
        foreach (double i in speedTimes)
        {
            div += i;
        }
        //Debug.Log((avgSpeed / div));
        return (float)(avgSpeed / div);
    }

    private void gravityTick()
    {
        foreach (gravityTransform i in staticGravity)
        {
            Vector3 angleVector = i.other.position - rb.position;
            float distance = angleVector.magnitude;
            if (distance < i.maxDistance)
            {
                angleVector.z = 0;
                angleVector.Normalize();
                angleVector = angleVector * i.gravity * Time.deltaTime;
                if (divByDistance && divByDistance2)
                {
                    rb.AddForce(angleVector / (distance * distance));
                }
                else if (divByDistance || divByDistance2)
                {
                    rb.AddForce(angleVector / distance);
                }
                else
                {
                    rb.AddForce(angleVector);
                }
            }
        }
        foreach (gravityRigid i in nonStaticGravity)
        {
            Vector3 angleVector = i.other.position - rb.position;
            float distance = angleVector.magnitude;
            if (distance < i.maxDistance)
            {
                angleVector.z = 0;
                angleVector.Normalize();
                angleVector = angleVector * i.gravity * Time.deltaTime;
                if (divByDistance && divByDistance2)
                {
                    rb.AddForce(angleVector / (distance * distance));
                }
                else if (divByDistance || divByDistance2)
                {
                    rb.AddForce(angleVector / distance);
                }
                else
                {
                    rb.AddForce(angleVector);
                }
            }
        }
    }
    
    private void playThrusterSound()
    {
        if (!thrusterAudioSource)
        {
            thrusterAudioSource = SoundFXPlayer.instance.playClipContinuosly(thrusterSound, thrusterVolume);
        }
    }
    
    private void stopPlayingThrusterSound()
    {
        if (thrusterAudioSource)
        {
            SoundFXPlayer.instance.recycleAudioSource(thrusterAudioSource, 0.3f);
            thrusterAudioSource = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        speedBuffer += (rb.position - lastPosition).magnitude;
        lastPosition = rb.position;
        timeBuffer += Time.deltaTime;
        if (timeBuffer >= bufferTime)
        {
            for (int i = 1; i < speedParts.Length; i++)
            {
                speedParts[i - 1] = speedParts[i];
                speedTimes[i - 1] = speedTimes[i];
            }
            speedParts[speedParts.Length - 1] = speedBuffer;
            speedTimes[speedParts.Length - 1] = timeBuffer;
            speedBuffer = 0;
            timeBuffer = 0;
            avgSpeed();
        }

        if (!inGui)
        {
            if (v != 0)
            {
                rb.AddRelativeForce(0, v * Time.deltaTime * 90, 0);
                Destroy(GetComponent<FixedJoint>());
                onPlanet = false;
                playThrusterSound();
            }
            else
            {
                stopPlayingThrusterSound();
            }
            if (h != 0)
            {
                rb.AddTorque(0, 0, -h * Time.deltaTime);
            }
            gravityTick();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inGui = true;
                landText.text = "Quit?";
                landPanel.SetActive(true);
                arrow.SetActive(false);
                shownGui = 5;
            }
        }
        else
        {
            stopPlayingThrusterSound();
            if (Input.GetButton("Jump") && shownGui == 1)
            {
                planetPanel.SetActive(false);
                landPanel.SetActive(true);
                landText.text = "Land?";
                shownGui = 2;
            }
            else if (shownGui == 2)
            {
                if (h < 0)
                {
                    yesClick();
                }
                else if (h > 0)
                {
                    exitGui();
                }
            }
            else if (Input.GetButton("Jump") && shownGui == 3)
            {
                LevelManager.reloadCurrentLevel();
            }
            else if (shownGui == 4 && Input.GetButton("Jump"))
            {
                exitGui();
            }
            if (shownGui == 5)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || h > 0)
                {
                    exitGui();
                    landText.text = "Land?";
                }
                else if (h < 0)
                {
                    yesClick();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        Planet planet = col.gameObject.GetComponent<Planet>();
        if (planet != null && !onPlanet)
        {
            if (inGui)
            {
                exitGui();
            }
            stopPlayingThrusterSound();
            if (avgSpeed() < landingSpeed)
            {
                //landed on something
                if (planet.hasNoLevel)
                {
                    shownGui = 4;
                    gameObject.AddComponent<FixedJoint>();
                    gameObject.GetComponent<FixedJoint>().connectedBody = col.rigidbody;
                }
                else if (!planet.sun)
                {
                    gameObject.AddComponent<FixedJoint>();
                    gameObject.GetComponent<FixedJoint>().connectedBody = col.rigidbody;
                    shownGui = 1;
                    level = planet.level;
                }
                else
                {
                    //some nice game over?
                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    shownGui = 3;
                }
                onPlanet = true;
                inGui = true;
                planetName.text = planet.planetName;
                planetDesc.text = planet.description;
                planetPanel.SetActive(true);
                arrow.SetActive(false);
                if (justLoaded)
                {
                    exitGui();
                }
            }
            else
            {
                if (!planet.sun)
                {
                    //going too fast -> take dmg or something
                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    shownGui = 3;
                    onPlanet = true;
                    inGui = true;
                    planetName.text = planet.planetName;
                    planetDesc.text = landingFailmsg;
                    planetPanel.SetActive(true);
                    arrow.SetActive(false);
                }
                else
                {
                    //gg, you made it to afterlife
                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    shownGui = 3;
                    onPlanet = true;
                    inGui = true;
                    planetName.text = planet.planetName;
                    planetDesc.text = planet.description;
                    planetPanel.SetActive(true);
                    arrow.SetActive(false);
                }
                rb.velocity = new Vector3(0, 0, 0);
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else if (avgSpeed() > landingSpeed)
        {
            //gg, you made it to afterlife
            stopPlayingThrusterSound();
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            shownGui = 3;
            onPlanet = true;
            inGui = true;
            planetName.text = "Asteroid Field";
            planetDesc.text = meteorCrash;
            planetPanel.SetActive(true);
            arrow.SetActive(false);
        }
    }
    
    // Call at start.
    // Sets 'justLoaded' to true, and after two full frames, sets it to to false.
    private IEnumerator afterStartBuffer()
    {
        justLoaded = true;
        yield return null;
        yield return null;
        justLoaded = false;
    }
}
