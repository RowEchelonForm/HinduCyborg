using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    private bool punchTriggered;
    private Animator anim;


    protected override void Start()
    {
    	base.Start();
        findComponents();
        enableAbilityParts();
    }
	
	private void Update()
	{
        if (hasAbility && Input.GetButtonDown("Punch") && 
        	actionHandler.isActionAllowed(action)) // TODO do the proper punch mechanic!!
        {
            anim.SetTrigger("punch");
        }
	}

	// TODO: do we need these?
    private void findComponents()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
			Debug.LogError("Error: No Animator found on the player from " + this.GetType().ToString() + " script! Please attach it.");
        }

    }
}
