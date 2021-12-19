using System;
using System.Linq;
using UnityEngine;

public class RaycastBullet : MonoBehaviour
{
	public GameObject raycastObject;

	public int checkDistance = 30;

	public bool isTrain;

	private Rigidbody rb;

	private PlayerBulletScript PBS;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		InvokeRepeating("CheckObjects", 0.1f, 0.1f);
		PBS = GetComponent<PlayerBulletScript>();
	}

	private void CheckObjects()
	{
		Vector3 vector = raycastObject.transform.TransformDirection(Vector3.forward);
		if (!isTrain)
		{
			checkDistance = 24;
			DrawReflectionPattern(base.transform.position, rb.velocity, lastCheck: false);
			return;
		}
		Debug.DrawRay(raycastObject.transform.position, vector * checkDistance, Color.cyan, 0.3f);
		RaycastHit[] array = (from h in Physics.RaycastAll(raycastObject.transform.position, vector, checkDistance)
			orderby h.distance
			select h).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit col = array[i];
			if (col.transform.tag == "Solid" || col.transform.tag == "MapBorder")
			{
				break;
			}
			DoHit(col, 0, Vector3.zero);
		}
	}

	private void DrawReflectionPattern(Vector3 position, Vector3 direction, bool lastCheck)
	{
		Vector3 vector = position;
		if (lastCheck)
		{
			checkDistance = 12;
		}
		RaycastHit[] array = (from h in Physics.RaycastAll(vector, direction, checkDistance)
			orderby h.distance
			select h).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit col = array[i];
			if (col.transform.tag == "Solid" || col.transform.tag == "MapBorder")
			{
				position = col.point;
				if (!lastCheck)
				{
					direction = Vector3.Reflect(direction.normalized, col.normal);
				}
				break;
			}
			if (lastCheck)
			{
				DoHit(col, 0, Vector3.zero);
			}
			else
			{
				DoHit(col, 1, Vector3.zero);
			}
		}
		_ = vector == position;
		if (PBS.TimesBounced < PBS.MaxBounces && !lastCheck)
		{
			DrawReflectionPattern(position, direction, lastCheck: true);
		}
	}

	public void DoHit(RaycastHit col, int smart, Vector3 reflectionPoint)
	{
		if (!(col.collider.tag == "EnemyDetectionField"))
		{
			return;
		}
		EnemyDetection component = col.transform.GetComponent<EnemyDetection>();
		if (!(component != null))
		{
			return;
		}
		PlayerBulletScript component2 = GetComponent<PlayerBulletScript>();
		if (component2 != null)
		{
			if (component2.isEnemyBullet)
			{
				if (component2.papaTank == component.papaTank && component2.TimesBounced > 0)
				{
					TargetED(component);
				}
				else if (component2.papaTank != component.papaTank)
				{
					TargetED(component);
				}
			}
			else if (component2.TankScriptAI != null)
			{
				if (component2.TankScriptAI.AIscript.gameObject == component.papaTank && component2.TimesBounced > 0)
				{
					TargetED(component);
				}
				else if (component2.TankScriptAI.AIscript.gameObject != component.papaTank)
				{
					TargetED(component);
				}
			}
			else
			{
				TargetED(component);
			}
		}
		else
		{
			TargetED(component);
		}
	}

	private void TargetED(EnemyDetection ED)
	{
		if (ED.ignoringBullets)
		{
			return;
		}
		bool flag = false;
		PlayerBulletScript component = GetComponent<PlayerBulletScript>();
		if ((bool)component && component.ShotByPlayer != 1)
		{
			flag = true;
		}
		EnemyAI component2 = ED.papaTank.GetComponent<EnemyAI>();
		if (ED.papaTank.transform.tag == "Player" && !OptionsMainMenu.instance.FriendlyFire && flag && component2.MyTeam == component.MyTeam && component2.MyTeam != 0)
		{
			return;
		}
		ED.isTargeted = true;
		float num = Vector3.Distance(base.transform.position, ED.transform.position);
		ED.TargetDanger = Mathf.RoundToInt(Math.Abs(32f - num));
		if (ED.isCenter && component2 != null && !component2.IncomingBullets.Contains(base.gameObject))
		{
			component2.IncomingBullets.Add(base.gameObject);
		}
		if (ED.Bullets.Contains(base.gameObject))
		{
			return;
		}
		ED.Bullets.Add(base.gameObject);
		if (ED.isRing1)
		{
			int lowerID = ((ED.ID == 0) ? 15 : (ED.ID - 1));
			int higerID = ((ED.ID != 15) ? (ED.ID + 1) : 0);
			EnemyDetection enemyDetection = ED.AIscript.Ring1Detection.Find((EnemyDetection x) => x.ID == lowerID);
			EnemyDetection enemyDetection2 = ED.AIscript.Ring1Detection.Find((EnemyDetection x) => x.ID == higerID);
			if ((bool)enemyDetection && !enemyDetection.Bullets.Contains(base.gameObject))
			{
				enemyDetection.Bullets.Add(base.gameObject);
			}
			if ((bool)enemyDetection2 && !enemyDetection2.Bullets.Contains(base.gameObject))
			{
				enemyDetection2.Bullets.Add(base.gameObject);
			}
		}
	}
}
