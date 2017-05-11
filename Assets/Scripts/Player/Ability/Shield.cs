using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Shield : PlayerAbility
{
	
	public override string ABILITY_NAME
	{
		get { return abilityName; }
	}

	public bool isShieldOn { get; private set; }

	[SerializeField]
	private string abilityName = "Shield";
	[SerializeField] [Range(1, 10)]
	private int shieldStrength = 1;
	[SerializeField] [Range(0f, 60f)]
	private float shieldTimer = 1f;
	[SerializeField] [Range(0f, 60f)]
	private float shieldCooldown = 5f;

	private int originalShieldStrength;
	private float originalShieldTimer;
	private float originalShieldCooldown;

	private Animator anim;


	// Hits the shield with damage. Damage has to be > 1.
	// Returns how much damage the shield COULDN'T absorb, that is, 
	// how much damage will be taken after the shield absorption.
	// Example: damage = 3, returns 2 => shield absorbed 1 damage, character takes 2 damage.
	public int hitShield(int damage)
	{
		if (damage <= 0) // invalid damage
		{
			return 0;
		}
		if (!isShieldOn || !hasAbility) // shield not on / no ability
		{
			return damage;
		}

		if (shieldStrength <= 0)
		{
			Debug.LogError("Error: Shield doesn't have strength but is still on, turning it off.");
			turnOffShield();
			return damage;
		}
		int oldShieldStrength = shieldStrength;
		shieldStrength -= damage;
		if (shieldStrength <= 0)
		{
			shieldStrength = 0;
			turnOffShield();
		}
		else
		{
			// could play shield hit effects here
		}
		if (oldShieldStrength > damage)
		{
			return 0;
		}
		return damage - oldShieldStrength;
	}

	// Enables the ability like in PlayerAbility
	public override void enableAbility()
	{
		base.enableAbility();
		init();
	}



	protected override void Start()
	{
		base.Start();
		findComponents();
	}

	private void Update()
	{
		if (!hasAbility)
		{
			return;
		}
		if (isShieldOn)
		{
			handleOnUpdate(Time.deltaTime);
		}
		else if (!isShieldOn)
		{
			handleOffUpdate(Time.deltaTime);
		}
	}


	private void handleOnUpdate(float deltaTime)
	{
		shieldTimer -= deltaTime;
		if (shieldTimer <= 0)
		{
			turnOffShield();
		}
	}

	private void handleOffUpdate(float deltaTime)
	{
		if (shieldCooldown > 0)
		{
			shieldCooldown -= deltaTime;
		}
		if (shieldCooldown <= 0 && Input.GetButtonDown("Shield"))
		{
			turnOnShield();
		}
	}


	private void turnOnShield()
	{
		if (isShieldOn)
		{
			Debug.LogWarning("Trying to turn on Shield although it is already on.");
			enableAbilityParts();
			return;
		}
		Debug.Log("Shield on");
		isShieldOn = true;
		anim.SetBool("shield", true);
		shieldTimer = originalShieldTimer;
		shieldStrength = originalShieldStrength;
		enableAbilityParts();
	}

	private void turnOffShield()
	{
		if (!isShieldOn)
		{
			Debug.LogWarning("Trying to turn off Shield although it is already off.");
			disableAbilityParts();
			return;
		}
		if (shieldTimer > 0) // off because of damage
		{
			// Could play 'break' effects here
		}
		isShieldOn = false;
		anim.SetBool("shield", false);
		shieldCooldown = originalShieldCooldown;
		disableAbilityParts();
	}


	private void init()
	{
		isShieldOn = false;
		originalShieldStrength = shieldStrength;
		originalShieldTimer = shieldTimer;
		if (originalShieldCooldown == 0f)
		{
			originalShieldCooldown = shieldCooldown;
		}
		shieldCooldown = 0f;
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
