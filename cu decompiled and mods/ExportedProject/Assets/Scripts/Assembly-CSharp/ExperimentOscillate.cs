using UnityEngine;

public class ExperimentOscillate : MonoBehaviour
{
	public Sprite[] sprites;

	public float timePerFrame;

	private float tm;

	private int index;

	private void Update()
	{
		tm += Time.deltaTime;
		if (tm > timePerFrame)
		{
			tm -= timePerFrame;
			index++;
			if (index >= sprites.Length)
			{
				index = 0;
			}
			GetComponent<SpriteRenderer>().sprite = sprites[index];
		}
	}
}
