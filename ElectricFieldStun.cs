using UnityEngine;

public class ElectricFieldStun : MonoBehaviour
{
	public int secStunned = 2;

	public EnemyAI papaScript;

	public bool isKilling = false;

	private void OnTriggerEnter(Collider other)
	{
		if (!papaScript.isElectric || !papaScript.isCharged)
		{
			return;
		}
		if (other.tag == "Player")
		{
			if (isKilling)
			{
				HealthTanks HT = other.GetComponent<HealthTanks>();
				if (HT != null)
				{
					HT.health--;
				}
			}
			else
			{
				MoveTankScript MTS = other.GetComponent<MoveTankScript>();
				if (MTS != null)
				{
					MTS.StunMe(secStunned);
				}
			}
		}
		else if (other.tag == "Enemy")
		{
			EnemyAI AI = other.GetComponent<EnemyAI>();
			if (AI != null && !AI.isElectric)
			{
				AI.StunMe(secStunned);
			}
		}
	}
}
