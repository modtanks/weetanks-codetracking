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
			MoveTankScript component = collider.GetComponent<MoveTankScript>();
			UpgradeField upgField = component.UpgField;
			if (upgField.canShow && !upgField.Show && (!isBuild || component.Upgrades[0] <= 3) && (!isSpeed || component.Upgrades[1] <= 3) && (!isShield || component.Upgrades[2] <= 3) && (!isRocket || component.Upgrades[3] <= 0) && (!isMine || component.Upgrades[4] <= 0) && (!isTurret || component.Upgrades[5] <= 1) && (!isTurret || !component.isPlayer2 || GameMaster.instance.TurretsPlaced[1] <= 0) && (!isTurret || component.isPlayer2 || GameMaster.instance.TurretsPlaced[0] <= 0) && (!isTurretRepair || ((WT.Health < WT.maxHealth || WT.upgradeLevel <= 1) && (WT.PlacedByPlayer != 0 || component.playerId == 0) && (WT.PlacedByPlayer != 1 || component.playerId == 1) && (WT.PlacedByPlayer != 2 || component.playerId == 2) && (WT.PlacedByPlayer != 3 || component.playerId == 3))))
			{
				upgField.Show = true;
				upgField.startTime = Time.time;
				upgField.ChangeMessage(isBuild, isSpeed, isShield, isRocket, isMine, isTurret, isTurretRepair, component, WT);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
		{
			UpgradeField upgField = collider.GetComponent<MoveTankScript>().UpgField;
			if (!upgField.inPlacingMode)
			{
				upgField.Show = false;
				upgField.StartCoroutine(upgField.WaitBox());
				upgField.startTime = Time.time;
			}
		}
	}
}
