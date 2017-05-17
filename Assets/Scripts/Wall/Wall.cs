using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Wall that breaks when a trigger in the 'destroyLayers' layer hits it.
*/
[RequireComponent(typeof(Animator))]
public class Wall : MonoBehaviour
{

    [SerializeField]
    LayerMask destroyLayers;

    private Animator anim;

	// Use this for initialization
	void Start()
    {
        findComponents();
	}
	
	
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( destroyLayers == (destroyLayers | (1 << collision.gameObject.layer)))
        {
            anim.SetTrigger("destroy");
        }
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
