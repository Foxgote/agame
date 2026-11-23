using UnityEngine;

public class ElderThornbackBehaviour : MonoBehaviour
{
	private float timeChecked;

	public static float maxDistance = 45f;

	private SpiderHandler me;

	private BuildingEntity build;

	private Material mat;

	private int stage;

	private Body body => PlayerCamera.main.body;

	private void Start()
	{
		me = GetComponent<SpiderHandler>();
		build = GetComponent<BuildingEntity>();
		mat = new Material(GetComponent<SpriteRenderer>().sharedMaterial);
		GetComponent<SpriteRenderer>().sharedMaterial = mat;
		WorldGeneration.world.GenerateBlockCircle(base.transform.position, 8, 0, 1f, 1f, autoUpdateChunk: true, force: true);
	}

	private void OnDestroy()
	{
		body.horrifiedLevel = 0f;
		if (build.health <= 0f && Vector2.Distance(base.transform.position, body.transform.position) < maxDistance)
		{
			body.happiness += 40f;
			body.caffeinated += 600f;
			MusicManager.main.StopDrone(15);
			Sound.Play("music/thornbackDefeat", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			MusicManager.main.StopSong();
			PlayerCamera.main.threatMusicTime = 0f;
			SteamManager.TryUnlockAchievement("TOPOFTHEFOODCHAIN");
		}
	}

	private void Update()
	{
		if (!(Time.time - timeChecked > 1f))
		{
			return;
		}
		timeChecked = Time.time;
		bool num = Vector2.Distance(base.transform.position, body.transform.position) < maxDistance;
		bool flag = Vector2.Distance(base.transform.position, body.transform.position) < maxDistance * 2.25f;
		if (num)
		{
			if (body.sleeping)
			{
				body.WakeUp();
			}
			body.energy = Mathf.Max(body.energy, 15f);
			body.adrenaline = 50f;
			int droneToPlay = ((build.health < 3750f) ? 15 : 10);
			me.droneToPlay = droneToPlay;
			me.PlayThreatMusic();
			body.horrifiedLevel = 100f;
			body.stamina++;
			if (stage != 0)
			{
				body.focusedLevel = 100f;
			}
		}
		else if (flag)
		{
			body.adrenaline = 100f;
			if (body.horrifiedLevel < 50f)
			{
				body.horrifiedLevel = 50f;
			}
			if (body.sleeping)
			{
				body.WakeUp();
				body.energy = Mathf.Max(body.energy, 10f);
			}
		}
		if (build.health < 3750f && stage == 0)
		{
			stage = 1;
			me.legSpeed++;
			me.moveForce *= 1.35f;
			me.moveWaitTime *= 0.6f;
			me.retreatMoveTime = 4f;
			me.droneToPlay = 15;
			MusicManager.main.StopDrone(10);
			MusicManager.main.StartDrone(15);
			me.PlayThreatMusic();
		}
		if (build.health < 2500f && stage == 1)
		{
			stage = 2;
			me.legSpeed++;
			me.moveForce *= 1.25f;
			me.moveWaitTime *= 0.5f;
			me.retreatHealth = 0f;
			me.lookForceMult *= 1.8f;
			me.retreatMoveTime = 4f;
		}
	}

	private void OnWillRenderObject()
	{
		mat.SetFloat("_Progress", (6000f - build.health) / 6000f * 8f);
	}
}
