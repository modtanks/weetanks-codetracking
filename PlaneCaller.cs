using System.Collections;
using UnityEngine;

public class PlaneCaller : MonoBehaviour
{
	public float CallingInInterval;

	private PlaneMaster PM;

	public MeshRenderer RadarSphere;

	public Color CalledInColor;

	public Color NoAirSpace;

	private void Start()
	{
		PM = GameMaster.instance.GetComponent<PlaneMaster>();
		StartCoroutine(InvokerPlane());
		CallingInInterval = ((OptionsMainMenu.instance.currentDifficulty > 1) ? 8 : 9);
	}

	private IEnumerator InvokerPlane()
	{
		float seconds = CallingInInterval + Random.Range(0f - CallingInInterval / 2f, CallingInInterval / 2f);
		yield return new WaitForSeconds(seconds);
		if (GameMaster.instance.GameHasStarted)
		{
			if (!PM.PS.isFlying && PM.SpawnInOrder.Count < 1 && GameMaster.instance.AmountCalledInTanks < 5)
			{
				if (RadarSphere != null)
				{
					RadarSphere.material.SetColor("_Color", CalledInColor);
					RadarSphere.material.SetColor("_EmissionColor", CalledInColor);
				}
				StartCoroutine(CallInPlane(-1));
			}
			else if (RadarSphere != null)
			{
				RadarSphere.material.SetColor("_Color", NoAirSpace);
				RadarSphere.material.SetColor("_EmissionColor", NoAirSpace);
			}
		}
		else if (RadarSphere != null)
		{
			RadarSphere.material.SetColor("_Color", Color.yellow);
			RadarSphere.material.SetColor("_EmissionColor", Color.yellow);
		}
		StartCoroutine(InvokerPlane());
		if (RadarSphere != null)
		{
			StartCoroutine(ResetColor());
		}
	}

	private IEnumerator CallInPlane(int TankID)
	{
		TankID = -1;
		yield return new WaitForSeconds(1f);
		EnemyAI component = GetComponent<EnemyAI>();
		int teamNumber = 2;
		if ((bool)component)
		{
			teamNumber = component.MyTeam;
		}
		PM.SpawnPlane(teamNumber, TankID);
	}

	private IEnumerator ResetColor()
	{
		yield return new WaitForSeconds(1f);
		RadarSphere.material.SetColor("_Color", Color.white);
		RadarSphere.material.SetColor("_EmissionColor", Color.white);
	}
}
