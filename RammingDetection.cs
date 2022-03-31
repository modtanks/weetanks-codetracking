using UnityEngine;

public class RammingDetection : MonoBehaviour
{
	public KingTankScript KTS;

	public bool CheckForZeroSpeed = false;

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
		foreach (ParticleSystem PS in ramParticles)
		{
			if (!PS.isPlaying)
			{
				PS.Play();
			}
		}
	}

	private void DisableRammingEffect()
	{
		RamAS.volume = 0f;
		ParticleSystem[] ramParticles = RamParticles;
		foreach (ParticleSystem PS in ramParticles)
		{
			if (PS.isPlaying)
			{
				PS.Stop();
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
				CameraShake CS = Camera.main.GetComponent<CameraShake>();
				if ((bool)CS)
				{
					CS.StartCoroutine(CS.Shake(0.25f, 0.25f));
				}
				SFXManager.instance.PlaySFX(TankImpact, 1f, null);
				GameObject Impact = Object.Instantiate(RamImpactingParticles, base.transform.position + base.transform.forward * 2f, Quaternion.identity);
				Object.Destroy(Impact, 3f);
			}
			BoostTower BT = collision.transform.gameObject.GetComponent<BoostTower>();
			if ((bool)BT)
			{
				BT.DestroyTower();
			}
			DestroyableWall DW = collision.transform.gameObject.GetComponent<DestroyableWall>();
			if ((bool)DW)
			{
				DW.StartCoroutine(DW.destroy());
			}
		}
		else if (collision.transform.tag == "Player")
		{
			HealthTanks HT = collision.transform.GetComponent<HealthTanks>();
			if ((bool)HT)
			{
				HT.DamageMe(999);
			}
		}
		else if (collision.transform.tag == "Enemy")
		{
			HealthTanks HT2 = collision.transform.GetComponent<HealthTanks>();
			if ((bool)HT2)
			{
				HT2.DamageMe(999);
			}
		}
	}
}
