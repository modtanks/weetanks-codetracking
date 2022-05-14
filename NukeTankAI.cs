using System.Collections;
using UnityEngine;

public class NukeTankAI : MonoBehaviour
{
	public Animator NukeButtonAnimator;

	public float DistToTarget;

	public float DistToTargetToArm;

	private EnemyAI EA;

	private HealthTanks HT;

	public bool IsDetonating;

	public bool IsArmed;

	public MeshRenderer AlarmLight;

	public AudioClip LightBeep;

	public AudioClip Activating;

	public ParticleSystem PS;

	public GameObject NukeParticles;

	private void Start()
	{
		HT = GetComponent<HealthTanks>();
		EA = GetComponent<EnemyAI>();
	}

	private IEnumerator Detonate()
	{
		PS.Play();
		SFXManager.instance.PlaySFX(Activating);
		IsDetonating = true;
		EA.TankSpeed = 0f;
		EA.CanMove = false;
		NukeButtonAnimator.SetBool("Detonate", IsDetonating);
		yield return new WaitForSeconds(2.2f);
		HT.health = 0;
	}

	private IEnumerator ArmedLight()
	{
		SFXManager.instance.PlaySFX(LightBeep);
		AlarmLight.material.EnableKeyword("_EMISSION");
		yield return new WaitForSeconds(0.25f);
		AlarmLight.material.DisableKeyword("_EMISSION");
		yield return new WaitForSeconds(0.75f);
		StartCoroutine(ArmedLight());
	}

	private void Update()
	{
		if (EA.ETSN.currentTarget != null && !IsDetonating)
		{
			float num = Vector3.Distance(base.transform.position, EA.ETSN.currentTarget.transform.position);
			if (num < DistToTarget && IsArmed)
			{
				StartCoroutine(Detonate());
			}
			else if (num < DistToTargetToArm && !IsArmed)
			{
				IsArmed = true;
				StartCoroutine(ArmedLight());
			}
		}
	}
}
