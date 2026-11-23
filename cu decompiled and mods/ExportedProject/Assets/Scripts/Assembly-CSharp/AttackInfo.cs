using UnityEngine;

public class AttackInfo
{
	public float damage;

	public float knockBack;

	public float distance;

	public GameObject attackAnim;

	public float structuralDamage;

	public float cooldown;

	public float staminaUse;

	public bool piercing;

	public string[] swingSounds;

	public float volume;

	public bool physicalSwing = true;

	public float rotateAmount;

	public bool doAttackAnim = true;

	public bool metalMoreDamage;

	public float attackCooldownMult = 1f;

	public bool unarmed;
}
