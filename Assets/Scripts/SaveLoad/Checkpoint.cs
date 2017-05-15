using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * When an object with the tag "Player" hits the checkpoint, the game is saved.
 * The game will only be saved if it wasn't saved on the same checkpoint the last time.
*/
public class Checkpoint : MonoBehaviour
{
	[SerializeField]
	private string saveFileName = "checkpoint";

	public static int lastCheckpointInstanceID {get; private set;}



	protected void OnTriggerEnter2D(Collider2D otherCollider)
	{
		// Player hit checkpoint and last checkpoint != this checkpoint
		if (otherCollider.tag == "Player" && lastCheckpointInstanceID != gameObject.GetInstanceID())
		{
			hitCheckpoint();
		}
	}
    
    
	private void hitCheckpoint()
	{
		Debug.Log("Player hit checkpoint '" + gameObject.name + "' with the instanceID " + gameObject.GetInstanceID());
		lastCheckpointInstanceID = gameObject.GetInstanceID();
		SaveLoad.Save(LevelManager.currentLevelName);
        SaveLoad.SaveToFile(saveFileName);
	}
    
    
    
    // Draw the checkpoint gizmo
    protected virtual void OnDrawGizmos()
    {
    	// Variables:
    	float lineHeight = 2f;
    	float triangleLength = lineHeight/4;
        Gizmos.color = Color.red;
        
		// Positions for the line:
        Vector3 startingPosition = transform.position;
        Vector3 endPosition = startingPosition + new Vector3(0, lineHeight, 0); // also used for the triangle
        
        // Positions for the triangle
        Vector3 triangleTip = new Vector3(endPosition.x + triangleLength, endPosition.y-triangleLength, endPosition.z);
        Vector3 triangleBottom = new Vector3(endPosition.x, endPosition.y-triangleLength, endPosition.z);
        
        // Draw the line:
		Gizmos.DrawLine(startingPosition, endPosition);
        
        // Draw the triangle
        Gizmos.DrawLine(endPosition, triangleTip);
        Gizmos.DrawLine(triangleTip, triangleBottom);
    }


}
