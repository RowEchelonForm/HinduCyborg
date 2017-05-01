using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The general movement system for the player character (running, jumping, falling etc.).
 * Works only with Rigidbody2D and Unity's built-in physics system.
 * Does not control any of the player's abilities.
*/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] [Range(0, 100f)]
    private float maxSpeed = 5f;
    [SerializeField] [Range(0, 100f)]
    private float jumpForce = 8f;
    [SerializeField] [Range(0, 1f)]
    private float airSpeedFactor = 0.7f;
    [SerializeField] [Range(0f, 1f)]
    private float landingSlownessFactor = 0.2f;
    [SerializeField] [Range(0f, 1f)]
    private float groundToAirForgiveTime = 0.1f; // How long (seconds) can the player be in the air and can still be considered to be grounded
	[SerializeField] [Range(0f, 1f)]
	private float jumpCooldown = 0.11f; // should be > groundToAirForgiveTime

    public bool facingRight { get; private set; }

	private List<Transform> groundChecks;
    private bool grounded = false;
    private float groundedTimer; // the actual timer for grounded forgiveness
    private float jumpTimer; // the actual timer for jumping
	private bool jump = false;
    private Rigidbody2D rb2d;
    private Transform cachedTransform;
    private Animator anim;

    void Start()
    {
		facingRight = true;
		groundedTimer = groundToAirForgiveTime;
		jumpTimer = 0f;
		initComponents();
    }

    // Update is called once per frame
    void Update() 
    {
        grounded = checkGroundedStatus(Time.deltaTime);
		setJumpFlag(Time.deltaTime);
    }


    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        handleAnimation(horizontalInput); //select played animation

        applyMovementVelocity(horizontalInput);
        handleFlipping(horizontalInput);
        handleJumping();
    }

    private bool checkGroundedStatus(float deltaTime)
    {
        // Only casting on the 'Ground' layer. Casts rays towards the all the GroundCheck tagged objects.
        bool onGround = false;
        for (int i = 0; i < groundChecks.Count; ++i)
        {
            if (Physics2D.Linecast(transform.position, groundChecks[i].position, 1 << LayerMask.NameToLayer("Ground")))
            {
                onGround = true;
                groundedTimer = groundToAirForgiveTime;
            }
        }
		if (!onGround && groundedTimer > 0)
        {
			groundedTimer -= deltaTime;
			onGround = true;
        }
        return onGround;
    }

	private void setJumpFlag(float deltaTime)
    {
        if (Input.GetButtonDown("Jump") && grounded && jumpTimer <= 0)
        {
            jump = true;
            jumpTimer = jumpCooldown;
        }
		else if (jumpTimer > 0) // can't jump if just jumped
		{
			jumpTimer -= deltaTime;
		}
    }

	private void applyMovementVelocity(float horizontalInput)
    {
        float curVelocityX = rb2d.velocity.x;

        float deltaVelocity; // the difference in velocity to get to maxSpeed
        if (facingRight)
        {
        	if (!grounded) // if in air
        	{
				deltaVelocity = maxSpeed * airSpeedFactor - curVelocityX;
        	}
        	else
        	{
        		deltaVelocity = maxSpeed - curVelocityX;
        	}
        }
        else
        {
			if (!grounded) // if in air
        	{
				deltaVelocity = (-1) * maxSpeed * airSpeedFactor - curVelocityX;
        	}
        	else
        	{
        		deltaVelocity = (-1) * maxSpeed - curVelocityX;
        	}
        }

		// Calculate the maximum force that we can apply so that we never go over maxSpeed
		float forceToApply = rb2d.mass * deltaVelocity / Time.fixedDeltaTime;

		// Don't artificially slow down if already going faster than maxSpeed (high speed could be caused by a dash etc.)
        if ( (curVelocityX > maxSpeed && forceToApply < 0f) || (curVelocityX < maxSpeed*(-1) && forceToApply > 0f) )
        {
        	forceToApply = 0f;
        }
		rb2d.AddForce(Vector2.right * Mathf.Abs(horizontalInput) * forceToApply);
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
            rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jump = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
		if ( collider.CompareTag("Kill") )
        {
			Debug.Log("Killed player");
            LevelManager.reloadCurrentLevel(); // TODO should load a checkpoint
        }
		else if ( collider.CompareTag("Projectile") )
		{
			Debug.Log("Player hit projectile");
			LevelManager.reloadCurrentLevel(); // TODO should load a checkpoint
		}
    }

    // Select played animation
    private void handleAnimation(float input)
    {
		if (grounded)
		{
			anim.SetBool("in_air", false);
		}
		else
		{
			anim.SetBool("in_air", true);
		}

		if (input == 0)
		{
			anim.SetBool("run", false);
		}
		else
		{
			anim.SetBool("run", true);
		}
    }

	private void initComponents()
    {
        cachedTransform = transform;

        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            Debug.LogError("Error: No Rigidbody2D found on the player from CharacterMovement script! Please attach it.");
        }

		groundChecks = new List<Transform>();
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

		anim = GetComponent<Animator>();
		if (anim == null)
        {
            Debug.LogError("Error: No Animator found on the player from CharacterMovement script! Please attach it.");
        }


    }



    // Animations call these:

    public void landingFinished()
    {
		maxSpeed = maxSpeed / (1-landingSlownessFactor);
    }

    public void slowOnLanding()
    {
		maxSpeed = maxSpeed * (1-landingSlownessFactor);
    }
}