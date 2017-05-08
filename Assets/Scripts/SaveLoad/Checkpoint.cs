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



    protected virtual void OnDrawGizmos()
    {
    	// Variables:
    	float lineHeight = 2f;
    	float triangleLength = lineHeight/4;

		// Starting position:
        Vector3 startingPosition = transform.position;

        // Draw line:
		Vector3 endPosition = startingPosition + new Vector3(0, lineHeight, 0);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(startingPosition, endPosition);

		// Draw mesh:
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[3]
		{
			new Vector3(0f, -triangleLength, 0f),
			new Vector3(triangleLength, 0f, 0f),
			new Vector3(0f, triangleLength, 0f)
		};
		mesh.triangles = new int[]
		{
			0, 1, 2
		};
		mesh.normals = new Vector3[]
		{
			Vector3.forward,
			Vector3.forward,
			Vector3.forward
		};
		mesh.colors = new Color[]
		{
			Color.red,
			Color.red,
			Color.red
		};
		Graphics.DrawMeshNow(mesh, endPosition, Quaternion.identity);
		DestroyImmediate(mesh);
    }

}
