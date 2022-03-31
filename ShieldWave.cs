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
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 14f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			Transform Shield = col.transform.Find("Shield");
			if (!Shield)
			{
				continue;
			}
			EnemyAI EA = col.GetComponent<EnemyAI>();
			if ((bool)EA && ((EA.MyTeam != myEA.MyTeam && myEA.MyTeam != 0) || EA.HTscript.EnemyID == 16))
			{
				continue;
			}
			MoveTankScript MTS = col.GetComponent<MoveTankScript>();
			if ((bool)MTS && MTS.MyTeam != myEA.MyTeam && myEA.MyTeam != 0)
			{
				continue;
			}
			Debug.Log("Found comrade!");
			ShieldScript SS = Shield.GetComponent<ShieldScript>();
			if ((bool)SS)
			{
				if (SS.isPeachShield)
				{
					break;
				}
				int maxHealth = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 2 : ((OptionsMainMenu.instance.currentDifficulty == 1) ? 3 : 5));
				if (SS.ShieldHealth < maxHealth)
				{
					SS.ShieldHealth++;
				}
			}
		}
	}
}
