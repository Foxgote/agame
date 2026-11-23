using UnityEngine;

internal class CrystalEMP : CrystalEffect
{
	private bool activated;

	private GameObject part;

	public override void Start()
	{
		crystal.SetColor(new Color(0.5f, 0.5f, 0.5f));
		part = Utils.Create("Special/TeslaMini", crystal.transform);
		part.transform.localScale = Vector3.one * crystal.crystalSize;
		part.transform.localPosition = Vector2.zero;
	}

	public void TryEMP()
	{
		if (activated)
		{
			return;
		}
		activated = true;
		foreach (Item item in PlayerCamera.main.body.GetAllItemsThorough())
		{
			if ((bool)item.battery)
			{
				item.battery.DrainCharge(0.33f);
			}
			if (item.Stats is BatteryInfo)
			{
				item.condition -= 0.33f;
			}
		}
		Sound.Play("crystalemp", crystal.transform.position, twoDimensional: true, pitchShift: false);
		PlayerCamera.main.shaker.Shake();
		crystal.SetColor(Color.white);
		part.SetActive(value: false);
	}

	public override void Touched(GameObject touched)
	{
		if (Utils.GetBody(touched, out var _))
		{
			TryEMP();
		}
	}

	public override void Hit(AttackInfo atk)
	{
		TryEMP();
	}

	public override void Update()
	{
		if (activated && crystal.sprite.color.r > 0.05f)
		{
			crystal.sprite.color = Color.Lerp(crystal.sprite.color, Color.black, Time.deltaTime);
		}
	}
}
