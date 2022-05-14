using UnityEngine;

public class triggerplate : MonoBehaviour
{
	public bool Triggered;

	public bool IsFinalBossTrigger;

	public MissionHundredController MHC;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag == "Player") || Triggered)
		{
			return;
		}
		Triggered = true;
		if (IsFinalBossTrigger)
		{
			MHC.FinalBossTrigger();
			return;
		}
		SFXManager.instance.PlaySFX(MHC.WallsMovingDown, 2f, null);
		GameObject[] blocksToMoveUp = MHC.BlocksToMoveUp;
		foreach (GameObject value in blocksToMoveUp)
		{
			MHC.StartCoroutine("MoveBlockDown", value);
		}
		blocksToMoveUp = MHC.BlocksToMoveDown;
		foreach (GameObject value2 in blocksToMoveUp)
		{
			MHC.StartCoroutine("MoveBlockUp", value2);
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.PlayerAlive || GameMaster.instance.Players.Count < 1)
		{
			Triggered = false;
		}
	}
}
