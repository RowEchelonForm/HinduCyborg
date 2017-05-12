using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Manages the health / dying of the player.
 * Can interact with 'Shield' ability if it exists and is on.
 * Needs 'SaveLoad' to exists somewhere.
*/
public class PlayerHealth : MonoBehaviour
{

	[SerializeField] [Range(1, 100)]
	private int health = 1; // the actual / current health

	private int originalHealth;

	private Shield shield;


	// Restores the players actual health to the original health
	public void restoreHealth()
	{
		health = originalHealth;
	}

	// Sets the player's actual health to hp.
	// hp has to be > 0 and player has to be alive.
	public void setHealth(int hp)
	{
		if (hp <= 0 || health <= 0)
		{
			return;
		}
		health = hp;
	}


	// Takes damage and takes the Shield into account.
    private void takeDamage(int damage)
    {
    	if (damage <= 0)
    	{
    		return;
    	}
    	if (shield != null && shield.hasAbility)
    	{
			damage = shield.hitShield(damage);
			if (damage <= 0)
	    	{
	    		return;
	    	}
		}
		health -= damage;
		if (health <= 0)
		{
			kill();
		}
    }

    private void kill()
    {
		Debug.Log("Killed player");
		SaveLoad.LoadFromFile("checkpoint");
        SaveLoad.Load();
    }


    private void Start()
    {
    	findComponents();
    }

	private void Awake()
	{
		originalHealth = health;
	}

	private void OnTriggerEnter2D(Collider2D collider)
    {
		if ( collider.CompareTag("Kill") )
        {
			kill();
        }
		else if ( collider.CompareTag("Projectile") )
		{
			Projectile proj = collider.GetComponent<Projectile>();
			if (proj == null)
			{
				Debug.LogError("Error: An object with the tag 'Projectile' hit the player but it doesn't have a Projectile component");
				return;
			}
			takeDamage(proj.damage);
		}
    }


    private void findComponents()
    {
		shield = GetComponent<Shield>();
    }
}
