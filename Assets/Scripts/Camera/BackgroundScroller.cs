using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Scrolls the background with a nice parallax effect. Just the background.
*/
[RequireComponent(typeof(MeshRenderer))]
public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] [Range(0f, 0.01f)]
    private float speedFactor = 0.002f;
    
    private MeshRenderer rend;
	
	void Start()
    {
        findComponents();
	}
	
	
	public void scrollBackground(Vector2 cameraChange)
    {
        rend.material.mainTextureOffset = new Vector2(-cameraChange.x * speedFactor, -cameraChange.y * speedFactor);
    }
    
    private void findComponents()
    {
        rend = GetComponent<MeshRenderer>();
    }
    
}
