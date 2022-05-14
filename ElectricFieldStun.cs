using UnityEngine;

public class ElectricFieldStun : MonoBehaviour
{
	public int secStunned = 2;

	public EnemyAI papaScript;

	public bool isKilling;

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
				HealthTanks component = other.GetComponent<HealthTanks>();
				if (component != null)
				{
					component.health--;
				}
			}
			else
			{
				MoveTankScript component2 = other.GetComponent<MoveTankScript>();
				if (component2 != null)
				{
					component2.StunMe(secStunned);
				}
			}
		}
		else if (other.tag == "Enemy")
		{
			EnemyAI component3 = other.GetComponent<EnemyAI>();
			if (component3 != null && !component3.isElectric)
			{
				component3.StunMe(secStunned);
			}
		}
	}
}
