using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Projectile that is spawned (shot) by a class that _inherits_ the ProjectileSpawner class.
 * The lifecycle of the projectile is managed by the spawner.
*/
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{

    public int damage {get { return damage_; } private set { damage_ = value; } }  // do we need this?

    [SerializeField]
    private AudioClip spawnSound;
    [SerializeField]
    private int damage_ = 1;
    [SerializeField]
    private float despawnDistance = 1000f;
	[SerializeField] [Range(0f, 60f)]
    private float despawnTimer = 30f; // seconds
    [SerializeField] [Range(0, 300)]
    private float angularVelocity = 50f;  // basically just a visual effect
    [SerializeField]
    private Rigidbody2D rb2d;

    private float internalDespawnTimer; // the actual timer
    private bool initStatus = false;  // true after init() is called
    private Transform cachedTransform;
    private Vector3 spawnPosition;
    private ProjectileSpawner spawner;

	
    public void hit()
    {
        // TODO play hit effects here (NOT USED RIGHT NOW)
        despawn();
    }

    // Initializes the projectile. ProjectileSpawner has to be set only once (but can be set again).
    // If projSpawner is left to null, older spawner will be used. If there's no older spawner, returns false.
    // Angle is in degrees.
    // Returns true if the projectile was initialized correctly OR if it was already initialized.
    // Returns false if the projectile couldn't be initialized properly.
    public bool init(float speed, float angle, ProjectileSpawner projSpawner = null)
    {
        if (projSpawner != null && !initStatus)
        {
            spawner = projSpawner;
        }
        if (spawner == null)
        {
            Debug.LogError("Error: Projectile doesn't have its ProjectileSpawner set and it can't be initialized.");
            if (initStatus)
            {
                Debug.LogWarning("Warning: Projectile has already been initialized, won't be initialized again.");
            }
            return false;
        }
        if (initStatus)
        {
            Debug.LogWarning("Warning: Projectile has already been initialized, won't be initialized again.");
            return true;
        }


        if ( !initMovement(speed, angle) )
        {
            return false;
        }
        playCreationEffects();

        cachedTransform = transform;
        spawnPosition = cachedTransform.position;
        initStatus = true;
		internalDespawnTimer = despawnTimer;
        return true;
    }

    private void despawn()
    {
    	if (initStatus) // not despawned already
    	{
	        initStatus = false;
	        spawner.recycleProjectile(this);
        }
    }

    private void Update()
    {
        float distance = Mathf.Abs(Vector3.Distance(spawnPosition, cachedTransform.position));
        internalDespawnTimer -= Time.deltaTime;
		if (distance >= despawnDistance || internalDespawnTimer <= 0)
        {
            despawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
    	if (otherCollider.transform.GetInstanceID() == spawner.transform.GetInstanceID()) // ignore collision with spawner
    	{
			return;
    	}
		despawn();
    }

    // angle is in degrees
    private bool initMovement(float speed, float angle)
    {
        if (rb2d == null)
        {
            rb2d = gameObject.GetComponent<Rigidbody2D>();
            if ( rb2d == null )
            {
                Debug.LogError("Can't find a RigidBody2D on Projectile.");
                return false;
            }
        }

        float xSpeed = Mathf.Cos(angle*Mathf.PI/180) * speed;
        float ySpeed = Mathf.Sin(angle*Mathf.PI/180) * speed;
        rb2d.velocity = new Vector2(xSpeed, ySpeed);
		rb2d.angularVelocity = angularVelocity;
        return true;
    }

    private void playCreationEffects()
    {
        if (spawnSound == null)
        {
            return;
        }

        SoundFXPlayer.instance.playClipOnce(spawnSound);
    }


}