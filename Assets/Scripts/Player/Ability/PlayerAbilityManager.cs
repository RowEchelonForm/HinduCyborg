using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Attach to the player.
 * Manages the PlayerAbility abilities that the player has.
 * Do not turn the abilites on or off manually, this class will handle it.
 * The ability scripts should be enabled at start, this script will disable them.
 * Does not manage the visual parts object(s) of the ability.
*/
public class PlayerAbilityManager : MonoBehaviour
{

	// <abilityName, ability>
    private Dictionary<string, PlayerAbility> enabledAbilities = new Dictionary<string, PlayerAbility>();
    private Dictionary<string, PlayerAbility> disabledAbilities = new Dictionary<string, PlayerAbility>();


    // Call this to enable a certain ability.
    // Nothing else should be called from 'the outside'.
    public void enableAbility(string abilityName)
    {
        PlayerAbility ability;
        if (enabledAbilities.ContainsKey(abilityName))
        {
            ability = enabledAbilities[abilityName];
            enabledAbilities.Remove(abilityName);
            Debug.LogWarning("The ability '" + abilityName + "' has already been enabled. Enabling again.");
        }
        else if (disabledAbilities.ContainsKey(abilityName))
        {
            ability = disabledAbilities[abilityName];
            disabledAbilities.Remove(abilityName);
        }
        else
        {
            Debug.LogError("PlayerAbilityManager does not have an ability with the name '" + abilityName + "'.");
            return;
        }
        Debug.Log("Enabling ability: " + abilityName);
        ability.enabled = true; // enable component
        ability.enableAbility(); // enable internally
        enabledAbilities.Add(abilityName, ability);
    }

	// Call this to disable a certain ability.
    // Nothing else should be called from 'the outside'.
    public void disableAbility(string abilityName)
    {
        PlayerAbility ability;
        if (disabledAbilities.ContainsKey(abilityName))
        {
            ability = disabledAbilities[abilityName];
            disabledAbilities.Remove(abilityName);
            Debug.LogWarning("The ability '" + abilityName + "' has already been disabled. Disabling again.");
        }
        else if (enabledAbilities.ContainsKey(abilityName))
        {
            ability = enabledAbilities[abilityName];
            enabledAbilities.Remove(abilityName);
        }
        else
        {
            Debug.LogError("PlayerAbilityManager does not have an ability with the name '" + abilityName + "'.");
            return;
        }
        Debug.Log("Disabling ability: " + abilityName);
        ability.disableAbility(); // disable internally
        ability.enabled = false; // disable component
        disabledAbilities.Add(abilityName, ability);
    }


	// Returns a list of the ability names that the player has enabled.
    public List<string> getEnabledAbilities()
    {
    	return enabledAbilities.Keys.ToList();
    }

	// Returns a list of the ability names that the player has disabled.
	public List<string> getDisabledAbilities()
    {
    	return disabledAbilities.Keys.ToList();
    }


	// Use this for initialization
	private void Awake()
	{
		findPlayerAbilityComponents();
	}

	private void OnTriggerEnter2D(Collider2D col)
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

	private void findPlayerAbilityComponents()
	{
		List<PlayerAbility> tempAbilityList = new List<PlayerAbility>();
		gameObject.GetComponents<PlayerAbility>(tempAbilityList);
		int abilityCount = 0;
		for (int i = 0; i < tempAbilityList.Count; ++i)
		{
            if ( !disabledAbilities.ContainsKey(tempAbilityList[i].ABILITY_NAME) )
			{
				disabledAbilities.Add(tempAbilityList[i].ABILITY_NAME, tempAbilityList[i]);
				tempAbilityList[i].enabled = false;
                ++abilityCount;
			}
			else
			{
				Debug.LogError("Error: PlayerAbilityManager found two PlayerAbility components with the same name on the player");
			}
		}
        Debug.Log("The player has " + disabledAbilities.Count + " abilities in total. " + 
            (disabledAbilities.Count - abilityCount) + " of those abilities are enabled");
	}

}
