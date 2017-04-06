using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The base class for player's unlockable abilities.
 * Now a bit of a stub, maybe gets some functionality regarding the logic for sprites etc.
*/
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

}
