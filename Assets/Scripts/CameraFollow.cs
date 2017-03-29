using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private bool lockY = true;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Vector3 cameraOffset;
    [SerializeField]
    private float lerpMe;

    private Transform mainCamTransform;
    private Transform playerTransform;
	private Vector3 prevCamPos;

	// Use this for initialization
	void Start ()
	{
		findPlayer();
        cacheTransforms();
        prevCamPos = new Vector3 (mainCamTransform.position.x, mainCamTransform.position.y, mainCamTransform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if ( player.activeSelf )
		{
            Vector3 camerapos = mainCamTransform.position;
            Vector3 playerpos = playerTransform.position + cameraOffset;
			Vector3 cameraChange = Vector3.Lerp(camerapos, playerpos, lerpMe*Time.deltaTime);
            if (lockY)
            {
                cameraChange.y = prevCamPos.y;
            }
            cameraChange.z = prevCamPos.z;
            mainCamTransform.position = cameraChange;
			prevCamPos = new Vector3(cameraChange.x, cameraChange.y, cameraChange.z);
		}

	}

	// Tries to find the player. Call in Start()
	private void findPlayer()
	{
		if (player == null)
		{
            GameObject pl = GameObject.FindGameObjectWithTag("Player");
			if (pl == null)
			{
				Debug.Log("Can't find 'Player' game object");
			}
			else
			{
				player = pl;
			}
		}
	}

    private void cacheTransforms()
    {
        if (player != null)
        {
            playerTransform = player.transform;
        }
        mainCamTransform = Camera.main.transform;
    }
}