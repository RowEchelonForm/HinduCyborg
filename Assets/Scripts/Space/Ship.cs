using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{

    [SerializeField]
    private GameObject sun;

    [SerializeField]
    private double bufferTime = 0.01;
    [SerializeField]
    private int bufferParts = 40;

    [SerializeField]
    private Text planetName;
    [SerializeField]
    private Text planetDesc;
    [SerializeField]
    private GameObject planetPanel;
    [SerializeField]
    private GameObject landPanel;
    [SerializeField]
    private Button yes;
    [SerializeField]
    private Button no;

    private Rigidbody rb;
    private bool onPlanet;
    private string level;
    private bool inGui;
    private int shownGui;


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
    }

    void yesClick()
    {
        exitGui();
        LevelManager.loadLevel(level);
    }

    void exitGui()
    {
        planetPanel.SetActive(false);
        landPanel.SetActive(false);
        inGui = false;
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
            }
            if (h != 0)
            {
                rb.AddTorque(0, 0, -h * Time.deltaTime);
            }
        }
        else
        {
            if (Input.GetButton("Jump") && shownGui == 1)
            {
                planetPanel.SetActive(false);
                landPanel.SetActive(true);
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
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        Planet planet = col.gameObject.GetComponent<Planet>();
        if (planet != null && !onPlanet)
        {
            if (avgSpeed() < 0.5)
            {
                //landed on something
                if (!planet.sun)
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
            }
            else
            {
                if (!planet.sun)
                {
                    //going too fast -> take dmg or something
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
                }
            }
        }
    }
}
