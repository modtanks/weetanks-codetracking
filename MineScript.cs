using System.Collections;
using UnityEngine;

public class MineScript : MonoBehaviour
{
	public float DetinationTime;

	public GameObject deathExplosion;

	public GameObject deathExplosionParty;

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

	public bool IsNuclear;

	public AudioClip NuclearSound;

	public Color NuclearColor;

	public GameObject NukePoof;

	public AudioClip NukeSound;

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
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MinePlace);
		DetinationTime = Random.Range(ExplosionMinInterval, ExplosionMaxInterval);
		InvokeRepeating("ScanForEnemies", 1f, 0.5f);
		InvokeRepeating("BlinkingTripmine", 2f, 2f);
		rend = GetComponent<MeshRenderer>();
		myLight.color = mineNormal;
		rend.material.SetColor("_Color", mineNormal);
		rend.material.SetColor("_EmissionColor", mineNormal);
		IsNuclear = false;
		if (IsNuclear)
		{
			GetComponent<AudioSource>().clip = NuclearSound;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().Play();
			DetinationTime = 5f;
			mineBlinking = NuclearColor;
			InvokeRepeating("Blinking", 0.5f, 0.5f);
		}
		else
		{
			InvokeRepeating("Blinking", 0.2f, 0.1f);
		}
	}

	private void Update()
	{
		if (!isTripMine)
		{
			DetinationTime -= Time.deltaTime;
		}
		if (GameMaster.instance != null && MapEditorMaster.instance == null && !GameMaster.instance.inTankeyTown)
		{
			if (DetinationTime < 0.1f || (((GameMaster.instance.AmountEnemyTanks < 1 && !GameMaster.instance.isZombieMode && GameMaster.instance.CurrentMission < 99) || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive) && !isHuntingEnemies))
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
		if (DetinationTime < 2f || IsNuclear)
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
		GameObject gameObject = null;
		gameObject = (placedByEnemies ? ((!IsNuclear) ? Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : Object.Instantiate(NukePoof, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity)) : (OptionsMainMenu.instance.AMselected.Contains(3) ? Object.Instantiate(deathExplosionParty, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : ((!IsNuclear) ? Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : Object.Instantiate(NukePoof, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity))));
		if (IsNuclear)
		{
			CameraShake component = Camera.main.GetComponent<CameraShake>();
			if ((bool)component)
			{
				component.StartCoroutine(component.Shake(0.6f, 0.95f));
			}
		}
		else
		{
			CameraShake component2 = Camera.main.GetComponent<CameraShake>();
			if ((bool)component2)
			{
				component2.StartCoroutine(component2.Shake(0.1f, 0.15f));
			}
		}
		Object.Destroy(gameObject.gameObject, 4f);
		if (myPapa != null)
		{
			myPapa.minesPlaced--;
		}
		if (GameMaster.instance.GameHasStarted || isHuntingEnemies || GameMaster.instance.isZombieMode)
		{
			if (IsNuclear)
			{
				SFXManager.instance.PlaySFX(NukeSound);
			}
			else
			{
				SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MineExplosion);
			}
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
				SFXManager.instance.PlaySFX(MineBeep);
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
				SFXManager.instance.PlaySFX(MineBeep);
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
		if (IsNuclear)
		{
			radius *= 3f;
		}
		Collider[] array = Physics.OverlapSphere(location, radius * 2f);
		Collider[] array2 = Physics.OverlapSphere(location, radius);
		bool flag = false;
		bool flag2 = false;
		Collider[] array3;
		if (IsNuclear)
		{
			array3 = array;
			foreach (Collider obj in array3)
			{
				PlayerBulletScript component = obj.GetComponent<PlayerBulletScript>();
				EnemyBulletScript component2 = obj.GetComponent<EnemyBulletScript>();
				MineScript component3 = obj.GetComponent<MineScript>();
				DestroyableWall component4 = obj.GetComponent<DestroyableWall>();
				obj.GetComponent<WeeTurret>();
				ExplosiveBlock component5 = obj.GetComponent<ExplosiveBlock>();
				BombSackScript component6 = obj.GetComponent<BombSackScript>();
				if ((bool)component5)
				{
					if (!component5.isExploding)
					{
						component5.StartCoroutine(component5.Death());
					}
				}
				else if (component != null)
				{
					if (!component.isSilver)
					{
						component.TimesBounced = 999;
					}
				}
				else if (component2 != null)
				{
					if (!component2.isElectric)
					{
						component2.BounceAmount = 999;
					}
				}
				else if (component3 != null)
				{
					component3.DetinationTime = 0f;
				}
				else if (component4 != null)
				{
					component4.StartCoroutine(component4.destroy());
				}
				else if (component6 != null)
				{
					component6.FlyBack();
				}
			}
		}
		array3 = array2;
		foreach (Collider collider in array3)
		{
			HealthTanks component7 = collider.GetComponent<HealthTanks>();
			PlayerBulletScript component8 = collider.GetComponent<PlayerBulletScript>();
			EnemyBulletScript component9 = collider.GetComponent<EnemyBulletScript>();
			MineScript component10 = collider.GetComponent<MineScript>();
			DestroyableWall component11 = collider.GetComponent<DestroyableWall>();
			WeeTurret component12 = collider.GetComponent<WeeTurret>();
			ExplosiveBlock component13 = collider.GetComponent<ExplosiveBlock>();
			BombSackScript component14 = collider.GetComponent<BombSackScript>();
			if ((bool)component13)
			{
				if (!component13.isExploding)
				{
					component13.StartCoroutine(component13.Death());
				}
			}
			else if (component12 != null)
			{
				if (GameMaster.instance.GameHasStarted)
				{
					component12.Health--;
				}
			}
			else if (component7 != null)
			{
				if (component7.isMainTank)
				{
					EnemyAI component15 = component7.GetComponent<EnemyAI>();
					if ((bool)component15 && component15.IsCompanion && MyPlacer == component15.gameObject)
					{
						return;
					}
				}
				if (!placedByEnemies)
				{
					if (component7.isMainTank)
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
				if (component7.immuneToExplosion)
				{
					continue;
				}
				if (GameMaster.instance.isZombieMode && component7.health > 0 && GameMaster.instance.GameHasStarted && component7.Buzz != null)
				{
					SFXManager.instance.PlaySFX(component7.Buzz);
				}
				int num = 0;
				if (component7.ShieldFade != null)
				{
					num = component7.ShieldFade.ShieldHealth;
				}
				if (component7.health == 1 && num < 1 && isMineByPlayer == 0 && !component7.isMainTank)
				{
					Object.Instantiate(HitTextP1, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						SFXManager.instance.PlaySFX(KillExtra);
						GameMaster.instance.Playerkills[0]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 1);
						}
					}
				}
				else if (component7.health == 1 && num < 1 && isMineByPlayer == 1 && !component7.isMainTank)
				{
					Object.Instantiate(HitTextP2, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						SFXManager.instance.PlaySFX(KillExtra);
						GameMaster.instance.Playerkills[1]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 2);
						}
					}
				}
				else if (component7.health == 1 && num < 1 && isMineByPlayer == 2 && !component7.isMainTank)
				{
					Object.Instantiate(HitTextP3, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						SFXManager.instance.PlaySFX(KillExtra);
						GameMaster.instance.Playerkills[2]++;
						GameMaster.instance.TotalKillsThisSession++;
						if (GameMaster.instance.PKU != null)
						{
							GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 3);
						}
					}
				}
				else if (component7.health == 1 && num < 1 && isMineByPlayer == 3 && !component7.isMainTank)
				{
					Object.Instantiate(HitTextP4, collider.transform.position, Quaternion.identity);
					if (GameMaster.instance != null)
					{
						SFXManager.instance.PlaySFX(KillExtra);
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
						component7.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						component7.DamageMe(1);
					}
				}
			}
			else if (component8 != null)
			{
				if (!component8.isSilver)
				{
					component8.TimesBounced = 999;
				}
			}
			else if (component9 != null)
			{
				if (!component9.isElectric)
				{
					component9.BounceAmount = 999;
				}
			}
			else if (component10 != null)
			{
				component10.DetinationTime = 0f;
			}
			else if (component11 != null)
			{
				component11.StartCoroutine(component11.destroy());
			}
			else if (component14 != null)
			{
				component14.FlyBack();
			}
		}
		array3 = Physics.OverlapSphere(location, radius * 2.75f);
		foreach (Collider collider2 in array3)
		{
			Rigidbody component16 = collider2.GetComponent<Rigidbody>();
			if (component16 != null && (collider2.tag == "Player" || collider2.tag == "Enemy"))
			{
				float num2 = Vector3.Distance(component16.transform.position, base.transform.position);
				float num3 = (radius * 2.75f - num2) * 2.2f;
				Vector3 vector = component16.transform.position - base.transform.position;
				if (num3 > 16f)
				{
					num3 = 16f;
				}
				component16.AddForce(vector * num3, ForceMode.Impulse);
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
			if (component.IsAirBullet)
			{
				return;
			}
		}
		if (!isTripMine || !(collision.transform.tag == "Player"))
		{
			Debug.LogWarning("poof..." + collision.name);
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
}
