using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Call this before performing an action to see if it's allowed.
 * E.g. for jumping, running, performing abilities.
*/
[RequireComponent(typeof(Animator))]
public class PlayerActionHandler : MonoBehaviour
{

	public enum Action
	{
		move,
		flip,
		jump,
		dash,
		shield,
		punch
	};

    // Animation name hashes:
	private int idleHash = Animator.StringToHash("Base Layer.idle");
	private int runHash = Animator.StringToHash("Base Layer.run");
	private int jumpHash = Animator.StringToHash("Base Layer.Jumping.jump_on_air");
	private int jumpRecoveryHash = Animator.StringToHash("Base Layer.Jumping.jump_finished");
	private int dashHash = Animator.StringToHash("Base Layer.Abilities.dash");
	private int shieldHash = Animator.StringToHash("Base Layer.Abilities.shield");
	private int punchHash = Animator.StringToHash("Base Layer.Abilities.punch");

    private bool dominatingActionPerfomed = false; // has a dominating action been performed this frame
    private bool dominatingActionPerfomedFU = false; // same but for fixed update

	private Animator anim;


	// Returns true if the action can be performed, otherwise false.
    //
    // !!! The action should be performed if returned true !!!
    // !!! Only the first dominating action (dash, shield, punch, jump) will return true per frame !!!
    //
    // I.e. can't perform several dominating actions per on one frame.
    // E.g. call for jump => return true, then, on the same frame, call for dash => return false.
	public bool isActionAllowed(Action action)
	{
        int animationState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        if (anim.IsInTransition(0))
        {
            animationState = anim.GetNextAnimatorStateInfo(0).fullPathHash;
        }
	    
        // Normal actions:
		if (action == Action.flip)
		{
			if (animationState == idleHash || animationState == runHash || animationState == jumpRecoveryHash || 
				animationState == jumpHash || animationState == dashHash || animationState == shieldHash)
			{
				return true;
			}
			return false;
		}
		else if (action == Action.move)
		{
			if (animationState == idleHash || animationState == runHash || 
				animationState == jumpRecoveryHash || animationState == jumpHash || animationState == shieldHash)
			{
				return true;
			}
			return false;
		}
        
        
        // If dominating action already asked on this frame => false
        if (!isDominatingActionAllowed())
        {
            return false;
        }

        // Dominating actions:
		if (action == Action.jump)
		{
			if (animationState == idleHash || animationState == runHash || animationState == jumpRecoveryHash
                || animationState == shieldHash)
			{
                performDominatingAction();
				return true;
			}
			return false;
		}
		else if (action == Action.dash)
		{
			if (animationState == idleHash || animationState == runHash || 
				animationState == jumpRecoveryHash || animationState == jumpHash)
			{
                performDominatingAction();
				return true;
			}
			return false;
		}
		else if (action == Action.shield)
		{
			if (animationState == idleHash || animationState == runHash || 
                animationState == jumpRecoveryHash || animationState == jumpHash)
			{
                performDominatingAction();
				return true;
			}
			return false;
		}
		else if (action == Action.punch)
		{
			if (animationState == idleHash || animationState == runHash || animationState == jumpRecoveryHash)
			{
                performDominatingAction();
				return true;
			}
			return false;
		}


		return false; // Shouldn't happen but for my sanity
	}

    
    // Returns true if a dominating action is allowed
    private bool isDominatingActionAllowed()
    {
        if (dominatingActionPerfomed || dominatingActionPerfomedFU)
        {
            return false;
        }
        return true;
    }
    
    // Sets the flags so that dominating action can't be performed before Update and FixedUpdate
    private void performDominatingAction()
    {
        dominatingActionPerfomed = true;
        dominatingActionPerfomedFU = true;
        StartCoroutine(delayedDominationReset());
    }
    
    
	private void Start()
	{
		findComponents();
	}
    
    // Needed if deltaTime >> fixedDeltaTime
	private void FixedUpdate()
	{
        dominatingActionPerfomedFU = false;
	}
    
    // Needed so that can't perform a new dominating action on the next frame 
    // before the 'old' one has even started
    private IEnumerator delayedDominationReset()
    {
        yield return new WaitForEndOfFrame(); // the end of the same frame isActionAllowed was called (if it was called from Update() )
        yield return new WaitForEndOfFrame(); // the end of the next frame
        dominatingActionPerfomed = false;
    }


	private void findComponents()
	{
		anim = GetComponent<Animator>();
		if (anim == null)
		{
			Debug.LogError("Error: No Animator found on the player from " + this.GetType().ToString() + " script! Please attach it.");
		}
	}
}
