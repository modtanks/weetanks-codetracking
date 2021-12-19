using UnityEngine;

public class GenerateTeleportField : MonoBehaviour
{
	public GameObject StartPoint;

	public bool isMainMenu;

	private bool Generated;

	public bool isMission100Field;

	public int[] Mission100Skipfields;

	private void Awake()
	{
		if (!isMainMenu && OptionsMainMenu.instance != null)
		{
			GenerateFields();
		}
	}

	private void GenerateFields()
	{
		PathfindingBlocksMaster component = GetComponent<PathfindingBlocksMaster>();
		int num = 0;
		int num2 = 0;
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			num = 14;
			num2 = 11;
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			num = 18;
			num2 = 14;
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			num = 21;
			num2 = 16;
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			num = 24;
			num2 = 18;
		}
		Generated = true;
		for (int i = 0; i < num2 + 1; i++)
		{
			for (int j = 0; j < num + 1; j++)
			{
				if (j != 0 || i != 0)
				{
					Object.Instantiate(StartPoint, StartPoint.transform.position + new Vector3(j * 2, 0f, -i * 2), Quaternion.identity, base.transform);
				}
			}
		}
		if ((bool)component)
		{
			component.MapSizeX = num;
			component.FindAllBlocks();
		}
	}

	private void Start()
	{
		if ((isMainMenu || !Generated) && OptionsMainMenu.instance != null)
		{
			GenerateFields();
		}
	}

	private void OnEnable()
	{
		if (!Generated && OptionsMainMenu.instance != null)
		{
			GenerateFields();
		}
	}

	private void Update()
	{
		if (!Generated)
		{
			GenerateFields();
		}
		if (GameMaster.instance.CurrentMission > 98 && !isMission100Field)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
