using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A turret object that shoots projectiles with a timer.
*/
public class Turret : ProjectileSpawner
{

    [SerializeField] [Range(0.01f, 10f)]
    private float shootTimer = 1f;
    [SerializeField] [Range(-3600f, 3600f)]
    private float rotationPerSecond = 20f;
    [SerializeField] [Range(0f, 359f)]
    private float maxAngleDiff = 0f;
    [SerializeField]
    private bool loopRotationOverride = false; // the turret only rotates one way and loops that

	// How much shooting is delayed initially (from 0 to 1 => 0 to 100 %).
	// Used to "desync" turrets.
    [SerializeField] [Range(0f, 1f)]
    private float delaySyncFactor = 0.2f;

    private bool rotationDirection = true;
    private float originalAngle;
    private bool isOriginalAngleSet = false;
    private float internalShootTimer; // used for actual calculations

	protected override void Start()
    {
        base.Start();
		internalShootTimer = shootTimer*(1 + delaySyncFactor);
        originalAngle = projectileAngle;
        isOriginalAngleSet = true;
	}
	
	
	protected void Update()
    {
		updateRotation(Time.deltaTime);
		internalShootTimer -= Time.deltaTime;
        if (internalShootTimer <= 0)
        {
			fireProjectiles(Time.deltaTime);
            internalShootTimer = shootTimer;
        }
	}



	private void fireProjectiles(float deltaTime)
	{
		base.spawnProjectile();
		int extraShootTimes = (int)(deltaTime / shootTimer) - 1;
        while (extraShootTimes > 0)
        {
			base.spawnProjectile();
        	--extraShootTimes;
        }
	}

    private void updateRotation(float deltaTime)
    {
		if (loopRotationOverride) // only loop rotation
        {
			loopRotationUpdate(deltaTime);
			return;
        }
        if (rotationPerSecond == 0f || maxAngleDiff == 0f) // no need to rotate
        {
            return;
        }

        // Rotate
        if (rotationDirection)
        {
			projectileAngle += rotationPerSecond * deltaTime;
        }
        else if (!rotationDirection)
        {
			projectileAngle -= rotationPerSecond * deltaTime;
        }

        // Change rotation direction
		if (projectileAngle >= originalAngle + maxAngleDiff) // change direction
        {
			projectileAngle = originalAngle + maxAngleDiff; // no more than max angle
			rotationDirection = !rotationDirection;
        }
		else if (projectileAngle <= originalAngle) // change direction
        {
			projectileAngle = originalAngle; // no less than max angle
			rotationDirection = !rotationDirection;
        }
    }

    private void loopRotationUpdate(float deltaTime)
    {
		projectileAngle += rotationPerSecond * deltaTime;

		// avoid overflow (not really needed but ehh)
		if (projectileAngle >= 360f)
		{
			projectileAngle -= 360f;
		}
		else if (projectileAngle < 0f)
		{
			projectileAngle += 360f;
		}
    }





    // Draws the editor gizmo arrows etc.
    protected override void OnDrawGizmos()
    {
        // Starting position:
        Vector3 startingPosition = transform.position;
        if (projectileSpawnPoint == null)
        {
            for (int i = 0; i < transform.childCount; ++i) // find projectileSpawnPoint position
            {
                Transform child = transform.GetChild(i);
                if (child.CompareTag("SpawnPoint"))
                {
                    startingPosition = child.position;
                    break;
                }
            }
        }
        else
        {
            startingPosition = projectileSpawnPoint.position;
        }

        if (!isOriginalAngleSet)
        {
            originalAngle = projectileAngle;
        }

        // Variables used in calculations:
        float radianConversion = Mathf.PI/180;
        float directionAngle = (originalAngle + maxAngleDiff + transform.rotation.eulerAngles.z)*radianConversion;
        float lengthFactor = projectileSpeed/20;
		if (lengthFactor < 0.33f)
		{
			lengthFactor = 0.33f;
		}

        Gizmos.color = Color.red;

        // Max:
        float xDir = Mathf.Cos(directionAngle) * lengthFactor;
        float yDir = Mathf.Sin(directionAngle) * lengthFactor;
        Vector3 direction = new Vector3(xDir, yDir, 0);
        Gizmos.DrawRay(startingPosition, direction);

        // Min:
        directionAngle = (originalAngle + transform.rotation.eulerAngles.z)*radianConversion;
        xDir = Mathf.Cos(directionAngle) * lengthFactor;
        yDir = Mathf.Sin(directionAngle) * lengthFactor;
        direction = new Vector3(xDir, yDir, 0);
        Gizmos.DrawRay(startingPosition, direction);

        base.OnDrawGizmos();
    }


}
