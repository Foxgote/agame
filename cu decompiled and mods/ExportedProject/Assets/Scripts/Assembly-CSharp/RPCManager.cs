using DiscordRPC;
using UnityEngine;

public class RPCManager : MonoBehaviour
{
	public static RPCManager instance;

	private DiscordRpcClient client;

	private float timer = 5f;

	private Timestamps timest;

	public static void CheckInstance()
	{
		if (!instance)
		{
			new GameObject("RPCManager", typeof(RPCManager));
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		timer -= Time.unscaledDeltaTime;
		if (timer < 0f)
		{
			timer = 5f;
			UpdateRichPresence();
			bool value = Settings.Get<SettingBool>("rpc").value;
			if (value && client == null)
			{
				Initialize();
			}
			if (!value && client != null)
			{
				DestroyClient();
			}
		}
	}

	public void UpdateRichPresence()
	{
		if (client != null)
		{
			client.SetPresence(GetCurrentRichPresence());
		}
	}

	public RichPresence GetCurrentRichPresence()
	{
		string state = "";
		string details = "";
		if ((bool)PreRunScript.instance)
		{
			details = "Preparing for an expedition";
		}
		else if ((bool)WorldGeneration.world)
		{
			details = ((!PlayerCamera.main.body.alive) ? "Deceased" : ((WorldGeneration.world.biomeOverride != WorldGeneration.OverrideSceneType.None) ? "In the labs" : (WorldGeneration.world.biomeDepth switch
			{
				9 => "Exploring layer " + (WorldGeneration.world.biomeDepth + 1), 
				10 => "Ascending layer " + (WorldGeneration.world.biomeDepth + 1), 
				11 => "At the deciding moment", 
				_ => "Descending layer " + (WorldGeneration.world.biomeDepth + 1), 
			})));
		}
		if ((bool)PlayerCamera.main)
		{
			Body body = PlayerCamera.main.body;
			if (body.alive)
			{
				float totalHappiness = body.totalHappiness;
				string text = (body.brainDying ? "In cardiac arrest" : (body.isCriticallyDying ? "On death's door" : (body.isDying ? "Dying" : ((totalHappiness > 90f) ? "Ecstatic" : ((totalHappiness > 70f) ? "Overjoyed" : ((totalHappiness > 50f) ? "Happy" : ((totalHappiness > 30f) ? "Excited" : ((totalHappiness > 10f) ? "Satisfied" : ((totalHappiness > -10f) ? "Steady" : ((totalHappiness > -30f) ? "Uneasy" : ((totalHappiness > -50f) ? "Scared" : ((totalHappiness > -70f) ? "Depressed" : ((!(totalHappiness > -90f)) ? "Suicidal" : "Miserable")))))))))))));
				state = ((!body.brainDying) ? (text + $" ({totalHappiness:0.0} mood)") : (text + " (" + WorldGeneration.world.TotalDepthString() + ")"));
			}
			else
			{
				state = WorldGeneration.world.TotalDepthString();
			}
		}
		return new RichPresence
		{
			Details = details,
			State = state,
			Timestamps = timest,
			Assets = new Assets
			{
				LargeImageKey = "gameicon",
				LargeImageText = ""
			}
		};
	}

	public void Initialize()
	{
		client = new DiscordRpcClient("1506330530394804254");
		client.Initialize();
		client.SetPresence(GetCurrentRichPresence());
		timest = Timestamps.Now;
	}

	public void DestroyClient()
	{
		client?.Dispose();
		client = null;
	}

	private void OnDestroy()
	{
		DestroyClient();
	}
}
