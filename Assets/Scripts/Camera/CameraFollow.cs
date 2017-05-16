using UnityEngine;
using System.Collections;

/*
 * Follows the player GameObject smoothly.
*/

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private bool lockY = true; // locks the y axis movement of the camera
    [SerializeField]
    private bool speedZoom = false; // allow zooming out with speed
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Vector3 cameraOffset;
    [SerializeField]
    private float lerpMe;
    [SerializeField]
    private float zoomLerp;
    [SerializeField]
    private float zoomDefault;
    [SerializeField]
    private float zoomSpeedMulti = 0.75f;
    [SerializeField]
    private BackgroundScroller bg;

    private Transform mainCamTransform;
    private Transform playerTransform;
	private Vector3 prevCamPos;
    private Ship playerScript;
    private Camera cam;

	// Use this for initialization
	void Start ()
	{
		findPlayer();
        cacheTransforms();
        initPosition();
        addZoomComponents();
        findBackgroundScroller();
        prevCamPos = new Vector3 (mainCamTransform.position.x, mainCamTransform.position.y, mainCamTransform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		if ( player.activeSelf )
		{
            Vector3 cameraPos = mainCamTransform.position;
            Vector3 playerPos = playerTransform.position + cameraOffset;
			Vector3 cameraChange = Vector3.Lerp(cameraPos, playerPos, lerpMe*Time.deltaTime);
            if (lockY)
            {
                cameraChange.y = prevCamPos.y;
            }
            cameraChange.z = prevCamPos.z;
            mainCamTransform.position = cameraChange;
			prevCamPos = new Vector3(cameraChange.x, cameraChange.y, cameraChange.z);
            if (speedZoom)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomDefault + playerScript.avgSpeed() * zoomSpeedMulti, zoomLerp * Time.deltaTime);
            }
            if (bg)
            {
                bg.scrollBackground(new Vector2(cameraChange.x, cameraChange.y));
            }
        }

        // TODO This should be in some other script!
        if ( Input.GetKeyDown(KeyCode.Escape) )
        {
            LevelManager.quitRequest();
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
				Debug.LogError("Can't find 'Player' game object");
			}
			else
			{
				player = pl;
            }
		}
	}
    
    // Call at Start after calling addZoomComponents (or otherwise after trying to find playerScript).
    private void findBackgroundScroller()
    {
        if (bg == null && playerScript == null)
        {
            GameObject bgObject = GameObject.FindGameObjectWithTag("Background");
            if (bgObject == null)
            {
                Debug.LogWarning("Can't find 'Background' tagged game object.");
                return;
            }
            bg = bgObject.GetComponent<BackgroundScroller>();
            if (bg == null)
            {
                Debug.LogWarning("Can't 'Background' tagged game object doesn't have BackgroundScroller component.");
                return;
            }
        }
    }

    private void addZoomComponents()
    {
        if (speedZoom && player != null)
        {
            Ship s1 = player.GetComponent<Ship>();
            if (s1 == null)
            {
                Debug.Log("Can't find ship script");
            }
            else
            {
                playerScript = s1;
            }
            Camera c1 = gameObject.GetComponent<Camera>();
            if (c1 == null)
            {
                Debug.Log("Can't find camera");
            }
            else
            {
                cam = c1;
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

    private void initPosition()
    {
        if (lockY)
        {
            mainCamTransform.position = new Vector3(playerTransform.position.x, mainCamTransform.position.y, mainCamTransform.position.z) + cameraOffset;
        }
        else
        {
            mainCamTransform.position = new Vector3 (playerTransform.position.x, playerTransform.position.y, mainCamTransform.position.z) + cameraOffset;
        }
    }
}