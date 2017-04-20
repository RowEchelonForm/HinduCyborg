using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;


public class Arrow : MonoBehaviour {

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject arrow1;
    [SerializeField]
    private GameObject arrow2;
    [SerializeField]
    private float minDistance = 5;
    [SerializeField]
    private float maxDistance = 100;
    [SerializeField]
    private float distanceScale = 100;

    private Rigidbody playerRb;
    private Transform targetRb;

    private bool active;
    private float fix;

    // Use this for initialization
    void Start ()
    {
        arrow1.SetActive(true);
        arrow2.SetActive(true);
        active = true;
        fix = 0;
    }

    void Awake()
    {
        playerRb = player.GetComponent<Rigidbody>();
        targetRb = target.GetComponent<Transform>();
        arrow1.SetActive(true);
        arrow2.SetActive(true);
        active = true;
        fix = 0;
    }

    // Update is called once per frame
    void Update () {
        if (fix < 0.1)
        {
            fix += Time.deltaTime;
            return;
        }
        Vector3 angleVector = targetRb.position - playerRb.position;
        if (angleVector.magnitude < minDistance)
        {
            if (active)
            {
                arrow1.SetActive(false);
                arrow2.SetActive(false);
                active = false;
            }
        }
        else if (angleVector.magnitude < maxDistance)
        {
            if (!active)
            {
                arrow1.SetActive(true);
                arrow2.SetActive(true);
                active = true;
            }
            angleVector.z = 0;
            transform.localScale = new Vector3(0.22f, 0.35f, 0.35f) * (distanceScale + minDistance) /(distanceScale + angleVector.magnitude);
            angleVector.Normalize();
            float angle = Mathf.Atan2(angleVector.y, angleVector.x) * Mathf.Rad2Deg + 180;
            transform.localEulerAngles = new Vector3(0, 0, angle);
            transform.localPosition = angleVector * (Screen.height / 2 - 20);
        }
        else
        {
            if (active)
            {
                arrow1.SetActive(false);
                arrow2.SetActive(false);
                active = false;
            }
        }
    }
}
