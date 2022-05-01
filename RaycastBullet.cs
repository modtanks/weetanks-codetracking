using System;
using System.Linq;
using UnityEngine;

public class RaycastBullet : MonoBehaviour
{
	public GameObject raycastObject;

	public int checkDistance = 30;

	public bool isTrain = false;

	private Rigidbody rb;

	private PlayerBulletScript PBS;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		float randomCheckTime = UnityEngine.Random.Range(0.1f, 0.2f);
		InvokeRepeating("CheckObjects", randomCheckTime, randomCheckTime);
		PBS = GetComponent<PlayerBulletScript>();
	}

	private void CheckObjects()
	{
		if (!isTrain)
		{
			checkDistance = 24;
			DrawReflectionPattern(base.transform.position, rb.velocity, lastCheck: false);
			return;
		}
		Vector3 fwd = raycastObject.transform.TransformDirection(Vector3.forward);
		Debug.DrawRay(raycastObject.transform.position, fwd * checkDistance, Color.cyan, 0.3f);
		RaycastHit[] allhits = (from h in Physics.RaycastAll(raycastObject.transform.position, fwd, checkDistance)
			orderby h.distance
			select h).ToArray();
		for (int i = 0; i < allhits.Length; i++)
		{
			RaycastHit hit = allhits[i];
			if (hit.transform.tag == "Solid" || hit.transform.tag == "MapBorder")
			{
				break;
			}
			DoHit(hit, 0, Vector3.zero);
		}
	}

	private void DrawReflectionPattern(Vector3 position, Vector3 direction, bool lastCheck)
	{
		Vector3 startingPosition = position;
		if (lastCheck)
		{
			checkDistance = 12;
		}
		LayerMask ignoreLayer = ~((1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("OneWayBlock")) | (1 << LayerMask.NameToLayer("EnemyBorder")));
		RaycastHit[] allhits = (from h in Physics.RaycastAll(startingPosition, direction, checkDistance, ignoreLayer)
			orderby h.distance
			select h).ToArray();
		for (int i = 0; i < allhits.Length; i++)
		{
			RaycastHit hit = allhits[i];
			if (hit.transform.tag == "Solid" || hit.transform.tag == "MapBorder")
			{
				position = hit.point;
				if (!lastCheck)
				{
					direction = Vector3.Reflect(direction.normalized, hit.normal);
				}
				break;
			}
			if (lastCheck)
			{
				DoHit(hit, 0, Vector3.zero);
			}
			else
			{
				DoHit(hit, 1, Vector3.zero);
			}
		}
		if (startingPosition == position)
		{
		}
		if (PBS.TimesBounced < PBS.MaxBounces && !lastCheck)
		{
			DrawReflectionPattern(position, direction, lastCheck: true);
		}
	}

	public void DoHit(RaycastHit col, int smart, Vector3 reflectionPoint)
	{
		if (col.collider.tag == "EnemyDetectionField")
		{
			EnemyDetection ED = col.transform.GetComponent<EnemyDetection>();
			if (!(ED != null))
			{
				return;
			}
			PlayerBulletScript PBS = GetComponent<PlayerBulletScript>();
			if (PBS != null)
			{
				if (PBS.isEnemyBullet)
				{
					if (PBS.papaTank == ED.papaTank && PBS.TimesBounced > 0)
					{
						TargetED(ED);
					}
					else if (PBS.papaTank != ED.papaTank)
					{
						TargetED(ED);
					}
				}
				else if (PBS.EnemyTankScript != null)
				{
					if (PBS.EnemyTankScript.AIscript.gameObject == ED.papaTank && PBS.TimesBounced > 0)
					{
						TargetED(ED);
					}
					else if (PBS.EnemyTankScript.AIscript.gameObject != ED.papaTank)
					{
						TargetED(ED);
					}
				}
				else
				{
					TargetED(ED);
				}
			}
			else
			{
				TargetED(ED);
			}
		}
		else if (col.collider.tag == "Enemy")
		{
			EnemyAI EA = col.collider.GetComponent<EnemyAI>();
			if (EA != null && (bool)EA.Ring0Detection)
			{
				TargetED(EA.Ring0Detection);
			}
		}
	}

	private void TargetED(EnemyDetection ED)
	{
		if (ED.ignoringBullets)
		{
			return;
		}
		bool shotByOtherPlayer = false;
		PlayerBulletScript PBS = GetComponent<PlayerBulletScript>();
		if ((bool)PBS && PBS.ShotByPlayer != 1)
		{
			shotByOtherPlayer = true;
		}
		EnemyAI hisAI = ED.papaTank.GetComponent<EnemyAI>();
		if (ED.papaTank.transform.tag == "Player" && !OptionsMainMenu.instance.FriendlyFire && shotByOtherPlayer && hisAI.MyTeam == PBS.MyTeam && hisAI.MyTeam != 0)
		{
			return;
		}
		ED.isTargeted = true;
		float dist = Vector3.Distance(base.transform.position, ED.transform.position);
		ED.TargetDanger = Mathf.RoundToInt(Math.Abs(32f - dist));
		if (ED.isCenter && hisAI != null && !hisAI.IncomingBullets.Contains(base.gameObject))
		{
			hisAI.IncomingBullets.Add(base.gameObject);
		}
		Collider BulletCollider = base.gameObject.GetComponent<Collider>();
		if (ED.Bullets.Contains(BulletCollider))
		{
			return;
		}
		ED.Bullets.Add(BulletCollider);
		if (ED.isRing1)
		{
			int lowerID = ((ED.ID == 0) ? 15 : (ED.ID - 1));
			int higerID = ((ED.ID != 15) ? (ED.ID + 1) : 0);
			EnemyDetection ED2 = ED.AIscript.Ring1Detection.Find((EnemyDetection x) => x.ID == lowerID);
			EnemyDetection ED3 = ED.AIscript.Ring1Detection.Find((EnemyDetection x) => x.ID == higerID);
			if ((bool)ED2 && !ED2.Bullets.Contains(BulletCollider))
			{
				ED2.Bullets.Add(BulletCollider);
			}
			if ((bool)ED3 && !ED3.Bullets.Contains(BulletCollider))
			{
				ED3.Bullets.Add(BulletCollider);
			}
		}
	}
}
