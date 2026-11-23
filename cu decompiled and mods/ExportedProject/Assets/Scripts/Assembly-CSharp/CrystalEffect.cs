using UnityEngine;

public abstract class CrystalEffect
{
	public CrystalBehaviour crystal;

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void Touched(GameObject touched)
	{
	}

	public virtual void Hit(AttackInfo atk)
	{
	}
}
