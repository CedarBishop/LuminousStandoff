using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAbilities : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (gameObject.CompareTag("Shield"))
		{
			TempShieldProtection(other);
		}
	}

	private void TempShieldProtection(Collider other)
	{
		// Assuming this int ties up with 'Projectiles'
		if (other.gameObject.layer == 8)
		{
			// Check if project is NOT your own
			if (other.GetComponent<Projectile>().isMyProjectile)
			{
				// Destroy that thang
				Destroy(other);
			}
		}
	}
}
