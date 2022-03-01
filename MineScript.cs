using System.Collections;
using UnityEngine;

public class MineScript : MonoBehaviour
{
	public float DetinationTime;

	public GameObject deathExplosion;

	public GameObject deathExplosionParty;

	public AudioClip Deathsound;

	public AudioClip Placesound;

	public AudioClip MineBeep;

	public AudioClip KillExtra;

	public float ExplosionMinInterval;

	public float ExplosionMaxInterval;

	public bool startBlink;

	public bool active;

	public int isMineByPlayer = -1;

	public GameObject HitTextP1;

	public GameObject HitTextP2;

	public GameObject HitTextP3;

	public GameObject HitTextP4;

	public Renderer rend;

	public Light myLight;

	public Color mineNormal;

	public Color mineBlinking;

	public FiringTank myPapa;

	public bool isHuntingEnemies;

	public bool isTripMine;

	public bool placedByEnemies;

	public GameObject MyPlacer;

	public int MineTeam;

	private void Start()
	{
		if ((bool)MyPlacer)
		{
			active = false;
		}
		else
		{
			StartCoroutine("setactive");
		}
		GameMaster.instance.AmountMinesPlaced++;
		Play2DClipAtPoint(Placesound);
		DetinationTime = Random.Range(ExplosionMinInterval, ExplosionMaxInterval);
		InvokeRepeating("Blinking", 0.2f, 0.1f);
		InvokeRepeating("ScanForEnemies", 1f, 0.5f);
		InvokeRepeating("BlinkingTripmine", 2f, 2f);
		rend = GetComponent<MeshRenderer>();
		myLight.color = mineNormal;
		rend.material.SetColor("_Color", mineNormal);
		rend.material.SetColor("_EmissionColor", mineNormal);
	}

	private void Update()
	{
		if (!isTripMine)
		{
			DetinationTime -= Time.deltaTime;
		}
		if (GameMaster.instance != null && MapEditorMaster.instance == null && !GameMaster.instance.inTankeyTown)
		{
			if (DetinationTime < 0.1f || (((GameMaster.instance.AmountEnemyTanks < 1 && !GameMaster.instance.isZombieMode && GameMaster.instance.CurrentMission != 99) || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive) && !isHuntingEnemies))
			{
				InitiateDeath();
			}
		}
		else if ((bool)MapEditorMaster.instance)
		{
			if (DetinationTime < 0.1f || GameMaster.instance.restartGame || !GameMaster.instance.GameHasStarted)
			{
				InitiateDeath();
			}
		}
		else if (GameMaster.instance.inTankeyTown && DetinationTime < 0.1f)
		{
			InitiateDeath();
		}
		if (DetinationTime < 2f)
		{
			startBlink = true;
		}
	}

	private void InitiateDeath()
	{
		if (!GameMaster.instance.inTankeyTown && (GameMaster.instance.GameHasStarted || isHuntingEnemies || GameMaster.instance.isZombieMode))
		{
			AreaDamageEnemies(base.transform.position, 3.5f, 1f);
			if (GameMaster.instance.CurrentMission == 49)
			{
				AreaDamageMission50(base.transform.position, 2.5f, 1f);
			}
		}
		GameMaster.instance.AmountMinesPlaced--;
		GameObject gameObject = (placedByEnemies ? Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : ((!OptionsMainMenu.instance.AMselected.Contains(3)) ? Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : Object.Instantiate(deathExplosionParty, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity)));
		CameraShake component = Camera.main.GetComponent<CameraShake>();
		if ((bool)component)
		{
			component.StartCoroutine(component.Shake(0.1f, 0.15f));
		}
		Object.Destroy(gameObject.gameObject, 2f);
		if (myPapa != null)
		{
			myPapa.minesPlaced--;
		}
		if (GameMaster.instance.GameHasStarted || isHuntingEnemies || GameMaster.instance.isZombieMode)
		{
			Play2DClipAtPoint(Deathsound);
		}
		Object.Destroy(base.gameObject);
	}

	private void ScanForEnemies()
	{
		if (!active || !(DetinationTime < 6f) || !(DetinationTime > 1f))
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 3f);
		foreach (Collider obj in array)
		{
			EnemyAI component = obj.GetComponent<EnemyAI>();
			MoveTankScript component2 = obj.GetComponent<MoveTankScript>();
			if ((bool)component)
			{
				if (component.MyTeam != MineTeam || MineTeam == 0)
				{
					DetinationTime = 1f;
				}
			}
			else if ((bool)component2 && (component2.MyTeam != MineTeam || MineTeam == 0))
			{
				DetinationTime = 1f;
			}
		}
	}

	private void Blinking()
	{
		_ = placedByEnemies;
		if (startBlink)
		{
			if (rend.material.GetColor("_Color") == mineBlinking)
			{
				myLight.color = mineNormal;
				rend.material.SetColor("_Color", mineNormal);
				rend.material.SetColor("_EmissionColor", mineNormal);
			}
			else
			{
				Play2DClipAtPoint(MineBeep);
				myLight.color = mineBlinking;
				rend.material.SetColor("_Color", mineBlinking);
				rend.material.SetColor("_EmissionColor", mineBlinking);
			}
		}
	}

	private void BlinkingTripmine()
	{
		if (isTripMine)
		{
			if (rend.material.GetColor("_Color") == mineBlinking)
			{
				myLight.color = mineNormal;
				rend.material.SetColor("_Color", mineNormal);
				rend.material.SetColor("_EmissionColor", mineNormal);
			}
			else
			{
				StartCoroutine("setMineNormal");
				Play2DClipAtPoint(MineBeep);
				myLight.color = mineBlinking;
				rend.material.SetColor("_Color", mineBlinking);
				rend.material.SetColor("_EmissionColor", mineBlinking);
			}
		}
	}

	private IEnumerator setMineNormal()
	{
		yield return new WaitForSeconds(0.1f);
		myLight.color = mineNormal;
		rend.material.SetColor("_Color", mineNormal);
		rend.material.SetColor("_EmissionColor", mineNormal);
	}

	public IEnumerator setactive()
	{
		yield return new WaitForSeconds(1f);
		active = true;
	}

	private void AreaDamageMission50(Vector3 location, float radius, float damage)
	{
		Collider[] array = Physics.OverlapSphere(location, radius);
		for (int i = 0; i < array.Length; i++)
		{
			DestroyableBlock component = array[i].GetComponent<DestroyableBlock>();
			if (GameMaster.instance.CurrentMission == 49 && (bool)component)
			{
				component.blockHealth = 0;
			}
		}
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] array = Physics.OverlapSphere(location, radius);
		bool flag = false;
		bool flag2 = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			HealthTanks component = collider.GetComponent<HealthTanks>();
			PlayerBulletScript component2 = collider.GetComponent<PlayerBulletScript>();
			EnemyBulletScript component3 = collider.GetComponent<EnemyBulletScript>();
			MineScript component4 = collider.GetComponent<MineScript>();
			DestroyableWall component5 = collider.GetComponent<DestroyableWall>();
			WeeTurret component6 = collider.GetComponent<WeeTurret>();
			ExplosiveBlock component7 = collider.GetComponent<ExplosiveBlock>();
			BombSackScript component8 = collider.GetComponent<BombSackScript>();
			if ((bool)component7)
			{
				if (!component7.isExploding)
				{
					component7.StartCoroutine(component7.Death());
				}
			}
			else if (component6 != null)
			{
				if (GameMaster.instance.GameHasStarted)
				{
					component6.Health--;
				}
			}
			else if (component != null)
			{
				if (component.isMainTank)
				{
					EnemyAI component9 = component.GetComponent<EnemyAI>();
					if ((bool)component9 && component9.IsCompanion && MyPlacer == component9.gameObject)
					{
						return;
					}
				}
				if (!placedByEnemies)
				{
					if (component.isMainTank)
					{
						flag = true;
						if (flag2)
						{
							AchievementsTracker.instance.completeAchievement(20);
						}
					}
					else
					{
						flag2 = true;
						AchievementsTracker.instance.completeAchievement(19);
						if (flag)
						{
							AchievementsTracker.instance.completeAchievement(20);
						}
					}
				}
				if (component.immuneToExplosion)
				{
					continue;
				}
				if (GameMaster.instance.isZombieMode && component.health > 0 && GameMaster.instance.GameHasStarted)
				{
					SFXManager.instance.PlaySFX(component.Buzz);
				}
				int num = 0;
				if (component.ShieldFade != null)
				{
					num = component.ShieldFade.ShieldHealth;
				}
				if (component.health == 1 && num < 1 && isMineByPlayer == 0 && !component.isMainTank)
				{
					Object.Instantiate(HitTextP1, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						Play2DClipAtPoint(KillExtra);
						GameMaster.instance.Playerkills[0]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 1);
						}
					}
				}
				else if (component.health == 1 && num < 1 && isMineByPlayer == 1 && !component.isMainTank)
				{
					Object.Instantiate(HitTextP2, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						Play2DClipAtPoint(KillExtra);
						GameMaster.instance.Playerkills[1]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 2);
						}
					}
				}
				else if (component.health == 1 && num < 1 && isMineByPlayer == 2 && !component.isMainTank)
				{
					Object.Instantiate(HitTextP3, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						Play2DClipAtPoint(KillExtra);
						GameMaster.instance.Playerkills[2]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 3);
						}
					}
				}
				else if (component.health == 1 && num < 1 && isMineByPlayer == 3 && !component.isMainTank)
				{
					Object.Instantiate(HitTextP4, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						Play2DClipAtPoint(KillExtra);
						GameMaster.instance.Playerkills[3]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 4);
						}
					}
				}
				if (GameMaster.instance.GameHasStarted)
				{
					if (num > 0)
					{
						component.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						component.DamageMe(1);
					}
				}
			}
			else if (component2 != null)
			{
				if (!component2.isSilver)
				{
					component2.TimesBounced = 999;
				}
			}
			else if (component3 != null)
			{
				if (!component3.isElectric)
				{
					component3.BounceAmount = 999;
				}
			}
			else if (component4 != null)
			{
				component4.DetinationTime = 0f;
			}
			else if (component5 != null)
			{
				component5.StartCoroutine(component5.destroy());
			}
			else if (component8 != null)
			{
				component8.FlyBack();
			}
		}
		array2 = Physics.OverlapSphere(location, radius * 2.75f);
		foreach (Collider collider2 in array2)
		{
			Rigidbody component10 = collider2.GetComponent<Rigidbody>();
			if (component10 != null && (collider2.tag == "Player" || collider2.tag == "Enemy"))
			{
				float num2 = Vector3.Distance(component10.transform.position, base.transform.position);
				float num3 = (radius * 2.75f - num2) * 2.2f;
				Vector3 vector = component10.transform.position - base.transform.position;
				component10.AddForce(vector * num3, ForceMode.Impulse);
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (((!(collision.transform.tag == "Player") || !placedByEnemies) && (!(collision.transform.tag == "Enemy") || placedByEnemies) && !(collision.gameObject.tag == "Bullet") && !(collision.transform.tag == "Bullet")) || !active)
		{
			return;
		}
		PlayerBulletScript component = collision.transform.GetComponent<PlayerBulletScript>();
		if (component != null)
		{
			if (component.isElectric)
			{
				return;
			}
			if (!component.isEnemyBullet)
			{
				isMineByPlayer = component.ShotByPlayer;
			}
		}
		if (!isTripMine || !(collision.transform.tag == "Player"))
		{
			Debug.LogError("poof..." + collision.name);
			DetinationTime = 0f;
		}
	}

	private void OnTriggerStay(Collider collision)
	{
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)MyPlacer && other.gameObject == MyPlacer)
		{
			active = true;
		}
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
