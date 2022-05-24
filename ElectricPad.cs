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

	public bool ActiveBecauseCloseToBoss;

	public ParticleSystem ElectroParticles;

	public GameObject PlayerOnMe;

	public bool picked;

	public MeshRenderer MR;

	public Color TargetColor;

	public Color lerpedColor;

	public Color currentColor;

	public Color OriginalColor;

	public bool ChangingColor;

	public float intensityEmission = 10f;

	public float Duration = 1f;

	public float t;

	public bool gameOver;

	private void Start()
	{
		MR = GetComponent<MeshRenderer>();
		OriginalColor = MR.material.color;
		if (!GameMaster.instance.NightLevels.Contains(GameMaster.instance.CurrentMission))
		{
			intensityEmission = 0.9f;
		}
		if ((bool)GameMaster.instance)
		{
			if (GameMaster.instance.CurrentMission == 69 && GameMaster.instance.isOfficialCampaign)
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
		MR.material.EnableKeyword("_EMISSION");
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
		Collider[] array = Physics.OverlapSphere(base.transform.position, 2.5f);
		foreach (Collider collider in array)
		{
			if (collider.tag == "Boss")
			{
				BossObject = collider.gameObject;
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
			if (GameMaster.instance.CurrentMission != 69 || !GameMaster.instance.isOfficialCampaign)
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
			MoveTankScript component = other.GetComponent<MoveTankScript>();
			if ((bool)component)
			{
				component.StunMe(1.5f);
				return;
			}
			EnemyAI component2 = other.GetComponent<EnemyAI>();
			if ((bool)component2)
			{
				component2.StunMe(1.5f);
			}
		}
		else if (other.tag == "Enemy" && (Active || ActiveBecauseCloseToBoss) && GameMaster.instance.CurrentMission != 69)
		{
			EnemyAI component3 = other.GetComponent<EnemyAI>();
			if ((bool)component3 && !component3.isElectric)
			{
				component3.StunMe(1.5f);
			}
		}
		else if (other.tag == "Bullet" && (Active || ActiveBecauseCloseToBoss))
		{
			PlayerBulletScript component4 = other.GetComponent<PlayerBulletScript>();
			if ((bool)component4 && (Active || ActiveBecauseCloseToBoss))
			{
				component4.ChargeElectric();
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
				MoveTankScript component = PlayerOnMe.GetComponent<MoveTankScript>();
				if ((bool)component && !component.isStunned)
				{
					component.StunMe(2f);
				}
				else
				{
					EnemyAI component2 = PlayerOnMe.GetComponent<EnemyAI>();
					if ((bool)component2 && !component2.isStunned)
					{
						component2.StunMe(2f);
					}
				}
			}
			if (ActiveBecauseCloseToBoss && Vector3.Distance(base.transform.position, BossObject.transform.position) > 5f)
			{
				ActiveBecauseCloseToBoss = false;
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
