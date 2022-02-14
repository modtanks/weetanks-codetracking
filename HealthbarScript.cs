using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
	private HealthTanks myHT;

	public Transform GreenBar;

	private float OriginalX;

	private bool HasBeenEnabled;

	private bool HealthBarEnabled;

	private MapEditorProp MEP;

	private int LatestKnownHealth;

	private void Start()
	{
		OriginalX = GreenBar.transform.localScale.x;
		if (MapEditorMaster.instance == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		myHT = base.transform.parent.gameObject.GetComponent<HealthTanks>();
		if ((bool)myHT)
		{
			MEP = myHT.GetComponent<MapEditorProp>();
		}
	}

	private void Disable()
	{
		HealthBarEnabled = false;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}

	private void Enable()
	{
		HealthBarEnabled = true;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			Disable();
		}
		else
		{
			if (!myHT)
			{
				return;
			}
			if ((bool)MEP && !MapEditorMaster.instance.CustomTankDatas[MEP.CustomAInumber].CustomShowHealthbar)
			{
				Disable();
			}
			else if (myHT.maxHealth > 0 || HasBeenEnabled)
			{
				if (LatestKnownHealth != myHT.health + myHT.health_armour || HealthBarEnabled)
				{
					HasBeenEnabled = true;
					Enable();
					float num = myHT.health + myHT.health_armour;
					float num2 = myHT.maxHealth + myHT.maxArmour;
					float x = num / num2 * OriginalX;
					GreenBar.transform.localScale = new Vector3(x, GreenBar.transform.localScale.y, GreenBar.transform.localScale.x);
					LatestKnownHealth = myHT.health + myHT.health_armour;
				}
			}
			else
			{
				Disable();
			}
		}
	}
}
