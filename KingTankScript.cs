using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingTankScript : MonoBehaviour
{
	public bool IsInBattle;

	public bool IsInFinalBattle;

	public int BossMode;

	public float TimeTillRam = 10f;

	public float Timer;

	public float CallInTimer;

	public float TimeBetweenCalls = 20f;

	public float RamSpeed = 6f;

	public RammingDetection RD;

	[Header("Attacks")]
	public AudioClip ShotgunShotSound;

	public AudioClip NormalRocketSound;

	public AudioClip OnlyShootingChargeSound;

	public AudioClip OnlyShootingChargeSound_quick;

	public GameObject RocketBullet;

	public GameObject ShotgunAttackBullet;

	public List<Transform> ShootShotGunPoints;

	public List<GameObject> ChargeCirclesBarrel;

	[Header("Boss Enable stuff")]
	public EnemyAI EA;

	public HealthTanks HT;

	public Collider BodyCollider;

	public EnemyTargetingSystemNew ETSN;

	public Rigidbody rb;

	public FollowTank FT;

	[Header("Mortar Stuff")]
	public bool IsInMortarMode;

	public float MortarFireSpeed_1b;

	public float MortarFireSpeed_2b;

	public int MortarShots_1b;

	public int MortarShots_2b;

	public ParticleSystem MortarParticles;

	public AudioClip[] MortarSound;

	public GameObject Mortar;

	public GameObject MortarPhase3;

	private Animator MyAnimator;

	public List<GameObject> ShotMortars = new List<GameObject>();

	public Transform ShootingPoint;

	[Header("Drop in enemy IDS")]
	public int[] DropIdsToddler;

	public int[] DropIdsKid;

	public int[] DropIdsAdult;

	public int[] DropIdsGrandpa;

	[Header("Platform Stuff")]
	public BossPlatform Throne;

	[Header("Other Stuff")]
	public Vector3 StartingPosition;

	public Quaternion StartingRotation;

	public LookAtObject LAO;

	public Transform Chute;

	public MissionHundredController MHC;

	public bool isRamming;

	public bool isRammingMoving;

	private Vector3 RamDirection;

	public bool BossDefeated;

	private bool IsDoingAShot;

	private void Start()
	{
		Timer = TimeTillRam;
		TimeBetweenCalls = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 14f : ((OptionsMainMenu.instance.currentDifficulty == 1) ? 10f : ((OptionsMainMenu.instance.currentDifficulty == 2) ? 8f : 7f)));
		CallInTimer = TimeBetweenCalls + Random.Range(0f, TimeBetweenCalls);
		StartingPosition = base.transform.position;
		StartingRotation = base.transform.rotation;
		MyAnimator = GetComponent<Animator>();
		RD.enabled = false;
		if (!MHC.DevMode)
		{
			DeactivateBoss();
		}
		StartCoroutine(ShootTank(CanDoSpecialAttack: false));
	}

	public void ResetAfterRam()
	{
		if (isRamming)
		{
			Debug.Log("DOne");
			isRamming = false;
			isRammingMoving = false;
			Timer = TimeTillRam;
			EA.CanMove = true;
			EA.isPathfinding = false;
			EA.hasGottenPath = false;
			RD.CheckForZeroSpeed = false;
		}
	}

	private void Update()
	{
		if (IsInFinalBattle && !BossDefeated)
		{
			CallInTimer -= Time.deltaTime;
			if (CallInTimer < 0f)
			{
				int[] array = ((OptionsMainMenu.instance.currentDifficulty != 0) ? ((OptionsMainMenu.instance.currentDifficulty != 1) ? ((OptionsMainMenu.instance.currentDifficulty != 2) ? new int[11]
				{
					0, 1, 2, 3, 4, 5, 6, 8, 9, 16,
					16
				} : new int[10] { 0, 1, 2, 3, 4, 5, 6, 16, 16, 16 }) : new int[3] { 16, 16, 16 }) : new int[2] { 16, 16 });
				int item = array[Random.Range(0, array.Length)];
				MHC.PM.SpawnInOrder.Add(item);
				MHC.PM.StartCoroutine(MHC.PM.DoOrder());
				CallInTimer = TimeBetweenCalls + Random.Range(0f, TimeBetweenCalls);
			}
			if (isRammingMoving && isRamming)
			{
				if ((bool)rb)
				{
					rb.isKinematic = false;
					rb.AddRelativeForce(RamDirection * RamSpeed * Time.deltaTime * 50f);
					EA.ETSN.ShootCountdown = 2f;
				}
				return;
			}
			Timer -= Time.deltaTime;
			if (Timer < 0f && !isRamming && !IsDoingAShot)
			{
				Debug.Log("PLAYING RAM CHARGING!!");
				SFXManager.instance.PlaySFX(RD.RamCharging);
				isRamming = true;
				isRammingMoving = false;
				RD.CheckForZeroSpeed = false;
				StartCoroutine(RamSequence());
				EA.enabled = true;
				EA.CanMove = false;
				Timer = TimeTillRam;
			}
		}
		else
		{
			EA.enabled = false;
			ETSN.enabled = false;
			Timer = TimeTillRam;
		}
	}

	private IEnumerator RamSequence()
	{
		_ = EA.transform.rotation;
		Vector3 from = GameMaster.instance.Players[0].transform.position - EA.transform.position;
		from.y = 0f;
		float angle = Vector3.Angle(from, EA.transform.forward);
		while (angle > 6f)
		{
			if (isRammingMoving)
			{
				yield break;
			}
			EA.CanMove = false;
			from = GameMaster.instance.Players[0].transform.position - EA.transform.position;
			from.y = 0f;
			Quaternion b = Quaternion.LookRotation(from);
			float num = (((float)HT.health < (float)HT.maxHealth / 2f) ? 7f : 5f);
			EA.transform.rotation = Quaternion.Slerp(EA.transform.rotation, b, Time.deltaTime * num);
			angle = Vector3.Angle(from, EA.transform.forward);
			yield return null;
		}
		yield return new WaitForSeconds(0.3f);
		RamDirection = Vector3.forward;
		isRammingMoving = true;
		yield return new WaitForSeconds(0.6f);
		if (isRamming)
		{
			RD.CheckForZeroSpeed = true;
		}
	}

	public void FallLayerDown()
	{
		MHC.PlayMusic(-1);
		GameObject[] blocksToDisableWhenThroneBroken = MHC.BlocksToDisableWhenThroneBroken;
		for (int i = 0; i < blocksToDisableWhenThroneBroken.Length; i++)
		{
			blocksToDisableWhenThroneBroken[i].SetActive(value: false);
		}
		StartCoroutine(FallingDown());
	}

	public void ActivateBoss()
	{
		MHC.PlayMusic(1);
		EA.GetComponent<Collider>().enabled = true;
		rb.isKinematic = false;
		EA.enabled = true;
		HT.enabled = true;
		BodyCollider.enabled = true;
		ETSN.enabled = true;
		MyAnimator.SetBool("MortarMode", value: false);
		StartCoroutine(DisableAnimator());
		LAO.enabled = false;
		FT.enabled = true;
	}

	private IEnumerator DisableAnimator()
	{
		yield return new WaitForSeconds(2f);
		MyAnimator.enabled = false;
	}

	public IEnumerator DestroyedBoss()
	{
		BossDefeated = true;
		isRamming = false;
		isRammingMoving = false;
		RD.RamAS.volume = 0f;
		ParticleSystem[] ramParticles = RD.RamParticles;
		for (int i = 0; i < ramParticles.Length; i++)
		{
			ramParticles[i].Stop();
		}
		RD.enabled = false;
		yield return new WaitForSeconds(0.01f);
		EA.enabled = false;
		HT.enabled = false;
		ETSN.enabled = false;
		FT.enabled = false;
		ETSN.gameObject.SetActive(value: false);
		EA.GetComponent<Collider>().enabled = true;
		rb.isKinematic = false;
		BodyCollider.enabled = true;
		MyAnimator.enabled = false;
		LAO.enabled = false;
	}

	public void DeactivateBoss()
	{
		EA.GetComponent<Collider>().enabled = false;
		rb.isKinematic = true;
		EA.enabled = false;
		HT.enabled = false;
		BodyCollider.enabled = false;
		ETSN.enabled = false;
		MyAnimator.enabled = true;
		LAO.enabled = true;
		FT.enabled = false;
		isRamming = false;
		isRammingMoving = false;
		EA.transform.localPosition = new Vector3(0f, 1.72f, 0f);
		ETSN.transform.localPosition = Vector3.zero;
		MHC.MoveArenaBlocksUp();
	}

	private IEnumerator FallingDown()
	{
		MHC.MoveArenaBlocksDown();
		IsInMortarMode = false;
		MyAnimator.SetBool("MortarMode", value: false);
		IsInFinalBattle = true;
		float t3 = 0f;
		Vector3 currentScale2 = Chute.transform.localScale;
		Vector3 ScaleToGo2 = new Vector3(300f, 300f, 300f);
		while (t3 < 1f)
		{
			t3 += Time.deltaTime * 4f;
			Chute.transform.localScale = Vector3.Lerp(currentScale2, ScaleToGo2, t3);
			yield return null;
		}
		Chute.transform.localScale = ScaleToGo2;
		t3 = 0f;
		Vector3 currentPos = base.transform.position;
		Vector3 PosToGo = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
		while (t3 < 1f)
		{
			t3 += Time.deltaTime / 4f;
			base.transform.position = Vector3.Lerp(currentPos, PosToGo, t3);
			yield return null;
		}
		base.transform.position = PosToGo;
		MyAnimator.SetBool("MortarMode", value: false);
		t3 = 0f;
		currentScale2 = Chute.transform.localScale;
		ScaleToGo2 = new Vector3(0f, 0f, 0f);
		while (t3 < 1f)
		{
			t3 += Time.deltaTime * 4f;
			Chute.transform.localScale = Vector3.Lerp(currentScale2, ScaleToGo2, t3);
			yield return null;
		}
		Chute.transform.localScale = ScaleToGo2;
		if (!IsInBattle)
		{
			base.transform.position = StartingPosition;
			base.transform.rotation = StartingRotation;
			yield return null;
		}
		else
		{
			IsInFinalBattle = true;
			ActivateBoss();
		}
	}

	private void Shoot(Transform firepoint, GameObject BulletPrefab, bool Charged)
	{
		GameObject obj = Object.Instantiate(BulletPrefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
		PlayerBulletScript component = obj.GetComponent<PlayerBulletScript>();
		component.MyTeam = EA.MyTeam;
		if (EA.isShiny)
		{
			component.isShiny = true;
		}
		if (EA.isSuperShiny)
		{
			component.isSuperShiny = true;
		}
		Rigidbody component2 = obj.GetComponent<Rigidbody>();
		if ((bool)component2)
		{
			component2.AddForce(firepoint.forward * 6f);
			component.StartingVelocity = firepoint.forward * 6f;
			_ = firepoint.position;
			Ray ray = new Ray(firepoint.position, firepoint.forward);
			LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall"));
			Debug.DrawRay(firepoint.position, firepoint.forward * 15f, Color.cyan, 3f);
			if (Physics.Raycast(ray, out var hitInfo, 1000f, layerMask))
			{
				component.WallGonnaBounceInTo = hitInfo.transform.gameObject;
			}
		}
		component.MaxBounces = 0;
		if (Charged)
		{
			component.isTowerCharged = true;
		}
		component.EnemyTankScript = EA.ETSN;
		component.isEnemyBullet = true;
		component.papaTank = EA.gameObject;
		Physics.IgnoreCollision(component.GetComponent<Collider>(), EA.GetComponent<Collider>());
		component.CollidersIgnoring.Add(EA.GetComponent<Collider>());
	}

	private IEnumerator DoAttack(int type)
	{
		if (IsDoingAShot)
		{
			yield break;
		}
		switch (type)
		{
		case 0:
		{
			SFXManager.instance.PlaySFX(ShotgunShotSound);
			IsDoingAShot = true;
			foreach (GameObject item in ChargeCirclesBarrel)
			{
				item.SetActive(value: false);
				item.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white);
			}
			yield return new WaitForSeconds(0.25f);
			ChargeCirclesBarrel[0].SetActive(value: true);
			ChargeCirclesBarrel[1].SetActive(value: true);
			yield return new WaitForSeconds(0.25f);
			ChargeCirclesBarrel[2].SetActive(value: true);
			ChargeCirclesBarrel[3].SetActive(value: true);
			yield return new WaitForSeconds(0.25f);
			ChargeCirclesBarrel[4].SetActive(value: true);
			ChargeCirclesBarrel[5].SetActive(value: true);
			yield return new WaitForSeconds(0.25f);
			for (int j = 0; j < ShootShotGunPoints.Count; j++)
			{
				Shoot(ShootShotGunPoints[j], ShotgunAttackBullet, Charged: true);
			}
			foreach (GameObject item2 in ChargeCirclesBarrel)
			{
				item2.SetActive(value: false);
			}
			IsDoingAShot = false;
			break;
		}
		case 1:
		{
			SFXManager.instance.PlaySFX(OnlyShootingChargeSound_quick);
			IsDoingAShot = true;
			foreach (GameObject item3 in ChargeCirclesBarrel)
			{
				item3.SetActive(value: false);
				item3.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.yellow);
			}
			yield return new WaitForSeconds(0.05f);
			ChargeCirclesBarrel[0].SetActive(value: true);
			ChargeCirclesBarrel[1].SetActive(value: true);
			yield return new WaitForSeconds(0.05f);
			ChargeCirclesBarrel[2].SetActive(value: true);
			ChargeCirclesBarrel[3].SetActive(value: true);
			yield return new WaitForSeconds(0.05f);
			ChargeCirclesBarrel[4].SetActive(value: true);
			ChargeCirclesBarrel[5].SetActive(value: true);
			yield return new WaitForSeconds(0.15f);
			for (int k = 0; k < 2; k++)
			{
				SFXManager.instance.PlaySFX(NormalRocketSound);
				Shoot(ShootShotGunPoints[k], RocketBullet, Charged: false);
			}
			IsDoingAShot = false;
			{
				foreach (GameObject item4 in ChargeCirclesBarrel)
				{
					item4.SetActive(value: false);
				}
				break;
			}
		}
		case 2:
		{
			SFXManager.instance.PlaySFX(OnlyShootingChargeSound);
			IsDoingAShot = true;
			foreach (GameObject item5 in ChargeCirclesBarrel)
			{
				item5.SetActive(value: false);
				item5.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
			}
			float OriginalSpeed = EA.TankSpeed;
			EA.CanMove = false;
			EA.TankSpeed = 0f;
			yield return new WaitForSeconds(0.25f);
			ChargeCirclesBarrel[0].SetActive(value: true);
			ChargeCirclesBarrel[1].SetActive(value: true);
			yield return new WaitForSeconds(0.25f);
			ChargeCirclesBarrel[2].SetActive(value: true);
			ChargeCirclesBarrel[3].SetActive(value: true);
			yield return new WaitForSeconds(0.25f);
			ChargeCirclesBarrel[4].SetActive(value: true);
			ChargeCirclesBarrel[5].SetActive(value: true);
			yield return new WaitForSeconds(0.35f);
			for (int i = 0; i < 11; i++)
			{
				SFXManager.instance.PlaySFX(NormalRocketSound);
				Shoot(ShootShotGunPoints[i % 2], RocketBullet, Charged: false);
				yield return new WaitForSeconds(0.15f);
			}
			EA.CanMove = true;
			EA.TankSpeed = OriginalSpeed;
			IsDoingAShot = false;
			{
				foreach (GameObject item6 in ChargeCirclesBarrel)
				{
					item6.SetActive(value: false);
				}
				break;
			}
		}
		}
	}

	public IEnumerator ShootTank(bool CanDoSpecialAttack)
	{
		Debug.Log("SHOOTING CHECK");
		float seconds = ((OptionsMainMenu.instance.currentDifficulty < 2) ? 2.5f : 1.75f);
		yield return new WaitForSeconds(seconds);
		if (!isRamming && Timer < 8f && !IsDoingAShot && IsInFinalBattle && EA.ETSN.currentTarget != null && !MHC.BomberPlane.isFlying)
		{
			float num = Vector3.Distance(EA.ETSN.currentTarget.transform.position, base.transform.position);
			if (OptionsMainMenu.instance.currentDifficulty == 0)
			{
				if (num < 14f)
				{
					StartCoroutine(DoAttack(0));
				}
				else if (Random.Range(0, 2) == 0)
				{
					StartCoroutine(DoAttack(0));
				}
				else
				{
					StartCoroutine(DoAttack(1));
				}
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 1 || OptionsMainMenu.instance.currentDifficulty == 2)
			{
				if ((float)EA.HTscript.health < (float)EA.HTscript.maxHealth * 0.6f && Random.Range(0, 1) == 0)
				{
					StartCoroutine(DoAttack(2));
				}
				if (num < 14f)
				{
					StartCoroutine(DoAttack(0));
				}
				else if (Random.Range(0, 2) == 0)
				{
					StartCoroutine(DoAttack(0));
				}
				else
				{
					StartCoroutine(DoAttack(1));
				}
			}
		}
		StartCoroutine(ShootTank(CanDoSpecialAttack: true));
	}

	public IEnumerator ShootABomb(bool first, int shotsToGo)
	{
		if (first && IsInBattle && !IsInFinalBattle)
		{
			IsInMortarMode = true;
			LAO.LookStraight = true;
			LAO.FollowPlayer = false;
			MyAnimator.SetBool("MortarMode", value: true);
			yield return new WaitForSeconds(3f);
		}
		if (!IsInBattle || !IsInMortarMode || IsInFinalBattle)
		{
			IsInMortarMode = false;
			MyAnimator.SetBool("MortarMode", value: false);
			yield break;
		}
		GameObject gameObject = Object.Instantiate(Mortar, ShootingPoint.position, ShootingPoint.rotation);
		ShotMortars.Add(gameObject);
		BombSackScript component = gameObject.GetComponent<BombSackScript>();
		component.MHC = MHC;
		if ((bool)component)
		{
			component.ShootMeTutorial.SetActive(value: false);
		}
		SFXManager.instance.PlaySFX(MortarSound[Random.Range(0, MortarSound.Length)], 1f, null);
		MortarParticles.Play(withChildren: true);
		yield return new WaitForSeconds(1f);
		if (shotsToGo < 1)
		{
			LAO.LookStraight = false;
			LAO.FollowPlayer = true;
			IsInMortarMode = false;
			MyAnimator.SetBool("MortarMode", value: false);
		}
		else
		{
			KingTankScript kingTankScript = this;
			KingTankScript kingTankScript2 = this;
			int num = shotsToGo - 1;
			shotsToGo = num;
			kingTankScript.StartCoroutine(kingTankScript2.ShootABomb(first: false, num));
		}
	}

	private IEnumerator Shoot()
	{
		MyAnimator.SetBool("Shoot", value: true);
		yield return new WaitForSeconds(0.05f);
		MyAnimator.SetBool("Shoot", value: false);
	}

	public void InitiateFall()
	{
	}

	private IEnumerator BossFall()
	{
		yield return new WaitForSeconds(0.1f);
	}
}
