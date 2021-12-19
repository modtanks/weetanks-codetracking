using UnityEngine;

public class RemoveAfterRound : MonoBehaviour
{
	public int missionnow;

	private void Start()
	{
		missionnow = GameMaster.instance.CurrentMission;
	}

	private void Update()
	{
		if (GameMaster.instance.AmountGoodTanks < 1 && GameMaster.instance.AmountEnemyTanks < 1)
		{
			Object.Destroy(base.gameObject);
		}
		if (missionnow != GameMaster.instance.CurrentMission)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
