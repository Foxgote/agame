using UnityEngine;

public class LifepodController : MonoBehaviour
{
	public SpriteRenderer heatSprite;

	public SpriteRenderer disinfectSprite;

	public int heatState;

	public Color onColor;

	public Color offColor;

	public Color hotColor;

	public Color coldColor;

	public Color baseColor;

	public Heater heater;

	public LifepodShower shower;

	public BuildingEntity heatButton;

	private void Start()
	{
		heatSprite.color = hotColor;
		disinfectSprite.color = onColor;
		heatButton.description = Locale.GetBuilding(heatButton.id + "dsc").Replace("<>", Locale.GetOther("heaterhot"));
	}

	public void ToggleHeatState()
	{
		heatState++;
		if (heatState > 2)
		{
			heatState = 0;
		}
		if (heatState == 0)
		{
			heater.desiredTemp = 39f;
			heater.enabled = true;
			heatSprite.color = hotColor;
			heatButton.description = Locale.GetBuilding(heatButton.id + "dsc").Replace("<>", Locale.GetOther("heaterhot"));
		}
		else if (heatState == 1)
		{
			heater.desiredTemp = 33f;
			heater.enabled = true;
			heatSprite.color = coldColor;
			heatButton.description = Locale.GetBuilding(heatButton.id + "dsc").Replace("<>", Locale.GetOther("heatercold"));
		}
		else
		{
			heater.enabled = false;
			heatSprite.color = baseColor;
			heatButton.description = Locale.GetBuilding(heatButton.id + "dsc").Replace("<>", Locale.GetOther("heateroff"));
		}
	}

	public void ActivateShower()
	{
		shower.Activate();
		disinfectSprite.color = offColor;
	}
}
