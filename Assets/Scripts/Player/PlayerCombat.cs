using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerCombat : MonoBehaviour
{
	public int health;
	PhotonView photonView;
	public Projectile bulletPrefab;
	[SerializeField] private float bulletSpawnOffset;
	public int roomNumber;

	private FixedJoystick fixedJoystick;
	public Projectile projectilePrefab;
	public float fireRate = 0.1f;
	Vector3 joystickDirection;
	bool canShoot;

	void Start()
	{
#if UNITY_IPHONE || UNITY_ANDROID
		fixedJoystick = GameObject.Find("Right Joystick").GetComponent<FixedJoystick>();
#endif

		photonView = GetComponent<PhotonView>();

		if (int.TryParse(PhotonNetwork.NickName, out roomNumber))
		{
			print("Room number parsed " + roomNumber);
		}

		joystickDirection = Vector3.forward;
		transform.forward = joystickDirection;
		canShoot = true;
	}
#if UNITY_ANDROID || UNITY_IPHONE
	void Update()
	{

		joystickDirection = new Vector3(fixedJoystick.Horizontal, 0, fixedJoystick.Vertical);

		if (Mathf.Abs(joystickDirection.x) > 0.25f || Mathf.Abs(joystickDirection.z) > 0.25f)
		{
			transform.forward = joystickDirection;
			if (canShoot)
			{
				canShoot = false;
				Shoot();
			}
		}
	}


#elif UNITY_EDITOR || UNITY_STANDALONE
	void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		Physics.Raycast(ray, out hit, 100.0f);

		Vector3 target = new Vector3(hit.point.x, transform.position.y, hit.point.z);
		Vector3 directionToTarget = target - transform.position;

		transform.forward = directionToTarget;

		if (Input.GetButtonDown("Fire1"))
		{
			if (canShoot)
			{
				canShoot = false;
				Shoot();
			}
		}
	}
#endif
	

	void Shoot()
	{
		if (photonView.IsMine)
		{
			photonView.RPC(
				"RPC_SpawnAndInitProjectile",
				RpcTarget.Others,
				new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
				transform.rotation
			);

			Projectile bullet = Instantiate(
				bulletPrefab,
				new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
				transform.rotation
			);
			//bullet.ChangeToAllyMaterial();
			bullet.isMyProjectile = true;

			Destroy(bullet, 3);
		}

		StartCoroutine("DelayShoot");
	}

	IEnumerator DelayShoot()
	{
		yield return new WaitForSeconds(fireRate);
		canShoot = true;
	}

	[PunRPC]
	void RPC_SpawnAndInitProjectile(Vector3 origin, Quaternion quaternion)
	{
		Projectile bullet = Instantiate(bulletPrefab, origin, quaternion);
		bullet.isMyProjectile = false;
	}

	public void TakeDamage(int damage)
	{
		if (UIManager.instance != null)
		{
			health -= damage;
			photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health, roomNumber);
			print(roomNumber.ToString() + " is on " + health + " health");
			if (health <= 0)
			{
				GetComponent<AvatarSetup>()?.Die();
			}
		}
	}

	[PunRPC]
	void RPC_UpdateHealth(int health, int playerNumber)
	{
		UIManager.instance.HealthUpdate(health, playerNumber);
	}

	public void ResetHealth()
	{
		health = 100;
		photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health, roomNumber);
	}
}
