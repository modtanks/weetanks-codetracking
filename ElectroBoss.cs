using System.Collections.Generic;
using UnityEngine;

public class ElectroBoss : MonoBehaviour
{
	public ParticleSystem[] ChargeAttack;

	public ParticleSystem ElectricBlast;

	public float ChargeTimer;

	public float ChargeTimerDuration;

	public bool isAlmostCharged = false;

	public List<ElectricPad> AllPads = new List<ElectricPad>();

	public List<ElectricPad> PadsToActivate = new List<ElectricPad>();

	private bool PlayedSound = false;

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
		GameObject[] Pads = GameObject.FindGameObjectsWithTag("ElectricPad");
		GameObject[] array = Pads;
		foreach (GameObject Pad in array)
		{
			AllPads.Add(Pad.GetComponent<ElectricPad>());
		}
	}

	private void OnDisable()
	{
		ParticleSystem[] chargeAttack = ChargeAttack;
		foreach (ParticleSystem PS in chargeAttack)
		{
			PS.Clear();
			PS.Stop();
		}
		isAlmostCharged = false;
		ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
		PadsToActivate.Clear();
	}

	private void SetBodyMaterial(Material m)
	{
		HeadRender.material = m;
		Material[] mats = BodyRender.materials;
		for (int i = 1; i < mats.Length; i++)
		{
			mats[i] = m;
		}
		BodyRender.materials = mats;
	}

	private void Update()
	{
		if (myHT.health < 1)
		{
			ParticleSystem[] chargeAttack = ChargeAttack;
			foreach (ParticleSystem PS3 in chargeAttack)
			{
				PS3.Clear();
				PS3.Stop();
			}
			isAlmostCharged = false;
			ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
			PadsToActivate.Clear();
			return;
		}
		ChargeTimer -= Time.deltaTime;
		if (ChargeTimer <= 3f && !isAlmostCharged)
		{
			ParticleSystem[] chargeAttack2 = ChargeAttack;
			foreach (ParticleSystem PS2 in chargeAttack2)
			{
				PS2.Clear();
				PS2.Play();
			}
			SetBodyMaterial(GlowyWhiteMaterial);
			isAlmostCharged = true;
			SFXManager.instance.PlaySFX(ElectricChargeSound, 1f, null);
			int extra = GetComponent<HealthTanks>().maxHealth - GetComponent<HealthTanks>().health;
			int difficulty = -5 + OptionsMainMenu.instance.currentDifficulty * 5;
			int RandomActivated = Random.Range(15, 25 + extra + difficulty);
			for (int i = 0; i < RandomActivated; i++)
			{
				int RandomPick = 0;
				do
				{
					RandomPick = Random.Range(0, AllPads.Count);
				}
				while (AllPads[RandomPick].Active || AllPads[RandomPick].ActiveBecauseCloseToBoss);
				PadsToActivate.Add(AllPads[RandomPick]);
				AllPads[RandomPick].picked = true;
				AllPads[RandomPick].SetColorTo(AllPads[RandomPick].PreEmissionColor, 0.45f);
			}
		}
		else if (ChargeTimer <= 0.5f && !PlayedSound)
		{
			SFXManager.instance.PlaySFX(ElectricPulseSound, 1f, null);
			PlayedSound = true;
		}
		else
		{
			if (!(ChargeTimer <= 0f) || AllPads.Count <= 0)
			{
				return;
			}
			foreach (ElectricPad EP in PadsToActivate)
			{
				EP.SetActive(3);
			}
			PadsToActivate.Clear();
			ChargeTimer = ChargeTimerDuration + Random.Range(0f, ChargeTimerDuration);
			isAlmostCharged = false;
			ParticleSystem[] chargeAttack3 = ChargeAttack;
			foreach (ParticleSystem PS in chargeAttack3)
			{
				PS.Clear();
				PS.Stop();
			}
			SetBodyMaterial(OriginalMaterial);
			PlayedSound = false;
			ElectricBlast.Play();
			CameraShake CS = Camera.main.GetComponent<CameraShake>();
			if ((bool)CS)
			{
				CS.StartCoroutine(CS.Shake(0.12f, 0.2f));
			}
			Collider[] bigobjectsInRange = Physics.OverlapSphere(base.transform.position, 12f);
			Collider[] array = bigobjectsInRange;
			foreach (Collider col in array)
			{
				Rigidbody rigi = col.GetComponent<Rigidbody>();
				if (rigi != null && (col.tag == "Player" || col.tag == "Enemy"))
				{
					float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
					float force = (16.8f - distance) * 1f;
					Vector3 direction = rigi.transform.position - base.transform.position;
					rigi.AddForce(direction * force, ForceMode.Impulse);
				}
			}
		}
	}
}
