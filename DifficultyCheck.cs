using UnityEngine;

public class DifficultyCheck : MonoBehaviour
{
	public int myDifficulty = 0;

	private void Awake()
	{
		if (base.transform.position.y < 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 0.06f, base.transform.position.z);
		}
	}

	private void Start()
	{
		CheckIt();
	}

	private void OnDisable()
	{
	}

	private void CheckIt()
	{
		if (GameMaster.instance != null && !GameMaster.instance.inMapEditor && !GameMaster.instance.inMenuMode && OptionsMainMenu.instance != null && OptionsMainMenu.instance.currentDifficulty < myDifficulty)
		{
			GameMaster.instance.AmountEnemyTanks--;
			Object.Destroy(base.gameObject);
		}
	}
}
