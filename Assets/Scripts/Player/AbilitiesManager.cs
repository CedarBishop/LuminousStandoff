using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement)), DisallowMultipleComponent]
public class AbilitiesManager : MonoBehaviour
{
	private GameObject abilityButton;

	[Header("Abilities Lists")]
	// Enums to work with
	[SerializeField] public PassiveSkills passiveSkills; 		// Our access to PASSIVE skills enum
	[SerializeField] public ActiveSkills activeSkills; 		// Our access to ACTIVE skills enum
	[Space]
	public Ability[] passiveAbilities;	 						// Where ALL PASSIVE abilities objects live
	public Ability[] activeAbilities; 							// Where ALL ACTIVE abilities objects live

	public delegate void AbilityButton();
	public static AbilityButton buttonClickDelegate;

	private delegate void AbilityDelegate();
	private AbilityDelegate methodToCall = null;

	private PhotonView PV;

	[Header("Action Button Tracking")]
	public bool cooldownComplete = true; 						// Waiting for skill cooldown? (True by default - implies skill is ready)
	private Ability currentPassive; 							// Whatever current PASSIVE skill is tied to character this round
	private Ability currentActive; 								// Whatever current ACTIVE skill is tied to character this round

	[Header("Current Stats Section")]
	[SerializeField] private GameObject originalMaterial;		// STEALTH - Current GameObject material we want to change
	private float movementSpeed; 								// SPEED UP - Current velocity/movement speed to increase/decrease

	[Header("Adjustments Section")]
	[SerializeField] [Range(0f, 5f)] private float speedIncreasePercentage; // SPEED UP - Increases damage by a percentage
	[SerializeField] private float maxMovementSpeed;			// SPEED UP - Increases damage by a percentage
	[SerializeField] private int maxBulletBounces; 				// BULLET BOUNCE - Increment how many times a projectable can bounce with trajectory before being destroyed
	[Space]
	private Material currentMaterial; 							// STEALTH - Where to store current material we want to change
	private Material revertMaterial; 							// STEALTH - Where to store original material to go back to
	[SerializeField] private Material stealthMaterial; 			// STEALTH - Assign invisibility shader for Stealth ability
	[SerializeField] private Material stealthActiveMaterial;	// STEALTH - Assign invisibility shader for Stealth ability

	private bool shieldActive;
	[SerializeField] private GameObject shieldEffect;

	private PlayerCombat playerCombat;
	private PlayerRewind playerRewind;

	private void OnEnable()
	{
		PV = GetComponent<PhotonView>();
		movementSpeed = GetComponent<PlayerMovement>().movementSpeed;
		playerCombat = GetComponent<PlayerCombat>();
		playerRewind = GetComponent<PlayerRewind>();

		// Add method as delegate to ability UI button
		AbilityInitiate.OnAbilityClick += ActivateAbility;

		// revertMaterial = originalMaterial.GetComponent<Material>();
		//revertMaterial = originalMaterial.GetComponent<MeshRenderer>().materials[0]; // TODO: Remove! This is only for ghost example prefab

		shieldEffect.SetActive(false);

		// Carry on with passive ability choice IF list is populated
		if (passiveAbilities.Length > 0)
		{
			// Go through list and pick random ability
			//currentPassive = passiveAbilities[UnityEngine.Random.Range(0, passiveAbilities.Length - 1)];
			// currentPassive = passiveAbilities[3]; // TODO: Hard code - get rid of this

			PassiveAbilityProcess(currentPassive);
		}

		// TODO: Hard coded temporarily (TEMPSHIELD)
		//currentActive = activeAbilities[4];
	}

	private void PassiveAbilityProcess(Ability chosenPassive)
	{
		if (chosenPassive != null)
		{
			// Assign enum by current random enum value
			passiveSkills = chosenPassive.passiveSkillId;

			switch (passiveSkills)
			{
				case PassiveSkills.None:
					break;
				case PassiveSkills.BouncyBullet:
					BouncyBullet(maxBulletBounces);
					break;
				case PassiveSkills.HelperBullet:
					break;
				case PassiveSkills.SlowdownBullet:
					break;
				case PassiveSkills.SpeedUp:
					SpeedUp();
					break;
				case PassiveSkills.TriShield:
					break;
				default:
					break;
			}
		}
		else
		{
			throw new NotImplementedException();
		}
	}

	// Bullets that bounce off the environment XX amount of times but destroy on people
	private void BouncyBullet(int bulletBounces)
	{
		bulletBounces++;
		bulletBounces = Mathf.Clamp(bulletBounces, 0, maxBulletBounces);
	}

	private void SpeedUp()
	{
		PlayerMovement playerMovement = GetComponent<PlayerMovement>();
		float moveSpeed = playerMovement.movementSpeed;
		moveSpeed *= speedIncreasePercentage;
		moveSpeed = Mathf.Clamp(moveSpeed, 0f, maxMovementSpeed);
		playerMovement.movementSpeed = moveSpeed;
	}

	public void ActivateAbility()
	{
		// If pressed and active ability assigned ...
		if (cooldownComplete)
		{
			// Disable active abilities until specific cooldown is complete
			switch (activeSkills)
			{
				case ActiveSkills.None:
					break;
				case ActiveSkills.DropMine:
					currentActive = activeAbilities[0];
					methodToCall = playerCombat.PlaceDropMine;
					StartCoroutine(AbilityDuration(currentActive, methodToCall));
					StartCoroutine(AbilityCooldown(currentActive, methodToCall));
					break;
				case ActiveSkills.Rewind:
					currentActive = activeAbilities[1];
					methodToCall = playerRewind.Rewind;
					StartCoroutine(AbilityDuration(currentActive, methodToCall));
					StartCoroutine(AbilityCooldown(currentActive, methodToCall));

					break;
				case ActiveSkills.Shotgun:
					currentActive = activeAbilities[2];
					methodToCall = playerCombat.ShotgunShoot;
					StartCoroutine(AbilityDuration(currentActive, methodToCall));
					StartCoroutine(AbilityCooldown(currentActive, methodToCall));
					break;
				case ActiveSkills.Stealth:
					currentActive = activeAbilities[3];
					methodToCall = Stealth;
					StartCoroutine(AbilityDuration(currentActive, methodToCall));
					StartCoroutine(AbilityCooldown(currentActive, methodToCall));
					break;
				case ActiveSkills.TempShield:
					currentActive = activeAbilities[4];
					methodToCall = TempShield;
					StartCoroutine(AbilityDuration(currentActive, methodToCall));
					StartCoroutine(AbilityCooldown(currentActive, methodToCall));
					break;
				default:
					break;
			}

			methodToCall = null;
		}
	}

	private IEnumerator AbilityDuration(Ability currentActive, AbilityDelegate methodToCall)
	{
		methodToCall(); // Activate skill
		yield return new WaitForSeconds(currentActive.duration);
		methodToCall(); // Deactivate skill
	}

	private IEnumerator AbilityCooldown(Ability currentActive, AbilityDelegate methodToCall)
	{
		cooldownComplete = false; // Deactivate button
		yield return new WaitForSeconds(currentActive.cooldownTime);
		cooldownComplete = true; // Reactivate button
	}

	private void Stealth()
	{
		PV.RPC("RPC_Stealth", RpcTarget.All);
	}

	[PunRPC]
	private void RPC_Stealth()
	{
		if (PV.IsMine)
		{
			if (currentMaterial != stealthActiveMaterial)
			{
				currentMaterial = stealthActiveMaterial;
				originalMaterial.GetComponentInChildren<SkinnedMeshRenderer>().castShadows = false; // TODO: Adapt to character accordingly (uses a SkinnedMeshRenderer instead of MeshRenderer)
			}
			else
			{
				currentMaterial = revertMaterial;
				originalMaterial.GetComponentInChildren<SkinnedMeshRenderer>().castShadows = true; // TODO: Adapt to character accordingly (uses a SkinnedMeshRenderer instead of MeshRenderer)
			}
		}
		else
		{
			if (currentMaterial != stealthMaterial)
			{
				currentMaterial = stealthMaterial;
				originalMaterial.GetComponentInChildren<SkinnedMeshRenderer>().castShadows = false; // TODO: Adapt to character accordingly (uses a SkinnedMeshRenderer instead of MeshRenderer)
			}
			else
			{
				currentMaterial = revertMaterial;
				originalMaterial.GetComponentInChildren<SkinnedMeshRenderer>().castShadows = true; // TODO: Adapt to character accordingly (uses a SkinnedMeshRenderer instead of MeshRenderer)
			}

			originalMaterial.GetComponentInChildren<SkinnedMeshRenderer>().materials[0] = currentMaterial;
		}
	}

	private void TempShield()
	{
		shieldActive = !shieldActive;
		Vector3 shieldFullSize = new Vector3(3, 3, 3); // TODO: For use with interpolating between sizes

		if (shieldActive) // Instantiate & apply effect (growing for active)
		{
			shieldEffect.SetActive(true);
			shieldEffect.transform.localScale = Vector3.Lerp(Vector3.zero, shieldFullSize, 100 * Time.deltaTime);
			Debug.Log("Shield is active");
		}
		else
		{
			// Apply effect & destroy (shrinking then deactivate)
			shieldEffect.transform.localScale = Vector3.Lerp(shieldFullSize, Vector3.zero, 100 * Time.deltaTime);
			shieldEffect.SetActive(false);
			Debug.Log("Shield NOT active");
		}
	}
}
