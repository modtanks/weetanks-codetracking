using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianScript : MonoBehaviour
{
	[Header("Start Settings")]
	public Vector3 MyStartPos;

	public bool IsStanding;

	public bool CanWalk;

	public bool IsWalking;

	public GameObject LookAtObject;

	public List<GameObject> GroupTanks;

	public List<GameObject> TanksCloseBy;

	public float AngryCooldown;

	public int AnnoyedState;

	private EnemyAI EA;

	private EnemyTargetingSystemNew ETSN;

	private void Start()
	{
		MyStartPos = base.transform.position;
		Debug.Log("setting stuff");
		EA = GetComponent<EnemyAI>();
		ETSN = EA.ETSN;
		ETSN.CanGetNewTarget = false;
		ETSN.canShoot = false;
		if (IsStanding)
		{
			EA.CanMove = false;
		}
		else if (IsWalking)
		{
			EA.CanMove = true;
		}
		if (LookAtObject == null)
		{
			ETSN.IsJustLookingAround = true;
		}
		else
		{
			Debug.Log("ETSN as well");
			ETSN.canShoot = false;
			ETSN.Targets.Clear();
			ETSN.Targets.Add(LookAtObject);
			ETSN.currentTarget = LookAtObject;
		}
		StartCoroutine(CheckSurroundings());
	}

	private void OnCollisionEnter(Collision other)
	{
		if (AngryCooldown > 0f)
		{
			return;
		}
		Debug.Log("COLLISION FOUND! " + other.gameObject.tag, other.gameObject);
		if (!TanksCloseBy.Contains(other.gameObject) || !ETSN.Targets.Contains(other.gameObject))
		{
			return;
		}
		if (GroupTanks.Count > 0)
		{
			foreach (GameObject groupTank in GroupTanks)
			{
				if ((bool)groupTank)
				{
					CivilianScript component = groupTank.GetComponent<CivilianScript>();
					if ((bool)component)
					{
						component.AngryCooldown = 8f;
						component.AnnoyedState = 3;
						component.ETSN.CanSpawnBullet = true;
						component.ETSN.StartCoroutine(component.ETSN.StartTheShooting(other.gameObject));
					}
				}
			}
		}
		BecomeAggressive(other.gameObject);
	}

	private void Update()
	{
		if (AnnoyedState >= 2 && CanWalk && !IsWalking)
		{
			IsWalking = true;
			EA.CanMove = true;
		}
		else if (AnnoyedState < 2 && CanWalk)
		{
			_ = IsWalking;
		}
		if (GameMaster.instance.Players.Count < 1)
		{
			AngryCooldown = 0f;
			AnnoyedState = 0;
			ETSN.CanSpawnBullet = false;
		}
		if (AngryCooldown > 0f && (!ETSN.TargetInSight || ETSN.currentTarget == null))
		{
			AngryCooldown -= Time.deltaTime;
		}
		else if (AngryCooldown <= 0f)
		{
			ETSN.CanSpawnBullet = false;
			AnnoyedState = 1;
		}
	}

	private void OnDestroy()
	{
		if (GroupTanks.Count <= 0)
		{
			return;
		}
		foreach (GameObject groupTank in GroupTanks)
		{
			if (!(groupTank != null))
			{
				continue;
			}
			CivilianScript component = groupTank.GetComponent<CivilianScript>();
			if ((bool)component)
			{
				if (ETSN.currentTarget != null)
				{
					component.BecomeAggressive(ETSN.currentTarget);
					continue;
				}
				if (TanksCloseBy.Count > 0)
				{
					component.BecomeAggressive(TanksCloseBy[0].gameObject);
					continue;
				}
				component.AngryCooldown = 8f;
				component.AnnoyedState = 3;
			}
		}
	}

	public void BecomeAggressive(GameObject Target)
	{
		AngryCooldown = 8f;
		ETSN.CanSpawnBullet = true;
		ETSN.StartCoroutine(ETSN.StartTheShooting(Target));
		AnnoyedState = 3;
	}

	private IEnumerator CheckSurroundings()
	{
		float seconds = Random.Range(1f, 3f);
		yield return new WaitForSeconds(seconds);
		LayerMask layerMask = (1 << LayerMask.NameToLayer("Tank")) | (1 << LayerMask.NameToLayer("PlayerTank")) | (1 << LayerMask.NameToLayer("Other"));
		Collider[] array = Physics.OverlapSphere(base.transform.position, 7f, layerMask);
		if (AnnoyedState < 3)
		{
			if (IsStanding && IsWalking && CanWalk && Vector3.Distance(base.transform.position, MyStartPos) < 2f)
			{
				IsWalking = false;
				EA.CanMove = false;
			}
			TanksCloseBy.Clear();
			bool flag = false;
			ETSN.Targets.Clear();
			AnnoyedState = 0;
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (!TanksCloseBy.Contains(collider.gameObject) && collider.gameObject != base.gameObject && !GroupTanks.Contains(collider.gameObject))
				{
					TanksCloseBy.Add(collider.gameObject);
					flag = true;
					ETSN.Targets.Add(collider.gameObject);
					if (AnnoyedState == 0)
					{
						AnnoyedState = 1;
					}
				}
				if (collider.tag == "Bullet")
				{
					AnnoyedState = 2;
					if (TanksCloseBy[0].gameObject != null)
					{
						BecomeAggressive(TanksCloseBy[0].gameObject);
					}
				}
			}
			if (!flag)
			{
				ETSN.CanSpawnBullet = false;
			}
			if (ETSN.Targets.Contains(LookAtObject) && flag)
			{
				ETSN.Targets.Remove(LookAtObject);
			}
			else if (!ETSN.Targets.Contains(LookAtObject) && !flag)
			{
				ETSN.Targets.Add(LookAtObject);
			}
			if (!ETSN.Targets.Contains(ETSN.currentTarget))
			{
				ETSN.currentTarget = null;
			}
		}
		StartCoroutine(CheckSurroundings());
	}
}
