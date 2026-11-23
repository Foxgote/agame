using UnityEngine;

public class ColdPuffScript : MonoBehaviour
{
	public bool blown;

	public Sprite openSprite;

	private BuildingEntity build;

	private void Start()
	{
		build = GetComponent<BuildingEntity>();
	}

	private void Update()
	{
		if (!blown && build.health < 150f)
		{
			Blow();
		}
	}

	public void Blow()
	{
		if (!blown)
		{
			blown = true;
			if (Vector2.Distance(PlayerCamera.main.body.transform.position, base.transform.position) < 8f)
			{
				Body body = PlayerCamera.main.body;
				body.temperature -= 8f;
				body.snowAmount = 100f;
				body.talker.Talk("...");
				body.rb.velocity += Vector2.up * 12f;
			}
			build.description = Locale.GetBuilding("coldpuffdsc2");
			Object.Destroy(Utils.Create("Special/ColdPuffParticle", base.transform.position, 0f), 10f);
			Sound.Play("puff", base.transform.position);
			GetComponent<SpriteRenderer>().sprite = openSprite;
		}
	}
}
