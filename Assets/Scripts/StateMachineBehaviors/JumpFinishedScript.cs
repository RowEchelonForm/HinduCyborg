using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * For jump_finished animation.
 * Calls CharacterMovement script to slow down on landing and then reset speed afterwards.
*/
public class JumpFinishedScript : StateMachineBehaviour
{
    
    private CharacterMovement charMov;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	    if (charMov == null)
        {
            charMov = animator.gameObject.GetComponent<CharacterMovement>();
        }
        if (charMov != null)
        {
            charMov.slowOnLanding();
        }
	}
    
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	    if (charMov != null)
        {
            charMov.landingFinished();
        }
	}
}
