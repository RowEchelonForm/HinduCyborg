using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The Dash ability that the player has.
 * Needs the Rigidbody2D and CharacterMovement components of the player.
*/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Animator))]
public class Dash : PlayerAbility
{
	public override string ABILITY_NAME
	{
		get { return abilityName; }
	}

	[SerializeField]
	private string abilityName = "Dash";
    [SerializeField] [Range(0, 60f)]
	private float cooldownTime = 5f;
    [SerializeField] [Range(0, 300f)]
    private float dashVelocity = 30f;
    [SerializeField] [Range(0, 1)]
    private float dashTime = 0.2f;

	private float timer = 0f;
	private float doDash = 0f;
	private bool triggeredDash = false;

	private Rigidbody2D rb2d;
	private CharacterMovement charMov;
	private Animator anim;
    private GameObject Effect_Dash;

    private void Start()
	{
		findComponents();

        Effect_Dash.SetActive(true);
	}

	private void Update()
	{
		handleTimer();
		handleDashInput();
	}

	private void FixedUpdate()
	{
        //Debug.Log(timer);
        if (doDash > 0)
		{
            applyDashing();
		}
	}


	private void handleTimer()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
        }
        else
        {
            Effect_Dash.SetActive(true);
        }
	}

	private void handleDashInput()
	{
		if (Input.GetButtonDown("Dash") && hasAbility_ && (timer <= 0f) )
		{
            doDash = dashTime;
			timer = cooldownTime;
		}
	}

	private void applyDashing()
	{
        doDash -= Time.fixedDeltaTime;
        if (doDash < 0f)
        {
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
			triggeredDash = false;
            Effect_Dash.SetActive(false);
        }
        else
        {
			if (charMov.facingRight)
	        {
	            rb2d.velocity = new Vector2(dashVelocity, rb2d.velocity.y);
                Effect_Dash.SetActive(false);
            }
			else
			{
	            rb2d.velocity = new Vector2(dashVelocity * (-1), rb2d.velocity.y);
			}

			if (!triggeredDash)
	        {
				anim.SetTrigger("dash");
				triggeredDash = true;
			}
		}
		Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);
	}


	private void findComponents()
	{
		charMov = GetComponent<CharacterMovement>();
		if (charMov == null)
		{
			Debug.LogError("Error: Dash ability can't find CharacterMovement component, disabling Dash.");
			this.enabled = false;
		}

		rb2d = GetComponent<Rigidbody2D>();
		if (rb2d == null)
        {
            Debug.LogError("Error: No Rigidbody2D found on the player from Dash script! Please attach it.");
        }

		anim = GetComponent<Animator>();
		if (anim == null)
        {
			Debug.LogError("Error: Animator found on the player from Dash script! Please attach it.");
        }

        Effect_Dash = transform.Find("Sprites").gameObject.transform.Find("Effect_Dash").gameObject;
        if (Effect_Dash == null)
        {
            Debug.LogError("Error: no GameObject found");
        }

    }

}
