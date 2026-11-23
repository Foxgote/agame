using UnityEngine;

public class AnimBody : MonoBehaviour
{
	public Body body;

	public void Footstep(int backFoot = 1)
	{
		if (body.standing && body.grounded && Mathf.Abs(body.rb.velocity.x) > 2f)
		{
			body.FootStep(0.5f);
		}
	}

	public void FootstepForced(int backFoot = 1)
	{
		if (body.standing && body.grounded)
		{
			body.FootStep(0.5f);
		}
	}
}
