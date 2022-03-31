using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
	public Color Targeted;

	public Color Normal;

	public Color Chosen;

	public bool isTargeted = false;

	public bool isChosen = false;

	public bool isPreferred = false;

	public bool isRing1 = false;

	public bool MineClose = false;

	public float DistToMine;

	public int TargetDanger = 0;

	public int ID = 0;

	public GameObject papaTank;

	public EnemyAI AIscript;

	public bool isCenter = false;

	public bool inWall = false;

	public bool onFloor = true;

	public bool ignoringBullets = false;

	public float mineCheckRange = 3f;

	public List<Collider> FloorPieces = new List<Collider>();

	public List<Collider> SolidPieces = new List<Collider>();

	public List<Collider> Enemies = new List<Collider>();

	public List<Collider> Bullets = new List<Collider>();

	private int MY_ID = -1;

	private void Start()
	{
		AIscript = papaTank.GetComponent<EnemyAI>();
		float RandomizedChecker = Random.Range(0.3f, 0.5f);
		InvokeRepeating("DisableTarget", RandomizedChecker, RandomizedChecker);
		RandomizedChecker = ((!(AIscript.TankSpeed > 70f) && !AIscript.isLevel50Boss) ? Random.Range(0.4f, 0.6f) : Random.Range(0.2f, 0.3f));
		InvokeRepeating("CheckTarget", RandomizedChecker, RandomizedChecker);
		if (isRing1 || isCenter)
		{
			mineCheckRange = 5f;
		}
		else
		{
			mineCheckRange = 3f;
		}
		if (AIscript != null && AIscript.CanMove)
		{
			InvokeRepeating("IgnoreBullets", 2f, 2f);
		}
		MY_ID = papaTank.GetComponent<HealthTanks>().EnemyID;
		GetComponent<MeshRenderer>().material.color = Normal;
	}

	private void IgnoreBullets()
	{
		int randomNumber = Random.Range(0, 100);
		if (randomNumber < AIscript.NotSeeBulletChancePercentage)
		{
			ignoringBullets = true;
			Bullets.Clear();
		}
		else
		{
			ignoringBullets = false;
		}
	}

	private bool checkPieces(List<Collider> pieces)
	{
		if (pieces.Count > 0)
		{
			isTargeted = true;
			List<Collider> PiecesToRemove = new List<Collider>();
			for (int i = 0; i < pieces.Count; i++)
			{
				if (pieces[i] == null)
				{
					PiecesToRemove.Add(pieces[i]);
				}
				else if (!pieces[i].enabled)
				{
					PiecesToRemove.Add(pieces[i]);
				}
			}
			foreach (Collider remove in PiecesToRemove)
			{
				pieces.Remove(remove);
			}
			if (pieces.Count > 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private bool checkMines()
	{
		if (AIscript.difficulty > 0 && GameMaster.instance.AmountMinesPlaced > 0)
		{
			LayerMask layersToCheck = (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("NoBounceWall"));
			Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, mineCheckRange, layersToCheck);
			Collider[] array = objectsInRange;
			foreach (Collider col in array)
			{
				if (col.tag == "Mine" && MY_ID != 9)
				{
					MineScript MS = col.GetComponent<MineScript>();
					if (MS.active || MS.MyPlacer == papaTank)
					{
						MineClose = true;
						DistToMine = Vector3.Distance(MS.transform.position, base.transform.position);
						return true;
					}
				}
				else if (col.tag == "Solid")
				{
					ExplosiveBlock EB = col.GetComponent<ExplosiveBlock>();
					if ((bool)EB && EB.isFusing)
					{
						MineClose = true;
						DistToMine = Vector3.Distance(EB.transform.position, base.transform.position);
						return true;
					}
				}
			}
		}
		DistToMine = 99999f;
		MineClose = false;
		return false;
	}

	private bool checkEnemies()
	{
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 3f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (col.tag == "Enemy" || col.tag == "Boss")
			{
				if (papaTank.GetComponent<EnemyAI>().difficulty < 1)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private void CheckTarget()
	{
		if (!GameMaster.instance.GameHasStarted && !GameMaster.instance.inMenuMode)
		{
			return;
		}
		if (GameMaster.instance.CurrentMission != 49 || (bool)MapEditorMaster.instance)
		{
			if (checkMines())
			{
				isTargeted = true;
				TargetDanger = 32;
			}
			else if (checkPieces(Bullets) || checkPieces(SolidPieces) || checkPieces(Enemies) || (checkEnemies() && AIscript.IsCompanion))
			{
				isTargeted = true;
			}
			else
			{
				isTargeted = false;
				TargetDanger = 0;
			}
			if (isCenter)
			{
				return;
			}
			if (ID > 32)
			{
				if (AIscript.Ring2Detection[ID - 33].SolidPieces.Count > 0)
				{
					isTargeted = true;
				}
			}
			else if (ID > 16 && AIscript.Ring1Detection[ID - 17].SolidPieces.Count > 0)
			{
				isTargeted = true;
			}
		}
		else
		{
			if (GameMaster.instance.CurrentMission != 49)
			{
				return;
			}
			if (checkPieces(FloorPieces))
			{
				isTargeted = false;
				List<Collider> ToRemove = new List<Collider>();
				for (int i = 0; i < FloorPieces.Count; i++)
				{
					if (FloorPieces[i] == null)
					{
						ToRemove.Add(FloorPieces[i]);
					}
				}
				{
					foreach (Collider Remove in ToRemove)
					{
						FloorPieces.Remove(Remove);
					}
					return;
				}
			}
			isTargeted = true;
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted && !GameMaster.instance.inMenuMode)
		{
			SolidPieces.Clear();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && !isCenter && (GameMaster.instance.CurrentMission != 49 || (bool)MapEditorMaster.instance))
		{
			if (!SolidPieces.Contains(other))
			{
				SolidPieces.Add(other);
			}
		}
		else if (other.transform.tag == "ElectricWall")
		{
			if (!SolidPieces.Contains(other))
			{
				SolidPieces.Add(other);
			}
		}
		else if ((GameMaster.instance.CurrentMission != 69 || (bool)MapEditorMaster.instance) && other.transform.tag == "ElectricPad" && !AIscript.isElectric)
		{
			if (!SolidPieces.Contains(other))
			{
				SolidPieces.Add(other);
			}
		}
		else if (other.transform.tag == "Enemy" && other.gameObject != papaTank)
		{
			if (!Enemies.Contains(other))
			{
				Enemies.Add(other);
			}
		}
		else if (other.transform.tag == "Player" && other.gameObject != papaTank)
		{
			if (!Enemies.Contains(other))
			{
				Enemies.Add(other);
			}
		}
		else
		{
			if (other.transform.tag == "Mine")
			{
				return;
			}
			if (other.transform.tag == "Bullet" && !ignoringBullets)
			{
				EnemyBulletScript EBS = other.transform.GetComponent<EnemyBulletScript>();
				if (EBS != null)
				{
					if (!(EBS.papaTank == papaTank) || EBS.BounceAmount != 0)
					{
						Bullets.Add(other);
					}
				}
				else if (!AIscript.IsCompanion)
				{
					Bullets.Add(other);
				}
			}
			else if ((other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && GameMaster.instance.CurrentMission == 49 && !MapEditorMaster.instance)
			{
				FloorPieces.Add(other);
				if (FloorPieces.Count > 0)
				{
					isTargeted = false;
					onFloor = true;
				}
			}
		}
	}

	private void OnDisable()
	{
		SolidPieces.Clear();
		Enemies.Clear();
		FloorPieces.Clear();
		Bullets.Clear();
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && (GameMaster.instance.CurrentMission != 49 || (bool)MapEditorMaster.instance))
		{
			SolidPieces.Remove(other);
		}
		else if (other.transform.tag == "ElectricWall")
		{
			SolidPieces.Remove(other);
		}
		else if ((GameMaster.instance.CurrentMission != 69 || (bool)MapEditorMaster.instance) && other.transform.tag == "ElectricPad")
		{
			SolidPieces.Remove(other);
		}
		else if (other.transform.tag == "Enemy" && other.gameObject != papaTank)
		{
			Enemies.Remove(other);
		}
		else if (other.transform.tag == "Player" && other.gameObject != papaTank)
		{
			Enemies.Remove(other);
		}
		else if (other.transform.tag == "Bullet")
		{
			Bullets.Remove(other);
		}
		else if (!(other.transform.tag == "Mine") && (other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && GameMaster.instance.CurrentMission == 49 && !MapEditorMaster.instance)
		{
			FloorPieces.Remove(other);
			if (FloorPieces.Count < 1)
			{
				isTargeted = true;
				onFloor = false;
			}
		}
	}

	private void DisableTarget()
	{
		if (SolidPieces.Count < 1 && Enemies.Count < 1 && Bullets.Count < 1 && !MineClose)
		{
			isTargeted = false;
			TargetDanger = 0;
		}
		if (SolidPieces.Count > 0)
		{
		}
		TargetDanger = 0;
		isPreferred = false;
		Bullets.Clear();
	}
}
