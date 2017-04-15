using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{

    [SerializeField]
    private GameObject sun;
    [SerializeField]
    private GameObject planet1;
    [SerializeField]
    private double bufferTime = 0.1;
    [SerializeField]
    private int bufferParts = 10;

    private Rigidbody rb;
    private bool onPlanet;
    private double[] speedParts;
    private double[] speedTimes;
    private double speedBuffer;
    private double timeBuffer;
    private Vector3 lastPosition;

    // Use this for initialization
    void Start() {
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        onPlanet = false;
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
        Debug.Log((avgSpeed / div));
        return (float)(avgSpeed/div);
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //transform.position.Set(transform.position.x, transform.position.y, 0);
        //rb.velocity.Set(rb.velocity.x, rb.velocity.y, 0);
        speedBuffer += (rb.position-lastPosition).magnitude;
        lastPosition = rb.position;
        timeBuffer += Time.deltaTime;
        if (timeBuffer >= bufferTime)
        {
            for (int i = 1; i < speedParts.Length; i++)
            {
                speedParts[i-1] = speedParts[i];
                speedTimes[i-1] = speedTimes[i];
            }
            speedParts[speedParts.Length-1] = speedBuffer;
            speedTimes[speedParts.Length-1] = timeBuffer;
            speedBuffer = 0;
            timeBuffer = 0;
            avgSpeed();
        }
        if  (v != 0)
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

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.Equals(sun))
        {
            //grats on landing on sun
            LevelManager.reloadCurrentLevel();
        }
        else if (col.gameObject.Equals(planet1) && !onPlanet)
        {
            if (avgSpeed() < 0.5) {
                //landed on planet
                gameObject.AddComponent<FixedJoint>();
                gameObject.GetComponent<FixedJoint>().connectedBody = col.rigidbody;
                onPlanet = true;
                LevelManager.loadLevel("Planet01"); // TODO make an another script for the planet that has the level's name etc.
            }
            else
            {
                //crashlanding
            }
        }
    }
}
