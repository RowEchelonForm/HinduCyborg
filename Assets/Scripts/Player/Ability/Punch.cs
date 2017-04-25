using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour {

    [SerializeField]
    private string abilityName = "Punch";

    private CharacterMovement charMov;
    private Animator anim;
    
    // Use this for initialization
    void Start () {
        findComponents();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void findComponents()
    {
        charMov = GetComponent<CharacterMovement>();
        if (charMov == null)
        {
            Debug.LogError("Error: Dash ability can't find CharacterMovement component, disabling Dash.");
            this.enabled = false;
        }


        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Error: Animator found on the player from Dash script! Please attach it.");
        }

    }
}
