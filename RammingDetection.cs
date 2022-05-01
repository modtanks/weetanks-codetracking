using UnityEngine;

public class RammingDetection : MonoBehaviour
{
	public KingTankScript KTS;

	public bool CheckForZeroSpeed;

	public AudioClip TankImpact;

	[Header("RAM BOOSTING")]
	public AudioSource RamAS;

	public ParticleSystem[] RamParticles;

	public GameObject RamImpactingParticles;

	private void EnableRammingEffect()
	{
		RamAS.volume = KTS.EA.rigi.velocity.magnitude;
		KTS.ETSN.canShoot = false;
		ParticleSystem[] ramParticles = RamParticles;
		foreach (ParticleSystem particleSystem in ramParticles)
		{
			if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
	}

	private void DisableRammingEffect()
	{
		RamAS.volume = 0f;
		ParticleSystem[] ramParticles = RamParticles;
		foreach (ParticleSystem particleSystem in ramParticles)
		{
			if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
		}
	}

	private void Update()
	{
		if (KTS.isRammingMoving)
		{
			if (KTS.EA.rigi.velocity.magnitude > 0.5f)
			{
				EnableRammingEffect();
			}
			else
			{
				DisableRammingEffect();
			}
		}
		else
		{
			DisableRammingEffect();
		}
		if (KTS.isRammingMoving && CheckForZeroSpeed && KTS.EA.rigi.velocity.magnitude < 0.3f)
		{
			KTS.ResetAfterRam();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!KTS.isRamming || !KTS.isRammingMoving)
		{
			return;
		}
		if (collision.transform.tag == "Solid" || collision.transform.tag == "MapBorder")
		{
			KTS.ResetAfterRam();
			if (KTS.EA.rigi.velocity.magnitude > 0.5f)
			{
				CameraShake component = Camera.main.GetComponent<CameraShake>();
				if ((bool)component)
				{
					component.StartCoroutine(component.Shake(0.25f, 0.25f));
				}
				SFXManager.instance.PlaySFX(TankImpact, 1f, null);
				Object.Destroy(Object.Instantiate(RamImpactingParticles, base.transform.position + base.transform.forward * 2f, Quaternion.identity), 3f);
			}
			BoostTower component2 = collision.transform.gameObject.GetComponent<BoostTower>();
			if ((bool)component2)
			{
				component2.DestroyTower();
			}
			DestroyableWall component3 = collision.transform.gameObject.GetComponent<DestroyableWall>();
			if ((bool)component3)
			{
				component3.StartCoroutine(component3.destroy());
			}
		}
		else if (collision.transform.tag == "Player")
		{
			HealthTanks component4 = collision.transform.GetComponent<HealthTanks>();
			if ((bool)component4)
			{
				component4.DamageMe(999);
			}
		}
		else if (collision.transform.tag == "Enemy")
		{
			HealthTanks component5 = collision.transform.GetComponent<HealthTanks>();
			if ((bool)component5)
			{
				component5.DamageMe(999);
			}
		}
	}
}
