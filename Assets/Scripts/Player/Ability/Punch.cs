using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : PlayerAbility
{

	public override string ABILITY_NAME
	{
		get { return abilityName; }
	}

	[SerializeField]
	private string abilityName = "Punch";

    private CharacterMovement charMov;
    private Animator anim;

    // Use this for initialization
    protected override void Start()
    {
    	base.Start();
        findComponents();
    }
	
	// Update is called once per frame
	void Update()
	{
		// TODO
	}

	// TODO: do we need these?
    private void findComponents()
    {
        charMov = GetComponent<CharacterMovement>();
        if (charMov == null)
        {
            Debug.LogError("Error: Punch ability can't find CharacterMovement component, disabling Punch.");
            this.enabled = false;
        }


        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Error: Animator found on the player from Dash script! Please attach it.");
        }

    }
}
