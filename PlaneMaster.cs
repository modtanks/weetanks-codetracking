using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMaster : MonoBehaviour
{
	public GameObject ThePlane;

	public PlaneScript PS;

	public List<int> SpawnInOrder = new List<int>();

	private void Update()
	{
	}

	public IEnumerator DoOrder()
	{
		yield return new WaitForSeconds(0.4f);
		if (SpawnInOrder.Count > 0 && !PS.isFlying)
		{
			Debug.Log("Doing order, spawn now!");
			SpawnPlane(0, SpawnInOrder[0]);
			SpawnInOrder.RemoveAt(0);
			StartCoroutine(DoOrder());
		}
		else if (SpawnInOrder.Count > 0)
		{
			StartCoroutine(DoOrder());
		}
	}

	public void SpawnPlane(int TeamNumber, int TankID)
	{
		if (!PS.isFlying)
		{
			Vector3 Location = GameMaster.instance.GetValidLocation(CheckForDist: false, 0f, Vector3.zero, TargetPlayer: false);
			if (Location != Vector3.zero)
			{
				PS.TargetLocation = new Vector3(Location.x, 10f, Location.z);
				PS.StartFlyToTB(TeamNumber, TankID);
			}
		}
	}
}
