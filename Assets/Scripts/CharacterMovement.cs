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


    void Start()
    {
        findComponents();
    }

    // Update is called once per frame
    void Update() 
    {
        grounded = checkGroundedStatus();
        jump = checkJumpStatus();
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        //anim.SetFloat("Speed", Mathf.Abs(h));

        applyVelocity(horizontalInput);
        handleFlipping(horizontalInput);
        handleJumping();
    }

    private bool checkGroundedStatus()
    {
        // Only casting towards the 'Ground' layer
        return Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool checkJumpStatus()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            return true;
        }
        return false;
    }

    private void applyVelocity(float horizontalInput)
    {
        float curVelocityX = rb2d.velocity.x;
        rb2d.AddForce(Vector2.right * horizontalInput * moveForce);

        // clamp velocity to maxSpeed
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
    }

    private void handleFlipping(float horizontalInput)
    {
        if (horizontalInput > 0 && !facingRight)
        {
            flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            flip();
        }
    }

    private void flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void handleJumping()
    {
        if (jump)
        {
            //animation here
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
    }


    private void findComponents()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            Debug.LogError("Error: No Rigidbody2D found on the player from CharacterMovement script! Please attah it.");
        }

        if (groundCheck == null)
        {
            groundCheck = transform.FindChild("groundCheck");
            if (groundCheck == null)
            {
                Debug.LogError("CharacterMovement script can't find groundCheck child object.");
            }
        }
    }

}