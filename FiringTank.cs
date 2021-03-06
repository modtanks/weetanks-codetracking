using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FiringTank : MonoBehaviour
{
	[Header("Settings")]
	public float fireRate = 1f;

	public float fireCountdown;

	public int maxFiredBullets = 5;

	public int firedBullets;

	public int amountBounces;

	public bool canShoot = true;

	public bool canMine = true;

	public bool canLayMine = true;

	public float mineCountdown;

	public int minesPlaced;

	public int maxMinesPlaced = 2;

	public GameObject bulletPrefab;

	public List<AudioClip> shootSound = new List<AudioClip>();

	public GameObject bulletPrefabUpgraded;

	public GameObject lvl50BulletPrefab;

	public GameObject minePrefab;

	public GameObject tripminePrefab;

	public Transform firePoint;

	public Vector3 target;

	public bool mobileMine;

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
		else if (tankMovingScript.player.GetAxis("Look Horizontal") > 0.5f || tankMovingScript.player.GetAxis("Look Horizontal") < -0.5f || tankMovingScript.player.GetAxis("Look Vertically") > 0.5f || tankMovingScript.player.GetAxis("Look Vertically") < -0.5f)
		{
			_ = tankMovingScript.playerId;
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
				for (int m = 0; m < bullets.Length; m++)
				{
					bullets[m].gameObject.SetActive(value: true);
				}
			}
			else if (tankMovingScript.playerId == 1)
			{
				RawImage[] bullets = GameMaster.instance.PKU.BulletsP2;
				for (int m = 0; m < bullets.Length; m++)
				{
					bullets[m].gameObject.SetActive(value: true);
				}
			}
			else if (tankMovingScript.playerId == 2)
			{
				RawImage[] bullets = GameMaster.instance.PKU.BulletsP3;
				for (int m = 0; m < bullets.Length; m++)
				{
					bullets[m].gameObject.SetActive(value: true);
				}
			}
			else if (tankMovingScript.playerId == 3)
			{
				RawImage[] bullets = GameMaster.instance.PKU.BulletsP4;
				for (int m = 0; m < bullets.Length; m++)
				{
					bullets[m].gameObject.SetActive(value: true);
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
		GameObject gameObject = ((tankMovingScript.Upgrades[4] >= 1) ? Object.Instantiate(tripminePrefab, base.transform.position, Quaternion.identity) : Object.Instantiate(minePrefab, base.transform.position, Quaternion.identity));
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, base.transform.position.y, gameObject.transform.position.z);
		MineScript component = gameObject.GetComponent<MineScript>();
		component.placedByEnemies = false;
		component.isMineByPlayer = tankMovingScript.playerId;
		if ((bool)GameMaster.instance.CM)
		{
			gameObject.transform.SetParent(GameMaster.instance.CM.transform);
		}
		else
		{
			gameObject.transform.parent = null;
		}
		component.myPapa = this;
		component.MyPlacer = tankMovingScript.gameObject;
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
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = new Vector2(x, y);
		tempRaycastResults.Clear();
		EventSystem.current.RaycastAll(pointerEventData, tempRaycastResults);
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
		GameObject original = ((!GameMaster.instance) ? bulletPrefab : ((tankMovingScript.Upgrades[3] <= 0) ? bulletPrefab : bulletPrefabUpgraded));
		GameObject obj = Object.Instantiate(original, firePoint.position, firePoint.transform.rotation);
		PlayerBulletScript component = obj.GetComponent<PlayerBulletScript>();
		Physics.IgnoreCollision(component.GetComponent<Collider>(), GetComponent<Collider>());
		component.CollidersIgnoring.Add(GetComponent<Collider>());
		if ((bool)AchievementsTracker.instance)
		{
			AchievementsTracker.instance.HasShooted = true;
			AchievementsTracker.instance.HasShootedThisRound = true;
		}
		if ((bool)tankMovingScript)
		{
			component.isTowerCharged = tankMovingScript.isBeingTowerBoosted;
			InstantiateOneParticle component2 = component.GetComponent<InstantiateOneParticle>();
			if ((bool)component2)
			{
				component2.IsTowerCharged = tankMovingScript.isBeingTowerBoosted;
			}
		}
		component.MyTeam = tankMovingScript.MyTeam;
		component.ShotByPlayer = tankMovingScript.playerId;
		component.TankScript = this;
		component.papaTank = base.gameObject;
		component.MaxBounces = amountBounces;
		component.StartingVelocity = firePoint.forward * 6f;
		obj.transform.Rotate(Vector3.right * 90f);
		obj.GetComponent<Rigidbody>().AddForce(firePoint.forward * 6f);
		SFXManager.instance.PlaySFX(shootSound);
	}
}
