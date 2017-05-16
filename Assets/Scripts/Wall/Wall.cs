using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {


    private Animator anim;

	// Use this for initialization
	void Start () {
        findComponents();
        //anim.Play("collapse_animation");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // TODO: do we need these?
    private void findComponents()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Error: No Animator found on the player from " + this.GetType().ToString() + " script! Please attach it.");
        }

    }
}
