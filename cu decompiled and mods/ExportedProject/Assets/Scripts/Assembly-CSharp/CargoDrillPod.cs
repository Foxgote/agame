using UnityEngine;

public class CargoDrillPod : MonoBehaviour
{
	private BuildingEntity bld;

	private bool didReady;

	private bool started;

	private PlayerCamera cam => PlayerCamera.main;

	private Body body => cam.body;

	private void Start()
	{
		bld = GetComponent<BuildingEntity>();
	}

	private void Update()
	{
		if (body.hasCargo && !didReady)
		{
			GetComponent<UsableObject>().toggleString = Locale.GetOther("finaltravel");
			bld.description = Locale.GetBuilding("highpowereddrillpoddsc2");
			Utils.Create("Special/Tesla", base.transform).transform.localScale = Vector3.one * 2f;
			didReady = true;
		}
	}

	public void OnUse()
	{
		if (!didReady)
		{
			body.talker.Talk(Locale.GetCharacter("endpodexamine"));
		}
		else if (!started)
		{
			started = true;
			WorldGeneration.world.StartCoroutine("RegenerateWorld", false);
			Sound.Play("drillpoduse", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true);
		}
	}
}
