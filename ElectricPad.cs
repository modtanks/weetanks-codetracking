using System.Collections;
using UnityEngine;

public class ElectricPad : MonoBehaviour
{
	public bool Active;

	public Color EmissionColor;

	public Color PreEmissionColor;

	public float DistToBoss;

	public GameObject BossObject;

	public HealthTanks HTboss;

	public bool ActiveBecauseCloseToBoss = false;

	public ParticleSystem ElectroParticles;

	public GameObject PlayerOnMe;

	public bool picked;

	public MeshRenderer MR;

	public Color TargetColor;

	public Color lerpedColor;

	public Color currentColor;

	public Color OriginalColor;

	public bool ChangingColor = false;

	public float intensityEmission = 10f;

	public float Duration = 1f;

	public float t = 0f;

	public bool gameOver = false;

	private void Start()
	{
		MR = GetComponent<MeshRenderer>();
		OriginalColor = MR.material.color;
		if ((bool)GameMaster.instance)
		{
			if (GameMaster.instance.CurrentMission == 69)
			{
				InvokeRepeating("ChangeTile", 0.2f, 0.2f);
				InvokeRepeating("CheckForBoss", 0.5f, 0.5f);
				ElectroParticles.Stop();
				HTboss = GameObject.FindGameObjectWithTag("Boss").GetComponent<HealthTanks>();
			}
			else if (!GameMaster.instance.inMapEditor)
			{
				LightUp();
			}
		}
	}

	private void LightUp()
	{
		MR.material.SetColor("_EmissionColor", EmissionColor * intensityEmission);
		if (OptionsMainMenu.instance.currentGraphicSettings > 4)
		{
			ElectroParticles.Play();
		}
		InvokeRepeating("CheckParticleStats", 1f, 1f);
		Active = true;
	}

	private void LightDown()
	{
		MR.material.SetColor("_EmissionColor", OriginalColor);
		ElectroParticles.Stop();
		Active = false;
	}

	private void CheckParticleStats()
	{
		if (!ElectroParticles.isPlaying || ElectroParticles.isStopped)
		{
			ElectroParticles.Clear();
			if (OptionsMainMenu.instance.currentGraphicSettings > 4)
			{
				ElectroParticles.Play();
			}
		}
	}

	private void CheckForBoss()
	{
		if (((bool)HTboss && HTboss.health < 1) || ActiveBecauseCloseToBoss)
		{
			return;
		}
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 2.5f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (col.tag == "Boss")
			{
				BossObject = col.gameObject;
				ActiveBecauseCloseToBoss = true;
			}
		}
	}

	private void Update()
	{
		if ((bool)GameMaster.instance)
		{
			if (OptionsMainMenu.instance.currentGraphicSettings < 5 && ElectroParticles.isPlaying)
			{
				ElectroParticles.Clear();
				ElectroParticles.Stop();
			}
			if (GameMaster.instance.inMapEditor)
			{
				if (!MapEditorMaster.instance.inPlayingMode)
				{
					if (MapEditorMaster.instance.isTesting && !Active)
					{
						LightUp();
					}
					else if (!MapEditorMaster.instance.isTesting && Active)
					{
						LightDown();
					}
				}
				else if (!Active)
				{
					LightUp();
				}
			}
			if (GameMaster.instance.CurrentMission != 69)
			{
				return;
			}
		}
		if (t < 1f)
		{
			t += Time.deltaTime / Duration;
		}
		if (t < 1f)
		{
			lerpedColor = Color.Lerp(currentColor, TargetColor, t);
			MR.material.SetColor("_EmissionColor", lerpedColor * intensityEmission);
		}
		else
		{
			currentColor = TargetColor;
			ChangingColor = false;
		}
		if ((bool)HTboss)
		{
			if (HTboss.health < 1 && !gameOver)
			{
				if (ElectroParticles.isPlaying)
				{
					ElectroParticles.Clear();
					ElectroParticles.Stop();
				}
				SetColorTo(Color.black, 0.3f);
				ChangingColor = true;
				PlayerOnMe = null;
				gameOver = true;
				Active = false;
				picked = false;
				ActiveBecauseCloseToBoss = false;
				return;
			}
			if (gameOver)
			{
				gameOver = false;
			}
		}
		if (!GameMaster.instance.GameHasStarted && (Active || picked))
		{
			PlayerOnMe = null;
			Active = false;
			picked = false;
		}
	}

	public void SetActive(int sec)
	{
		StartCoroutine(ActivateMe(sec));
	}

	private IEnumerator ActivateMe(int sec)
	{
		Active = true;
		yield return new WaitForSeconds(sec);
		Active = false;
		picked = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && (Active || ActiveBecauseCloseToBoss))
		{
			MoveTankScript MTS = other.GetComponent<MoveTankScript>();
			if ((bool)MTS)
			{
				MTS.StunMe(1.5f);
				return;
			}
			EnemyAI AIscript = other.GetComponent<EnemyAI>();
			if ((bool)AIscript)
			{
				AIscript.StunMe(1.5f);
			}
		}
		else if (other.tag == "Enemy" && (Active || ActiveBecauseCloseToBoss) && GameMaster.instance.CurrentMission != 69)
		{
			EnemyAI EA = other.GetComponent<EnemyAI>();
			if ((bool)EA && !EA.isElectric)
			{
				EA.StunMe(1.5f);
			}
		}
		else if (other.tag == "Bullet" && (Active || ActiveBecauseCloseToBoss))
		{
			PlayerBulletScript PBS = other.GetComponent<PlayerBulletScript>();
			if ((bool)PBS && (Active || ActiveBecauseCloseToBoss))
			{
				PBS.ChargeElectric();
			}
		}
		else if (other.tag == "Player")
		{
			PlayerOnMe = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			PlayerOnMe = null;
		}
	}

	public void SetColorTo(Color clr, float speed)
	{
		Duration = speed;
		t = 0f;
		TargetColor = clr;
	}

	private void ChangeTile()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			PlayerOnMe = null;
			Active = false;
		}
		else if ((bool)HTboss && HTboss.health < 1)
		{
			PlayerOnMe = null;
			Active = false;
			picked = false;
			return;
		}
		if (Active || ActiveBecauseCloseToBoss)
		{
			if (!ChangingColor)
			{
				SetColorTo(EmissionColor, 0.15f);
				ChangingColor = true;
			}
			if (ElectroParticles.isStopped && OptionsMainMenu.instance.currentGraphicSettings > 4)
			{
				ElectroParticles.Clear();
				ElectroParticles.Play();
			}
			if ((bool)PlayerOnMe)
			{
				MoveTankScript MTS = PlayerOnMe.GetComponent<MoveTankScript>();
				if ((bool)MTS && !MTS.isStunned)
				{
					MTS.StunMe(2f);
				}
				else
				{
					EnemyAI AIscript = PlayerOnMe.GetComponent<EnemyAI>();
					if ((bool)AIscript && !AIscript.isStunned)
					{
						AIscript.StunMe(2f);
					}
				}
			}
			if (ActiveBecauseCloseToBoss)
			{
				float dist = Vector3.Distance(base.transform.position, BossObject.transform.position);
				if (dist > 5f)
				{
					ActiveBecauseCloseToBoss = false;
				}
			}
		}
		else if (!picked)
		{
			if (ElectroParticles.isPlaying && OptionsMainMenu.instance.currentGraphicSettings > 4)
			{
				ElectroParticles.Clear();
				ElectroParticles.Stop();
			}
			if (!ChangingColor)
			{
				SetColorTo(Color.black, 0.3f);
				ChangingColor = true;
			}
		}
	}
}
