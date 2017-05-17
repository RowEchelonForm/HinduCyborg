using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private AudioClip getAbilitySound;
    [SerializeField]
    private GameObject helpTextUIPrefab;
    [SerializeField] [TextArea]
    private string helpText = "Press [BUTTON]\nto perform [ACTION]";
    
    
    // Plays the getAbilitySound
    public void playGetSound()
    {
        SoundFXPlayer.instance.playClipOnce(getAbilitySound, 0.2f);
    }
    
    // Shows the tutorial text for the ability
    public void showHelpText()
    {
        if (helpTextUIPrefab == null)
        {
            return;
        }
        GameObject hTextUI = Instantiate(helpTextUIPrefab, transform.position, Quaternion.identity);
        Text helpTextObject = hTextUI.GetComponentInChildren<Text>();
        helpTextObject.text = helpText;
    }
    
    
    private void Start()
    {
        StartCoroutine( checkIfPlayerHasAbility() );
        if (helpTextUIPrefab == null)
        {
            Debug.LogError("AbilityTrigger for " + abilityName_ + " needs helpTextUIPrefab.");
        }
    }
    
    private IEnumerator checkIfPlayerHasAbility()
    {
        // Wait for a couple frames.
        yield return null;
        yield return null;
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        if (pl == null)
        {
            Debug.LogError("AbilityTrigger " + gameObject.name + " can't find the Player GameObject (no GameObject found with the tag 'Player'.");
        }
        else
        {
            PlayerAbilityManager am = pl.GetComponent<PlayerAbilityManager>();
            if (am == null)
            {
                Debug.LogError("AbilityTrigger " + gameObject.name + " can't get 'PlayerAbilityManager' component on the Player GameObject.");
            }
            else
            {
                // set this to disabled if the player has this ability already
                if (am.getEnabledAbilities().Contains(abilityName))
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
    
};
