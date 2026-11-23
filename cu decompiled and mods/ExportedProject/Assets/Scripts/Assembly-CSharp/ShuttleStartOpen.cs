using UnityEngine;

public class ShuttleStartOpen : MonoBehaviour
{
	public GameObject[] doors;

	public float origY;

	public float progress;

	private bool activated;

	private bool playedSound;

	private bool didTalk;

	private void Start()
	{
		origY = doors[0].transform.position.y;
	}

	private void Update()
	{
		if (WorldGeneration.world.generatingWorld || !activated)
		{
			return;
		}
		progress += Time.deltaTime;
		GameObject[] array = doors;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.transform.position = new Vector2(gameObject.transform.position.x, Mathf.Lerp(origY, origY + 6f, (progress - 2f) * 0.75f));
			}
		}
		if (progress > 2f && !playedSound)
		{
			playedSound = true;
			Sound.Play("shuttleOpen", base.transform.position, twoDimensional: false, pitchShift: false);
		}
		if (progress > 4f && !didTalk)
		{
			didTalk = true;
			PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("seeGravel"), null, force: true);
		}
		if (progress > 10f)
		{
			Object.Destroy(this);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!activated && (bool)collision.GetComponent<Body>())
		{
			if (Random.Range(0f, 1f) < 0.5f)
			{
				PlayerCamera.main.body.talker.Talk("..?", null, force: true);
			}
			GetComponent<AudioSource>().Stop();
			Sound.Play("shuttleNotice", base.transform.position, twoDimensional: false, pitchShift: false);
			activated = true;
		}
	}
}
