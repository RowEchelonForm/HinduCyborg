using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The punch ability.
 * The punch effect GameObject should be controlled by Animator and be DISABLED at start.
 * The punch effect GameObject should have a DISABLED Collider2D and be on the 'Punch' layer.
*/
[RequireComponent(typeof(Animator))]
public class Punch : PlayerAbility
{

	public override string ABILITY_NAME
	{
		get { return abilityName; }
	}

	protected override PlayerActionHandler.Action action 
	{
		get { return PlayerActionHandler.Action.punch; }
	}

	[SerializeField]
	private string abilityName = "Punch";

    [SerializeField] [Range(0f, 30f)]
    private float punchCooldown = 5f; // after punch started.

    private float punchTimer;
    private Animator anim;


    protected override void Start()
    {
    	base.Start();
        findComponents();
        enableAbilityParts();
        punchTimer = 0;
    }
	
	private void Update()
	{
        if (!hasAbility)
        {
            return;
        }

        handleTimer(Time.deltaTime);
        if (shouldPunch())
        {
            doPunch();
        }
	}


    private void handleTimer(float deltaTime)
    {
        if (punchTimer > 0)
        {
            punchTimer -= deltaTime;
            if (punchTimer <= 0)
            {
                enableAbilityParts();
            }
        }
    }

    // Should be called after handleTimer().
    // Returns true if should perform punch.
    private bool shouldPunch()
    {
        if (punchTimer <= 0 && Input.GetButtonDown("Punch") &&
            actionHandler.isActionAllowed(action))
        {
            return true;
        }
        return false;
    }

    // Performs punching.
    private void doPunch()
    {
        anim.SetTrigger("punch");
        punchTimer = punchCooldown;
        disableAbilityParts();
    }

	
    private void findComponents()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
			Debug.LogError("Error: No Animator found on the player from " + this.GetType().ToString() + " script! Please attach it.");
        }

    }
}
