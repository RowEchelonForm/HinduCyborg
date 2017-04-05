using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : PlayerAbility
{
	public override string ABILITY_NAME
	{
		get { return abilityName; }
	}

	[SerializeField]
	private string abilityName = "Dash";
	[SerializeField]
	private float cooldownTime = 5f;
	[SerializeField]
	private float dashForce = 20f;

	private float timer = 0f;
	private bool doDash = false;

	private Rigidbody2D rb2d;
	private CharacterMovement charMov;


	void Start()
	{
		findComponents();
	}

	void Update()
	{
		handleTimer();
		handleDashInput();
	}

	void FixedUpdate()
	{
		if (doDash)
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
	}

	private void handleDashInput()
	{
		if (Input.GetButtonDown("Dash") && hasAbility_ && (timer <= 0f) )
		{
			doDash = true;
			timer = cooldownTime;
		}
	}

	private void applyDashing()
	{
		if (charMov.facingRight)
		{
			rb2d.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
		}
		else
		{
			rb2d.AddForce(Vector2.right * dashForce * (-1), ForceMode2D.Impulse);
		}
		doDash = false;
	}


	private void findComponents()
	{
		charMov = gameObject.GetComponent<CharacterMovement>();
		if (charMov == null)
		{
			Debug.LogError("Error: Dash ability can't find CharacterMovement component, disabling Dash.");
			this.enabled = false;
		}
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		if (rb2d == null)
        {
            Debug.LogError("Error: No Rigidbody2D found on the player from Dash script! Please attach it.");
        }
	}

}
