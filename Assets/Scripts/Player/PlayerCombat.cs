using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerCombat : MonoBehaviour
{
	public int health;
	public float fireRate = 0.1f;
	public float shotGunSpreadRate = 0.5f;
	public int bulletsFiredPerShot;
	public Projectile bulletPrefab;
	public Projectile helperBulletPrefab;
	public DropMine landMinePrefab;
	public Image healthBar;
	
	[SerializeField] private float bulletSpawnOffset;
	[HideInInspector] public int roomNumber;

	private PhotonView photonView;
	private FixedJoystick fixedJoystick;
	private AbilitiesManager abilitiesManager;
	private PlayerMovement playerMovement;
	private Vector3 joystickDirection;
	private bool canShoot;
	private uint bulletCount;
	private bool hasHelperBullet;
	

	void Start()
	{
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WEBGL
		fixedJoystick = GameObject.Find("Right Joystick").GetComponent<FixedJoystick>();
#endif

		photonView = GetComponent<PhotonView>();
		
		playerMovement = GetComponent<PlayerMovement>();

		if (int.TryParse(PhotonNetwork.NickName, out roomNumber))
		{
			print("Room number parsed " + roomNumber);
		}

		joystickDirection = Vector3.forward;
		transform.forward = joystickDirection;
		canShoot = true;

		healthBar.fillAmount = 1.0f;
	}

	private void OnEnable()
	{
		abilitiesManager = GetComponent<AbilitiesManager>();
		hasHelperBullet = (abilitiesManager.passiveSkills == PassiveSkills.HelperBullet);
	}


#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WEBGL
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

		// TODO: Debuging shot gun ability delete after confirming this works
		if (Input.GetButtonDown("Fire2"))
		{

			PlaceDropMine();
		}
	}
#endif


	void Shoot()
	{
		if (photonView.IsMine)
		{
			bool helperBullet = false;
			if (hasHelperBullet)
			{
				bulletCount++;
				if (bulletCount % 3 == 0)
				{
					helperBullet = true;
				}
			}
			int bulletBounces = (abilitiesManager.passiveSkills == PassiveSkills.BouncyBullet) ? 2 : 0;
			bool slowDownBullet = (abilitiesManager.passiveSkills == PassiveSkills.SlowdownBullet) ? true : false;
			photonView.RPC(
				"RPC_SpawnAndInitProjectile",
				RpcTarget.Others,
				new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
				transform.rotation,
				bulletBounces,
				slowDownBullet,
				helperBullet
			);

			Projectile bullet = Instantiate(
				(helperBullet)?helperBulletPrefab:bulletPrefab,
				new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
				transform.rotation
			);
			bullet.ChangeToAllyMaterial();
			bullet.isMyProjectile = true;
			bullet.bounces = bulletBounces;
			bullet.isSlowDownBullet = slowDownBullet;

			Destroy(bullet, 3);
		}

		StartCoroutine("DelayShoot");
	}

	public void ShotgunShoot()
	{
		Quaternion[] shotGunRotations = new Quaternion[bulletsFiredPerShot];
		int middleIndex = Mathf.RoundToInt(shotGunRotations.Length / 2);
		shotGunRotations[0] = transform.rotation;
		shotGunRotations[0].y -= shotGunSpreadRate * middleIndex;
		for (int i = 1; i < shotGunRotations.Length; i++)
		{
			shotGunRotations[i] = shotGunRotations[0];
			shotGunRotations[i].y += shotGunSpreadRate * i;
		}

		if (photonView.IsMine)
		{
			for (int i = 0; i < shotGunRotations.Length; i++)
			{
				bool helperBullet = false;
				if (hasHelperBullet)
				{
					bulletCount++;
					if (bulletCount % 3 == 0)
					{
						helperBullet = true;
					}
				}

				int bulletBounces = (abilitiesManager.passiveSkills == PassiveSkills.BouncyBullet) ? 2 : 0;
				bool slowDownBullet = (abilitiesManager.passiveSkills == PassiveSkills.SlowdownBullet) ? true : false;

				photonView.RPC(
				"RPC_SpawnAndInitProjectile",
				RpcTarget.Others,
				new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
				shotGunRotations[i],
				bulletBounces,
				slowDownBullet,
				helperBullet
				);

				Projectile bullet = Instantiate(
					(helperBullet) ? helperBulletPrefab : bulletPrefab,
					new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset ), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
					shotGunRotations[i]
				);
				bullet.ChangeToAllyMaterial();
				bullet.isMyProjectile = true;
				bullet.bounces = bulletBounces;
				bullet.isSlowDownBullet = slowDownBullet;

				Destroy(bullet, 3);
			}
		}
	}

	IEnumerator DelayShoot()
	{
		yield return new WaitForSeconds(fireRate);
		canShoot = true;
	}

	[PunRPC]
	void RPC_SpawnAndInitProjectile(Vector3 origin, Quaternion quaternion, int bounces, bool isSlowDownBullet, bool isHelperBullet)
	{
		Projectile bullet;
		if (isHelperBullet)
		{
			bullet = Instantiate(helperBulletPrefab, origin, quaternion);
		}
		else
		{
			bullet = Instantiate(bulletPrefab, origin, quaternion);
		}
		
		bullet.isMyProjectile = false;
		bullet.bounces = bounces;
		bullet.isSlowDownBullet = isSlowDownBullet;
	}



	public void PlaceDropMine()
	{
		GameObject g = PhotonNetwork.Instantiate(("PhotonPrefabs/DropMine"),
			new Vector3(transform.position.x + (transform.forward.x * bulletSpawnOffset), transform.position.y, transform.position.z + (transform.forward.z * bulletSpawnOffset)),
				transform.rotation,
				0
				);
		DropMine mine = g.GetComponent<DropMine>();
		mine.roomNumber = roomNumber;
	}	



	public void TakeDamage(int damage, bool isSlowdown)
	{
		if (GameManager.instance != null)
		{
			health -= damage;
			photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health, roomNumber);
			//print(roomNumber.ToString() + " is on " + health + " health");
			if (health <= 0)
			{
				GetComponent<AvatarSetup>()?.Die();
			}
		}
		if (isSlowdown)
		{
			playerMovement.Slowed();
		}
	}

	public void ReplenishHealth(float percentage)
	{
		if (GameManager.instance != null)
		{
			int amount = Mathf.RoundToInt((100 - health) * percentage);

			health += amount;
			if (health > 100)
			{
				health = 100;
			}
			photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health, roomNumber);
			//print(roomNumber.ToString() + " is on " + health + " health");
		}
	}

	[PunRPC]
	void RPC_UpdateHealth(int health, int playerNumber)
	{
		//GameManager.instance.HealthUpdate(health, playerNumber);
		SoundManager.instance.PlaySFX("TakeDamage");
		float fillAmount = health / 100.0f;
		healthBar.fillAmount = fillAmount;
		if (health <= 0)
		{
			GameManager.instance.PlayerDied(playerNumber,roomNumber);
		}

	}

	public void ResetHealth()
	{
		health = 100;
		photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health, roomNumber);
	}
}
