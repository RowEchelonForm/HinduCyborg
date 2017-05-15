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
[RequireComponent(typeof(PlayerActionHandler))]
[RequireComponent(typeof(Collider2D))]
public class CharacterMovement : MonoBehaviour
{

    [SerializeField] [Range(0, 100f)]
    private float maxSpeed = 5f;
    [SerializeField] [Range(0, 100f)]
    private float jumpForce = 8f; // impulse
    [SerializeField] [Range(0, 20f)]
    private float jumpBoostForce = 20f; // continuous force, that's why it's higher than jumpForce
    [SerializeField] [Range(0, 1f)]
    private float airSpeedFactor = 0.7f;
    [SerializeField] [Range(0f, 0.999f)]
    private float landingSlownessFactor = 0.2f;
    [SerializeField] [Range(0f, 1f)]
    private float groundToAirForgiveTime = 0.1f; // How long (seconds) can the player be in the air and can still be considered to be grounded
	[SerializeField] [Range(0f, 1f)]
	private float jumpCooldown = 0.11f; // should be > groundToAirForgiveTime
    [SerializeField] [Range(0f, 1f)]
    private float jumpBoostTime = 0.3f; // how long can jump key be pressed after jump to boost jump
    [SerializeField] [Range(0f, 1f)]
    private float spikeThreshold = 0.3f; // how large spikes can the player walk over

    public bool facingRight { get; private set; }

	private List<Transform> groundChecks;
    private float hInput;
    private bool grounded = false;
    private float groundedTimer; // the actual timer for grounded forgiveness
    private float jumpTimer; // the actual timer for allowing jumping
    private float jumpBoostTimer; // the actual timer for the jump boost
	private bool jump = false;
    private bool jumpBoost = false;
    private float originalMaxSpeed;
    private float originalJumpForce;
    private float originalJumpBoostForce;
    private Rigidbody2D rb2d;
    private Transform cachedTransform;
    private Animator anim;
    private PlayerActionHandler actionHandler;
    private Collider2D playerCollider;
    private List<ContactPoint2D> leftContactPoints; // the left side contact points the player hits per frame
    private List<ContactPoint2D> rightContactPoints; // the right side contact points the player hits per frame
    
    // Slows down the general movement speed for 'time' seconds.
    // factor should be ]0...1] and time should be [0...60].
    public void slowDownMovement(float factor, float time)
    {
        if (factor <= 0f)
        {
            factor = 0.001f;
        }
        else if (factor > 1f)
        {
            factor = 1f;
        }
        if (time > 60f)
        {
            time = 60f;
        }
        maxSpeed = maxSpeed * (factor);
        StartCoroutine(restoreMovementSpeed(factor, time));
    }
    
    // Reduces jumping forces for 'time' seconds.
    // factor should be ]0...1] and time should be [0...60].
    public void slowDownJumping(float factor, float time)
    {
        if (factor <= 0f)
        {
            factor = 0.001f;
        }
        else if (factor > 1f)
        {
            factor = 1f;
        }
        if (time > 60f)
        {
            time = 60f;
        }
        jumpForce = jumpForce * (factor);
        jumpBoostForce = jumpBoostForce * (factor);
        StartCoroutine(restoreJumpingForce(factor, time));
    }
    
    // Resets movement speed
    public void resetMaxSpeed()
    {
        maxSpeed = originalMaxSpeed;
    }
    
    // Resets the jumping forces
    public void resetJumpForces()
    {
        jumpForce = originalJumpForce;
        jumpBoostForce = originalJumpBoostForce;
    }
    
    
    // Animations call these:
    public void slowOnLanding()
    {
        maxSpeed = maxSpeed * (1 - landingSlownessFactor);
    }
    
    public void landingFinished()
    {
        maxSpeed = maxSpeed / (1 - landingSlownessFactor);
    }
    
    
    private void Start()
    {
		facingRight = true;
		groundedTimer = groundToAirForgiveTime;
		jumpTimer = 0f;
        jumpBoostTimer = 0f;
        originalMaxSpeed = maxSpeed;
        originalJumpForce = jumpForce;
        originalJumpBoostForce = jumpBoostForce;
        leftContactPoints = new List<ContactPoint2D>();
        rightContactPoints = new List<ContactPoint2D>();
		initComponents();
    }

    // Update is called once per frame
    private void Update() 
    {
        grounded = checkGroundedStatus(Time.deltaTime);
		setJumpFlag(Time.deltaTime);
        handleMovementInput(); // sets hInput
    }

    private void FixedUpdate()
    {
        applyMovementVelocity(hInput, Time.fixedDeltaTime);
        stabilizeDownhillMovement(rb2d.velocity, Time.fixedDeltaTime);
        handleAnimationParameters(hInput); // select played animation
        handleFlipping(hInput);
        handleJumping();
        leftContactPoints.Clear();
        rightContactPoints.Clear();
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        collectContactPoints(col);
    }
    
    void OnCollisionStay2D(Collision2D col)
    {
        collectContactPoints(col);
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
		if (!onGround)
        {
			if (groundedTimer > 0)
			{
				groundedTimer -= deltaTime;
				if (groundedTimer > 0)
				{
					onGround = true;
				}
			}
        }
		if (jumpBoostTimer > 0) // just jumped => don't need forgiveness
		{
			onGround = false;
		}
        return onGround;
    }

	private void setJumpFlag(float deltaTime)
    {
		if (Input.GetButtonDown("Jump") && grounded && jumpTimer <= 0 && 
			actionHandler.isActionAllowed(PlayerActionHandler.Action.jump)) // start jump
        {
            jump = true;
            jumpTimer = jumpCooldown;
            jumpBoostTimer = jumpBoostTime;
        }
        else if (Input.GetButton("Jump") && jumpBoostTimer > 0) // boost jump
        {
            jumpBoost = true;
            jumpBoostTimer -= deltaTime;
        }
        else if (jumpBoostTimer > 0) // lifted jump button up (not boosting anymore)
        {
            jumpBoostTimer = 0f;
        }

		if (jumpTimer > 0) // can't jump if just jumped
		{
			jumpTimer -= deltaTime;
		}
    }
    
    private void handleMovementInput()
    {
        hInput = Input.GetAxis("Horizontal");
    }
    
    // Applies the movement force.
	private void applyMovementVelocity(float horizontalInput, float deltaTime)
    {
    	if (horizontalInput == 0 || !actionHandler.isActionAllowed(PlayerActionHandler.Action.move))
    	{
    		return; // no input or not allowed to move
    	}

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
		float forceToApply = rb2d.mass * deltaVelocity / deltaTime;

		// Don't artificially slow down if already going faster than maxSpeed (high speed could be caused by a dash etc.)
        if ( (curVelocityX > maxSpeed && forceToApply < 0f) || (curVelocityX < maxSpeed*(-1) && forceToApply > 0f) )
        {
        	forceToApply = 0f;
        }
        
        Vector2 finalForce = Vector2.right * Mathf.Abs(horizontalInput) * forceToApply;
        if (checkWallCollision(finalForce, deltaTime))
        {
            return;
        }
        handleSpikes(finalForce);
        
        rb2d.AddForce(finalForce);
    }

    private void handleFlipping(float horizontalInput)
    {
    	if (!actionHandler.isActionAllowed(PlayerActionHandler.Action.flip))
    	{
    		return; // flipping not allowed
    	}

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
        else if (jumpBoost)
        {
            rb2d.AddForce(new Vector2(0f, jumpBoostForce), ForceMode2D.Force);
            jumpBoost = false;
        }
    }

    // Set correct animation triggers
    private void handleAnimationParameters(float input)
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
    
    // In order to not get stuck on walls when applying force.
    // Checks, if with the force that is to be added, we will be hitting a wall (on the 'Ground' layer)
    // and we're not grounded. Returns true if will hit a wall.
    private bool checkWallCollision(Vector2 force, float deltaTime)
    {
        // Get the position change based on the force, mass and delta time
        Vector2 posChange = new Vector2((force.x / rb2d.mass) * deltaTime*deltaTime, (force.y / rb2d.mass) * deltaTime*deltaTime);
        
        // Raise y pos by % of the player collider's height (not needed because of grounded check at the end of this function)
        // float yCompensation = Mathf.Abs(playerCollider.bounds.max.y - playerCollider.bounds.min.y) * 0.00f;
        // posChange.y += yCompensation;
        
        // Get the bounds of the collider.
        Vector2 bottomRight = new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.min.y);
        Vector2 topLeft = new Vector2(playerCollider.bounds.min.x, playerCollider.bounds.max.y);
        
        // Move collider in direction that we are moving in
        bottomRight += posChange;
        topLeft += posChange;
        
        // Check if the body's current velocity will result in a collision and not grounded
        if (Physics2D.OverlapArea(topLeft, bottomRight, 1 << LayerMask.NameToLayer("Ground")) && !grounded)
        {
            return true;
        }
        return false;
    }
    
    // Checks the contact points to see if the player is stuck on a tiny spike/"wall".
    private void handleSpikes(Vector2 forceToApply)
    {
        if (forceToApply.x < 0 && leftContactPoints.Count > 0) // left side
        {
            overcomeSmallSpikes(ref leftContactPoints);
        }
        else if (forceToApply.x > 0 && rightContactPoints.Count > 0) // right side
        {
            overcomeSmallSpikes(ref rightContactPoints);
        }
    }
    
    
    // Be sure to check for both left and right contact points.
    // First check that minWallPoint 
    // Uses leftContactPoints and rightContactPoints.
    // The player will be raised up a bit if player is wrongly stuck.
    private void overcomeSmallSpikes(ref List<ContactPoint2D> contactPoints)
    {
        // check min/max contact points
        float minWallPoint = playerCollider.bounds.max.y;
        float maxWallPoint = playerCollider.bounds.min.y;
        for (int i = 0; i < contactPoints.Count; ++i)
        {
            if (contactPoints[i].point.y < minWallPoint)
            {
                minWallPoint = contactPoints[i].point.y;
            }
            if (contactPoints[i].point.y > maxWallPoint)
            {
                maxWallPoint = contactPoints[i].point.y;
            }
        }
        if (minWallPoint <= playerCollider.bounds.min.y && maxWallPoint > minWallPoint &&
            maxWallPoint <= minWallPoint + spikeThreshold)
        {
            rb2d.position = new Vector2(rb2d.position.x, rb2d.position.y + maxWallPoint - minWallPoint);
        }
        else if (grounded && maxWallPoint <= playerCollider.bounds.min.y + spikeThreshold)
        {
            rb2d.position = new Vector2(rb2d.position.x, rb2d.position.y + (maxWallPoint - playerCollider.bounds.min.y)*1.05f);
        }
    }
    
    // Stabilizes movement downhill.
    // Works for right and left downhills.
    // Should be called AFTER applying movement velocity.
    // curVelocity is the velocity right now.
    private void stabilizeDownhillMovement(Vector2 curVelocity, float deltaTime)
    {
        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            // Won't stabilize if moving too fast (could be caused by e.g. dashing)
            return;
        }
        
        if (curVelocity.x > 0 && leftContactPoints.Count > 0) // moving right
        {
            Vector2 maxNormal = new Vector2(0, 0); // the gentler the slope, the higher normal.x
            for (int i = 0; i < leftContactPoints.Count; ++i)
            {
                if (leftContactPoints[i].normal.x > maxNormal.x && maxNormal.x < 0.7f) // 0.7f corresponds to ~45 DEG
                {
                    maxNormal = leftContactPoints[i].normal;
                }
            }
            if (maxNormal.x > 0.05f && maxNormal.x < 0.95f)
            {
                rb2d.position = new Vector2(rb2d.position.x - rb2d.velocity.x * maxNormal.x * deltaTime,
                                            rb2d.position.y - rb2d.velocity.x * maxNormal.y * deltaTime);
            }
        }
        else if (curVelocity.x < 0 && rightContactPoints.Count > 0) // moving left
        {
            Vector2 minNormal = new Vector2(0, 0); // the gentler the slope, the smaller normal.x (negative)
            for (int i = 0; i < rightContactPoints.Count; ++i)
            {
                if (rightContactPoints[i].normal.x < minNormal.x && minNormal.x > -0.7f) // 0.7f corresponds to ~45 DEG
                {
                    minNormal = rightContactPoints[i].normal;
                }
            }
            if (minNormal.x < -0.05f && minNormal.x > -0.95f)
            {
                rb2d.position = new Vector2(rb2d.position.x + rb2d.velocity.x * minNormal.x * deltaTime,
                                            rb2d.position.y + rb2d.velocity.x * minNormal.y * deltaTime);
            }
        }
    }
    
    // Collects the contact points in collision to leftContactPoints and rightContactPoints
    private void collectContactPoints(Collision2D col)
    {
        ContactPoint2D[] colPoints = col.contacts;
        for (int i = 0; i < colPoints.Length; ++i)
        {
            if (colPoints[i].normal.x > 0.1f) // player moving in negative direction and hitting a wall
            {
                leftContactPoints.Add(colPoints[i]);
            }
            else if (colPoints[i].normal.x < -0.1f) // player moving in positive direction and hitting a wall
            {
                rightContactPoints.Add(colPoints[i]);
            }
        }
    }
    
    // factor has to be > 0 and <= 1
    private IEnumerator restoreMovementSpeed(float factor, float time)
    {
        yield return new WaitForSeconds(time);
        if (maxSpeed != originalMaxSpeed) // make sure we haven't reset this already
        {
            maxSpeed = maxSpeed / (factor);
            if (maxSpeed > originalMaxSpeed) // safety check
            {
                maxSpeed = originalMaxSpeed;
            }
        }
    }
    
    // factor has to be > 0 and <= 1
    private IEnumerator restoreJumpingForce(float factor, float time)
    {
        yield return new WaitForSeconds(time);
        if (jumpForce != originalJumpForce) // make sure we haven't reset this already
        {
            jumpForce = jumpForce / (factor);
            if (jumpForce > originalJumpForce) // safety check
            {
                jumpForce = originalJumpForce;
            }
        }
        if (jumpBoostForce != originalJumpBoostForce) // make sure we haven't reset this already
        {
            jumpBoostForce = jumpBoostForce / (factor);
            if (jumpBoostForce > originalJumpBoostForce) // safety check
            {
                jumpBoostForce = originalJumpBoostForce;
            }
        }
    }

	private void initComponents()
    {
        cachedTransform = transform;

        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            Debug.LogError("Error: No Rigidbody2D found on the player in CharacterMovement script! Please attach it.");
        }

		anim = GetComponent<Animator>();
		if (anim == null)
        {
            Debug.LogError("Error: No Animator found on the player in CharacterMovement script! Please attach it.");
        }

		actionHandler = GetComponent<PlayerActionHandler>();
		if (actionHandler == null)
        {
			Debug.LogError("Error: No PlayerActionHandler found on the player in CharacterMovement script! Please attach it.");
        }
        
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Error: No Collider2D found on the player in CharacterMovement script! Please attach it.");
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
    }
    
}