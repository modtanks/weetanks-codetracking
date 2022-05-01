using UnityEngine;

public class GenerateTeleportField : MonoBehaviour
{
	public GameObject StartPoint;

	public bool isMainMenu = false;

	private bool Generated = false;

	public bool isMission100Field = false;

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
		PathfindingBlocksMaster PBM = GetComponent<PathfindingBlocksMaster>();
		int SizeX = 0;
		int SizeY = 0;
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			SizeX = 14;
			SizeY = 11;
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			SizeX = 18;
			SizeY = 14;
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			SizeX = 21;
			SizeY = 16;
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			SizeX = 24;
			SizeY = 18;
		}
		Generated = true;
		for (int z = 0; z < SizeY + 1; z++)
		{
			for (int x = 0; x < SizeX + 1; x++)
			{
				if (x != 0 || z != 0)
				{
					GameObject NewTeleportField = Object.Instantiate(StartPoint, StartPoint.transform.position + new Vector3(x * 2, 0f, -z * 2), Quaternion.identity, base.transform);
				}
			}
		}
		if ((bool)PBM)
		{
			PBM.MapSizeX = SizeX;
			PBM.FindAllBlocks();
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
