using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{

    [SerializeField]
    private float rotation = 5;
    [SerializeField]
    private float selfRotation = 20;
    [SerializeField]
    private float minDistance = 5;
    [SerializeField]
    private float maxDistance = 6;
    [SerializeField]
    private float randomRotation = 0;
    [SerializeField]
    private float randomSelfRotation = 0;

    private Rigidbody rb;

    void Start()
    {
        rotation += randomRotation < 0 ? Random.Range(randomRotation, 0) : Random.Range(0, randomRotation);
        rotation /= (minDistance + maxDistance)/2;
        randomSelfRotation += randomSelfRotation < 0 ? Random.Range(randomSelfRotation, 0) : Random.Range(0, randomSelfRotation);
    }
    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update()
    {
        //transform.position.Set(transform.position.x, transform.position.y, 0);
        //rb.velocity.Set(rb.velocity.x, rb.velocity.y, 0);
        transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 0, 1), rotation * Time.deltaTime);
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * selfRotation));
        Vector3 normlizedPos = transform.position.normalized * Time.deltaTime;
        if (transform.position.magnitude < minDistance)
        {
            rb.AddForce(Mathf.Abs(normlizedPos.x) * (transform.position.x > 0 ? 1 : -1) , Mathf.Abs(normlizedPos.y) * (transform.position.y > 0 ? 1 : -1), 0);
        }
        else if (transform.position.magnitude > maxDistance)
        {
            rb.AddForce(Mathf.Abs(normlizedPos.x) * (transform.position.x > 0 ? -1 : 1), Mathf.Abs(normlizedPos.y) * (transform.position.y > 0 ? -1 : 1), 0);
        }
	}
}
