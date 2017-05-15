using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The shield ability.
 * Requires the Animator component. Having CharacterMovement component is recommended.
*/
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterMovement))]
public class Shield : PlayerAbility
{
	
	public override string ABILITY_NAME
	{
		get { return abilityName; }
	}

	protected override PlayerActionHandler.Action action
	{
		get { return PlayerActionHandler.Action.shield; }
	}

	[SerializeField]
	private string abilityName = "Shield";
	[SerializeField] [Range(1, 10)]
	private int shieldStrength = 1;
	[SerializeField] [Range(0f, 60f)]
	private float shieldTimer = 1f;
	[SerializeField] [Range(0f, 60f)]
	private float shieldCooldown = 5f;
    [SerializeField] [Range(0f, 1f)]
    private float shieldForgivenessTime = 0.1f; // how long after turning off the shield will protect (only if didn't break)
    [SerializeField] [Range(0f, 0.999f)]
    private float shieldMovementSpeedFactor = 0.5f; // for moving while shield is on
    [SerializeField] [Range(0f, 0.999f)]
    private float shieldJumpFactor = 0.75f; // for jumping while shield is on
    [SerializeField]
    private AudioClip shieldOnSound;

	private int originalShieldStrength;
	private float originalShieldTimer;
	private float originalShieldCooldown;
    private float originalShieldForgivenessTime;
    private bool isShieldOn = false;
    private AudioSource shieldOnSoundSource;

	private Animator anim;
    private CharacterMovement charMov;


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
		if (!hasAbility || (!isShieldOn && shieldForgivenessTime <= 0)) // shield not on / no ability
		{
			return damage;
		}

		if (shieldStrength <= 0)
		{
			Debug.LogError("Error: Shield doesn't have strength but is still on, turning it off.");
			turnOffShield(true);
			return damage;
		}
		int oldShieldStrength = shieldStrength;
		shieldStrength -= damage;
		if (shieldStrength <= 0)
		{
			shieldStrength = 0;
            if (shieldForgivenessTime <= 0)
            {
			    turnOffShield(true);
            }
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
        enableAbilityParts();
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


	// isShieldOn must be true
	private void handleOnUpdate(float deltaTime)
	{
		shieldTimer -= deltaTime;
		if (shieldTimer <= 0 || Input.GetButtonDown("Shield"))
		{
			turnOffShield();
		}
	}

	// isShieldOn must be false
	private void handleOffUpdate(float deltaTime)
	{
		if (shieldCooldown > 0)
		{
			shieldCooldown -= deltaTime;
            if (shieldCooldown <= 0)
            {
                enableAbilityParts();
            }
        }

        if (shieldForgivenessTime > 0)
        {
            shieldForgivenessTime -= deltaTime;
        }
		if ((shieldCooldown <= 0 && Input.GetButtonDown("Shield")) && 
			actionHandler.isActionAllowed(action))
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
		isShieldOn = true;
		anim.SetBool("shield", true);
		shieldTimer = originalShieldTimer;
		shieldStrength = originalShieldStrength;
        shieldForgivenessTime = 0f;
        charMov.slowDownJumping(shieldJumpFactor, shieldTimer);
        charMov.slowDownMovement(shieldMovementSpeedFactor, shieldTimer);
        shieldOnSoundSource = SoundFXPlayer.instance.playClipContinuosly(shieldOnSound);
	}

	private void turnOffShield(bool wasHit = false)
	{
		if (!isShieldOn && shieldForgivenessTime <= 0)
		{
			Debug.LogWarning("Trying to turn off Shield although it is already off.");
			disableAbilityParts();
			return;
		}
		if (wasHit) // because of damage
		{
			// Could play 'break' effects here
            shieldForgivenessTime = 0f;
		}
        else if (shieldForgivenessTime <= 0)
        {
            shieldForgivenessTime = originalShieldForgivenessTime;
        }
		isShieldOn = false;
		anim.SetBool("shield", false);
		shieldCooldown = originalShieldCooldown;
        charMov.resetMaxSpeed();
        charMov.resetJumpForces();
        SoundFXPlayer.instance.recycleAudioSource(shieldOnSoundSource, 0.05f);
        shieldOnSoundSource = null;
		disableAbilityParts();
	}


	private void init()
	{
		isShieldOn = false;
		originalShieldStrength = shieldStrength;
		originalShieldTimer = shieldTimer;
        originalShieldForgivenessTime = shieldForgivenessTime;
        shieldForgivenessTime = 0f;
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
        
        charMov = GetComponent<CharacterMovement>();
        if (charMov == null)
        {
            Debug.LogError("Error: No CharacterMovement found on the player from " + this.GetType().ToString() + " script! Please attach it.");
        }
    }

}
