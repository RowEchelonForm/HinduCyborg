﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The Dash ability that the player has.
 * Needs the Rigidbody2D, Animator and CharacterMovement components of the player.
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

	protected override PlayerActionHandler.Action action
	{
		get { return PlayerActionHandler.Action.dash; }
	}

	[SerializeField]
	private string abilityName = "Dash";
    [SerializeField] [Range(0, 60f)]
	private float cooldownTime = 5f;
    [SerializeField] [Range(0, 300f)]
    private float dashVelocity = 30f;
    [SerializeField] [Range(0, 1)]
    private float dashTime = 0.2f;
    [SerializeField] [Range(0f, 300f)]
    private float dashMomentum = 100f;
    [SerializeField]
    private AudioClip dashSound;

	private float timer = 0f;
	private float doDash = 0f;
	private bool triggeredDash = false; // to know if animation should be triggered

	private Rigidbody2D rb2d;
	private CharacterMovement charMov;
	private Animator anim;

    protected override void Start()
	{
		base.Start();
		findComponents();
        enableAbilityParts();
	}

	private void Update()
	{
		handleTimer();
		handleDashInput();
	}

	private void FixedUpdate()
	{
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
            if (timer <= 0f)
            {
                enableAbilityParts();
            }
        }
	}

	private void handleDashInput()
	{
		if (Input.GetButtonDown("Dash") && hasAbility_ && (timer <= 0f) && 
			actionHandler.isActionAllowed(action))
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
			disableAbilityParts();
            if (charMov.facingRight)
            {
                rb2d.AddForce(new Vector2(dashMomentum, 0f));
            }
            else
            {
                rb2d.AddForce(new Vector2(-dashMomentum, 0f));
            }
            
        }
        else
        {
			if (charMov.facingRight)
	        {
	            rb2d.velocity = new Vector2(dashVelocity, rb2d.velocity.y);
            }
			else
			{
	            rb2d.velocity = new Vector2(dashVelocity * (-1), rb2d.velocity.y);
			}

			if (!triggeredDash) // starting dash
	        {
				anim.SetTrigger("dash");
                rb2d.AddForce(new Vector2(0, dashMomentum));
				triggeredDash = true;
                SoundFXPlayer.instance.playClipOnce(dashSound);
			}
		}
	}


	private void findComponents()
	{
		charMov = GetComponent<CharacterMovement>();
		if (charMov == null)
		{
			Debug.LogError("Error: No CharacterMovement found on the player from" + this.GetType().ToString() + "script! Please attach it.");
			this.enabled = false;
		}

		rb2d = GetComponent<Rigidbody2D>();
		if (rb2d == null)
        {
			Debug.LogError("Error: No Rigidbody2D found on the player from " + this.GetType().ToString() + " script! Please attach it.");
            this.enabled = false;
        }

		anim = GetComponent<Animator>();
		if (anim == null)
        {
			Debug.LogError("Error: No Animator found on the player from " + this.GetType().ToString() + " script! Please attach it.");
			this.enabled = false;
        }

    }

}
