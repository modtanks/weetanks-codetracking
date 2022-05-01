using UnityEngine;

public class MapEditorGridPiece : MonoBehaviour
{
	public Renderer myRend;

	public Color selectedColor;

	public Color notSelectedColor;

	public Color OriginalNotSelectedColor;

	public GameObject[] myProp;

	public GameObject[] myPropPrefab;

	public float Yheight;

	public bool mouseOnMe;

	public bool selected;

	public int mission;

	public bool[] propOnMe;

	private bool check;

	public int[] myPropID;

	public int SpawnDifficulty;

	public int ID;

	public int IDlayer;

	public int offsetX;

	public int offsetY;

	public int CustomID = -1;

	public string CustomUniqueID;

	public int MyTeamNumber = -1;

	public bool FieldSelected;

	public Color[] TeamColors;

	public GameObject PlacingSmoke;

	public MapEditorProp[] myMEP;

	public int[] rotationDirection;

	private bool ShowingProps;

	private int lastKnownLayer = -1;

	public int lastKnownMission = -1;

	public bool RunOnceSinceTesting;

	private void Start()
	{
		myRend = GetComponent<Renderer>();
		myRend.material.color = notSelectedColor;
		OriginalNotSelectedColor = notSelectedColor;
		SetGridPieceColor();
		InvokeRepeating("CheckCustomTanks", 0.25f, 0.25f);
	}

	private void OnMouseEnter()
	{
		if (!(MapEditorMaster.instance == null) && !(GameMaster.instance == null) && (!MapEditorMaster.instance.OTM || !(MapEditorMaster.instance.OTM.SelectedMEP != null)) && !MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode && !GameMaster.instance.GameHasPaused && MapEditorMaster.instance.SelectedProp >= 0 && selected && !check)
		{
			checkSelections();
		}
	}

	private void OnMouseOver()
	{
		if (!(MapEditorMaster.instance == null) && !(GameMaster.instance == null) && (!MapEditorMaster.instance.OTM || !(MapEditorMaster.instance.OTM.SelectedMEP != null)) && !MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode && !GameMaster.instance.GameHasPaused && MapEditorMaster.instance.SelectedProp >= 0 && !MapEditorMaster.instance.RemoveMode)
		{
			if (myRend != null)
			{
				myRend.material.color = selectedColor;
			}
			if (!mouseOnMe)
			{
				mouseOnMe = true;
			}
		}
	}

	private void OnMouseExit()
	{
		if (!(MapEditorMaster.instance == null) && !(GameMaster.instance == null) && !MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode)
		{
			myRend.material.color = notSelectedColor;
			mouseOnMe = false;
		}
	}

	public void checkSelections()
	{
		if (!Input.GetKey(KeyCode.LeftShift) || MapEditorMaster.instance.selectedFields <= 0)
		{
			return;
		}
		if (MapEditorMaster.instance.startSelectionField.position.x == base.transform.position.x || MapEditorMaster.instance.startSelectionField.position.z == base.transform.position.z)
		{
			selected = true;
			float maxDistance = Vector3.Distance(base.transform.position, MapEditorMaster.instance.startSelectionField.position);
			Vector3 direction = MapEditorMaster.instance.startSelectionField.position - base.transform.position;
			RaycastHit[] array = Physics.RaycastAll(base.transform.position, direction, maxDistance);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.tag == "MapeditorField")
				{
					MapEditorGridPiece component = array[i].transform.GetComponent<MapEditorGridPiece>();
					if (!component.selected)
					{
						component.selected = true;
					}
				}
			}
			maxDistance = Vector3.Distance(base.transform.position, MapEditorMaster.instance.startSelectionField.position);
			direction = MapEditorMaster.instance.startSelectionField.position - base.transform.position;
			RaycastHit[] array2 = Physics.RaycastAll(base.transform.position, -direction, 500f);
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].transform.tag == "MapeditorField")
				{
					MapEditorGridPiece component2 = array2[j].transform.GetComponent<MapEditorGridPiece>();
					if (component2.selected)
					{
						component2.selected = false;
					}
				}
			}
			return;
		}
		selected = false;
		float num = Mathf.Abs(MapEditorMaster.instance.startSelectionField.position.x - base.transform.position.x);
		float num2 = Mathf.Abs(MapEditorMaster.instance.startSelectionField.position.z - base.transform.position.z);
		GameObject[] array4;
		if (num > num2)
		{
			Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y, MapEditorMaster.instance.startSelectionField.position.z);
			GameObject[] array3 = GameObject.FindGameObjectsWithTag("MapeditorField");
			MapEditorGridPiece mapEditorGridPiece = null;
			array4 = array3;
			foreach (GameObject obj in array4)
			{
				MapEditorGridPiece component3 = obj.GetComponent<MapEditorGridPiece>();
				if (obj.transform.position == vector)
				{
					mapEditorGridPiece = component3;
				}
				else if (component3.selected)
				{
					component3.selected = false;
				}
			}
			if (mapEditorGridPiece != null)
			{
				mapEditorGridPiece.selected = true;
				mapEditorGridPiece.checkSelections();
			}
			return;
		}
		Vector3 vector2 = new Vector3(MapEditorMaster.instance.startSelectionField.position.x, base.transform.position.y, base.transform.position.z);
		GameObject[] array5 = GameObject.FindGameObjectsWithTag("MapeditorField");
		MapEditorGridPiece mapEditorGridPiece2 = null;
		array4 = array5;
		foreach (GameObject obj2 in array4)
		{
			MapEditorGridPiece component4 = obj2.GetComponent<MapEditorGridPiece>();
			if (obj2.transform.position == vector2)
			{
				mapEditorGridPiece2 = component4;
			}
			else if (component4.selected)
			{
				component4.selected = false;
			}
		}
		if (mapEditorGridPiece2 != null)
		{
			mapEditorGridPiece2.selected = true;
			mapEditorGridPiece2.checkSelections();
		}
	}

	private void HideProps()
	{
		for (int i = 0; i < 5; i++)
		{
			if (IDlayer != i && myProp[i] != null)
			{
				myProp[i].SetActive(value: false);
			}
		}
	}

	private void ShowProps()
	{
		for (int i = 0; i < 5; i++)
		{
			if (myProp[i] != null)
			{
				myProp[i].SetActive(value: true);
			}
		}
	}

	private void CheckCustomTanks()
	{
		if (myPropID[0] < 100 || myPropID[0] >= 120)
		{
			return;
		}
		if (!MapEditorMaster.instance.CustomTankDatas.Exists((CustomTankData x) => x.UniqueTankID == CustomUniqueID))
		{
			myPropID[0] = -1;
			myPropPrefab[0] = null;
			myProp[0] = null;
			CustomUniqueID = "";
			MyTeamNumber = -1;
			myMEP[0].RemoveThisGridObject(force: true);
			return;
		}
		for (int i = 0; i < MapEditorMaster.instance.CustomTankDatas.Count; i++)
		{
			if (MapEditorMaster.instance.CustomTankDatas[i].UniqueTankID == CustomUniqueID)
			{
				myMEP[0].CustomAInumber = i;
			}
		}
	}

	private void Update()
	{
		if (MapEditorMaster.instance.isTesting || MapEditorMaster.instance.inPlayingMode || MapEditorMaster.instance.MenuCurrent == 8)
		{
			RunOnceSinceTesting = false;
			myRend.material.color = Color.clear;
			return;
		}
		if (!RunOnceSinceTesting)
		{
			RunOnceSinceTesting = true;
			SetGridPieceColor();
		}
		if (MapEditorMaster.instance.SelectedParticles != null && FieldSelected && myMEP[0] != null)
		{
			MapEditorMaster.instance.SelectedParticles.transform.position = base.transform.position;
		}
		base.transform.position = new Vector3(base.transform.position.x, MapEditorMaster.instance.CurrentLayer * 2, base.transform.position.z);
		IDlayer = MapEditorMaster.instance.CurrentLayer;
		if (lastKnownLayer != IDlayer)
		{
			SetGridPieceColor();
			lastKnownLayer = IDlayer;
			if (MapEditorMaster.instance.ShowAllLayers)
			{
				ShowingProps = true;
				ShowProps();
			}
			else
			{
				ShowingProps = false;
				HideProps();
			}
			if ((bool)myProp[IDlayer])
			{
				myProp[IDlayer].SetActive(value: true);
			}
		}
		if (!ShowingProps && MapEditorMaster.instance.ShowAllLayers)
		{
			ShowingProps = true;
			ShowProps();
		}
		else if (ShowingProps && !MapEditorMaster.instance.ShowAllLayers)
		{
			ShowingProps = false;
			HideProps();
		}
		if (!propOnMe[IDlayer])
		{
			if (mouseOnMe && !check)
			{
				myRend.material.color = selectedColor;
				if (Input.GetMouseButtonUp(0) && selected)
				{
					PlaceProp();
					MapEditorMaster.instance.startSelectionField = null;
					MapEditorMaster.instance.selectedFields = 0;
					selected = false;
				}
				else if (Input.GetMouseButton(0) && !selected)
				{
					if (MapEditorMaster.instance.selectedFields < 1)
					{
						MapEditorMaster.instance.startSelectionField = base.transform;
						selected = true;
					}
					if (Input.GetKey(KeyCode.LeftShift))
					{
						check = true;
						checkSelections();
					}
					else
					{
						selected = true;
					}
					MapEditorMaster.instance.selectedFields++;
				}
			}
			else if (!mouseOnMe && Input.GetKey(KeyCode.LeftShift))
			{
				check = false;
			}
			if (Input.GetMouseButtonUp(0) && selected)
			{
				PlaceProp();
				MapEditorMaster.instance.startSelectionField = null;
				MapEditorMaster.instance.selectedFields = 0;
				selected = false;
			}
			if (selected)
			{
				myRend.material.color = selectedColor;
				if (!Input.GetMouseButton(0))
				{
					selected = false;
				}
			}
			else if (!mouseOnMe)
			{
				myRend.material.color = notSelectedColor;
			}
		}
		else
		{
			check = false;
		}
		if (myProp[IDlayer] == null && propOnMe[IDlayer])
		{
			propOnMe[IDlayer] = false;
			myPropID[IDlayer] = -1;
		}
	}

	private void OnEnable()
	{
		SetGridPieceColor();
	}

	private void PlaceProp()
	{
		if (!MapEditorMaster.instance.canPlaceProp || propOnMe[IDlayer] || (MapEditorMaster.instance.SelectedProp == 4 && MapEditorMaster.instance.playerOnePlaced[GameMaster.instance.CurrentMission] == 1) || (MapEditorMaster.instance.SelectedProp == 5 && MapEditorMaster.instance.playerTwoPlaced[GameMaster.instance.CurrentMission] == 1) || (MapEditorMaster.instance.SelectedProp == 28 && MapEditorMaster.instance.playerThreePlaced[GameMaster.instance.CurrentMission] == 1) || (MapEditorMaster.instance.SelectedProp == 29 && MapEditorMaster.instance.playerFourPlaced[GameMaster.instance.CurrentMission] == 1))
		{
			return;
		}
		if ((MapEditorMaster.instance.SelectedProp > 3 && MapEditorMaster.instance.SelectedProp < 40) || MapEditorMaster.instance.SelectedProp == 46 || (MapEditorMaster.instance.SelectedProp >= 100 && MapEditorMaster.instance.SelectedProp < 120))
		{
			if (IDlayer > 0)
			{
				MapEditorMaster.instance.ShowErrorMessage("ERROR: Must be placed on the floor!");
				return;
			}
		}
		else if ((MapEditorMaster.instance.SelectedProp == 3 || MapEditorMaster.instance.SelectedProp == 46 || MapEditorMaster.instance.SelectedProp == 47 || MapEditorMaster.instance.SelectedProp == 48) && IDlayer > 0 && myPropID[IDlayer - 1] != 0 && myPropID[IDlayer - 1] != 1 && myPropID[IDlayer - 1] != 2 && myPropID[IDlayer - 1] != 2002)
		{
			MapEditorMaster.instance.ShowErrorMessage("ERROR: No Block Below!");
			return;
		}
		if ((MapEditorMaster.instance.SelectedProp > 5 && MapEditorMaster.instance.SelectedProp < 40 && MapEditorMaster.instance.SelectedProp != 28 && MapEditorMaster.instance.SelectedProp != 29) || (MapEditorMaster.instance.SelectedProp >= 100 && MapEditorMaster.instance.SelectedProp < 120) || MapEditorMaster.instance.SelectedProp == 1000 || MapEditorMaster.instance.SelectedProp == 2003)
		{
			if (MapEditorMaster.instance.enemyTanksPlaced[GameMaster.instance.CurrentMission] >= MapEditorMaster.instance.maxEnemyTanks)
			{
				MapEditorMaster.instance.ShowErrorMessage("ERROR: Max enemies reached!");
				return;
			}
			MapEditorMaster.instance.enemyTanksPlaced[GameMaster.instance.CurrentMission]++;
		}
		if (MapEditorMaster.instance.SelectedProp == 4)
		{
			if (MapEditorMaster.instance.playerOnePlaced[GameMaster.instance.CurrentMission] >= 1)
			{
				return;
			}
			MapEditorMaster.instance.playerOnePlaced[GameMaster.instance.CurrentMission] = 1;
		}
		if (MapEditorMaster.instance.SelectedProp == 5)
		{
			if (MapEditorMaster.instance.playerTwoPlaced[GameMaster.instance.CurrentMission] >= 1)
			{
				return;
			}
			MapEditorMaster.instance.playerTwoPlaced[GameMaster.instance.CurrentMission] = 1;
		}
		if (MapEditorMaster.instance.SelectedProp == 28)
		{
			if (MapEditorMaster.instance.playerThreePlaced[GameMaster.instance.CurrentMission] >= 1)
			{
				return;
			}
			MapEditorMaster.instance.playerThreePlaced[GameMaster.instance.CurrentMission] = 1;
		}
		if (MapEditorMaster.instance.SelectedProp == 29)
		{
			if (MapEditorMaster.instance.playerFourPlaced[GameMaster.instance.CurrentMission] >= 1)
			{
				return;
			}
			MapEditorMaster.instance.playerFourPlaced[GameMaster.instance.CurrentMission] = 1;
		}
		Object.Destroy(Object.Instantiate(PlacingSmoke, base.transform.position, Quaternion.identity), 2f);
		if (MapEditorMaster.instance.SelectedProp < 4 || MapEditorMaster.instance.SelectedProp >= 40)
		{
			MapEditorMaster.instance.PlayAudio(MapEditorMaster.instance.PlaceHeavy);
		}
		else
		{
			MapEditorMaster.instance.PlayAudio(MapEditorMaster.instance.PlaceLight);
		}
		if (MapEditorMaster.instance.SelectedProp >= 1000)
		{
			_ = MapEditorMaster.instance.SelectedProp;
			Yheight = GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == MapEditorMaster.instance.SelectedProp).MapEditorYoffset;
			myProp[IDlayer] = Object.Instantiate(GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == MapEditorMaster.instance.SelectedProp).MapEditorPrefab, base.transform.position + new Vector3(0f, Yheight, 0f), Quaternion.identity);
			myPropPrefab[IDlayer] = GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == MapEditorMaster.instance.SelectedProp).MapEditorPrefab;
		}
		else if (MapEditorMaster.instance.SelectedProp >= 100 && MapEditorMaster.instance.SelectedProp < 120)
		{
			CustomID = MapEditorMaster.instance.SelectedProp - 100;
			Yheight = MapEditorMaster.instance.PropStartHeight[19];
			CustomUniqueID = MapEditorMaster.instance.CustomTankDatas[CustomID].UniqueTankID;
			myProp[IDlayer] = Object.Instantiate(MapEditorMaster.instance.Props[19], base.transform.position + new Vector3(0f, Yheight, 0f), Quaternion.identity);
			myPropPrefab[IDlayer] = MapEditorMaster.instance.Props[19];
		}
		else
		{
			Yheight = MapEditorMaster.instance.PropStartHeight[MapEditorMaster.instance.SelectedProp];
			myProp[IDlayer] = Object.Instantiate(MapEditorMaster.instance.Props[MapEditorMaster.instance.SelectedProp], base.transform.position + new Vector3(0f, Yheight, 0f), Quaternion.identity);
			myPropPrefab[IDlayer] = MapEditorMaster.instance.Props[MapEditorMaster.instance.SelectedProp];
		}
		MapEditorProp mapEditorProp = myProp[IDlayer].GetComponent<MapEditorProp>();
		if (mapEditorProp == null)
		{
			mapEditorProp = myProp[IDlayer].GetComponentInChildren<MapEditorProp>();
		}
		mapEditorProp.myMEGP = this;
		mapEditorProp.LayerNumber = IDlayer;
		myMEP[IDlayer] = mapEditorProp;
		if ((bool)mapEditorProp)
		{
			if (MapEditorMaster.instance.SelectedProp >= 100 && MapEditorMaster.instance.SelectedProp < 120)
			{
				mapEditorProp.CustomUniqueTankID = MapEditorMaster.instance.CustomTankDatas[CustomID].UniqueTankID;
			}
			rotationDirection[IDlayer] = MapEditorMaster.instance.LastPlacedRotation;
			for (int i = 0; i < MapEditorMaster.instance.LastPlacedRotation; i++)
			{
				mapEditorProp.RotateProp();
			}
		}
		if (mapEditorProp.isEnemyTank)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[MapEditorMaster.instance.LastPlacedColor]++;
			mapEditorProp.TeamNumber = MapEditorMaster.instance.LastPlacedColor;
			MyTeamNumber = MapEditorMaster.instance.LastPlacedColor;
			mapEditorProp.MyDifficultySpawn = MapEditorMaster.instance.OTM.PreviousDifficulty;
		}
		else if (mapEditorProp.isPlayerOne || mapEditorProp.isPlayerTwo || mapEditorProp.isPlayerThree || mapEditorProp.isPlayerFour)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[1]++;
			mapEditorProp.TeamNumber = 1;
			MyTeamNumber = 1;
		}
		myProp[IDlayer].transform.parent = GameMaster.instance.Levels[0].transform;
		propOnMe[IDlayer] = true;
		myPropID[IDlayer] = MapEditorMaster.instance.SelectedProp;
		SetCustomMaterial(mapEditorProp);
		SetGridPieceColor();
		if (mapEditorProp.CanBeColored)
		{
			mapEditorProp.SetMyColor();
		}
	}

	public void SetGridPieceColor()
	{
		if (MapEditorMaster.instance.isTesting || MapEditorMaster.instance.inPlayingMode)
		{
			if ((bool)myRend)
			{
				myRend.material.color = OriginalNotSelectedColor;
			}
		}
		else if (myMEP[IDlayer] != null && myRend != null)
		{
			if (myMEP[IDlayer].isEnemyTank || myMEP[IDlayer].isPlayerOne || myMEP[IDlayer].isPlayerTwo || myMEP[IDlayer].isPlayerThree || myMEP[IDlayer].isPlayerFour)
			{
				if (myMEP[IDlayer].TeamNumber > -1)
				{
					myRend.material.color = TeamColors[myMEP[IDlayer].TeamNumber];
					notSelectedColor = TeamColors[myMEP[IDlayer].TeamNumber];
				}
			}
			else
			{
				myRend.material.color = OriginalNotSelectedColor;
				notSelectedColor = OriginalNotSelectedColor;
			}
		}
		else if (myRend != null)
		{
			myRend.material.color = OriginalNotSelectedColor;
			notSelectedColor = OriginalNotSelectedColor;
		}
	}

	public void SetCustomMaterial(MapEditorProp MEP)
	{
		if (myPropID[IDlayer] == 19 || (myPropID[IDlayer] >= 100 && myPropID[IDlayer] < 120))
		{
			MEP.CustomAInumber = CustomID;
			MEP.myCustomMaterial = MapEditorMaster.instance.CustomMaterial[CustomID];
			SetCustomTanksMaterials(CustomID, MEP.ColoredObjects);
		}
	}

	private void SetCustomTanksMaterials(int ID, MeshRenderer[] objects)
	{
		foreach (MeshRenderer meshRenderer in objects)
		{
			for (int j = 0; j < meshRenderer.sharedMaterials.Length; j++)
			{
				if (meshRenderer.sharedMaterials[j].name.Contains("CustomTank"))
				{
					Material[] sharedMaterials = meshRenderer.sharedMaterials;
					sharedMaterials[j] = MapEditorMaster.instance.CustomMaterial[ID];
					meshRenderer.sharedMaterials = sharedMaterials;
				}
			}
		}
	}

	public void SpawnInProps(int propID, int direction, int Team, int LayerHeight, int SpawnDifficulty)
	{
		if (propOnMe[LayerHeight])
		{
			return;
		}
		rotationDirection[LayerHeight] = direction;
		if (propID >= 1000)
		{
			Yheight = GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == propID).MapEditorYoffset;
			myProp[LayerHeight] = Object.Instantiate(GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == propID).MapEditorPrefab, new Vector3(base.transform.position.x, Yheight + (float)(LayerHeight * 2), base.transform.position.z), Quaternion.identity);
		}
		else if (propID >= 100 && propID < 120)
		{
			CustomID = propID - 100;
			Yheight = MapEditorMaster.instance.PropStartHeight[19];
			CustomUniqueID = MapEditorMaster.instance.CustomTankDatas[CustomID].UniqueTankID;
			myProp[LayerHeight] = Object.Instantiate(MapEditorMaster.instance.Props[19], new Vector3(base.transform.position.x, Yheight + (float)(LayerHeight * 2), base.transform.position.z), Quaternion.identity);
			propID = 19;
		}
		else
		{
			Yheight = MapEditorMaster.instance.PropStartHeight[propID];
			myProp[LayerHeight] = Object.Instantiate(MapEditorMaster.instance.Props[propID], new Vector3(base.transform.position.x, Yheight + (float)(LayerHeight * 2), base.transform.position.z), Quaternion.identity);
		}
		MapEditorProp mapEditorProp = myProp[LayerHeight].GetComponent<MapEditorProp>();
		if (mapEditorProp == null)
		{
			mapEditorProp = myProp[LayerHeight].GetComponentInChildren<MapEditorProp>();
		}
		if (mapEditorProp == null)
		{
			mapEditorProp = myProp[LayerHeight].transform.GetChild(0).GetComponentInChildren<MapEditorProp>();
		}
		if (mapEditorProp == null)
		{
			mapEditorProp = myProp[LayerHeight].transform.GetChild(0).GetChild(0).GetComponentInChildren<MapEditorProp>();
		}
		if (!(mapEditorProp != null))
		{
			return;
		}
		mapEditorProp.myMEGP = this;
		myMEP[LayerHeight] = mapEditorProp;
		myPropID[LayerHeight] = propID;
		if (propID >= 1000)
		{
			myPropPrefab[LayerHeight] = GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == propID).MapEditorPrefab;
		}
		else if (propID == 19)
		{
			mapEditorProp.CustomUniqueTankID = CustomUniqueID;
			myPropPrefab[LayerHeight] = MapEditorMaster.instance.Props[propID];
			myPropID[LayerHeight] = 100 + CustomID;
		}
		else
		{
			myPropPrefab[LayerHeight] = MapEditorMaster.instance.Props[propID];
		}
		myProp[LayerHeight].transform.parent = GameMaster.instance.Levels[0].transform;
		propOnMe[LayerHeight] = true;
		mapEditorProp.LayerNumber = LayerHeight;
		if (Team > -1)
		{
			mapEditorProp.TeamNumber = Team;
			MapEditorMaster.instance.ColorsTeamsPlaced[Team]++;
		}
		if (mapEditorProp != null)
		{
			mapEditorProp.MyDifficultySpawn = SpawnDifficulty;
			applyPropRotation(mapEditorProp, rotationDirection[LayerHeight]);
			SetCustomMaterial(mapEditorProp);
			SetGridPieceColor();
			DifficultyCheck component = mapEditorProp.transform.parent.GetComponent<DifficultyCheck>();
			if ((bool)component)
			{
				component.myDifficulty = SpawnDifficulty;
			}
			if (mapEditorProp.CanBeColored)
			{
				mapEditorProp.StartCoroutine(mapEditorProp.SetMaterialsDelay());
			}
		}
	}

	public void Reset()
	{
		GameObject[] array = myProp;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				Object.Destroy(gameObject);
			}
		}
		for (int j = 0; j < 5; j++)
		{
			propOnMe[j] = false;
			myPropID[j] = -1;
			myProp[j] = null;
			myPropPrefab[j] = null;
			myMEP[j] = null;
			rotationDirection[j] = 0;
			MyTeamNumber = 0;
		}
		SpawnDifficulty = 0;
		lastKnownMission = -1;
		Yheight = 0f;
		mission = 0;
		notSelectedColor = OriginalNotSelectedColor;
		myRend.material.color = OriginalNotSelectedColor;
		SetGridPieceColor();
	}

	public void applyPropRotation(MapEditorProp MEP, int dir)
	{
		Quaternion quaternion = Quaternion.Euler(0f, 90f * (float)dir, 0f);
		if (MEP.transform.tag == "Player" || MEP.isEnemyTank)
		{
			MEP.transform.parent.transform.parent.transform.rotation *= quaternion;
		}
		else if (MEP.transform.parent != null)
		{
			if (MEP.removeParentOnDelete)
			{
				MEP.transform.parent.transform.rotation *= quaternion;
			}
			else if (MEP.transform.parent.name == "Hole" || MEP.transform.parent.name == "Hole(Clone)")
			{
				MEP.transform.parent.transform.rotation *= quaternion;
			}
			else
			{
				MEP.transform.rotation *= quaternion;
			}
		}
		else
		{
			MEP.transform.rotation *= quaternion;
		}
	}
}
