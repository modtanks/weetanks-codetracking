using UnityEngine;

public class triggerplate : MonoBehaviour
{
	public bool Triggered = false;

	public bool IsFinalBossTrigger = false;

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
		foreach (GameObject Block in blocksToMoveUp)
		{
			MHC.StartCoroutine("MoveBlockDown", Block);
		}
		GameObject[] blocksToMoveDown = MHC.BlocksToMoveDown;
		foreach (GameObject Block2 in blocksToMoveDown)
		{
			MHC.StartCoroutine("MoveBlockUp", Block2);
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.PlayerAlive)
		{
			Triggered = false;
		}
	}
}
