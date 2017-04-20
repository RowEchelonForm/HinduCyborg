using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A turret object that shoots projectiles with a timer.
*/
public class Turret : ProjectileSpawner
{

    [SerializeField] [Range(0f, 10f)]
    private float shootTimer = 1f;

    private float internalShootTimer; // used for actual calculations

	protected override void Start()
    {
        base.Start();
        internalShootTimer = shootTimer;
	}
	
	
	protected void Update()
    {
        if (internalShootTimer <= 0)
        {
            base.spawnProjectile();
            internalShootTimer = shootTimer;
        }
        internalShootTimer -= Time.deltaTime;
	}




}
