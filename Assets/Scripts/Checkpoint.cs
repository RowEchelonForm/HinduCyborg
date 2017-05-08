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
	private Transform spawnPoint;
	[SerializeField]
	private string saveFileName = "checkpoint";

	public static int lastCheckpointInstanceID {get; private set;}



	protected void Start()
	{
		findSpawnPoint();
	}

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


	// Finds the spawnPoint child object of this checkpoint.
	// It should be tagged with 'SpawnPoint'.
    // If it's not found, projectile spawn point will be set to the transform of this GameObject.
    private void findSpawnPoint()
    {
        if (spawnPoint != null) // spawnPoint is set
        {
			if ( !spawnPoint.IsChildOf(transform) ) // spawnPoint is not set correctly (not child of this GameObject)
        	{
				Debug.LogError("Error: The spawnPoint variable of class '" + this.GetType().ToString() + 
							   "' (object name: " + gameObject.name + ") is not a child of the Transform of '" 
							   + gameObject.name + ". Changing the spawnPoint to be the  Transform of '" + gameObject.name +
                               ". If this isn't what you want, please give the checkpoint a child object and tag it 'SpawnPoint'.");
				spawnPoint = transform;
				return;
            }
        }

        for (int i = 0; i < transform.childCount; ++i) // find the spawnPoint
        {
			Transform child = transform.GetChild(i);
            if (child.CompareTag("SpawnPoint")) // spawnPoint found
            {
                spawnPoint = child;
                break;
            }
        }
        if (spawnPoint == null) // didn't find spawnPoint
        {
            string className = this.GetType().ToString();
            Debug.LogError("Error: The spawnPoint variable of class '" + className + "' (object name: " + gameObject.name + 
						   ") is null, the Transform of '" + gameObject.name + "' will be used as the spawn point for the checkpoint. " +
                           "If this isn't what you want, please give the checkpoint a child object and tag it 'SpawnPoint'.");
            spawnPoint = transform;
        }
    }

}
