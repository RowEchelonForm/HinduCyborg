using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used on a trigger object that enables an unlockable ability on the player.
*/
public class AbilityTrigger : MonoBehaviour
{
	public string abilityName
	{
		get { return abilityName_; }
		private set { abilityName_ = value; }
	}

	[SerializeField]
    private string abilityName_ = "";
        
};
