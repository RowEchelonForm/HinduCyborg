using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private List<Transform> groundChecks;

    private bool facingRight = true;
    private bool jump = false;

    private bool grounded = false;
    private Rigidbody2D rb2d;
    private Transform cachedTransform;
    //private Animator anim;


    void Start()
    {
        findComponents();
    }

    // Update is called once per frame
    void Update() 
    {
        grounded = checkGroundedStatus();
        checkJumpFlag();
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
        // Only casting on the 'Ground' layer. Casts rays towards the all the GroundCheck tagged objects.
        for (int i = 0; i < groundChecks.Count; ++i)
        {
            if (Physics2D.Linecast(transform.position, groundChecks[i].position, 1 << LayerMask.NameToLayer("Ground")))
            {
                return true;
            }
        }
        return false;
    }

	private void checkJumpFlag()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Killed player");
        if ( collider.CompareTag("Kill") )
        {
            LevelManager.reloadCurrentLevel(); // TODO should load a checkpoint
        }
    }

    private void findComponents()
    {
        cachedTransform = transform;

        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            Debug.LogError("Error: No Rigidbody2D found on the player from CharacterMovement script! Please attah it.");
        }

        if (groundChecks.Count <= 0)
        {
            for (int i = 0; i < cachedTransform.childCount; ++i)
            {
                Transform child = cachedTransform.GetChild(i);
                if (child.CompareTag("GroundCheck"))
                {
                    groundChecks.Add(child);
                }
            }
            if (groundChecks.Count <= 0)
            {
                Debug.LogError("Error: CharacterMovement class can't find any child Transforms tagged 'GroundCheck'.");
            }
        }
    }

}