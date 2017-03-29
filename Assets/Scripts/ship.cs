using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship : MonoBehaviour {

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
		
	}

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update ()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //transform.position.Set(transform.position.x, transform.position.y, 0);
        //rb.velocity.Set(rb.velocity.x, rb.velocity.y, 0);
        if  (v != 0)
        {
            rb.AddRelativeForce(0, v * Time.deltaTime * 90, 0);
        }
        if (h != 0)
        {
            rb.AddTorque(0, 0, -h * Time.deltaTime);
        }
    }
}
