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
    private double timePart = 0.1;

    private Rigidbody rb;
    private bool onPlanet;
    private double[] speedParts;
    private double[] speedTimes;
    private double speedBuffer;
    private double timeBuffer;
    private Vector3 lastPosition;

    // Use this for initialization
    void Start() {
        onPlanet = false;
        speedParts = new double[5] { 0, 0, 0, 0, 0 };
        speedTimes = new double[5] { timePart, timePart, timePart, timePart, timePart };
        speedBuffer = 0;
        timeBuffer = 0;
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        onPlanet = false;
    }

    public float avgSpeed()
    {
        double avgSpeed = speedParts[0] * (speedTimes[0] - timeBuffer) + speedParts[1] + speedParts[3] + speedParts[4] + speedBuffer;
        avgSpeed = avgSpeed / (speedTimes[0] + speedTimes[1] + speedTimes[2] + speedTimes[3] + speedTimes[4]);
        return (float)avgSpeed;
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
        if (timeBuffer >= timePart)
        {
            for (int i = 0; i < 4; i++)
            {
                speedParts[i] = speedParts[i + 1];
                speedTimes[i] = speedTimes[i + 1];
            }
            speedParts[4] = speedBuffer;
            speedTimes[4] = timeBuffer;
            speedBuffer = 0;
            timeBuffer = 0;
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
            print("schorhed by sun");
            LevelManager.reloadCurrentLevel();
        }
        else if (col.gameObject.Equals(planet1) && !onPlanet && rb.velocity.magnitude < 30)
        {
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = col.rigidbody;
            onPlanet = true;
            print("landed on planet");
            LevelManager.loadLevel("Planet01"); // TODO make an another script for the planet that has the level's name etc.
        }
    }
}
