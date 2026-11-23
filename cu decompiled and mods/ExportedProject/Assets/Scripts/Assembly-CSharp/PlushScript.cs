using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
[Saveable]
public class PlushScript : MonoBehaviour
{
	public Sprite[] possibleSprites;

	public AudioClip[] possibleSounds;

	public AudioClip selectedSound;

	[JsonProperty]
	public int index = -1;

	public void Squeak()
	{
		Sound.Play(selectedSound, base.transform.position, twoDimensional: false, pitchShift: false);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.relativeVelocity.magnitude > 2f)
		{
			Squeak();
		}
	}

	private void Start()
	{
		if (index < 0)
		{
			index = Random.Range(0, possibleSprites.Length);
		}
		GetComponent<SpriteRenderer>().sprite = possibleSprites[index];
		selectedSound = possibleSounds[index];
	}
}
