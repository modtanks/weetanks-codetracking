using System.Collections;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
	public PathfindingBlock myPB;

	public bool Picked = false;

	private void Start()
	{
		StartCoroutine(GetMyPB());
	}

	public void GotPicked()
	{
		Picked = true;
		StartCoroutine(ResetPick());
	}

	private IEnumerator ResetPick()
	{
		yield return new WaitForSeconds(8f);
		Picked = false;
	}

	private IEnumerator GetMyPB()
	{
		float searchTime = Random.Range(3f, 7f);
		yield return new WaitForSeconds(searchTime);
		PathfindingBlock ClosestPB = null;
		float recentDist = 99999f;
		foreach (PathfindingBlock PB in PathfindingBlocksMaster.instance.AllBlocks)
		{
			float dist = Vector3.Distance(base.transform.position, PB.transform.position);
			if (dist < recentDist)
			{
				ClosestPB = PB;
				recentDist = dist;
			}
		}
		myPB = ClosestPB;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position, new Vector3(2f, 2f, 2f));
	}
}
