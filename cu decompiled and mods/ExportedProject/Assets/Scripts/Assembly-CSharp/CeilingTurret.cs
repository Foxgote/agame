using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CeilingTurret : MonoBehaviour
{
	public Transform upperGun;

	public Transform lowerGun;

	private BuildingEntity build;

	public SpriteRenderer lights;

	private float gunVelocity;

	private float gunAngle;

	private float desiredAngle;

	private const float minBodyVelocty = 15f;

	private float timeSawMovement;

	public float damageMultiplier;

	private float timeSinceShot;

	public AnimationCurve barrelAnimCurve;

	public bool alwaysSee;

	private AudioSource source;

	public Light2D openLight;

	private Body body => PlayerCamera.main.body;

	private void Start()
	{
		build = GetComponent<BuildingEntity>();
	}

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void OnBecameInvisible()
	{
		source.Stop();
	}

	private void Update()
	{
		if (alwaysSee)
		{
			openLight.intensity = Mathf.PingPong(Time.time * 10f, 1f) * timeSawMovement * 5f;
		}
	}

	private void OnWillRenderObject()
	{
		timeSinceShot += Time.deltaTime;
		lowerGun.transform.localPosition = new Vector2(0f, barrelAnimCurve.Evaluate(timeSinceShot));
		bool flag = true;
		RaycastHit2D[] array = Physics2D.LinecastAll(base.transform.position, body.transform.position, LayerMask.GetMask("Ground"));
		foreach (RaycastHit2D raycastHit2D in array)
		{
			if (raycastHit2D.transform != base.transform)
			{
				flag = false;
				break;
			}
		}
		bool flag2 = false;
		if (flag && (body.rb.velocity.magnitude > 15f || alwaysSee) && body.transform.position.y < base.transform.position.y)
		{
			desiredAngle = Vector2.SignedAngle(Vector2.down, (body.transform.position - base.transform.position).normalized);
			lights.enabled = true;
			timeSawMovement += Time.deltaTime * (alwaysSee ? 0.3f : 1f);
			flag2 = true;
			if (WorldGeneration.world.biomeDepth == 11)
			{
				PlayerCamera.main.threatMusicTime = 2f;
			}
		}
		else
		{
			lights.enabled = Mathf.PingPong(Time.time, 0.2f) > 0.1f;
		}
		if (flag2)
		{
			source.volume = Mathf.MoveTowards(source.volume, 1f, Time.deltaTime * 3f);
		}
		else
		{
			source.volume = Mathf.MoveTowards(source.volume, 0f, Time.deltaTime * 3f);
		}
		if (source.volume > 0f && !source.isPlaying)
		{
			source.Play();
		}
		if (source.volume <= 0f && source.isPlaying)
		{
			source.Stop();
		}
		source.pitch = 0.5f + timeSawMovement * 1.5f;
		float num = Mathf.Clamp(desiredAngle - gunAngle, -30f, 30f);
		gunVelocity += num * Time.deltaTime * 15f;
		gunVelocity *= 1f / (1f + Time.deltaTime);
		gunAngle += gunVelocity * Time.deltaTime;
		gunAngle = Mathf.Clamp(gunAngle, -80f, 80f);
		upperGun.eulerAngles = new Vector3(0f, 0f, gunAngle);
		upperGun.localPosition = upperGun.up * -0.75f;
		if (timeSawMovement > 0.7f && body.conscious)
		{
			timeSawMovement = 0f;
			timeSinceShot = 0f;
			Sound.Play("rifleshot", lowerGun.position, twoDimensional: true, pitchShift: false);
			TurretScript.Shoot(new FireInfo
			{
				pos = lowerGun.position,
				dir = -lowerGun.up,
				ignoreTrans = base.transform,
				playerDamageMultiplier = 1f
			});
		}
	}
}
