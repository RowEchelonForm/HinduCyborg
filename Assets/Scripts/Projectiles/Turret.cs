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

	// How much shooting is delayed initially (from 0 to 1 => 0 to 100 %).
	// Used to "desync" turrets.
    [SerializeField] [Range(0f, 1f)]
    private float delaySyncFactor = 0.2f;

    private float internalShootTimer; // used for actual calculations

	protected override void Start()
    {
        base.Start();
		internalShootTimer = shootTimer*(1 + delaySyncFactor);
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
