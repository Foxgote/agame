using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
	public SongList[] biomeMusic;

	public AudioClip dyingClip;

	public AudioClip deadClip;

	private AudioSource source;

	[HideInInspector]
	public float timeSinceFinishedPlaying;

	public AudioMixer mixer;

	[SerializeField]
	private float lowCutoff;

	public float origVolume;

	public float droneVolume;

	public static MusicManager main;

	public float trackInterval;

	public bool sadnessDistortion;

	private List<AudioSource> droneSources = new List<AudioSource>();

	public List<DroneTrack> droneTracks;

	private float maxDroneAmount;

	private bool playedDead;

	private AudioClip lastSong;

	public AnimationCurve sadFlangeWet;

	public AnimationCurve sadFlangeDry;

	public AnimationCurve sadFlangeDepth;

	public AnimationCurve sadEchoWet;

	public AnimationCurve sadDistort;

	public AudioSource critSource;

	public AudioSource critSourceUnc;

	public float cutsceneVolMult = 1f;

	public bool specialLoop;

	private Body body => PlayerCamera.main.body;

	public void StopDrone(int index)
	{
		droneSources[index].volume = 0f;
	}

	public void StartDrone(int index)
	{
		droneSources[index].volume = 1f;
	}

	private void Awake()
	{
		main = this;
	}

	public void SkipTime(float amount)
	{
		if (!source.clip || !source.isPlaying)
		{
			throw new Exception("Can't skip time when no music track is playing!");
		}
		float num = source.time + amount;
		if (num >= source.clip.length)
		{
			num = source.clip.length - 1f;
		}
		source.time = num;
	}

	private void Start()
	{
		source = GetComponent<AudioSource>();
		lowCutoff = 30000f;
		origVolume = Settings.Get<SettingFloat>("musicvolume").value;
		droneVolume = Settings.Get<SettingFloat>("dronevolume").value;
		sadnessDistortion = Settings.Get<SettingBool>("sadnessdistortion").value;
		trackInterval = Settings.Get<SettingFloat>("trackinterval").value;
		for (int i = 0; i < droneTracks.Count; i++)
		{
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.bypassReverbZones = true;
			audioSource.dopplerLevel = 0f;
			audioSource.spatialBlend = 0f;
			audioSource.loop = true;
			audioSource.playOnAwake = false;
			audioSource.clip = droneTracks[i].clip;
			droneSources.Add(audioSource);
		}
		if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
		{
			biomeMusic[0].songs = new AudioClip[1] { Resources.Load<AudioClip>("Sounds/music/THETUTORIAL") };
		}
	}

	public void PlaySong(AudioClip song)
	{
		source.Stop();
		source.clip = song;
		timeSinceFinishedPlaying = 0f;
		source.Play();
	}

	public void StopSong()
	{
		source.Stop();
	}

	public void PlayLayerSong()
	{
		AudioClip audioClip;
		do
		{
			audioClip = biomeMusic[WorldGeneration.world.biomeDepth].songs.PickRandom();
		}
		while (!(audioClip != lastSong) && biomeMusic[WorldGeneration.world.biomeDepth].songs.Length >= 2);
		PlaySong(audioClip);
		lastSong = audioClip;
	}

	private void Update()
	{
		if (!source.isPlaying)
		{
			timeSinceFinishedPlaying += Time.unscaledDeltaTime;
		}
		source.loop = (WorldGeneration.world.biomeDepth == 9 || specialLoop) && body.alive;
		float num = Time.deltaTime;
		if ((bool)SettingsMenu.instance)
		{
			num = Time.unscaledDeltaTime;
		}
		if (PlayerCamera.main.body.alive && timeSinceFinishedPlaying > trackInterval && !body.mindWipe && WorldGeneration.world.biomeOverride != WorldGeneration.OverrideSceneType.Tutorial && WorldGeneration.world.biomeDepth != 10 && WorldGeneration.world.biomeDepth != 11)
		{
			PlayLayerSong();
		}
		if ((PlayerCamera.main.body.conscious && !PlayerCamera.main.body.inWater) || body.brainDying || !PlayerCamera.main.body.alive)
		{
			lowCutoff = Mathf.MoveTowards(lowCutoff, Mathf.Lerp(30000f, 250f, maxDroneAmount), num * 4000f);
		}
		else
		{
			lowCutoff = Mathf.MoveTowards(lowCutoff, Mathf.Lerp(300f, 250f, maxDroneAmount), num * 8000f);
		}
		mixer.SetFloat("LowCutoff", lowCutoff);
		if (body.alive && body.brainDying)
		{
			if (source.clip != dyingClip || !source.isPlaying)
			{
				PlaySong(dyingClip);
			}
		}
		else if (!PlayerCamera.main.body.alive && !playedDead)
		{
			playedDead = true;
			PlaySong(deadClip);
		}
		else if (source.clip == dyingClip)
		{
			source.Stop();
		}
		if (playedDead && PlayerCamera.main.body.alive)
		{
			playedDead = false;
		}
		float target = 0.07f;
		if (!source.isPlaying && maxDroneAmount < 0.5f)
		{
			target = 0.38f;
		}
		bool[] array = new bool[droneSources.Count];
		float[] array2 = new float[droneSources.Count];
		if (PlayerCamera.main.body.averagePain > 60f)
		{
			array[0] = true;
			array2[0] = Mathf.Clamp01(PlayerCamera.main.body.averagePain.Remap(60f, 85f, 0.2f, 1f));
		}
		if (body.totalHappiness <= -81.5f)
		{
			array[6] = true;
			array2[6] = 1f;
		}
		else if (body.isDying && !PlayerCamera.main.lastStandPanel.activeSelf && WorldGeneration.world.biomeOverride != WorldGeneration.OverrideSceneType.Tutorial)
		{
			array[1] = true;
			array2[1] = 1f;
		}
		if (body.stamina < 50f)
		{
			array[2] = true;
			array2[2] = 1f - body.stamina * 0.02f;
		}
		if (body.energy < 25f)
		{
			array[3] = true;
			array2[3] = 1f - body.energy * 0.04f;
		}
		if (PlayerCamera.main.irradiateIntensity > 0f && !WorldGeneration.unchipped)
		{
			array[4] = true;
			array2[4] = Mathf.Clamp01(PlayerCamera.main.irradiateIntensity * 3.5f);
		}
		if (PlayerCamera.main.threatMusicTime > 0f && WorldGeneration.world.biomeDepth != 11)
		{
			array[PlayerCamera.main.currentThreatTheme] = true;
			array2[PlayerCamera.main.currentThreatTheme] = 1f;
		}
		if (body.inWater && body.bloodOxygen < 60f && !body.hasScubaGear)
		{
			array[8] = true;
			array2[8] = Body.Remap(body.bloodOxygen, 60f, 20f, 0f, 1f);
		}
		if (WorldGeneration.world.earthquakeIntensity > 0f && WorldGeneration.world.biomeDepth != 9)
		{
			array[13] = true;
			array2[13] = WorldGeneration.world.earthquakeIntensity;
		}
		if (RadiationLine.line.active && RadiationLine.line.transform.position.y - body.transform.position.y < 75f)
		{
			array[14] = true;
			array2[14] = 1f - (RadiationLine.line.transform.position.y - body.transform.position.y) / 75f;
		}
		if (PlayerCamera.main.tradeMenu.activeSelf)
		{
			array[16] = true;
			array2[16] = 1f;
		}
		for (int i = 0; i < droneSources.Count; i++)
		{
			if (array[i] && !body.brainDying && PlayerCamera.main.body.alive && !body.mindWipe && PlayerCamera.main.globalMuteTime <= 2f && !WorldGeneration.world.cutsceneActive)
			{
				if (!droneSources[i].isPlaying)
				{
					droneSources[i].Play();
				}
				droneSources[i].volume = Mathf.MoveTowards(droneSources[i].volume, array2[i] * droneVolume, num / droneTracks[i].timeToFadeIn);
			}
			else
			{
				if (droneSources[i].volume <= 0f && droneSources[i].isPlaying)
				{
					droneSources[i].Pause();
				}
				droneSources[i].volume = Mathf.MoveTowards(droneSources[i].volume, 0f, num / droneTracks[i].timeToFadeOut);
			}
		}
		maxDroneAmount = droneSources.Max((AudioSource x) => x.volume) / ((droneVolume > 0.01f) ? droneVolume : 9999999f);
		WorldGeneration.world.caveAudio.volume = Mathf.MoveTowards(WorldGeneration.world.caveAudio.volume, target, num * 0.1f);
		source.volume = origVolume * ((PlayerCamera.main.body.harmer.timeWasStill > 20f) ? Mathf.Clamp01((30f - PlayerCamera.main.body.harmer.timeWasStill) * 0.1f) : 1f) * (1f - maxDroneAmount * 0.85f) * cutsceneVolMult;
		float time = (sadnessDistortion ? Mathf.Clamp01((0f - body.totalHappiness) * 0.01f) : 0f);
		mixer.SetFloat("EchoWet", sadEchoWet.Evaluate(time));
		mixer.SetFloat("Distort", sadDistort.Evaluate(time));
		mixer.SetFloat("FlangeDepth", sadFlangeDepth.Evaluate(time));
		mixer.SetFloat("FlangeWet", sadFlangeWet.Evaluate(time));
		mixer.SetFloat("FlangeDry", sadFlangeDry.Evaluate(time));
		mixer.SetFloat("Volume", PlayerCamera.main.muteTimeVolume);
		if (body.isCriticallyDying && !body.brainDying && body.heartRate > 5f)
		{
			float num2 = (body.conscious ? 1f : 0f);
			if (!critSource.isPlaying)
			{
				critSource.Play();
			}
			if (!critSourceUnc.isPlaying)
			{
				critSourceUnc.Play();
			}
			critSource.volume = Mathf.MoveTowards(critSource.volume, num2 * droneVolume, Time.unscaledDeltaTime * 0.5f);
			critSourceUnc.volume = Mathf.MoveTowards(critSourceUnc.volume, (1f - num2) * droneVolume, Time.unscaledDeltaTime * 0.5f);
		}
		else
		{
			critSource.volume = Mathf.MoveTowards(critSource.volume, 0f, Time.unscaledDeltaTime * 0.5f);
			critSourceUnc.volume = Mathf.MoveTowards(critSourceUnc.volume, 0f, Time.unscaledDeltaTime * 0.5f);
			if (critSource.volume == 0f)
			{
				critSource.Stop();
			}
			if (critSourceUnc.volume == 0f)
			{
				critSource.Stop();
			}
		}
	}

	public IEnumerator AscentMusic()
	{
		source.Stop();
		AudioSource[] sources = new AudioSource[4];
		float[] fades = new float[4];
		for (int i = 0; i < 4; i++)
		{
			sources[i] = base.gameObject.AddComponent<AudioSource>();
			sources[i].bypassReverbZones = true;
			sources[i].dopplerLevel = 0f;
			sources[i].spatialBlend = 0f;
			sources[i].loop = true;
			sources[i].playOnAwake = false;
			sources[i].volume = 0f;
			sources[i].clip = biomeMusic[10].songs[i];
		}
		AudioSource[] array = sources;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Play();
		}
		while (true)
		{
			if (WorldGeneration.world.biomeDepth == 10 && body.conscious)
			{
				float num = body.transform.position.y.Remap(0L - (long)WorldGeneration.world.halfHeight, WorldGeneration.world.halfHeight, 0f, 1f);
				int num2 = 0;
				if (num > 0.3f)
				{
					num2 = 1;
				}
				if (num > 0.6f)
				{
					num2 = 2;
				}
				if (num > 0.9f)
				{
					num2 = 3;
				}
				for (int k = 0; k < 4; k++)
				{
					float target = ((k == num2 && !WorldGeneration.world.cutsceneActive && body.alive && !body.brainDying) ? 1f : 0f);
					fades[k] = Mathf.MoveTowards(fades[k], target, 0.33f * Time.unscaledDeltaTime);
				}
			}
			else
			{
				for (int l = 0; l < 4; l++)
				{
					fades[l] = Mathf.MoveTowards(fades[l], 0f, 0.33f * Time.unscaledDeltaTime);
				}
			}
			for (int m = 0; m < 4; m++)
			{
				sources[m].volume = fades[m] * Utils.OutQuad(origVolume) * cutsceneVolMult * (1f - maxDroneAmount * 0.85f);
			}
			if (WorldGeneration.world.biomeDepth != 10)
			{
				break;
			}
			yield return null;
		}
		array = sources;
		for (int j = 0; j < array.Length; j++)
		{
			UnityEngine.Object.Destroy(array[j]);
		}
	}

	public IEnumerator EscapeMusic()
	{
		source.Stop();
		AudioSource[] sources = new AudioSource[2];
		float[] fades = new float[2];
		for (int i = 0; i < 2; i++)
		{
			sources[i] = base.gameObject.AddComponent<AudioSource>();
			sources[i].bypassReverbZones = true;
			sources[i].dopplerLevel = 0f;
			sources[i].spatialBlend = 0f;
			sources[i].loop = true;
			sources[i].playOnAwake = false;
			sources[i].volume = 0f;
			sources[i].clip = biomeMusic[11].songs[i + 1];
		}
		fades[0] = 1f;
		AudioSource[] array = sources;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Play();
		}
		while (WorldGeneration.world.biomeDepth == 11 && !EndLabScript.instance.doTimer)
		{
			if (body.conscious)
			{
				for (int k = 0; k < 2; k++)
				{
					float target = ((k == ((PlayerCamera.main.threatMusicTime > 0f) ? 1 : 0) && !WorldGeneration.world.cutsceneActive && body.alive && !body.brainDying) ? 1f : 0f);
					fades[k] = Mathf.MoveTowards(fades[k], target, 0.5f * Time.unscaledDeltaTime);
				}
			}
			else
			{
				for (int l = 0; l < 2; l++)
				{
					fades[l] = Mathf.MoveTowards(fades[l], 0f, 0.33f * Time.unscaledDeltaTime);
				}
			}
			for (int m = 0; m < 2; m++)
			{
				sources[m].volume = fades[m] * Utils.OutQuad(origVolume) * cutsceneVolMult;
			}
			yield return null;
		}
		array = sources;
		for (int j = 0; j < array.Length; j++)
		{
			UnityEngine.Object.Destroy(array[j]);
		}
	}
}
