using UnityEngine;

public class ShieldWave : MonoBehaviour
{
	private Animator myAnim;

	public EnemyAI myEA;

	private void Start()
	{
		myAnim = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			myAnim.enabled = false;
		}
		else
		{
			myAnim.enabled = true;
		}
	}

	public void CheckForShields()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 14f);
		foreach (Collider collider in array)
		{
			Transform transform = collider.transform.Find("Shield");
			if (!transform)
			{
				continue;
			}
			EnemyAI component = collider.GetComponent<EnemyAI>();
			if ((bool)component && ((component.MyTeam != myEA.MyTeam && myEA.MyTeam != 0) || component.HTscript.EnemyID == 16))
			{
				continue;
			}
			MoveTankScript component2 = collider.GetComponent<MoveTankScript>();
			if ((bool)component2 && component2.MyTeam != myEA.MyTeam && myEA.MyTeam != 0)
			{
				continue;
			}
			Debug.Log("Found comrade!");
			ShieldScript component3 = transform.GetComponent<ShieldScript>();
			if ((bool)component3)
			{
				if (component3.isPeachShield)
				{
					break;
				}
				int num = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 2 : ((OptionsMainMenu.instance.currentDifficulty == 1) ? 3 : 5));
				if (component3.ShieldHealth < num)
				{
					component3.ShieldHealth++;
				}
			}
		}
	}
}
