using UnityEngine;

public class HookPoint : MonoBehaviour
{
	public GrapplingHook hook;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 6)
		{
			hook.HookHit();
		}
	}
}
