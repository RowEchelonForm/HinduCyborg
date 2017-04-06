using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Attach to the player.
 * Manages the PlayerAbility abilities that the player has.
 * Do not turn the abilites on or off manually, this class will handle it.
 * The ability scripts should be enabled at start, this script will disable them.
*/
public class PlayerAbilityManager : MonoBehaviour
{

	// <abilityName, ability>
	private Dictionary<string, PlayerAbility> abilities;
	
	// Use this for initialization
	void Start()
	{
		findPlayerAbilityComponents();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("AbilityTrigger"))
		{
			AbilityTrigger aTrigger = col.GetComponent<AbilityTrigger>();
			if (aTrigger == null)
			{
				Debug.LogError("Error: An object with the 'AbilityTrigger' tag doesn't have an AbilityTrigger script component.");
			}
			else
			{
				enableAbility(aTrigger.abilityName);
			}
			col.gameObject.SetActive(false);
		}
	}


	private void enableAbility(string abilityName)
	{
		PlayerAbility ability = abilities[abilityName];
		if (ability == null)
		{
			Debug.LogError("PlayerAbilityManager does not have an ability with the name '" + abilityName + "'.");
			return;
		}
		Debug.Log("Enabling ability: " + abilityName);
		ability.enabled = true; // enable component
		ability.enableAbility(); // enable internally
	}


	private void findPlayerAbilityComponents()
	{
		abilities = new Dictionary<string, PlayerAbility>();
		List<PlayerAbility> tempAbilityList = new List<PlayerAbility>();
		gameObject.GetComponents<PlayerAbility>(tempAbilityList);
		int disabledAbilities = 0;
		for (int i = 0; i < tempAbilityList.Count; ++i)
		{
			if ( !abilities.ContainsKey(tempAbilityList[i].ABILITY_NAME) )
			{
				abilities.Add(tempAbilityList[i].ABILITY_NAME, tempAbilityList[i]);
				tempAbilityList[i].enabled = false;
				++disabledAbilities;
			}
			else
			{
				Debug.LogError("Error: PlayerAbilityManager found two PlayerAbility components with the same name on the player");
			}
		}
		Debug.Log("The player has " + abilities.Count + " abilities in total. " + 
				  (abilities.Count - disabledAbilities) + " of those abilities are enabled");
	}



}
