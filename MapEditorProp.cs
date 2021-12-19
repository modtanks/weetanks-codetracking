using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorProp : MonoBehaviour
{
	public int TeamNumber = -1;

	public int MyDifficultySpawn;

	public int LayerNumber;

	public Renderer myRend;

	public Renderer SecondRend;

	public Color selectedColor;

	public Color hoverColor;

	public Color[] notSelectedColor;

	public Color notSelectedColor2;

	public MapEditorGridPiece myMEGP;

	public bool Selected;

	public bool mouseOverMe;

	public bool isPlayerOne;

	public bool isPlayerTwo;

	public bool isPlayerThree;

	public bool isPlayerFour;

	public bool isEnemyTank;

	public bool SelectAllChildren;

	private HealthTanks HT;

	public bool removeParentOnDelete;

	private EnemyAI myEnemyAI;

	private MoveTankScript MTS;

	private FiringTank FT;

	public int CustomAInumber;

	public Material myCustomMaterial;

	public MeshRenderer[] ColoredObjects;

	private bool runonce;

	public Color OriginalBodyColor;

	public GameObject DarkLight;

	private bool LatestKnownColorState = true;

	private void SetMaterials(Color clr, bool reset)
	{
		if ((bool)myEnemyAI && myEnemyAI.HTscript.isGary && MapEditorMaster.instance.inPlayingMode)
		{
			return;
		}
		if (SelectAllChildren)
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			list.Clear();
			foreach (Transform item in base.transform)
			{
				if (item.GetComponent<MeshRenderer>() != null)
				{
					list.Add(item.GetComponent<MeshRenderer>());
				}
			}
			if (reset)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null && list[i].materials.Length != 0)
					{
						list[i].material.color = notSelectedColor[0];
					}
				}
				return;
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j] != null && list[j].materials.Length != 0)
				{
					list[j].material.color = clr;
				}
			}
		}
		else if (reset)
		{
			Material[] materials = myRend.materials;
			for (int k = 0; k < myRend.materials.Length; k++)
			{
				materials[k].color = notSelectedColor[k];
			}
			myRend.materials = materials;
			if ((bool)SecondRend)
			{
				SecondRend.material.color = notSelectedColor2;
			}
		}
		else
		{
			Material[] materials2 = myRend.materials;
			for (int l = 0; l < myRend.materials.Length; l++)
			{
				materials2[l].color = clr;
			}
			myRend.materials = materials2;
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
		if (!MapEditorMaster.instance.inPlayingMode)
		{
			SetCustomTankProperties();
		}
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
		if ((bool)myRend)
		{
			Material[] materials = myRend.materials;
			notSelectedColor = new Color[materials.Length];
			for (int i = 0; i < myRend.materials.Length; i++)
			{
				notSelectedColor[i] = materials[i].color;
			}
		}
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
			if (!MapEditorMaster.instance.inPlayingMode)
			{
				SetCustomTankProperties();
			}
			if (myEnemyAI.MyTeam != TeamNumber)
			{
				myEnemyAI.MyTeam = TeamNumber;
			}
			if (!myRend)
			{
				return;
			}
			Material[] materials = myRend.materials;
			if (MapEditorMaster.instance.TeamColorEnabled[TeamNumber])
			{
				if (materials[0].color != MapEditorMaster.instance.TeamColors[TeamNumber])
				{
					materials[0].color = MapEditorMaster.instance.TeamColors[TeamNumber];
					if (DarkLight != null)
					{
						DarkLight.GetComponent<MeshRenderer>().material.color = MapEditorMaster.instance.TeamColors[TeamNumber];
						DarkLight.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", MapEditorMaster.instance.TeamColors[TeamNumber]);
						DarkLight.GetComponentInChildren<Light>().color = MapEditorMaster.instance.TeamColors[TeamNumber];
					}
					myRend.materials = materials;
				}
			}
			else
			{
				if (OriginalBodyColor != Color.black)
				{
					materials[0].color = OriginalBodyColor;
				}
				myRend.materials = materials;
				if ((bool)myEnemyAI && (bool)myEnemyAI.skidMarkCreator)
				{
					SkidmarkController component = myEnemyAI.skidMarkCreator.GetComponent<SkidmarkController>();
					component.startingColor = component.originalColor;
				}
			}
		}
		else
		{
			if (!MTS)
			{
				return;
			}
			MTS.MyTeam = TeamNumber;
			Material[] materials2 = myRend.materials;
			if (MapEditorMaster.instance.TeamColorEnabled[TeamNumber])
			{
				if (materials2[0].color != MapEditorMaster.instance.TeamColors[TeamNumber])
				{
					materials2[0].color = MapEditorMaster.instance.TeamColors[TeamNumber];
					if (DarkLight != null)
					{
						DarkLight.GetComponent<MeshRenderer>().material.color = MapEditorMaster.instance.TeamColors[TeamNumber];
						DarkLight.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", MapEditorMaster.instance.TeamColors[TeamNumber]);
						DarkLight.GetComponentInChildren<Light>().color = MapEditorMaster.instance.TeamColors[TeamNumber];
					}
					myRend.materials = materials2;
				}
			}
			else
			{
				materials2[0].color = OriginalBodyColor;
				myRend.materials = materials2;
			}
		}
	}

	private void OnDisable()
	{
		if (!MapEditorMaster.instance.inPlayingMode)
		{
			SetCustomTankProperties();
		}
		SetTankBodyColor();
		if (base.transform.parent.gameObject.GetComponent<Animator>() != null && !GameMaster.instance.GameHasStarted)
		{
			base.transform.parent.gameObject.GetComponent<Animator>().enabled = false;
		}
	}

	private void OnEnable()
	{
		SetCustomTankProperties();
		if (myRend != null && notSelectedColor != null && notSelectedColor.Length != 0)
		{
			SetMaterials(Color.white, reset: true);
		}
		if ((bool)base.transform.parent && base.transform.parent.gameObject.GetComponent<Animator>() != null && !GameMaster.instance.GameHasStarted)
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.isKinematic = true;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			Animator component2 = base.transform.parent.gameObject.GetComponent<Animator>();
			component2.enabled = true;
			if ((bool)MapEditorMaster.instance)
			{
				if (!MapEditorMaster.instance.inPlayingMode)
				{
					component2.Play("TankPLOP", -1, 0f);
				}
			}
			else
			{
				component2.Play("TankPLOP", -1, 0f);
			}
		}
		SetTankBodyColor();
	}

	private void OnMouseEnter()
	{
		if (MapEditorMaster.instance.isTesting || MapEditorMaster.instance.inPlayingMode || MapEditorMaster.instance.CurrentLayer != LayerNumber || (MapEditorMaster.instance.OnTeamsMenu && (!(MapEditorMaster.instance.OTM != null) || ((!(MapEditorMaster.instance.OTM.SelectedMEP == this) || !MapEditorMaster.instance.OnTeamsMenu) && !MapEditorMaster.instance.RemoveMode && MapEditorMaster.instance.OnTeamsMenu))))
		{
			return;
		}
		SetMaterials(hoverColor, reset: false);
		if ((isEnemyTank || isPlayerOne || isPlayerTwo || isPlayerThree || isPlayerFour) && !MapEditorMaster.instance.OnTeamsMenu && !MapEditorMaster.instance.RemoveMode)
		{
			MapEditorMaster.instance.OTM.CurrentDifficulty = MyDifficultySpawn;
			MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: true);
			MapEditorMaster.instance.OTM.SelectedMEP = this;
			MapEditorMaster.instance.OTM.CurrentTeamNumber = TeamNumber;
			Vector3 vector = Camera.main.GetComponent<Camera>().WorldToScreenPoint(base.transform.position);
			Debug.Log("target is " + vector.x + " pixels from the left");
			Debug.Log("target is " + vector.y + " pixels from the bottom");
			float height = MapEditorMaster.instance.OTM.GetComponent<RectTransform>().rect.height;
			Debug.Log("height is " + height);
			if (base.transform.position.z < 0f)
			{
				MapEditorMaster.instance.OTM.transform.position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(base.transform.position) + new Vector3(0f, height / 2f, 0f);
			}
			else
			{
				MapEditorMaster.instance.OTM.transform.position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(base.transform.position) - new Vector3(0f, height / 2f, 0f);
			}
		}
		if ((bool)SecondRend)
		{
			SecondRend.material.color = hoverColor;
		}
		mouseOverMe = true;
	}

	private void OnMouseExit()
	{
		if (!MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode)
		{
			SetMaterials(Color.white, reset: true);
			if ((isEnemyTank || isPlayerOne || isPlayerTwo || isPlayerThree || isPlayerFour) && !MapEditorMaster.instance.OnTeamsMenu)
			{
				MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: false);
			}
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

	private void SetCustomTankProperties()
	{
		if (myEnemyAI != null && !isPlayerTwo && !isPlayerThree && !isPlayerFour)
		{
			if (!(myMEGP != null) || (GameMaster.instance.GameHasStarted && MapEditorMaster.instance.isTesting) || (MapEditorMaster.instance.inPlayingMode && runonce))
			{
				return;
			}
			runonce = true;
			if (myMEGP.myPropID[LayerNumber] != 19 && myMEGP.myPropID[LayerNumber] != 20 && myMEGP.myPropID[LayerNumber] != 21)
			{
				return;
			}
			float num = ((MapEditorMaster.instance.CustomFireSpeed[CustomAInumber] > 0f) ? (1f / MapEditorMaster.instance.CustomFireSpeed[CustomAInumber]) : 0f);
			if (myEnemyAI.ShootSpeed != num)
			{
				myEnemyAI.ShootSpeed = num;
			}
			if (myEnemyAI.ETSN != null)
			{
				if (myEnemyAI.bulletBounces != MapEditorMaster.instance.CustomBounces[CustomAInumber])
				{
					myEnemyAI.bulletBounces = MapEditorMaster.instance.CustomBounces[CustomAInumber];
				}
				if (myEnemyAI.ETSN.myTurnSpeed != (float)MapEditorMaster.instance.CustomTurnHead[CustomAInumber])
				{
					myEnemyAI.ETSN.myTurnSpeed = MapEditorMaster.instance.CustomTurnHead[CustomAInumber];
				}
				if (myEnemyAI.ETSN.bulletPrefab != MapEditorMaster.instance.BulletPrefabs[MapEditorMaster.instance.CustomBulletType[CustomAInumber]])
				{
					myEnemyAI.ETSN.bulletPrefab = MapEditorMaster.instance.BulletPrefabs[MapEditorMaster.instance.CustomBulletType[CustomAInumber]];
				}
				if (myEnemyAI.BouncyBullets != MapEditorMaster.instance.CustomCalculateShots[CustomAInumber])
				{
					myEnemyAI.BouncyBullets = MapEditorMaster.instance.CustomCalculateShots[CustomAInumber];
				}
				if (myEnemyAI.amountOfBounces != MapEditorMaster.instance.CustomBounces[CustomAInumber])
				{
					myEnemyAI.amountOfBounces = MapEditorMaster.instance.CustomBounces[CustomAInumber];
				}
			}
			if (notSelectedColor[1] != MapEditorMaster.instance.CustomTankColor[CustomAInumber])
			{
				notSelectedColor[1] = MapEditorMaster.instance.CustomTankColor[CustomAInumber];
				notSelectedColor[2] = MapEditorMaster.instance.CustomTankColor[CustomAInumber];
				notSelectedColor2 = MapEditorMaster.instance.CustomTankColor[CustomAInumber];
				SetMaterials(Color.white, reset: true);
			}
			if ((bool)HT)
			{
				int num2 = MapEditorMaster.instance.CustomMusic[CustomAInumber];
				int num3 = ((num2 < 8) ? num2 : (num2 + 1));
				if (HT.EnemyID != num3)
				{
					HT.EnemyID = num3;
				}
			}
			if (myEnemyAI.LayMines != MapEditorMaster.instance.CustomLayMines[CustomAInumber])
			{
				myEnemyAI.LayMines = MapEditorMaster.instance.CustomLayMines[CustomAInumber];
			}
			if (myEnemyAI.armoured != MapEditorMaster.instance.CustomArmoured[CustomAInumber])
			{
				myEnemyAI.armoured = MapEditorMaster.instance.CustomArmoured[CustomAInumber];
				myEnemyAI.HTscript.IsArmoured = myEnemyAI.armoured;
				myEnemyAI.HTscript.health = -1;
				myEnemyAI.HTscript.maxHealth = -1;
			}
			Vector3 vector = new Vector3(MapEditorMaster.instance.CustomTankScale[CustomAInumber], MapEditorMaster.instance.CustomTankScale[CustomAInumber], MapEditorMaster.instance.CustomTankScale[CustomAInumber]);
			if (base.transform.parent.parent.localScale != vector)
			{
				base.transform.parent.parent.localScale = vector;
			}
			if (myEnemyAI.HTscript.health != MapEditorMaster.instance.CustomArmourPoints[CustomAInumber] && myEnemyAI.armoured)
			{
				int num4 = MapEditorMaster.instance.CustomArmourPoints[CustomAInumber];
				myEnemyAI.HTscript.health = num4;
				myEnemyAI.HTscript.maxHealth = num4;
				if (MapEditorMaster.instance.inPlayingMode)
				{
					myEnemyAI.armour = myEnemyAI.gameObject.transform.Find("Armour").gameObject;
					if (myEnemyAI.armour != null)
					{
						myEnemyAI.armour.SetActive(value: true);
						myEnemyAI.armoured = true;
						for (int i = 0; i < num4; i++)
						{
							myEnemyAI.armour.transform.GetChild(i).gameObject.SetActive(value: true);
						}
					}
				}
			}
			else if (!myEnemyAI.armoured && myEnemyAI.HTscript.health != MapEditorMaster.instance.CustomArmourPoints[CustomAInumber])
			{
				MapEditorMaster.instance.CustomArmourPoints[CustomAInumber] = 1;
				myEnemyAI.HTscript.health = 1;
				myEnemyAI.HTscript.maxHealth = 1;
				myEnemyAI.armour = myEnemyAI.gameObject.transform.Find("Armour").gameObject;
				if (myEnemyAI.armour != null)
				{
					for (int j = 0; j < 3; j++)
					{
						myEnemyAI.armour.transform.GetChild(j).gameObject.SetActive(value: false);
					}
				}
			}
			if (myEnemyAI.isInvisible != MapEditorMaster.instance.CustomInvisibility[CustomAInumber])
			{
				myEnemyAI.isInvisible = MapEditorMaster.instance.CustomInvisibility[CustomAInumber];
			}
			if (myEnemyAI.ETSN.maxFiredBullets != MapEditorMaster.instance.CustomBullets[CustomAInumber])
			{
				myEnemyAI.ETSN.maxFiredBullets = MapEditorMaster.instance.CustomBullets[CustomAInumber];
			}
			if (myEnemyAI.LayMinesSpeed != MapEditorMaster.instance.CustomMineSpeed[CustomAInumber])
			{
				myEnemyAI.LayMinesSpeed = 10.5f - MapEditorMaster.instance.CustomMineSpeed[CustomAInumber];
			}
			float num5 = Mathf.Abs(100 - MapEditorMaster.instance.CustomAccuracy[CustomAInumber]);
			if (myEnemyAI.Accuracy != num5)
			{
				myEnemyAI.Accuracy = num5;
			}
			if (myEnemyAI.TankSpeed != (float)MapEditorMaster.instance.CustomTankSpeed[CustomAInumber])
			{
				myEnemyAI.TankSpeed = MapEditorMaster.instance.CustomTankSpeed[CustomAInumber];
				myEnemyAI.OriginalTankSpeed = MapEditorMaster.instance.CustomTankSpeed[CustomAInumber];
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
				}
			}
			if (HT != null && (!myEnemyAI || !myEnemyAI.HTscript.isGary) && HT.health != MapEditorMaster.instance.PlayerArmourPoints + 1)
			{
				HT.health = 1 + MapEditorMaster.instance.PlayerArmourPoints;
				HT.maxHealth = HT.health;
				HT.SetArmourPlates();
			}
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
		if (myEnemyAI != null || MTS != null)
		{
			SetCustomTankProperties();
		}
		if (MapEditorMaster.instance.isTesting || GameMaster.instance.GameHasPaused || MapEditorMaster.instance.inPlayingMode || MapEditorMaster.instance.CurrentLayer != LayerNumber)
		{
			return;
		}
		if (mouseOverMe && !Selected)
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
		if (Input.GetMouseButtonDown(1) && mouseOverMe)
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
		if (Input.GetMouseButtonUp(0) && Selected && MapEditorMaster.instance.canPlaceProp)
		{
			MapEditorMaster.instance.RemoveMode = false;
			MapEditorMaster.instance.OnTeamsMenu = false;
			MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: false);
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
				return;
			}
			if (base.transform.parent != null)
			{
				MapEditorMaster.instance.PlayAudio(MapEditorMaster.instance.RemoveObject);
				if (removeParentOnDelete)
				{
					Object.Destroy(base.transform.parent.gameObject);
					return;
				}
				if (base.transform.parent.name == "Hole" || base.transform.parent.name == "Hole(Clone)")
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
}
