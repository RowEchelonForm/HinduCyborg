using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The base class for player's unlockable abilities.
 * Requires the PlayerPartManager component.
*/
[RequireComponent(typeof(PlayerPartManager))]
public abstract class PlayerAbility : MonoBehaviour
{
	
	public bool hasAbility
	{
		get { return hasAbility_; }
		protected set { hasAbility_ = value; }
	}

	public virtual string ABILITY_NAME
	{
		get { return ""; }
	}

	protected bool hasAbility_ = false;

	protected PlayerPartManager partManager;


	// This should be called once the player has the ability and the script component is enabled, too.
	public void enableAbility()
	{
		if (hasAbility_)
		{
			Debug.LogError("Error: Trying to enable an ability '" + this.GetType().ToString() + "' even though it's already enabled!");
			return;
		}
		hasAbility_ = true;
	}




	protected virtual void Start()
	{
		partManager = GetComponent<PlayerPartManager>();
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
