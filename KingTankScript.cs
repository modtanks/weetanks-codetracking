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

	public bool BossDefeated;

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
				int[] array = ((OptionsMainMenu.instance.currentDifficulty != 0) ? ((OptionsMainMenu.instance.currentDifficulty != 1) ? ((OptionsMainMenu.instance.currentDifficulty != 2) ? new int[9] { 0, 1, 2, 3, 4, 5, 6, 8, 9 } : new int[8] { 0, 1, 2, 3, 4, 5, 6, 16 }) : new int[6] { 0, 1, 3, 4, 5, 16 }) : new int[4] { 0, 1, 3, 16 });
				int item = array[Random.Range(0, array.Length)];
				MHC.PM.SpawnInOrder.Add(item);
				MHC.PM.StartCoroutine(MHC.PM.DoOrder());
				CallInTimer = TimeBetweenCalls + Random.Range(0f, TimeBetweenCalls);
			}
			if (isRammingMoving && isRamming)
			{
				if ((bool)rb)
				{
					Debug.Log("FORCE");
					rb.isKinematic = false;
					rb.AddRelativeForce(Vector3.forward * RamSpeed * Time.deltaTime * 50f);
				}
				else
				{
					Debug.Log("AAHHH");
				}
				return;
			}
			Timer -= Time.deltaTime;
			if (Timer < 0f && !isRamming)
			{
				isRamming = true;
				isRammingMoving = false;
				RD.CheckForZeroSpeed = false;
				StartCoroutine(RamSequence());
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
		Debug.Log("GO!!!");
		isRammingMoving = true;
		yield return new WaitForSeconds(0.6f);
		Debug.Log("done..!!!");
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
	}

	private IEnumerator FallingDown()
	{
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
