using UnityEngine;

public class LookTarget : MonoBehaviour
{
	public float distance;

	public bool scaredFace;

	private void OnWillRenderObject()
	{
		if (Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < distance)
		{
			PlayerCamera.main.body.overrideLookTime = 0.5f;
			PlayerCamera.main.body.overrideLookPos = base.transform.position;
			if (scaredFace)
			{
				PlayerCamera.main.body.eyeScareTime = 0.5f;
			}
		}
	}
}
