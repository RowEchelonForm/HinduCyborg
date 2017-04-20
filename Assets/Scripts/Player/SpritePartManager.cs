using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manages changing the visual sprite parts on the player.
 * Should be attached to the player GameObject.
 * 
 * The parts should be the childern of a child object called 'Sprites' on the player.
 * For exammple:
 * Player
 *   Sprites
 *     head
 *     torso
 *     ...
*/
public class SpritePartManager : MonoBehaviour
{
    [System.Serializable]
    public struct CyborgPart // the name of the part GameObject on the player and the Sprite for the part
    {
        public string partName; // e.g. head, torso
        public Sprite sprite; // the sprite for the part

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
    private Dictionary<string, List<CyborgPart>> abilityParts; // the parts for all the abilities

    // <partName, partSpriteRenderer>
    private Dictionary<string, SpriteRenderer> playerPartObjects; // the child GameObjects on the player


	void Start()
    {
        playerPartObjects = new Dictionary<string, SpriteRenderer>();
        findPlayerSpriteParts();

        abilityParts = new Dictionary<string, List<CyborgPart>>();
        fillAbilityParts();
        abilityPartsArray = null;
	}


    // Enables the visual sprites associated with the abilityName
    public void enableAbilitySprites(string abilityName)
    {
        /*if ( !abilityParts.ContainsKey(abilityName) )
        {
            Debug.LogWarning("SpritePartManager does not know an ability with the name '" + abilityName + "'.");
            return;
        }

        List<CyborgPart> parts = abilityParts[abilityName];
        for (int i = 0; i < parts.Count; ++i)
        {
            playerPartObjects[parts[i].partName].sprite = parts[i].sprite;
        }*/
    }


    // Call at Start first.
    // Fills playerPartObjects Dictionary with the sprite parts that the player has.
    private void findPlayerSpriteParts()
    {/*
        Transform sprites = transform.FindChild("Sprites");
        if (sprites == null)
        {
            Debug.LogError("Error: SpritePartManager can't find a child object on the player called 'Sprites'.");
            return;
        }

        for (int i = 0; i < sprites.childCount; ++i)
        {
            SpriteRenderer bodyPart = sprites.GetChild(i).GetComponent<SpriteRenderer>();
            if (!bodyPart)
            {
                Debug.LogWarning("Error: The player has a child GameObject under the 'Sprites' object that doesn't have a SpriteRenderer component. " +
                    "A SpriteRenderer component was attached to it now.");
                sprites.GetChild(i).gameObject.AddComponent<SpriteRenderer>();
            }
            else if (playerPartObjects.ContainsKey(bodyPart.gameObject.name))
            {
                Debug.LogError("Error: The player has more than one sprite parts with the name '" + bodyPart.name + "'.");
                continue;
            }
            playerPartObjects.Add(bodyPart.name, bodyPart);
        }*/
    }

    // Call at Start after calling findPlayerSpriteParts.
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
                Debug.LogError("Error: abilityPartsArray in SpritePartManager contains an empty/incorrect entry on index " + i );
            }
            else if ( abilityParts.ContainsKey( abilityName ) ) // ability with the same name already exists
            {
                Debug.LogError("Error: Two abilities have the name '" + abilityName + "' in abilityPartsArray " + 
                          "in the SpritePartManager class. See entry with index " + i);
            }
            else // ability is fine
            {
                CyborgPart cPart;
                for (int j = 0; j < abilityPartsArray[i].parts.Length; ++j) // go thorugh all the parts of an ability
                {
                    cPart = abilityPartsArray[i].parts[j];
                    if (cPart.partName == "" || !cPart.sprite) // invalid CyborgPart
                    {
                        Debug.LogError("Error: abilityPartsArray in SpritePartManager contains an ability '" + abilityName + "' " +
                            "with parts that have an empty/incorrect entry on index " + j);
                    }
                    else if (!playerPartObjects.ContainsKey(cPart.partName)) // no part with the name is on the player
                    {
                        Debug.LogError("Error: abilityPartsArray in SpritePartManager contains an ability '" + abilityName + "' " +
                            "with the part '" + cPart.partName + "' but the player doesn't have that part.");
                    }
                    else if (containsName(cPart.partName, partList)) // this ability already has a part with the same name
                    {
                        Debug.LogError("Error: The ability '" + abilityName + "' in abilityPartsArray on SpritePartManager " +
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
