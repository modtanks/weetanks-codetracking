using System.Collections;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
	public PathfindingBlock myPB;

	public bool Picked;

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
		Debug.Log("GEtimg COLLIDERS !!!!");
		float seconds = Random.Range(3f, 7f);
		yield return new WaitForSeconds(seconds);
		PathfindingBlock pathfindingBlock = null;
		float num = 99999f;
		foreach (PathfindingBlock allBlock in PathfindingBlocksMaster.instance.AllBlocks)
		{
			float num2 = Vector3.Distance(base.transform.position, allBlock.transform.position);
			if (num2 < num)
			{
				pathfindingBlock = allBlock;
				num = num2;
			}
		}
		myPB = pathfindingBlock;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position, new Vector3(2f, 2f, 2f));
	}
}
