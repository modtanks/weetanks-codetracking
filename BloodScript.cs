using UnityEngine;

public class BloodScript : MonoBehaviour
{
	public Color myColor;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("player entered!");
			MoveTankScript component = other.gameObject.GetComponent<MoveTankScript>();
			if ((bool)component && component.skidMarkCreator != null)
			{
				SkidmarkController component2 = component.skidMarkCreator.GetComponent<SkidmarkController>();
				if ((bool)component2)
				{
					component2.ActivateBlood(myColor);
				}
			}
		}
		else
		{
			if (!(other.tag == "Enemy") && !(other.tag == "Boss"))
			{
				return;
			}
			EnemyAI component3 = other.gameObject.GetComponent<EnemyAI>();
			if ((bool)component3 && component3.skidMarkCreator != null)
			{
				SkidmarkController component4 = component3.skidMarkCreator.GetComponent<SkidmarkController>();
				if ((bool)component4)
				{
					component4.ActivateBlood(myColor);
				}
			}
		}
	}
}
