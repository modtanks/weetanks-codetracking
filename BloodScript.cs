using UnityEngine;

public class BloodScript : MonoBehaviour
{
	public Color myColor;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("player entered!");
			MoveTankScript MTS = other.gameObject.GetComponent<MoveTankScript>();
			if ((bool)MTS && MTS.skidMarkCreator != null)
			{
				SkidmarkController SC2 = MTS.skidMarkCreator.GetComponent<SkidmarkController>();
				if ((bool)SC2)
				{
					SC2.ActivateBlood(myColor);
				}
			}
		}
		else
		{
			if (!(other.tag == "Enemy") && !(other.tag == "Boss"))
			{
				return;
			}
			EnemyAI EA = other.gameObject.GetComponent<EnemyAI>();
			if ((bool)EA && EA.skidMarkCreator != null)
			{
				SkidmarkController SC = EA.skidMarkCreator.GetComponent<SkidmarkController>();
				if ((bool)SC)
				{
					SC.ActivateBlood(myColor);
				}
			}
		}
	}
}
