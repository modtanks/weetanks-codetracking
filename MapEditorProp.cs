using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class MapEditorProp : MonoBehaviour
{
	public int TeamNumber = -1;

	public int MyDifficultySpawn = 0;

	public int LayerNumber = 0;

	public Renderer myRend;

	public Renderer SecondRend;

	public Color selectedColor;

	public Color hoverColor;

	public Color[] notSelectedColor;

	public Color notSelectedColor2;

	public MapEditorGridPiece myMEGP;

	public bool Selected = false;

	public bool mouseOverMe = false;

	public bool isPlayerOne = false;

	public bool isPlayerTwo = false;

	public bool isPlayerThree = false;

	public bool isPlayerFour = false;

	public bool isEnemyTank = false;

	public bool SelectAllChildren = false;

	public bool CanBeColored = false;

	private HealthTanks HT;

	public bool removeParentOnDelete = false;

	private EnemyAI myEnemyAI;

	private MoveTankScript MTS;

	private FiringTank FT;

	public int CustomAInumber = 0;

	public Material myCustomMaterial;

	public MeshRenderer[] ColoredObjects;

	private bool runonce = false;

	public Color OriginalBodyColor;

	public GameObject DarkLight;

	public string CustomUniqueTankID = "";

	private int lastknownMenuNumber = -1;

	private bool LatestKnownColorState = true;

	private bool ColorSetPlayingMode = false;

	public IEnumerator SetMaterialsDelay()
	{
		yield return new WaitForSeconds(0.1f);
		SetMaterials(Color.white, reset: true);
	}

	public void SetMaterials(Color clr, bool reset)
	{
		if ((bool)myEnemyAI && myEnemyAI.HTscript.isGary && MapEditorMaster.instance.inPlayingMode)
		{
			return;
		}
		if (SelectAllChildren)
		{
			List<MeshRenderer> rends = new List<MeshRenderer>();
			rends.Clear();
			foreach (Transform child in base.transform)
			{
				if (child.GetComponent<MeshRenderer>() != null)
				{
					rends.Add(child.GetComponent<MeshRenderer>());
				}
				if (child.childCount <= 0)
				{
					continue;
				}
				foreach (Transform child2 in child)
				{
					if (child2.GetComponent<MeshRenderer>() != null)
					{
						rends.Add(child2.GetComponent<MeshRenderer>());
					}
				}
			}
			if (reset)
			{
				for (int l = 0; l < rends.Count; l++)
				{
					if (rends[l] != null && rends[l].materials.Length != 0)
					{
						Material[] mats2 = rends[l].materials;
						for (int n = 0; n < mats2.Length; n++)
						{
							mats2[n].color = notSelectedColor[0];
						}
						rends[l].materials = mats2;
					}
				}
				return;
			}
			for (int k = 0; k < rends.Count; k++)
			{
				if (rends[k] != null && rends[k].materials.Length != 0)
				{
					Material[] mats = rends[k].materials;
					for (int m = 0; m < mats.Length; m++)
					{
						mats[m].color = clr;
					}
					rends[k].materials = mats;
				}
			}
		}
		else if (reset)
		{
			Material[] rend2 = myRend.materials;
			for (int j = 0; j < myRend.materials.Length; j++)
			{
				if (notSelectedColor.Length > j)
				{
					rend2[j].color = notSelectedColor[j];
				}
			}
			myRend.materials = rend2;
			if ((bool)SecondRend)
			{
				SecondRend.material.color = notSelectedColor2;
			}
		}
		else
		{
			Material[] rend = myRend.materials;
			for (int i = 0; i < myRend.materials.Length; i++)
			{
				rend[i].color = clr;
			}
			myRend.materials = rend;
		}
	}

	private void Awake()
	{
		if (myRend == null)
		{
			myRend = GetComponent<Renderer>();
		}
		if ((bool)myRend && myRend.materials[0].color != Color.black)
		{
			OriginalBodyColor = myRend.materials[0].color;
		}
		myEnemyAI = GetComponent<EnemyAI>();
		MTS = GetComponent<MoveTankScript>();
		HT = GetComponent<HealthTanks>();
		FT = GetComponent<FiringTank>();
	}

	public void SetRendColors()
	{
		if ((bool)myRend)
		{
			Material[] rend = myRend.materials;
			notSelectedColor = new Color[rend.Length];
			for (int i = 0; i < myRend.materials.Length; i++)
			{
				notSelectedColor[i] = rend[i].color;
			}
		}
	}

	public void SetMyColor()
	{
		if (!CanBeColored)
		{
			return;
		}
		if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor == null)
		{
			MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor = new SerializableColor[5];
		}
		if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber] == null)
		{
			MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber] = new SerializableColor();
			MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber].Color = MapEditorMaster.instance.OTM.FCP_mat.material.GetColor("_Color1");
			Color clr = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber].Color;
			for (int i = 0; i < notSelectedColor.Length; i++)
			{
				notSelectedColor[i] = clr;
			}
			SetMaterials(Color.white, reset: true);
		}
	}

	private void Start()
	{
		myEnemyAI = GetComponent<EnemyAI>();
		MTS = GetComponent<MoveTankScript>();
		HT = GetComponent<HealthTanks>();
		FT = GetComponent<FiringTank>();
		if ((bool)myRend && OriginalBodyColor == Color.black && myRend.materials[0].color != Color.black)
		{
			OriginalBodyColor = myRend.materials[0].color;
		}
		if (CanBeColored)
		{
			SetMyColor();
			StartCoroutine(SetMaterialsDelay());
		}
		else
		{
			SetMyColor();
		}
		SetCustomTankProperties();
		if (TeamNumber < 0)
		{
			if (isEnemyTank)
			{
				TeamNumber = 2;
			}
			else if (isPlayerOne || isPlayerTwo || isPlayerThree || isPlayerFour)
			{
				TeamNumber = 1;
			}
		}
		SetRendColors();
		if ((bool)SecondRend)
		{
			notSelectedColor2 = SecondRend.material.color;
		}
		if (!MapEditorMaster.instance.inPlayingMode)
		{
			SetCustomTankProperties();
		}
		SetTankBodyColor();
		StartCoroutine(SetColorAfterPlacement());
		if (MapEditorMaster.instance != null && !MapEditorMaster.instance.inPlayingMode)
		{
			if (MapEditorMaster.instance.playerOnePlaced[GameMaster.instance.CurrentMission] != 1 && isPlayerOne)
			{
				MapEditorMaster.instance.playerOnePlaced[GameMaster.instance.CurrentMission] = 1;
			}
			if (MapEditorMaster.instance.playerTwoPlaced[GameMaster.instance.CurrentMission] != 1 && isPlayerTwo)
			{
				MapEditorMaster.instance.playerTwoPlaced[GameMaster.instance.CurrentMission] = 1;
			}
			if (MapEditorMaster.instance.playerThreePlaced[GameMaster.instance.CurrentMission] != 1 && isPlayerThree)
			{
				MapEditorMaster.instance.playerThreePlaced[GameMaster.instance.CurrentMission] = 1;
			}
			if (MapEditorMaster.instance.playerFourPlaced[GameMaster.instance.CurrentMission] != 1 && isPlayerFour)
			{
				MapEditorMaster.instance.playerFourPlaced[GameMaster.instance.CurrentMission] = 1;
			}
		}
	}

	private IEnumerator SetColorAfterPlacement()
	{
		yield return new WaitForSeconds(1f);
		if (!MapEditorMaster.instance.inPlayingMode)
		{
			SetTankBodyColor();
		}
	}

	private void SetTankBodyColor()
	{
		if (myEnemyAI != null)
		{
			if (myEnemyAI.MyTeam != TeamNumber)
			{
				myEnemyAI.MyTeam = TeamNumber;
			}
			if (!myRend || TeamNumber <= -1)
			{
				return;
			}
			Material[] rend2 = myRend.materials;
			if (MapEditorMaster.instance.TeamColorEnabled[TeamNumber])
			{
				if (rend2[0].color != MapEditorMaster.instance.TeamColors[TeamNumber])
				{
					rend2[0].color = MapEditorMaster.instance.TeamColors[TeamNumber];
					if (DarkLight != null)
					{
						DarkLight.GetComponent<MeshRenderer>().material.color = MapEditorMaster.instance.TeamColors[TeamNumber];
						DarkLight.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", MapEditorMaster.instance.TeamColors[TeamNumber]);
						DarkLight.GetComponentInChildren<Light>().color = MapEditorMaster.instance.TeamColors[TeamNumber];
					}
					myRend.materials = rend2;
				}
			}
			else
			{
				if (OriginalBodyColor != Color.black)
				{
					rend2[0].color = OriginalBodyColor;
				}
				myRend.materials = rend2;
				if ((bool)myEnemyAI && (bool)myEnemyAI.skidMarkCreator)
				{
					SkidmarkController SC = myEnemyAI.skidMarkCreator.GetComponent<SkidmarkController>();
					SC.startingColor = SC.originalColor;
				}
			}
		}
		else
		{
			if (!MTS || TeamNumber <= -1)
			{
				return;
			}
			MTS.MyTeam = TeamNumber;
			Material[] rend = myRend.materials;
			if (MapEditorMaster.instance.TeamColorEnabled[TeamNumber])
			{
				if (rend[0].color != MapEditorMaster.instance.TeamColors[TeamNumber])
				{
					rend[0].color = MapEditorMaster.instance.TeamColors[TeamNumber];
					if (DarkLight != null)
					{
						DarkLight.GetComponent<MeshRenderer>().material.color = MapEditorMaster.instance.TeamColors[TeamNumber];
						DarkLight.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", MapEditorMaster.instance.TeamColors[TeamNumber]);
						DarkLight.GetComponentInChildren<Light>().color = MapEditorMaster.instance.TeamColors[TeamNumber];
					}
					myRend.materials = rend;
				}
			}
			else
			{
				rend[0].color = OriginalBodyColor;
				myRend.materials = rend;
			}
		}
	}

	private void OnDisable()
	{
		SetCustomTankProperties();
		SetTankBodyColor();
		if (base.transform.parent.gameObject.GetComponent<Animator>() != null && !GameMaster.instance.GameHasStarted)
		{
			Animator ParentAnimator = base.transform.parent.gameObject.GetComponent<Animator>();
			ParentAnimator.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)MapEditorMaster.instance && isEnemyTank && MapEditorMaster.instance.MenuCurrent != lastknownMenuNumber && !MapEditorMaster.instance.inPlayingMode)
		{
			SetCustomTankProperties();
			lastknownMenuNumber = MapEditorMaster.instance.MenuCurrent;
		}
	}

	private void OnEnable()
	{
		SetCustomTankProperties();
		if (myRend != null && notSelectedColor != null && !CanBeColored && notSelectedColor.Length != 0)
		{
			SetMaterials(Color.white, reset: true);
		}
		if ((bool)base.transform.parent && base.transform.parent.gameObject.GetComponent<Animator>() != null && !GameMaster.instance.GameHasStarted)
		{
			Rigidbody RB = GetComponent<Rigidbody>();
			if ((bool)RB)
			{
				RB.isKinematic = true;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			Animator ParentAnimator = base.transform.parent.gameObject.GetComponent<Animator>();
			ParentAnimator.enabled = true;
			if ((bool)MapEditorMaster.instance)
			{
				if (!MapEditorMaster.instance.inPlayingMode)
				{
					ParentAnimator.Play("TankPLOP", -1, 0f);
				}
			}
			else
			{
				ParentAnimator.Play("TankPLOP", -1, 0f);
			}
		}
		SetTankBodyColor();
	}

	private void OnMouseEnter()
	{
		if (!MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode && MapEditorMaster.instance.CurrentLayer == LayerNumber && MapEditorMaster.instance.MenuCurrent != 8)
		{
			SetMaterials(hoverColor, reset: false);
			if ((bool)SecondRend)
			{
				SecondRend.material.color = hoverColor;
			}
			mouseOverMe = true;
		}
	}

	private void OnMouseExit()
	{
		if (!MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode)
		{
			SetMaterials(Color.white, reset: true);
			if ((bool)SecondRend)
			{
				SecondRend.material.color = notSelectedColor2;
			}
			mouseOverMe = false;
		}
	}

	public void RotateProp()
	{
		if (base.transform.tag == "Player" || isEnemyTank)
		{
			base.transform.parent.transform.parent.transform.rotation *= Quaternion.Euler(new Vector3(0f, 90f, 0f));
		}
		else if (base.transform.parent != null)
		{
			if (removeParentOnDelete)
			{
				base.transform.parent.transform.rotation *= Quaternion.Euler(new Vector3(0f, 90f, 0f));
			}
			else if (base.transform.parent.name == "Hole" || base.transform.parent.name == "Hole(Clone)")
			{
				base.transform.parent.transform.rotation *= Quaternion.Euler(new Vector3(0f, 90f, 0f));
			}
			else
			{
				base.transform.rotation *= Quaternion.Euler(new Vector3(0f, 90f, 0f));
			}
		}
		else
		{
			base.transform.rotation *= Quaternion.Euler(new Vector3(0f, 90f, 0f));
		}
	}

	public void UpdateTankProperties()
	{
		float calculation = ((MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomFireSpeed > 0f) ? (1f / MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomFireSpeed) : 0f);
		if (myEnemyAI.ShootSpeed != calculation)
		{
			myEnemyAI.ShootSpeed = calculation;
		}
		if (myEnemyAI.ETSN != null)
		{
			if (MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomCanTeleport)
			{
				myEnemyAI.isElectric = true;
				if (!myEnemyAI.ElectricTeleportRunning)
				{
					myEnemyAI.ActivateElectric();
				}
			}
			else
			{
				myEnemyAI.isElectric = false;
				if (myEnemyAI.ElectricTeleportRunning)
				{
					myEnemyAI.DeactiveElectric();
				}
			}
			myEnemyAI.bulletBounces = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBounces;
			myEnemyAI.ETSN.myTurnSpeed = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTurnHead;
			myEnemyAI.ETSN.bulletPrefab = MapEditorMaster.instance.BulletPrefabs[MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBulletType];
			myEnemyAI.ETSN.ShootSound.Clear();
			myEnemyAI.ETSN.ShootSound.Add(MapEditorMaster.instance.PlayerBulletSound[MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBulletType]);
			myEnemyAI.BouncyBullets = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomCalculateShots;
			myEnemyAI.amountOfBounces = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBounces;
			myEnemyAI.ETSN.AmountShotgunShots = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBulletsPerShot;
		}
		if (notSelectedColor.Length != 0 && notSelectedColor[1] != MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankColor.Color)
		{
			notSelectedColor[1] = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankColor.Color;
			notSelectedColor[2] = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankColor.Color;
			notSelectedColor2 = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankColor.Color;
			SetMaterials(Color.white, reset: true);
		}
		if ((bool)HT)
		{
			int musicId = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomMusic;
			int ID = ((musicId < 8) ? musicId : (musicId + 1));
			if (HT.EnemyID != ID)
			{
				HT.EnemyID = ID;
			}
		}
		myEnemyAI.HTscript.health = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankHealth;
		myEnemyAI.HTscript.maxHealth = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankHealth;
		myEnemyAI.LayMines = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomLayMines;
		myEnemyAI.armoured = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomArmoured;
		myEnemyAI.HTscript.IsArmoured = myEnemyAI.armoured;
		if (myEnemyAI.armoured)
		{
			int amountPoints = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomArmourPoints;
			myEnemyAI.HTscript.health_armour = amountPoints;
			myEnemyAI.HTscript.maxArmour = amountPoints;
			if (myEnemyAI.armour != null)
			{
				myEnemyAI.armour.SetActive(value: true);
				myEnemyAI.armoured = true;
				amountPoints = ((amountPoints > 3) ? 3 : amountPoints);
				for (int i = 0; i < amountPoints; i++)
				{
					myEnemyAI.armour.transform.GetChild(i).gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomArmourPoints = 0;
			myEnemyAI.HTscript.health_armour = 0;
			myEnemyAI.HTscript.maxArmour = 0;
			if (myEnemyAI.armour != null)
			{
				myEnemyAI.armour.SetActive(value: false);
				myEnemyAI.armoured = false;
			}
		}
		Vector3 CustomSize = new Vector3(MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankScale, MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankScale, MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankScale);
		if (base.transform.parent.parent.localScale != CustomSize)
		{
			base.transform.parent.parent.localScale = CustomSize;
		}
		myEnemyAI.isInvisible = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomInvisibility;
		myEnemyAI.ETSN.maxFiredBullets = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBullets;
		myEnemyAI.LayMinesSpeed = 10.5f - MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomMineSpeed;
		myEnemyAI.ETSN.CustomShootSpeed = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomBulletSpeed;
		float calcAcc = Mathf.Abs(100 - MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomAccuracy);
		if (myEnemyAI.Accuracy != calcAcc)
		{
			myEnemyAI.Accuracy = calcAcc;
		}
		if (myEnemyAI.TankSpeed != (float)MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankSpeed)
		{
			myEnemyAI.TankSpeed = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankSpeed;
			myEnemyAI.OriginalTankSpeed = MapEditorMaster.instance.CustomTankDatas[CustomAInumber].CustomTankSpeed;
			if (myEnemyAI.TankSpeed < 1f)
			{
				myEnemyAI.CanMove = false;
			}
			else
			{
				myEnemyAI.CanMove = true;
			}
		}
	}

	private void SetCustomTankProperties()
	{
		if (myEnemyAI != null && !isPlayerTwo && !isPlayerThree && !isPlayerFour)
		{
			if (myMEGP != null && (!GameMaster.instance.GameHasStarted || !MapEditorMaster.instance.isTesting) && (!MapEditorMaster.instance.inPlayingMode || !runonce))
			{
				runonce = true;
				if (myMEGP.myPropID[LayerNumber] >= 100 && myMEGP.myPropID[LayerNumber] < 120)
				{
					UpdateTankProperties();
				}
			}
		}
		else
		{
			if ((!isPlayerOne && !isPlayerTwo && !isPlayerThree && !isPlayerFour) || GameMaster.instance.GameHasStarted)
			{
				return;
			}
			if (!MTS && (bool)myEnemyAI)
			{
				if (myEnemyAI.HTscript.isGary)
				{
					return;
				}
				if (myEnemyAI.TankSpeed != (float)MapEditorMaster.instance.PlayerSpeed)
				{
					myEnemyAI.TankSpeed = MapEditorMaster.instance.PlayerSpeed;
					myEnemyAI.OriginalTankSpeed = MapEditorMaster.instance.PlayerSpeed;
				}
				if (myEnemyAI.ETSN.maxFiredBullets != MapEditorMaster.instance.PlayerMaxBullets)
				{
					myEnemyAI.ETSN.maxFiredBullets = MapEditorMaster.instance.PlayerMaxBullets;
				}
				if (myEnemyAI.LayMines != MapEditorMaster.instance.PlayerCanLayMines)
				{
					myEnemyAI.LayMines = MapEditorMaster.instance.PlayerCanLayMines;
				}
				if (myEnemyAI.ETSN.bulletPrefab != MapEditorMaster.instance.PlayerBulletPrefabs[MapEditorMaster.instance.PlayerBulletType])
				{
					myEnemyAI.ETSN.bulletPrefab = MapEditorMaster.instance.PlayerBulletPrefabs[MapEditorMaster.instance.PlayerBulletType];
					myEnemyAI.ETSN.ShootSound.Clear();
					myEnemyAI.ETSN.ShootSound.Add(MapEditorMaster.instance.PlayerBulletSound[MapEditorMaster.instance.PlayerBulletType]);
				}
			}
			else if ((bool)MTS)
			{
				if (MTS.TankSpeed != (float)MapEditorMaster.instance.PlayerSpeed)
				{
					MTS.TankSpeed = MapEditorMaster.instance.PlayerSpeed;
				}
				if (FT.maxFiredBullets != MapEditorMaster.instance.PlayerMaxBullets)
				{
					FT.maxFiredBullets = MapEditorMaster.instance.PlayerMaxBullets;
				}
				if (FT.canLayMine != MapEditorMaster.instance.PlayerCanLayMines)
				{
					FT.canLayMine = MapEditorMaster.instance.PlayerCanLayMines;
				}
				if (FT.bulletPrefab != MapEditorMaster.instance.PlayerBulletPrefabs[MapEditorMaster.instance.PlayerBulletType])
				{
					FT.bulletPrefab = MapEditorMaster.instance.PlayerBulletPrefabs[MapEditorMaster.instance.PlayerBulletType];
					FT.shootSound.Clear();
					FT.shootSound.Add(MapEditorMaster.instance.PlayerBulletSound[MapEditorMaster.instance.PlayerBulletType]);
				}
			}
			if (HT != null && (!myEnemyAI || !myEnemyAI.HTscript.isGary) && HT.health_armour != MapEditorMaster.instance.PlayerArmourPoints)
			{
				HT.health_armour = MapEditorMaster.instance.PlayerArmourPoints;
				HT.maxArmour = HT.health_armour;
				HT.SetArmourPlates();
			}
		}
	}

	private void DeselectThisProp()
	{
		MapEditorMaster.instance.OTM.OnCloseMenu();
		MapEditorMaster.instance.OTM.CurrentDifficulty = MyDifficultySpawn;
		MapEditorMaster.instance.OTM.CurrentTeamNumber = TeamNumber;
	}

	private void SelectThisProp()
	{
		if ((bool)MapEditorMaster.instance.OTM.SelectedMEP && (bool)MapEditorMaster.instance.OTM.SelectedMEP.myMEGP)
		{
			MapEditorMaster.instance.OTM.SelectedMEP.myMEGP.FieldSelected = false;
		}
		int menuID = 0;
		if (CanBeColored)
		{
			menuID = 1;
		}
		MapEditorMaster.instance.OTM.OnOpenMenu(menuID, this);
		MapEditorMaster.instance.OTM.CurrentDifficulty = MyDifficultySpawn;
		MapEditorMaster.instance.OTM.DifficultyPick.value = MyDifficultySpawn;
		MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: true);
		MapEditorMaster.instance.OTM.SelectedMEP = this;
		MapEditorMaster.instance.OTM.CurrentTeamNumber = TeamNumber;
		myMEGP.FieldSelected = true;
		if (CanBeColored)
		{
			MapEditorMaster.instance.OTM.FCP.startingColor = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber].Color;
			MapEditorMaster.instance.OTM.FCP.color = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber].Color;
		}
	}

	private void Update()
	{
		if (myEnemyAI != null)
		{
			myEnemyAI.MyTeam = TeamNumber;
			if (myRend.materials[0].color != MapEditorMaster.instance.TeamColors[TeamNumber] || LatestKnownColorState != MapEditorMaster.instance.TeamColorEnabled[TeamNumber])
			{
				LatestKnownColorState = MapEditorMaster.instance.TeamColorEnabled[TeamNumber];
				SetTankBodyColor();
			}
		}
		else if ((bool)MTS)
		{
			if (MTS.MyTeam != TeamNumber)
			{
				MTS.MyTeam = TeamNumber;
			}
			if (myRend.materials[0].color != MapEditorMaster.instance.TeamColors[TeamNumber] || LatestKnownColorState != MapEditorMaster.instance.TeamColorEnabled[TeamNumber])
			{
				LatestKnownColorState = MapEditorMaster.instance.TeamColorEnabled[TeamNumber];
				SetTankBodyColor();
			}
		}
		if (MapEditorMaster.instance.isTesting || GameMaster.instance.GameHasPaused || MapEditorMaster.instance.inPlayingMode || MapEditorMaster.instance.CurrentLayer != LayerNumber || MapEditorMaster.instance.MenuCurrent == 8)
		{
			if (ColorSetPlayingMode || !CanBeColored)
			{
				return;
			}
			ColorSetPlayingMode = true;
			if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber] != null)
			{
				Color clr2 = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber].Color;
				for (int i = 0; i < notSelectedColor.Length; i++)
				{
					notSelectedColor[i] = clr2;
				}
			}
			return;
		}
		if (CanBeColored)
		{
			if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber] == null)
			{
				Debug.LogError("NO COLOR SET");
				MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber] = new SerializableColor();
			}
			else
			{
				Color clr = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber].Color;
				for (int j = 0; j < notSelectedColor.Length; j++)
				{
					notSelectedColor[j] = clr;
				}
			}
		}
		if (mouseOverMe && (isEnemyTank || isPlayerOne || isPlayerTwo || isPlayerThree || isPlayerFour || CanBeColored) && !MapEditorMaster.instance.RemoveMode && ReInput.players.GetPlayer(0).GetButtonDown("Use"))
		{
			if (MapEditorMaster.instance.OTM.SelectedMEP != this)
			{
				SelectThisProp();
			}
			else
			{
				DeselectThisProp();
			}
		}
		if (mouseOverMe)
		{
			if (!Selected)
			{
				SetMaterials(hoverColor, reset: false);
				if ((bool)SecondRend)
				{
					SecondRend.material.color = hoverColor;
				}
				if (Input.GetMouseButtonDown(0) && !Selected)
				{
					MapEditorMaster.instance.RemoveMode = true;
					MapEditorMaster.instance.OnTeamsMenu = false;
					Selected = true;
				}
				else if (Input.GetMouseButton(0) && !Selected && MapEditorMaster.instance.RemoveMode)
				{
					Selected = true;
				}
			}
			if (Input.GetMouseButtonDown(1))
			{
				RotateProp();
				MapEditorMaster.instance.PlayAudio(MapEditorMaster.instance.RotateObject);
				if (myMEGP != null)
				{
					if (myMEGP.rotationDirection[LayerNumber] < 3)
					{
						myMEGP.rotationDirection[LayerNumber]++;
					}
					else
					{
						myMEGP.rotationDirection[LayerNumber] = 0;
					}
					MapEditorMaster.instance.LastPlacedRotation = myMEGP.rotationDirection[LayerNumber];
				}
			}
		}
		if (Input.GetMouseButtonUp(0) && Selected && MapEditorMaster.instance.canPlaceProp)
		{
			RemoveThisGridObject(force: false);
		}
		if (Selected)
		{
			SetMaterials(selectedColor, reset: false);
			if ((bool)SecondRend)
			{
				SecondRend.material.color = selectedColor;
			}
			mouseOverMe = true;
		}
	}

	public void RemoveThisGridObject(bool force)
	{
		if (!force)
		{
			MapEditorMaster.instance.RemoveMode = false;
			MapEditorMaster.instance.OnTeamsMenu = false;
			MapEditorMaster.instance.OTM.OnCloseMenu();
		}
		myMEGP.myPropID[myMEGP.IDlayer] = -1;
		myMEGP.myPropPrefab[myMEGP.IDlayer] = null;
		if (MapEditorMaster.instance.OTM.SelectedMEP == this)
		{
			DeselectThisProp();
		}
		if (MapEditorMaster.instance.CurrentLayer < MapEditorMaster.instance.MaxLayer)
		{
			if (myMEGP.myPropID[myMEGP.IDlayer + 1] > 3 && myMEGP.myPropID[myMEGP.IDlayer + 1] < 40)
			{
				SetMaterials(Color.white, reset: false);
				mouseOverMe = false;
				Selected = false;
				MapEditorMaster.instance.ShowErrorMessage("ERROR: Tank on top!");
				return;
			}
			if (myMEGP.myPropID[myMEGP.IDlayer + 1] == 3 || myMEGP.myPropID[myMEGP.IDlayer + 1] == 46 || myMEGP.myPropID[myMEGP.IDlayer + 1] == 47 || myMEGP.myPropID[myMEGP.IDlayer + 1] == 48)
			{
				SetMaterials(Color.white, reset: false);
				mouseOverMe = false;
				Selected = false;
				MapEditorMaster.instance.ShowErrorMessage("ERROR: Remove object on top!");
				return;
			}
		}
		if (CanBeColored)
		{
			MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[myMEGP.ID].CustomColor[LayerNumber] = null;
		}
		if (base.transform.tag == "Player" || isEnemyTank)
		{
			if (isPlayerOne)
			{
				MapEditorMaster.instance.playerOnePlaced[GameMaster.instance.CurrentMission] = 0;
			}
			else if (isPlayerTwo)
			{
				MapEditorMaster.instance.playerTwoPlaced[GameMaster.instance.CurrentMission] = 0;
			}
			else if (isPlayerThree)
			{
				MapEditorMaster.instance.playerThreePlaced[GameMaster.instance.CurrentMission] = 0;
			}
			else if (isPlayerFour)
			{
				MapEditorMaster.instance.playerFourPlaced[GameMaster.instance.CurrentMission] = 0;
			}
			else if (isEnemyTank)
			{
				MapEditorMaster.instance.enemyTanksPlaced[GameMaster.instance.CurrentMission]--;
			}
			if ((bool)myMEGP)
			{
				isEnemyTank = false;
				isPlayerOne = false;
				isPlayerTwo = false;
				isPlayerThree = false;
				isPlayerFour = false;
				myMEGP.SetGridPieceColor();
			}
			Object.Destroy(base.transform.parent.transform.parent.gameObject);
			MapEditorMaster.instance.PlayAudio(MapEditorMaster.instance.RemoveObject);
		}
		else if (base.transform.parent != null)
		{
			MapEditorMaster.instance.PlayAudio(MapEditorMaster.instance.RemoveObject);
			if (removeParentOnDelete)
			{
				Object.Destroy(base.transform.parent.gameObject);
			}
			else if (base.transform.parent.name == "Hole" || base.transform.parent.name == "Hole(Clone)")
			{
				Object.Destroy(base.transform.parent.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
