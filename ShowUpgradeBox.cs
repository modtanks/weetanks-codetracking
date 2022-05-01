using UnityEngine;

public class ShowUpgradeBox : MonoBehaviour
{
	public bool isSpeed;

	public bool isRocket;

	public bool isBuild;

	public bool isShield;

	public bool isMine;

	public bool isTurret;

	public bool isTurretRepair;

	public WeeTurret WT;

	private void OnTriggerEnter(Collider collider)
	{
		ShowBox(collider);
	}

	private void OnTriggerStay(Collider collider)
	{
		ShowBox(collider);
	}

	private void ShowBox(Collider collider)
	{
		if ((!isTurretRepair || WT.enabled) && collider.tag == "Player")
		{
			MoveTankScript MTS = collider.GetComponent<MoveTankScript>();
			UpgradeField Field = MTS.UpgField;
			if (Field.canShow && !Field.Show && (!isBuild || MTS.Upgrades[0] <= 3) && (!isSpeed || MTS.Upgrades[1] <= 3) && (!isShield || MTS.Upgrades[2] <= 3) && (!isRocket || MTS.Upgrades[3] <= 0) && (!isMine || MTS.Upgrades[4] <= 0) && (!isTurret || MTS.Upgrades[5] <= 1) && (!isTurret || !MTS.isPlayer2 || GameMaster.instance.TurretsPlaced[1] <= 0) && (!isTurret || MTS.isPlayer2 || GameMaster.instance.TurretsPlaced[0] <= 0) && (!isTurretRepair || ((WT.Health < WT.maxHealth || WT.upgradeLevel <= 1) && (WT.PlacedByPlayer != 0 || MTS.playerId == 0) && (WT.PlacedByPlayer != 1 || MTS.playerId == 1) && (WT.PlacedByPlayer != 2 || MTS.playerId == 2) && (WT.PlacedByPlayer != 3 || MTS.playerId == 3))))
			{
				Field.Show = true;
				Field.startTime = Time.time;
				Field.ChangeMessage(isBuild, isSpeed, isShield, isRocket, isMine, isTurret, isTurretRepair, MTS, WT);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
		{
			MoveTankScript MTS = collider.GetComponent<MoveTankScript>();
			UpgradeField Field = MTS.UpgField;
			if (!Field.inPlacingMode)
			{
				Field.Show = false;
				Field.StartCoroutine(Field.WaitBox());
				Field.startTime = Time.time;
			}
		}
	}
}
