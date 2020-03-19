using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
	PhotonView photonView;
	Rigidbody rigidbody;
	public int damage;
	public float force;
	[HideInInspector] public bool isMyProjectile;
	Vector3 _direction;
	[HideInInspector]public bool isDoubleDamage;
	public Material allyMaterial;
	[HideInInspector]public int bounces;
	[HideInInspector]public bool isSlowDownBullet;

	[SerializeField]
	private ParticleSystem sparks; // For when bouncing off walls

	void Start()
	{
		photonView = GetComponent<PhotonView>();
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.AddForce(force * transform.forward);
		StartCoroutine("DelayedDestroy");
		isDoubleDamage = GameManager.instance.isDoubleDamage;
	}

	IEnumerator DelayedDestroy()
	{
		yield return new WaitForSeconds(5);
		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponentInParent<PlayerCombat>())
		{
			if (isMyProjectile)
			{
				if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine)
				{
					return;
				}
			}

			if (collision.gameObject.GetComponentInParent<PhotonView>())
			{
				if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine)
				{
					collision.gameObject.GetComponentInParent<PlayerCombat>().TakeDamage((isDoubleDamage)?damage *2:damage, isSlowDownBullet);

					print("hit by enemy");
				}
			}
		}

		if (collision.gameObject.CompareTag("Wall"))
		{
			if (sparks != null)
			{
				sparks.Play();
			}

			if (bounces > 0)
			{
				bounces--;
				return;
			}
		}

		Destroy(gameObject);
	}



	public void ChangeToAllyMaterial()
	{
		GetComponent<MeshRenderer>().material = allyMaterial;
	}
}
