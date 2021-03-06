using System.Collections.Generic;
using UnityEngine;

public class ElectroBoss : MonoBehaviour
{
	public ParticleSystem[] ChargeAttack;

	public ParticleSystem ElectricBlast;

	public float ChargeTimer;

	public float ChargeTimerDuration;

	public bool isAlmostCharged;

	public List<ElectricPad> AllPads = new List<ElectricPad>();

	public List<ElectricPad> PadsToActivate = new List<ElectricPad>();

	private bool PlayedSound;

	public AudioClip ElectricPulseSound;

	public AudioClip ElectricChargeSound;

	private HealthTanks myHT;

	public Material GlowyWhiteMaterial;

	public Material OriginalMaterial;

	public MeshRenderer HeadRender;

	public MeshRenderer BodyRender;

	private void Start()
	{
		ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
		myHT = GetComponent<HealthTanks>();
		if (GameMaster.instance.isOfficialCampaign)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("ElectricPad");
			foreach (GameObject gameObject in array)
			{
				AllPads.Add(gameObject.GetComponent<ElectricPad>());
			}
		}
	}

	private void OnEnable()
	{
		if (GameMaster.instance.isOfficialCampaign)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("ElectricPad");
			foreach (GameObject gameObject in array)
			{
				AllPads.Add(gameObject.GetComponent<ElectricPad>());
			}
		}
	}

	private void OnDisable()
	{
		ParticleSystem[] chargeAttack = ChargeAttack;
		foreach (ParticleSystem obj in chargeAttack)
		{
			obj.Clear();
			obj.Stop();
		}
		isAlmostCharged = false;
		ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
		PadsToActivate.Clear();
	}

	private void SetBodyMaterial(Material m)
	{
		HeadRender.material = m;
		Material[] materials = BodyRender.materials;
		for (int i = 1; i < materials.Length; i++)
		{
			materials[i] = m;
		}
		BodyRender.materials = materials;
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
			isAlmostCharged = false;
			SetBodyMaterial(OriginalMaterial);
			return;
		}
		if (myHT.health < 1)
		{
			ParticleSystem[] chargeAttack = ChargeAttack;
			foreach (ParticleSystem obj in chargeAttack)
			{
				obj.Clear();
				obj.Stop();
			}
			isAlmostCharged = false;
			ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
			PadsToActivate.Clear();
			return;
		}
		ChargeTimer -= Time.deltaTime;
		if (ChargeTimer <= 3f && !isAlmostCharged)
		{
			ParticleSystem[] chargeAttack = ChargeAttack;
			foreach (ParticleSystem obj2 in chargeAttack)
			{
				obj2.Clear();
				obj2.Play();
			}
			SetBodyMaterial(GlowyWhiteMaterial);
			isAlmostCharged = true;
			SFXManager.instance.PlaySFX(ElectricChargeSound, 1f, null);
			int num = GetComponent<HealthTanks>().maxHealth - GetComponent<HealthTanks>().health;
			int num2 = -5 + OptionsMainMenu.instance.currentDifficulty * 5;
			int num3 = Random.Range(15, 25 + num + num2);
			if (!GameMaster.instance.isOfficialCampaign || AllPads.Count <= 0)
			{
				return;
			}
			for (int j = 0; j < num3; j++)
			{
				int num4 = 0;
				do
				{
					num4 = Random.Range(0, AllPads.Count);
				}
				while (AllPads[num4].Active || AllPads[num4].ActiveBecauseCloseToBoss);
				PadsToActivate.Add(AllPads[num4]);
				AllPads[num4].picked = true;
				AllPads[num4].SetColorTo(AllPads[num4].PreEmissionColor, 0.45f);
			}
		}
		else if (ChargeTimer <= 0.5f && !PlayedSound)
		{
			SFXManager.instance.PlaySFX(ElectricPulseSound, 1f, null);
			PlayedSound = true;
		}
		else
		{
			if (!(ChargeTimer <= 0f))
			{
				return;
			}
			ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
			isAlmostCharged = false;
			ParticleSystem[] chargeAttack = ChargeAttack;
			foreach (ParticleSystem obj3 in chargeAttack)
			{
				obj3.Clear();
				obj3.Stop();
			}
			PlayedSound = false;
			ElectricBlast.Play();
			SetBodyMaterial(OriginalMaterial);
			CameraShake component = Camera.main.GetComponent<CameraShake>();
			if ((bool)component)
			{
				component.StartCoroutine(component.Shake(0.12f, 0.2f));
			}
			Collider[] array = Physics.OverlapSphere(base.transform.position, 12f);
			foreach (Collider collider in array)
			{
				Rigidbody component2 = collider.GetComponent<Rigidbody>();
				if (component2 != null && (collider.tag == "Player" || collider.tag == "Enemy"))
				{
					float num5 = Vector3.Distance(component2.transform.position, base.transform.position);
					float num6 = (16.8f - num5) * 1f;
					Vector3 vector = component2.transform.position - base.transform.position;
					component2.AddForce(vector * num6, ForceMode.Impulse);
				}
			}
			if (!GameMaster.instance.isOfficialCampaign || AllPads.Count <= 0)
			{
				return;
			}
			foreach (ElectricPad item in PadsToActivate)
			{
				item.SetActive(3);
			}
			PadsToActivate.Clear();
		}
	}
}
