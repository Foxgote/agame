using UnityEngine;

public class CorpseScript : MonoBehaviour
{
	public Sprite[] startSprites;

	private bool didComment;

	public bool animalCorpse;

	public string[] categories;

	private void Awake()
	{
		GetComponent<SpriteRenderer>().sprite = startSprites[Random.Range(0, startSprites.Length)];
		GetComponent<SpriteRenderer>().flipX = Random.Range(0f, 1f) < 0.5f;
	}

	private void Start()
	{
		if (!animalCorpse)
		{
			if (PlayerCamera.main.body.charType == 2)
			{
				GetComponent<BuildingEntity>().cantHit = false;
			}
			int num = 0;
			float num2 = Random.Range(0f, 1f);
			if (num2 > 0.5f)
			{
				num++;
			}
			if (num2 > 0.85f)
			{
				num++;
			}
			if (num2 > 0.95f)
			{
				num++;
			}
			for (int i = 0; i < num; i++)
			{
				string[] array = ItemLootPool.pool[categories[Random.Range(0, categories.Length)]].ToArray();
				GameObject obj = (GameObject)Object.Instantiate(Resources.Load(array[Random.Range(0, array.Length)]), base.transform.position + new Vector3(Random.Range(-3f, 3f), 3f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
				obj.GetComponent<Item>().SetCondition(Random.Range(0f, 1f));
				obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
			}
		}
	}

	private void OnDestroy()
	{
		if (base.gameObject.scene.isLoaded && GetComponent<BuildingEntity>().health <= 0f)
		{
			Sound.Play("gore", base.transform.position);
			Object.Instantiate(Resources.Load("BloodExplosion"), base.transform.position, Quaternion.identity);
			if (PlayerCamera.main.body.charType != 2 && !animalCorpse && Vector2.Distance(PlayerCamera.main.body.transform.position, base.transform.position) < 10f && PlayerCamera.main.body.attackCooldown > 0f)
			{
				PlayerCamera.main.body.happiness -= 5f;
				PlayerCamera.main.body.eyeScareTime = 5f;
				PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("breakcorpse"));
			}
		}
	}

	private void OnWillRenderObject()
	{
		bool flag = Vector2.Distance(PlayerCamera.main.body.transform.position, base.transform.position) < 7f;
		if ((!animalCorpse && !didComment) & flag)
		{
			didComment = true;
			GetComponent<BuildingEntity>().fullName = Locale.GetBuilding("corpseseen");
			if (PlayerCamera.main.body.charType != 2)
			{
				PlayerCamera.main.body.happiness -= 3.5f * PlayerCamera.main.body.desensitizedMult;
				PlayerCamera.main.body.sicknessAmount += 6f * PlayerCamera.main.body.desensitizedMult;
			}
			PlayerCamera.main.body.desensitizedMult *= 0.9f;
			PlayerCamera.main.body.corpsesSeen++;
			if (PlayerCamera.main.body.totalHappiness < -55f)
			{
				PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("seecorpsesuicidal"), null, force: true);
			}
			else if (PlayerCamera.main.body.corpsesSeen < 9)
			{
				PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("seecorpse"));
			}
			else
			{
				PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("seecorpsedesensitized"));
			}
		}
		if (flag)
		{
			PlayerCamera.main.body.overrideLookTime = 0.5f;
			PlayerCamera.main.body.overrideLookPos = base.transform.position;
			if (PlayerCamera.main.body.corpsesSeen < 9 && PlayerCamera.main.body.charType != 2)
			{
				PlayerCamera.main.body.eyeScareTime = 0.5f;
			}
		}
	}
}
