using System.Collections;
using UnityEngine;

public abstract class TutorialCourse : MonoBehaviour
{
	public static TutorialHandler tutorial => TutorialHandler.main;

	public static Body body => PlayerCamera.main.body;

	public abstract IEnumerator Sequence();

	public abstract string LocaleName();

	public IEnumerator GiveItemToBody(string item)
	{
		tutorial.objectToCreate = item;
		yield return new WaitUntil(() => tutorial.grabInfo.grabbed);
		tutorial.handPos.y -= 15f;
		yield return new WaitUntil(() => tutorial.handPos == tutorial.handPosCurrent);
		tutorial.handPos = body.transform.position + Vector3.up * 4f;
		yield return new WaitUntil(() => tutorial.handPos == tutorial.handPosCurrent);
		tutorial.Ungrab();
	}

	public IEnumerator PlaceObjectAt(string obj, Vector2 pos)
	{
		tutorial.objectToCreate = obj;
		yield return new WaitUntil(() => tutorial.grabInfo.grabbed);
		tutorial.handPos.y -= 15f;
		yield return new WaitUntil(() => tutorial.handPos == tutorial.handPosCurrent);
		tutorial.handPos = pos;
		yield return new WaitUntil(() => tutorial.handPos == tutorial.handPosCurrent);
		tutorial.Ungrab();
	}

	public IEnumerator PourLiquidAt(byte id, Vector2 pos)
	{
		tutorial.handPos = pos;
		yield return new WaitUntil(() => tutorial.handPos == tutorial.handPosCurrent);
		FluidManager.main.SetLiquid(pos, id);
	}
}
