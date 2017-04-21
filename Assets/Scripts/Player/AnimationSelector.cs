using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


/*
 * Contains all the animations of the character this is attached to in priority.
 * Top (first) animation has the highest priority.
 * Handles playing the highest priority animation of the frame.
*/
[RequireComponent(typeof(Animator))]
public class AnimationSelector : MonoBehaviour
{

	// For the editor.
	// Contains all the animations (names) in priority order.
	// Top is the highest priority (first index).
	[SerializeField]
	private List<string> animationPriorities = new List<string>();


	// Actually used during runtime: <animationName, priority>
	// Priorities are in the range of 1...priorityDict.Count
	private Dictionary<string, int> priorityDict = new Dictionary<string, int>();

	// Animation to play during a frame.
	private int highestPriority;
	private string highestPriorityName;
	private Animator animator;


	// Call this to play an animation with the name 'animationName'.
	// The overall highest priority animation will be played.
	public void playAnimation(string animationName)
	{
		if ( priorityDict.ContainsKey(animationName) )
		{
			int priority = priorityDict[animationName];
			if ( priority > highestPriority )
			{
				highestPriority = priority;
				highestPriorityName = animationName;
			}
		}
		else
		{
			Debug.LogError(this.GetType().ToString() + " doesn't have an animation called '" + animationName + "'.");
		}
	}


	private void Start()
	{
		animator = GetComponent<Animator>();
		for (int i = 0; i < animationPriorities.Count; ++i)
		{
			priorityDict.Add(animationPriorities[i], animationPriorities.Count-i);
		}
		highestPriorityName = "";
		highestPriority = 0;
	}
	
	
	private void LateUpdate()
	{
		Debug.Log("Before: " + highestPriorityName);
		handleAnimationPlaying();
		Debug.Log("After: " + highestPriorityName);
	}


	private void handleAnimationPlaying()
	{
		if (highestPriorityName != "")
		{
			animator.Play(highestPriorityName);
		}
		highestPriorityName = "";
		highestPriority = 0;
	}

}
