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

	public bool startBlink = false;

	public bool active = false;

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

	public bool placedByEnemies = false;

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
		GameObject poof = null;
		poof = (placedByEnemies ? ((!IsNuclear) ? Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : Object.Instantiate(NukePoof, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity)) : (OptionsMainMenu.instance.AMselected.Contains(3) ? Object.Instantiate(deathExplosionParty, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : ((!IsNuclear) ? Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity) : Object.Instantiate(NukePoof, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity))));
		if (IsNuclear)
		{
			CameraShake CS = Camera.main.GetComponent<CameraShake>();
			if ((bool)CS)
			{
				CS.StartCoroutine(CS.Shake(0.6f, 0.95f));
			}
		}
		else
		{
			CameraShake CS2 = Camera.main.GetComponent<CameraShake>();
			if ((bool)CS2)
			{
				CS2.StartCoroutine(CS2.Shake(0.1f, 0.15f));
			}
		}
		Object.Destroy(poof.gameObject, 4f);
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
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 3f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			EnemyAI EA = col.GetComponent<EnemyAI>();
			MoveTankScript MTS = col.GetComponent<MoveTankScript>();
			if ((bool)EA)
			{
				if (EA.MyTeam != MineTeam || MineTeam == 0)
				{
					DetinationTime = 1f;
				}
			}
			else if ((bool)MTS && (MTS.MyTeam != MineTeam || MineTeam == 0))
			{
				DetinationTime = 1f;
			}
		}
	}

	private void Blinking()
	{
		if (placedByEnemies)
		{
		}
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
		Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			DestroyableBlock DB = col.GetComponent<DestroyableBlock>();
			if (GameMaster.instance.CurrentMission == 49 && (bool)DB)
			{
				DB.blockHealth = 0;
			}
		}
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		if (IsNuclear)
		{
			radius *= 3f;
		}
		Collider[] objectsInRangeBIG = Physics.OverlapSphere(location, radius * 2f);
		Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
		bool killedSelf = false;
		bool killedEnemy = false;
		if (IsNuclear)
		{
			Collider[] array = objectsInRangeBIG;
			foreach (Collider col in array)
			{
				PlayerBulletScript friendbullet = col.GetComponent<PlayerBulletScript>();
				EnemyBulletScript enemybullet = col.GetComponent<EnemyBulletScript>();
				MineScript mines = col.GetComponent<MineScript>();
				DestroyableWall DestroyWall = col.GetComponent<DestroyableWall>();
				WeeTurret WT = col.GetComponent<WeeTurret>();
				ExplosiveBlock EB = col.GetComponent<ExplosiveBlock>();
				BombSackScript BSS = col.GetComponent<BombSackScript>();
				if ((bool)EB)
				{
					if (!EB.isExploding)
					{
						EB.StartCoroutine(EB.Death());
					}
				}
				else if (friendbullet != null)
				{
					if (!friendbullet.isSilver)
					{
						friendbullet.TimesBounced = 999;
					}
				}
				else if (enemybullet != null)
				{
					if (!enemybullet.isElectric)
					{
						enemybullet.BounceAmount = 999;
					}
				}
				else if (mines != null)
				{
					mines.DetinationTime = 0f;
				}
				else if (DestroyWall != null)
				{
					DestroyWall.StartCoroutine(DestroyWall.destroy());
				}
				else if (BSS != null)
				{
					BSS.FlyBack();
				}
			}
		}
		Collider[] array2 = objectsInRange;
		foreach (Collider col2 in array2)
		{
			HealthTanks enemy = col2.GetComponent<HealthTanks>();
			PlayerBulletScript friendbullet2 = col2.GetComponent<PlayerBulletScript>();
			EnemyBulletScript enemybullet2 = col2.GetComponent<EnemyBulletScript>();
			MineScript mines2 = col2.GetComponent<MineScript>();
			DestroyableWall DestroyWall2 = col2.GetComponent<DestroyableWall>();
			WeeTurret WT2 = col2.GetComponent<WeeTurret>();
			ExplosiveBlock EB2 = col2.GetComponent<ExplosiveBlock>();
			BombSackScript BSS2 = col2.GetComponent<BombSackScript>();
			if ((bool)EB2)
			{
				if (!EB2.isExploding)
				{
					EB2.StartCoroutine(EB2.Death());
				}
			}
			else if (WT2 != null)
			{
				if (GameMaster.instance.GameHasStarted)
				{
					WT2.Health--;
				}
			}
			else if (enemy != null)
			{
				if (enemy.isMainTank)
				{
					EnemyAI EA = enemy.GetComponent<EnemyAI>();
					if ((bool)EA && EA.IsCompanion && MyPlacer == EA.gameObject)
					{
						return;
					}
				}
				if (!placedByEnemies)
				{
					if (enemy.isMainTank)
					{
						killedSelf = true;
						if (killedEnemy)
						{
							AchievementsTracker.instance.completeAchievement(20);
						}
					}
					else
					{
						killedEnemy = true;
						AchievementsTracker.instance.completeAchievement(19);
						if (killedSelf)
						{
							AchievementsTracker.instance.completeAchievement(20);
						}
					}
				}
				if (enemy.immuneToExplosion)
				{
					continue;
				}
				if (GameMaster.instance.isZombieMode && enemy.health > 0 && GameMaster.instance.GameHasStarted)
				{
					SFXManager.instance.PlaySFX(enemy.Buzz);
				}
				int ShieldHealth = 0;
				if (enemy.ShieldFade != null)
				{
					ShieldHealth = enemy.ShieldFade.ShieldHealth;
				}
				if (enemy.health == 1 && ShieldHealth < 1 && isMineByPlayer == 0 && !enemy.isMainTank)
				{
					GameObject hittag4 = Object.Instantiate(HitTextP1, col2.transform.position, Quaternion.identity);
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
				else if (enemy.health == 1 && ShieldHealth < 1 && isMineByPlayer == 1 && !enemy.isMainTank)
				{
					GameObject hittag3 = Object.Instantiate(HitTextP2, col2.transform.position, Quaternion.identity);
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
				else if (enemy.health == 1 && ShieldHealth < 1 && isMineByPlayer == 2 && !enemy.isMainTank)
				{
					GameObject hittag2 = Object.Instantiate(HitTextP3, col2.transform.position, Quaternion.identity);
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
				else if (enemy.health == 1 && ShieldHealth < 1 && isMineByPlayer == 3 && !enemy.isMainTank)
				{
					GameObject hittag = Object.Instantiate(HitTextP4, col2.transform.position, Quaternion.identity);
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
					if (ShieldHealth > 0)
					{
						enemy.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						enemy.DamageMe(1);
					}
				}
			}
			else if (friendbullet2 != null)
			{
				if (!friendbullet2.isSilver)
				{
					friendbullet2.TimesBounced = 999;
				}
			}
			else if (enemybullet2 != null)
			{
				if (!enemybullet2.isElectric)
				{
					enemybullet2.BounceAmount = 999;
				}
			}
			else if (mines2 != null)
			{
				mines2.DetinationTime = 0f;
			}
			else if (DestroyWall2 != null)
			{
				DestroyWall2.StartCoroutine(DestroyWall2.destroy());
			}
			else if (BSS2 != null)
			{
				BSS2.FlyBack();
			}
		}
		Collider[] bigobjectsInRange = Physics.OverlapSphere(location, radius * 2.75f);
		Collider[] array3 = bigobjectsInRange;
		foreach (Collider col3 in array3)
		{
			Rigidbody rigi = col3.GetComponent<Rigidbody>();
			if (rigi != null && (col3.tag == "Player" || col3.tag == "Enemy"))
			{
				float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
				float force = (radius * 2.75f - distance) * 2.2f;
				Vector3 direction = rigi.transform.position - base.transform.position;
				if (force > 16f)
				{
					force = 16f;
				}
				rigi.AddForce(direction * force, ForceMode.Impulse);
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (((!(collision.transform.tag == "Player") || !placedByEnemies) && (!(collision.transform.tag == "Enemy") || placedByEnemies) && !(collision.gameObject.tag == "Bullet") && !(collision.transform.tag == "Bullet")) || !active)
		{
			return;
		}
		PlayerBulletScript EBS = collision.transform.GetComponent<PlayerBulletScript>();
		if (EBS != null)
		{
			if (EBS.isElectric)
			{
				return;
			}
			if (!EBS.isEnemyBullet)
			{
				isMineByPlayer = EBS.ShotByPlayer;
			}
			if (EBS.IsAirBullet)
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
