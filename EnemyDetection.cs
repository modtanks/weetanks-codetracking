using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
	public Color Targeted;

	public Color Normal;

	public Color Chosen;

	public bool isTargeted;

	public bool isChosen;

	public bool isPreferred;

	public bool isRing1;

	public bool MineClose;

	public float DistToMine;

	public int TargetDanger;

	public int ID;

	public GameObject papaTank;

	public EnemyAI AIscript;

	public bool isCenter;

	public bool inWall;

	public bool onFloor = true;

	public bool ignoringBullets;

	public float mineCheckRange = 3f;

	public List<GameObject> FloorPieces = new List<GameObject>();

	public List<GameObject> SolidPieces = new List<GameObject>();

	public List<GameObject> Enemies = new List<GameObject>();

	public List<GameObject> Bullets = new List<GameObject>();

	private void Start()
	{
		AIscript = papaTank.GetComponent<EnemyAI>();
		float num = Random.Range(0.3f, 0.5f);
		InvokeRepeating("DisableTarget", num, num);
		num = ((!(AIscript.TankSpeed > 70f)) ? Random.Range(0.2f, 0.4f) : Random.Range(0.1f, 0.3f));
		InvokeRepeating("CheckTarget", num, num);
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
	}

	private void IgnoreBullets()
	{
		if (Random.Range(0, 100) < AIscript.NotSeeBulletChancePercentage)
		{
			ignoringBullets = true;
			Bullets.Clear();
		}
		else
		{
			ignoringBullets = false;
		}
	}

	private bool checkPieces(List<GameObject> pieces)
	{
		if (pieces.Count > 0)
		{
			isTargeted = true;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < pieces.Count; i++)
			{
				if (pieces[i] == null)
				{
					list.Add(pieces[i]);
				}
			}
			foreach (GameObject item in list)
			{
				pieces.Remove(item);
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
		if (papaTank.GetComponent<EnemyAI>().difficulty > 0)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, mineCheckRange);
			foreach (Collider collider in array)
			{
				if (collider.tag == "Mine" && papaTank.GetComponent<HealthTanks>().EnemyID != 9)
				{
					MineScript component = collider.GetComponent<MineScript>();
					if (component.active || component.MyPlacer == papaTank)
					{
						MineClose = true;
						DistToMine = Vector3.Distance(component.transform.position, base.transform.position);
						return true;
					}
				}
				else if (collider.tag == "Solid")
				{
					ExplosiveBlock component2 = collider.GetComponent<ExplosiveBlock>();
					if ((bool)component2 && component2.isFusing)
					{
						MineClose = true;
						DistToMine = Vector3.Distance(component2.transform.position, base.transform.position);
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
		Collider[] array = Physics.OverlapSphere(base.transform.position, 3f);
		foreach (Collider collider in array)
		{
			if (collider.tag == "Enemy" || collider.tag == "Boss")
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
		if (isChosen)
		{
			_ = isPreferred;
		}
		if (GameMaster.instance.CurrentMission != 49)
		{
			if (checkMines())
			{
				isTargeted = true;
				TargetDanger = 32;
			}
			if (checkPieces(Bullets) || checkPieces(SolidPieces) || checkPieces(Enemies) || checkMines() || (checkEnemies() && AIscript.IsCompanion))
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
				List<GameObject> list = new List<GameObject>();
				for (int i = 0; i < FloorPieces.Count; i++)
				{
					if (FloorPieces[i] == null)
					{
						list.Add(FloorPieces[i]);
					}
				}
				{
					foreach (GameObject item in list)
					{
						FloorPieces.Remove(item);
					}
					return;
				}
			}
			isTargeted = true;
		}
	}

	private void Update()
	{
		if (isTargeted)
		{
			GetComponent<MeshRenderer>().material.color = Targeted;
			isChosen = false;
		}
		else if (isChosen)
		{
			GetComponent<MeshRenderer>().material.color = Chosen;
		}
		else
		{
			GetComponent<MeshRenderer>().material.color = Normal;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && !isCenter && GameMaster.instance.CurrentMission != 49)
		{
			if (!SolidPieces.Contains(other.gameObject))
			{
				SolidPieces.Add(other.gameObject);
			}
		}
		else if (other.transform.tag == "ElectricWall")
		{
			if (!SolidPieces.Contains(other.gameObject))
			{
				SolidPieces.Add(other.gameObject);
			}
		}
		else if (GameMaster.instance.CurrentMission != 69 && other.transform.tag == "ElectricPad" && !AIscript.isElectric)
		{
			if (!SolidPieces.Contains(other.gameObject))
			{
				SolidPieces.Add(other.gameObject);
			}
		}
		else if (other.transform.tag == "Enemy" && other.gameObject != papaTank)
		{
			if (!Enemies.Contains(other.gameObject))
			{
				Enemies.Add(other.gameObject);
			}
		}
		else if (other.transform.tag == "Player" && other.gameObject != papaTank)
		{
			if (!Enemies.Contains(other.gameObject))
			{
				Enemies.Add(other.gameObject);
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
				EnemyBulletScript component = other.transform.GetComponent<EnemyBulletScript>();
				if (component != null)
				{
					if ((!(component.papaTank == papaTank) || component.BounceAmount != 0) && !Bullets.Contains(other.gameObject))
					{
						Bullets.Add(other.gameObject);
					}
				}
				else if (!AIscript.IsCompanion && !Bullets.Contains(other.gameObject))
				{
					Bullets.Add(other.gameObject);
				}
			}
			else if ((other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && GameMaster.instance.CurrentMission == 49)
			{
				if (!FloorPieces.Contains(other.gameObject))
				{
					FloorPieces.Add(other.gameObject);
				}
				if (FloorPieces.Count > 0)
				{
					isTargeted = false;
					onFloor = true;
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && GameMaster.instance.CurrentMission != 49)
		{
			if (SolidPieces.Contains(other.gameObject))
			{
				SolidPieces.Remove(other.gameObject);
			}
		}
		else if (other.transform.tag == "ElectricWall")
		{
			if (SolidPieces.Contains(other.gameObject))
			{
				SolidPieces.Remove(other.gameObject);
			}
		}
		else if (GameMaster.instance.CurrentMission != 69 && other.transform.tag == "ElectricPad")
		{
			if (SolidPieces.Contains(other.gameObject))
			{
				SolidPieces.Remove(other.gameObject);
			}
		}
		else if (other.transform.tag == "Enemy" && other.gameObject != papaTank)
		{
			if (Enemies.Contains(other.gameObject))
			{
				Enemies.Remove(other.gameObject);
			}
		}
		else if (other.transform.tag == "Player" && other.gameObject != papaTank)
		{
			if (Enemies.Contains(other.gameObject))
			{
				Enemies.Remove(other.gameObject);
			}
		}
		else if (other.transform.tag == "Bullet")
		{
			if (Bullets.Contains(other.gameObject))
			{
				Bullets.Remove(other.gameObject);
			}
		}
		else if (!(other.transform.tag == "Mine") && (other.transform.tag == "Solid" || other.transform.tag == "MapBorder") && GameMaster.instance.CurrentMission == 49)
		{
			if (FloorPieces.Contains(other.gameObject))
			{
				FloorPieces.Remove(other.gameObject);
			}
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
		TargetDanger = 0;
		isPreferred = false;
		Bullets.Clear();
	}
}
