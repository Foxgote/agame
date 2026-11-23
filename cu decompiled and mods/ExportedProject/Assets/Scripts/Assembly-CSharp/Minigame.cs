using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class Minigame
{
	public enum HandSpriteType
	{
		Grasp = 0,
		Point = 1,
		Tweezers = 2,
		Wrench = 3,
		SelfHarm = 4,
		SelfHarmForceOn = 5,
		Blade = 6,
		Lockpick = 7
	}

	public static MinigameBase game => MinigameBase.main;

	public static implicit operator bool(Minigame minigame)
	{
		return minigame != null;
	}

	public abstract HandSpriteType HandType();

	public abstract string GuideLocaleString();

	public abstract bool NeedsItem();

	public virtual float HandRotOffset()
	{
		return 0f;
	}

	public abstract void Start();

	public virtual void PhysicsUpdate(float deltaTime)
	{
	}

	public virtual bool CanExit()
	{
		return true;
	}

	public abstract void Update(List<RaycastResult> uiCasts);
}
