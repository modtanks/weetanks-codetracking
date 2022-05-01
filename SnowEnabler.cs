using UnityEngine;

public class SnowEnabler : MonoBehaviour
{
	public GameObject SnowPlate;

	public float posYoffset;

	public bool SpawnProp;

	public GameObject SpawnedProp;

	public GameObject[] SnowChildren;

	public GameObject mySpawnedSnow;

	private void Start()
	{
		if (!GameMaster.instance.CM)
		{
			if (GetComponent<AOcreation>() == null)
			{
				CheckSnow();
			}
			if (GameMaster.instance.inMenuMode)
			{
				InvokeRepeating("CheckSnow", 1f, 0.25f);
			}
		}
	}

	public void CheckSnow()
	{
		if (GameMaster.instance.inMapEditor)
		{
			if ((bool)mySpawnedSnow)
			{
				Object.Destroy(mySpawnedSnow);
			}
			if (SnowChildren.Length != 0)
			{
				int length = SnowChildren.Length;
				for (int i = 0; i < length; i++)
				{
					if ((bool)SnowChildren[i])
					{
						Object.Destroy(SnowChildren[i]);
					}
				}
			}
			base.enabled = false;
		}
		else if (!SpawnProp && SnowChildren.Length < 1)
		{
			if ((bool)GameMaster.instance)
			{
				if (OptionsMainMenu.instance.SnowMode && mySpawnedSnow == null)
				{
					mySpawnedSnow = Object.Instantiate(SnowPlate, base.transform.position + new Vector3(0f, posYoffset, 0f), Quaternion.identity);
					mySpawnedSnow.transform.parent = base.transform;
					mySpawnedSnow.transform.Rotate(Vector3.right, 90f);
				}
				else if (!OptionsMainMenu.instance.SnowMode && (bool)mySpawnedSnow)
				{
					Object.Destroy(mySpawnedSnow);
				}
			}
		}
		else if (SnowChildren.Length < 1)
		{
			if ((bool)GameMaster.instance)
			{
				if (OptionsMainMenu.instance.SnowMode && GameMaster.instance.CurrentMission != 49)
				{
					SpawnedProp.SetActive(value: true);
				}
				else if (!OptionsMainMenu.instance.SnowMode)
				{
					SpawnedProp.SetActive(value: false);
				}
			}
		}
		else
		{
			if (SnowChildren.Length == 0)
			{
				return;
			}
			if (OptionsMainMenu.instance.SnowMode)
			{
				GameObject[] snowChildren = SnowChildren;
				foreach (GameObject child2 in snowChildren)
				{
					if ((bool)child2)
					{
						child2.SetActive(value: true);
					}
				}
				return;
			}
			GameObject[] snowChildren2 = SnowChildren;
			foreach (GameObject child in snowChildren2)
			{
				if ((bool)child)
				{
					child.SetActive(value: false);
				}
			}
		}
	}
}
