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

	[SerializeField]
	private string abilityName = "Punch";

    private bool punchTriggered;
    private Animator anim;

    // Use this for initialization
    protected override void Start()
    {
    	base.Start();
        findComponents();
        enableAbilityParts();
    }
	
	// Update is called once per frame
	private void Update()
	{
        if (hasAbility && Input.GetButtonDown("Punch")) // TODO delete
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
