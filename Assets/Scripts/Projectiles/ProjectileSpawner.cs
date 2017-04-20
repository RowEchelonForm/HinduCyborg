using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for things that spawn projectiles.
 * The projectiles are managed by this script,
 * that is, enabled and disabled.
*/
public abstract class ProjectileSpawner : MonoBehaviour
{

    [SerializeField]
    protected GameObject projectilePrefab;
    [SerializeField]
    protected Transform projectileSpawnPoint;
    [SerializeField] [Range(0, 360)]
    protected float projectileAngle = 0f;  // degrees; 0 is shooting in +x direction, 90 in +y direction
    [SerializeField] [Range(0, 100f)]
    protected float projectileSpeed = 30f;

    [SerializeField]
    private string projectileTag = "Projectile"; // the tag for all the Projectile GameObjects (remember to create this tag in Unity)

    private Stack<Projectile> disabledProjectiles =  new Stack<Projectile>(); // contains disabled Projectiles
    private Transform cachedTransform;

	
	protected virtual void Start()
    {
        cachedTransform = transform;
        verifyTag(ref projectileTag);
        verifyPrefabType();
        findProjectileSpawnPoint();
	}

    // Spawns a projectile.
    protected Projectile spawnProjectile()
    {
        Projectile proj = getProjectileObject();
        return proj;
    }

    // A projectile calls this to recycle (disable) itself.
    // Call this when the projectile is destroyed / removed from the gameplay.
    // DO NOT TRY TO USE THE PROJECTILE OR THE GAMEOBJECT IT IS ATTACHED TO AFTER CALLING THIS.
    public void recycleProjectile(Projectile proj)
    {
        if (proj.CompareTag(projectileTag))
        {
            proj.gameObject.SetActive(false);
            disabledProjectiles.Push(proj);
        }
        else
        {
            Debug.LogWarning("recycleProjectile was called for a Projectile GameObject that isn't tagged with " + projectileTag +
                             ". Please only use Projectiles created by the ProjectileSpawner as parameters for recycleProjectile function.");
        }
    }


    // Returns the Projectile component of an active and initialized projectile GameObject (calls projectile.init()).
    // transform.position is projectileSpawnPoint.position and transform.rotation Quarternion.identity.
    private Projectile getProjectileObject()
    {
        Projectile proj;
        if (disabledProjectiles.Count > 0)
        {
            proj = disabledProjectiles.Pop();
            proj.transform.position = projectileSpawnPoint.position;
            proj.transform.rotation = Quaternion.identity;
            proj.gameObject.SetActive(true);
            proj.init(projectileSpeed, projectileAngle + cachedTransform.eulerAngles.z);
        }
        else
        {
            GameObject projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            projObj.tag = projectileTag;
            proj = projObj.GetComponent<Projectile>();
            proj.init(projectileSpeed, projectileAngle + cachedTransform.eulerAngles.z, this);
        }
        return proj;
    }

    // Makes sure that the tag exists for the projectiles.
    private void verifyTag(ref string tag)
    {
        try
        {
            gameObject.CompareTag(tag);
        }
        catch (UnityException ex)
        {
            Debug.LogError("Error: In " + this.GetType().ToString() + ": Tag  " + tag + "  does not exist. Please create it in Unity.");
            tag = "Untagged";
            throw ex;
        }
    }

    // Makes sure that projectilePrefab is ok (contains a Projectile component).
    private void verifyPrefabType()
    {
        if (projectilePrefab == null) // base class has this
        {
            string className = this.GetType().ToString();
            Debug.LogError("Error: " + className + " has a projectilePrefab that is null, disabling the " + className);
            this.enabled = false;
        }

        Projectile proj = projectilePrefab.GetComponent<Projectile>();
        if (proj == null)
        {
            string className = this.GetType().ToString();
            Debug.LogError("Error: " + className + " has a projectilePrefab that doesn't have a Projectile component, disabling the " 
                           + className + " script.");
            this.enabled = false;
        }
    }

    // cahcedTransform MUST NOT BE NULL
    // Finds the projectile spawn point child object.
    // It should be tagged with 'SpawnPoint'.
    // If it's not found, projectile spawn point will be set to the transform of this GameObject.
    private void findProjectileSpawnPoint()
    {
        if (projectileSpawnPoint != null)
        {
            return;
        }
        for (int i = 0; i < cachedTransform.childCount; ++i)
        {
            Transform child = cachedTransform.GetChild(i);
            if (child.CompareTag("SpawnPoint"))
            {
                projectileSpawnPoint = child;
                break;
            }
        }
        if (projectileSpawnPoint == null)
        {
            string className = this.GetType().ToString();
            Debug.LogError("Error: The projectileSpawnPoint variable of " + className + "  is null, the Transform of " 
                           + className + " will be used as the spawn point for its projectiles.");
            projectileSpawnPoint = cachedTransform;
        }
    }

}
