using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{

    [SerializeField]
    private GameObject sun;
    [SerializeField]
    private GameObject planet1;

    private Rigidbody rb;
    private bool onPlanet;

    // Use this for initialization
    void Start() {
        onPlanet = false;
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        onPlanet = false;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //transform.position.Set(transform.position.x, transform.position.y, 0);
        //rb.velocity.Set(rb.velocity.x, rb.velocity.y, 0);
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
