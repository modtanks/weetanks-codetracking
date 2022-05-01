using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FiringTank : MonoBehaviour
{
	[Header("Settings")]
	public float fireRate = 1f;

	public float fireCountdown = 0f;

	public int maxFiredBullets = 5;

	public int firedBullets = 0;

	public int amountBounces = 0;

	public bool canShoot = true;

	public bool canMine = true;

	public bool canLayMine = true;

	public float mineCountdown = 0f;

	public int minesPlaced = 0;

	public int maxMinesPlaced = 2;

	public GameObject bulletPrefab;

	public List<AudioClip> shootSound = new List<AudioClip>();

	public GameObject bulletPrefabUpgraded;

	public GameObject lvl50BulletPrefab;

	public GameObject minePrefab;

	public GameObject tripminePrefab;

	public Transform firePoint;

	public Vector3 target;

	public bool mobileMine = false;

	public MousePosition mousescript;

	private HealthTanks HealthScript;

	private AudioSource source;

	public MoveTankScript tankMovingScript;

	public bool controllerReleased = true;

	public Animator TankTop;

	public LookAtMouse LAM;

	public ObjectPlacing OP;

	private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();

	private void Start()
	{
		HealthScript = GetComponent<HealthTanks>();
		tankMovingScript = GetComponent<MoveTankScript>();
		source = GetComponent<AudioSource>();
		mousescript = GameObject.Find("MouseDetectorPosition").GetComponent<MousePosition>();
		OP = GetComponent<ObjectPlacing>();
		shootSound = GlobalAssets.instance.AudioDB.NormalBulletShootSound;
	}

	private void Update()
	{
		if (tankMovingScript.player.GetAxis("Look Horizontal") < 0.5f)
		{
			controllerReleased = true;
		}
		else if ((tankMovingScript.player.GetAxis("Look Horizontal") > 0.5f || tankMovingScript.player.GetAxis("Look Horizontal") < -0.5f || tankMovingScript.player.GetAxis("Look Vertically") > 0.5f || tankMovingScript.player.GetAxis("Look Vertically") < -0.5f) && tankMovingScript.playerId != 0)
		{
		}
		if ((bool)MapEditorMaster.instance)
		{
			amountBounces = MapEditorMaster.instance.PlayerAmountBounces;
		}
		if ((bool)GameMaster.instance.PKU && GameMaster.instance.PKU.Bullets.Length != 0)
		{
			if (firedBullets > 0)
			{
				if (tankMovingScript.playerId == 0)
				{
					for (int i = 0; i < GameMaster.instance.PKU.Bullets.Length; i++)
					{
						if (i < firedBullets)
						{
							GameMaster.instance.PKU.Bullets[i].gameObject.SetActive(value: false);
						}
						else
						{
							GameMaster.instance.PKU.Bullets[i].gameObject.SetActive(value: true);
						}
					}
				}
				else if (tankMovingScript.playerId == 1)
				{
					for (int j = 0; j < GameMaster.instance.PKU.BulletsP2.Length; j++)
					{
						if (j < firedBullets)
						{
							GameMaster.instance.PKU.BulletsP2[j].gameObject.SetActive(value: false);
						}
						else
						{
							GameMaster.instance.PKU.BulletsP2[j].gameObject.SetActive(value: true);
						}
					}
				}
				else if (tankMovingScript.playerId == 2)
				{
					for (int k = 0; k < GameMaster.instance.PKU.BulletsP3.Length; k++)
					{
						if (k < firedBullets)
						{
							GameMaster.instance.PKU.BulletsP3[k].gameObject.SetActive(value: false);
						}
						else
						{
							GameMaster.instance.PKU.BulletsP3[k].gameObject.SetActive(value: true);
						}
					}
				}
				else if (tankMovingScript.playerId == 3)
				{
					for (int l = 0; l < GameMaster.instance.PKU.BulletsP3.Length; l++)
					{
						if (l < firedBullets)
						{
							GameMaster.instance.PKU.BulletsP4[l].gameObject.SetActive(value: false);
						}
						else
						{
							GameMaster.instance.PKU.BulletsP4[l].gameObject.SetActive(value: true);
						}
					}
				}
			}
			else if (tankMovingScript.playerId == 0)
			{
				RawImage[] bullets = GameMaster.instance.PKU.Bullets;
				foreach (RawImage Bullet4 in bullets)
				{
					Bullet4.gameObject.SetActive(value: true);
				}
			}
			else if (tankMovingScript.playerId == 1)
			{
				RawImage[] bulletsP = GameMaster.instance.PKU.BulletsP2;
				foreach (RawImage Bullet3 in bulletsP)
				{
					Bullet3.gameObject.SetActive(value: true);
				}
			}
			else if (tankMovingScript.playerId == 2)
			{
				RawImage[] bulletsP2 = GameMaster.instance.PKU.BulletsP3;
				foreach (RawImage Bullet2 in bulletsP2)
				{
					Bullet2.gameObject.SetActive(value: true);
				}
			}
			else if (tankMovingScript.playerId == 3)
			{
				RawImage[] bulletsP3 = GameMaster.instance.PKU.BulletsP4;
				foreach (RawImage Bullet in bulletsP3)
				{
					Bullet.gameObject.SetActive(value: true);
				}
			}
		}
		target = new Vector3(mousescript.mousePos.x, firePoint.transform.position.y, mousescript.mousePos.z);
		if (mineCountdown <= 0f && minesPlaced < maxMinesPlaced)
		{
			canMine = true;
		}
		mineCountdown -= Time.deltaTime;
		if (HealthScript.health > 0 && !GameMaster.instance.GameHasPaused && (GameMaster.instance.GameHasStarted || GameMaster.instance.isZombieMode) && (!OP || !OP.InPlacingMode))
		{
			if (tankMovingScript.player.GetButtonDown("Fire") && canShoot && firedBullets < maxFiredBullets)
			{
				Shoot();
			}
			if (tankMovingScript.player.GetButtonDown("Mine") && canMine && canLayMine)
			{
				Mine();
			}
			if (mobileMine && canMine && canLayMine)
			{
				Mine();
			}
		}
	}

	private void Mine()
	{
		if (GameMaster.instance.CurrentMission == 2)
		{
			GameMaster.instance.mission3HasMined = true;
		}
		GameObject mine = ((tankMovingScript.Upgrades[4] >= 1) ? Object.Instantiate(tripminePrefab, base.transform.position, Quaternion.identity) : Object.Instantiate(minePrefab, base.transform.position, Quaternion.identity));
		mine.transform.position = new Vector3(mine.transform.position.x, base.transform.position.y, mine.transform.position.z);
		MineScript mineScript = mine.GetComponent<MineScript>();
		mineScript.placedByEnemies = false;
		mineScript.isMineByPlayer = tankMovingScript.playerId;
		if ((bool)GameMaster.instance.CM)
		{
			mine.transform.SetParent(GameMaster.instance.CM.transform);
		}
		else
		{
			mine.transform.parent = null;
		}
		mineScript.myPapa = this;
		mineScript.MyPlacer = tankMovingScript.gameObject;
		minesPlaced++;
		canMine = false;
		mineCountdown = 0.4f;
	}

	private IEnumerator shootAgain()
	{
		yield return new WaitForSeconds(fireRate);
		canShoot = true;
	}

	public bool PointIsOverUI(float x, float y)
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(x, y);
		tempRaycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, tempRaycastResults);
		foreach (RaycastResult tempRaycastResult in tempRaycastResults)
		{
			if (tempRaycastResult.gameObject.transform.tag == "IgnoreMobile")
			{
				return true;
			}
		}
		return tempRaycastResults.Count > 0;
	}

	private void Shoot()
	{
		if (GameMaster.instance.CurrentMission == 0)
		{
			GameMaster.instance.mission1HasShooted = true;
		}
		firedBullets++;
		canShoot = false;
		if ((bool)TankTop)
		{
			TankTop.Play("ShootingTank", -1, 0f);
			if ((bool)LAM && (bool)tankMovingScript && (bool)tankMovingScript.rigi)
			{
				tankMovingScript.rigi.AddForce(-LAM.transform.forward * 4f, ForceMode.Impulse);
			}
		}
		StartCoroutine(shootAgain());
		GameObject prefab = ((!GameMaster.instance) ? bulletPrefab : ((tankMovingScript.Upgrades[3] <= 0) ? bulletPrefab : bulletPrefabUpgraded));
		GameObject bulletGO = Object.Instantiate(prefab, firePoint.position, firePoint.transform.rotation);
		PlayerBulletScript bullet = bulletGO.GetComponent<PlayerBulletScript>();
		Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
		bullet.CollidersIgnoring.Add(GetComponent<Collider>());
		if ((bool)AchievementsTracker.instance)
		{
			AchievementsTracker.instance.HasShooted = true;
			AchievementsTracker.instance.HasShootedThisRound = true;
		}
		if ((bool)tankMovingScript)
		{
			bullet.isTowerCharged = tankMovingScript.isBeingTowerBoosted;
			InstantiateOneParticle IOP = bullet.GetComponent<InstantiateOneParticle>();
			if ((bool)IOP)
			{
				IOP.IsTowerCharged = tankMovingScript.isBeingTowerBoosted;
			}
		}
		bullet.MyTeam = tankMovingScript.MyTeam;
		bullet.ShotByPlayer = tankMovingScript.playerId;
		bullet.TankScript = this;
		bullet.papaTank = base.gameObject;
		bullet.MaxBounces = amountBounces;
		bullet.StartingVelocity = firePoint.forward * 6f;
		bulletGO.transform.Rotate(Vector3.right * 90f);
		Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
		bulletBody.AddForce(firePoint.forward * 6f);
		SFXManager.instance.PlaySFX(shootSound);
	}
}
