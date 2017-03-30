using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField]
    private float moveForce = 365f;
    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
    private float jumpForce = 1000f;
    [SerializeField]
    private float airSpeedFactor = 0.3f;
    [SerializeField]
    private Transform groundCheck;

    private bool facingRight = true;
    private bool jump = false;

    private bool grounded = false;
    private Rigidbody2D rb2d;
    //private Animator anim;


    // Use this for initialization
    void Awake () 
    {
        //anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update () 
    {
    	// Only casting towards the 'Ground' layer
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        //anim.SetFloat("Speed", Mathf.Abs(h));

        float curVelocityX = rb2d.velocity.x;
        //Debug.Log(horizontalInput);
        rb2d.AddForce(Vector2.right * horizontalInput * moveForce);

        if (Mathf.Abs(curVelocityX) > maxSpeed)
        {
            if (!grounded)
            {
                rb2d.velocity = new Vector2(Mathf.Sign(curVelocityX) * maxSpeed * airSpeedFactor, rb2d.velocity.y);
            }
            else
            {
                rb2d.velocity = new Vector2(Mathf.Sign(curVelocityX) * maxSpeed, rb2d.velocity.y);
            }
        }

        if (horizontalInput > 0 && !facingRight)
        {
            flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            flip();
        }

        if (jump)
        {
            //animation here
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
    }


    void flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}