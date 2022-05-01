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

	public bool mouseOnMe = false;

	public bool selected = false;

	public int mission = 0;

	public bool[] propOnMe;

	private bool check = false;

	public int[] myPropID;

	public int SpawnDifficulty = 0;

	public int ID = 0;

	public int IDlayer = 0;

	public int offsetX = 0;

	public int offsetY = 0;

	public int CustomID = -1;

	public string CustomUniqueID;

	public int MyTeamNumber = -1;

	public bool FieldSelected = false;

	public Color[] TeamColors;

	public GameObject PlacingSmoke;

	public MapEditorProp[] myMEP;

	public int[] rotationDirection;

	private bool ShowingProps = false;

	private int lastKnownLayer = -1;

	public int lastKnownMission = -1;

	public bool RunOnceSinceTesting = false;

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
			float distance = Vector3.Distance(base.transform.position, MapEditorMaster.instance.startSelectionField.position);
			Vector3 direction = MapEditorMaster.instance.startSelectionField.position - base.transform.position;
			RaycastHit[] hits = Physics.RaycastAll(base.transform.position, direction, distance);
			for (int j = 0; j < hits.Length; j++)
			{
				if (hits[j].transform.tag == "MapeditorField")
				{
					MapEditorGridPiece MEGP = hits[j].transform.GetComponent<MapEditorGridPiece>();
					if (!MEGP.selected)
					{
						MEGP.selected = true;
					}
				}
			}
			distance = Vector3.Distance(base.transform.position, MapEditorMaster.instance.startSelectionField.position);
			direction = MapEditorMaster.instance.startSelectionField.position - base.transform.position;
			RaycastHit[] removehits = Physics.RaycastAll(base.transform.position, -direction, 500f);
			for (int i = 0; i < removehits.Length; i++)
			{
				if (removehits[i].transform.tag == "MapeditorField")
				{
					MapEditorGridPiece MEGP2 = removehits[i].transform.GetComponent<MapEditorGridPiece>();
					if (MEGP2.selected)
					{
						MEGP2.selected = false;
					}
				}
			}
			return;
		}
		selected = false;
		float xDiff = Mathf.Abs(MapEditorMaster.instance.startSelectionField.position.x - base.transform.position.x);
		float zDiff = Mathf.Abs(MapEditorMaster.instance.startSelectionField.position.z - base.transform.position.z);
		if (xDiff > zDiff)
		{
			Vector3 pos2 = new Vector3(base.transform.position.x, base.transform.position.y, MapEditorMaster.instance.startSelectionField.position.z);
			GameObject[] tiles2 = GameObject.FindGameObjectsWithTag("MapeditorField");
			MapEditorGridPiece ChosenPlace2 = null;
			GameObject[] array = tiles2;
			foreach (GameObject tile2 in array)
			{
				MapEditorGridPiece MEGP4 = tile2.GetComponent<MapEditorGridPiece>();
				if (tile2.transform.position == pos2)
				{
					ChosenPlace2 = MEGP4;
				}
				else if (MEGP4.selected)
				{
					MEGP4.selected = false;
				}
			}
			if (ChosenPlace2 != null)
			{
				ChosenPlace2.selected = true;
				ChosenPlace2.checkSelections();
			}
			return;
		}
		Vector3 pos = new Vector3(MapEditorMaster.instance.startSelectionField.position.x, base.transform.position.y, base.transform.position.z);
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("MapeditorField");
		MapEditorGridPiece ChosenPlace = null;
		GameObject[] array2 = tiles;
		foreach (GameObject tile in array2)
		{
			MapEditorGridPiece MEGP3 = tile.GetComponent<MapEditorGridPiece>();
			if (tile.transform.position == pos)
			{
				ChosenPlace = MEGP3;
			}
			else if (MEGP3.selected)
			{
				MEGP3.selected = false;
			}
		}
		if (ChosenPlace != null)
		{
			ChosenPlace.selected = true;
			ChosenPlace.checkSelections();
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
		GameObject smoke = Object.Instantiate(PlacingSmoke, base.transform.position, Quaternion.identity);
		Object.Destroy(smoke, 2f);
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
			int id = MapEditorMaster.instance.SelectedProp - 1000;
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
		MapEditorProp MEP = myProp[IDlayer].GetComponent<MapEditorProp>();
		if (MEP == null)
		{
			MEP = myProp[IDlayer].GetComponentInChildren<MapEditorProp>();
		}
		MEP.myMEGP = this;
		MEP.LayerNumber = IDlayer;
		myMEP[IDlayer] = MEP;
		if ((bool)MEP)
		{
			if (MapEditorMaster.instance.SelectedProp >= 100 && MapEditorMaster.instance.SelectedProp < 120)
			{
				MEP.CustomUniqueTankID = MapEditorMaster.instance.CustomTankDatas[CustomID].UniqueTankID;
			}
			rotationDirection[IDlayer] = MapEditorMaster.instance.LastPlacedRotation;
			for (int i = 0; i < MapEditorMaster.instance.LastPlacedRotation; i++)
			{
				MEP.RotateProp();
			}
		}
		if (MEP.isEnemyTank)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[MapEditorMaster.instance.LastPlacedColor]++;
			MEP.TeamNumber = MapEditorMaster.instance.LastPlacedColor;
			MyTeamNumber = MapEditorMaster.instance.LastPlacedColor;
			MEP.MyDifficultySpawn = MapEditorMaster.instance.OTM.PreviousDifficulty;
		}
		else if (MEP.isPlayerOne || MEP.isPlayerTwo || MEP.isPlayerThree || MEP.isPlayerFour)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[1]++;
			MEP.TeamNumber = 1;
			MyTeamNumber = 1;
		}
		myProp[IDlayer].transform.parent = GameMaster.instance.Levels[0].transform;
		propOnMe[IDlayer] = true;
		myPropID[IDlayer] = MapEditorMaster.instance.SelectedProp;
		SetCustomMaterial(MEP);
		SetGridPieceColor();
		if (MEP.CanBeColored)
		{
			MEP.SetMyColor();
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
		foreach (MeshRenderer obj in objects)
		{
			for (int i = 0; i < obj.sharedMaterials.Length; i++)
			{
				if (obj.sharedMaterials[i].name.Contains("CustomTank"))
				{
					Material[] sharedMaterialsCopy = obj.sharedMaterials;
					sharedMaterialsCopy[i] = MapEditorMaster.instance.CustomMaterial[ID];
					obj.sharedMaterials = sharedMaterialsCopy;
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
		MapEditorProp MEP = myProp[LayerHeight].GetComponent<MapEditorProp>();
		if (MEP == null)
		{
			MEP = myProp[LayerHeight].GetComponentInChildren<MapEditorProp>();
		}
		if (MEP == null)
		{
			MEP = myProp[LayerHeight].transform.GetChild(0).GetComponentInChildren<MapEditorProp>();
		}
		if (MEP == null)
		{
			MEP = myProp[LayerHeight].transform.GetChild(0).GetChild(0).GetComponentInChildren<MapEditorProp>();
		}
		if (!(MEP != null))
		{
			return;
		}
		MEP.myMEGP = this;
		myMEP[LayerHeight] = MEP;
		myPropID[LayerHeight] = propID;
		if (propID >= 1000)
		{
			myPropPrefab[LayerHeight] = GlobalAssets.instance.StockDatabase.Find((TankeyTownStockItem x) => x.MapEditorPropID == propID).MapEditorPrefab;
		}
		else if (propID == 19)
		{
			MEP.CustomUniqueTankID = CustomUniqueID;
			myPropPrefab[LayerHeight] = MapEditorMaster.instance.Props[propID];
			myPropID[LayerHeight] = 100 + CustomID;
		}
		else
		{
			myPropPrefab[LayerHeight] = MapEditorMaster.instance.Props[propID];
		}
		myProp[LayerHeight].transform.parent = GameMaster.instance.Levels[0].transform;
		propOnMe[LayerHeight] = true;
		MEP.LayerNumber = LayerHeight;
		if (Team > -1)
		{
			MEP.TeamNumber = Team;
			MapEditorMaster.instance.ColorsTeamsPlaced[Team]++;
		}
		if (MEP != null)
		{
			MEP.MyDifficultySpawn = SpawnDifficulty;
			applyPropRotation(MEP, rotationDirection[LayerHeight]);
			SetCustomMaterial(MEP);
			SetGridPieceColor();
			DifficultyCheck DC = MEP.transform.parent.GetComponent<DifficultyCheck>();
			if ((bool)DC)
			{
				DC.myDifficulty = SpawnDifficulty;
			}
			if (MEP.CanBeColored)
			{
				MEP.StartCoroutine(MEP.SetMaterialsDelay());
			}
		}
	}

	public void Reset()
	{
		GameObject[] array = myProp;
		foreach (GameObject Prop in array)
		{
			if ((bool)Prop)
			{
				Object.Destroy(Prop);
			}
		}
		for (int i = 0; i < 5; i++)
		{
			propOnMe[i] = false;
			myPropID[i] = -1;
			myProp[i] = null;
			myPropPrefab[i] = null;
			myMEP[i] = null;
			rotationDirection[i] = 0;
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
		Quaternion theRot = Quaternion.Euler(0f, 90f * (float)dir, 0f);
		if (MEP.transform.tag == "Player" || MEP.isEnemyTank)
		{
			MEP.transform.parent.transform.parent.transform.rotation *= theRot;
		}
		else if (MEP.transform.parent != null)
		{
			if (MEP.removeParentOnDelete)
			{
				MEP.transform.parent.transform.rotation *= theRot;
			}
			else if (MEP.transform.parent.name == "Hole" || MEP.transform.parent.name == "Hole(Clone)")
			{
				MEP.transform.parent.transform.rotation *= theRot;
			}
			else
			{
				MEP.transform.rotation *= theRot;
			}
		}
		else
		{
			MEP.transform.rotation *= theRot;
		}
	}
}
