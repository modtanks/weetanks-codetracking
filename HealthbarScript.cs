using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
	private HealthTanks myHT;

	public Transform GreenBar;

	private float OriginalX;

	private bool HasBeenEnabled;

	private bool HealthBarEnabled;

	private int LatestKnownHealth;

	private void Start()
	{
		OriginalX = GreenBar.transform.localScale.x;
		if (MapEditorMaster.instance == null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			myHT = base.transform.parent.gameObject.GetComponent<HealthTanks>();
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
			if (myHT.maxHealth > 9 || HasBeenEnabled)
			{
				if (LatestKnownHealth != myHT.health || HealthBarEnabled)
				{
					HasBeenEnabled = true;
					Enable();
					float num = myHT.health;
					float num2 = myHT.maxHealth;
					float x = num / num2 * OriginalX;
					GreenBar.transform.localScale = new Vector3(x, GreenBar.transform.localScale.y, GreenBar.transform.localScale.x);
					LatestKnownHealth = myHT.health;
				}
			}
			else
			{
				Disable();
			}
		}
	}
}
