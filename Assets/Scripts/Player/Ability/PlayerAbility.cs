using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The base class for player's unlockable abilities.
 * Requires the PlayerPartManager and PlayerActionHandler components.
*/
[RequireComponent(typeof(PlayerPartManager))]
[RequireComponent(typeof(PlayerActionHandler))]
public abstract class PlayerAbility : MonoBehaviour
{
	
	public bool hasAbility
	{
		get { return hasAbility_; }
		protected set { hasAbility_ = value; }
	}

	public abstract string ABILITY_NAME { get; }

	// All abilities must have a correcponding PlayerActionHandler.Action
	protected abstract PlayerActionHandler.Action action { get; }

	protected bool hasAbility_ = false;

	protected PlayerPartManager partManager;
	protected PlayerActionHandler actionHandler;


	// This should be called once the player has the ability and the script component is enabled, too. Does not enbale ability parts.
    // Should be called from PlayerAbilityManager.
	public virtual void enableAbility()
	{
		if (hasAbility_)
		{
			Debug.LogError("Error: Trying to enable an ability '" + this.GetType().ToString() + "' even though it's already enabled!");
			return;
		}
		hasAbility_ = true;
	}

    // Disables the ability. Will also disable ability parts.
    // Should be called from PlayerAbilityManager.
    public virtual void disableAbility()
    {
        if (!hasAbility_)
        {
            Debug.LogError("Error: Trying to disable an ability '" + this.GetType().ToString() + "' even though it's already disabled!");
            return;
        }
        hasAbility_ = false;
        disableAbilityParts();
    }



    // !!! Call this ( base.Start() ) from the child class !!!
	protected virtual void Start()
	{
		partManager = GetComponent<PlayerPartManager>();
		actionHandler = GetComponent<PlayerActionHandler>();
	}

	// Enables the visual part objects of this ability
	protected void enableAbilityParts()
	{
		partManager.enableAbilityParts(ABILITY_NAME);
	}

	// Disables the visual part objects of this ability
	protected void disableAbilityParts()
	{
		partManager.disableAbilityParts(ABILITY_NAME);
	}

}
