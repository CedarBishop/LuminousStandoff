using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Create Ability")]
public class Ability : ScriptableObject
{
	[Header("Details")]
	public string description;
	public Sprite icon;
	public PassiveSkills passiveSkillId;
	public ActiveSkills activeSkillId;

	[Header("Stats")]
	public float amount;
	public float maximum;
	public float duration;
	public float cooldownTime;

	[Header("Special FX")]
	public AudioClip spawnSound;
	public AudioClip onHitSound;
	public ParticleSystem spawnFx;
	public ParticleSystem onHitFx;
}
