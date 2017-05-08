using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manages (sets active and not-active) the visual parts of the different abilities on the player.
 * Should only manage the parts of the player that are controlled in scripts (not animation).
 * Should be attached to the player GameObject.
 * 
 * The parts should be the childern of a child object called 'Sprites' on the player.
 * For exammple:
 * Player
 *   Sprites
 *     head
 *     torso
 *     Effect_Dash
 *     ...
*/
public class PlayerPartManager : MonoBehaviour
{
    [System.Serializable]
    public struct CyborgPart // the name of the part GameObject on the player and the Sprite for the part
    {
        public string partName; // e.g. head, torso, Effect_Dash
		public GameObject partObject; // the  for the part

    }

    [System.Serializable]
    public struct AbilityParts // all the parts for an ability
    {
        public string abilityName; // the name of the ability, e.g. Dash
        public CyborgPart[] parts; // the parts for the ability
    }

    [SerializeField]
    private AbilityParts[] abilityPartsArray; // for editor

    // <abilityName, parts>
	private Dictionary<string, List<CyborgPart>> abilityParts = new Dictionary<string, List<CyborgPart>>(); // the parts for all the abilities

    // <partName, partObject>
	private Dictionary<string, GameObject> playerPartObjects = new Dictionary<string, GameObject>(); // the child GameObjects on the player


	void Start()
    {
        findPlayerParts();
        fillAbilityParts();
        abilityPartsArray = null;
	}



    // Enables the visual parts associated with the abilityName
    public void enableAbilityParts(string abilityName)
    {
		if ( checkAbilityNameExistance(abilityName) )
		{
	        List<CyborgPart> parts = abilityParts[abilityName];
	        for (int i = 0; i < parts.Count; ++i)
	        {
	            playerPartObjects[parts[i].partName].SetActive(true);
	        }
        }
    }

	// Disables the visual parts associated with the abilityName
	public void disableAbilityParts(string abilityName)
	{
		if ( checkAbilityNameExistance(abilityName) )
        {
			List<CyborgPart> parts = abilityParts[abilityName];
	        for (int i = 0; i < parts.Count; ++i)
	        {
	            playerPartObjects[parts[i].partName].SetActive(false);
	        }
        }
	}



	// Checks that an ability with the name abilityName exists here
	private bool checkAbilityNameExistance(string abilityName)
	{
		if ( !abilityParts.ContainsKey(abilityName) )
        {
			Debug.LogWarning(this.GetType().ToString() + " does not know an ability with the name '" + abilityName + "'.");
            return false;
        }
        return true;
	}

    // Call at Start first.
    // Fills playerPartObjects Dictionary with the parts that the player has.
    private void findPlayerParts()
    {
        Transform partParent = transform.FindChild("Sprites");
        if (partParent == null)
        {
			Debug.LogError("Error: " + this.GetType().ToString() + " can't find a child object on the player called 'Sprites'.");
            return;
        }

        for (int i = 0; i < partParent.childCount; ++i)
        {
			GameObject partObject = partParent.GetChild(i).gameObject;
			if (playerPartObjects.ContainsKey(partObject.name))
            {
                Debug.LogError("Error: The player has more than one parts with the name '" + partObject.name + "'.");
                continue;
            }
			playerPartObjects.Add(partObject.name, partObject);
        }
    }

    // Call at Start after calling findPlayerParts.
    // Fills abilityParts Dictionary with the ability parts in abilityPartsArray and checks their validity.
    private void fillAbilityParts()
    {
        string abilityName;
        for (int i = 0; i < abilityPartsArray.Length; ++i)
        {
            abilityName = abilityPartsArray[i].abilityName;
            List<CyborgPart> partList = new List<CyborgPart>();
            if ( abilityName == "" ) // needs to have an actual name
            {
                Debug.LogError("Error: abilityPartsArray in PlayerPartManager contains an empty/incorrect entry on index " + i );
            }
            else if ( abilityParts.ContainsKey( abilityName ) ) // ability with the same name already exists
            {
                Debug.LogError("Error: Two abilities have the name '" + abilityName + "' in abilityPartsArray " + 
                          "in the PlayerPartManager class. See entry with index " + i);
            }
            else // ability is fine
            {
                CyborgPart cPart;
                for (int j = 0; j < abilityPartsArray[i].parts.Length; ++j) // go thorugh all the parts of an ability
                {
                    cPart = abilityPartsArray[i].parts[j];
                    if (cPart.partName == "" || !cPart.partObject) // invalid CyborgPart
                    {
                        Debug.LogError("Error: abilityPartsArray in PlayerPartManager contains an ability '" + abilityName + "' " +
                            "with parts that have an empty/incorrect entry on index " + j);
                    }
                    else if (!playerPartObjects.ContainsKey(cPart.partName)) // no part with the name is on the player
                    {
                        Debug.LogError("Error: abilityPartsArray in PlayerPartManager contains an ability '" + abilityName + "' " +
                            "with the part '" + cPart.partName + "' but the player doesn't have that part.");
                    }
                    else if (containsName(cPart.partName, partList)) // this ability already has a part with the same name
                    {
                        Debug.LogError("Error: The ability '" + abilityName + "' in abilityPartsArray on PlayerPartManager " +
                            "has more than one parts with the same name '" + cPart.partName + "'.");
                    }
                    else // success
                    {
                        partList.Add(cPart);
                    }
                }
                if (partList.Count > 0)
                {
                    abilityParts.Add(abilityName, partList);
                }
            }
        }
    }

    // Returns true if partName is contained in partList. 
    private bool containsName(string partName, List<CyborgPart> partList)
    {
        for (int k = 0; k < partList.Count; ++k) // check for the same part names for one ability
        {
            if (partName == partList[k].partName) // one ability has more than one parts with the same name
            {
                return true;
            }
        }
        return false;
    }

}
