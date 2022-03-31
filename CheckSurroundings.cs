using UnityEngine;

public class CheckSurroundings : MonoBehaviour
{
	public GameObject ElectricPartNorth;

	public GameObject ElectricPartEast;

	public GameObject ElectricPartSouth;

	public GameObject ElectricPartWest;

	public Renderer MyRend;

	public Collider NorthCollider;

	public Collider EastCollider;

	public Collider SouthCollider;

	public Collider WestCollider;

	private Collider myCollider;

	private void Start()
	{
		InvokeRepeating("CheckForWalls", 0.01f, 0.5f);
		myCollider = GetComponent<Collider>();
	}

	private void CheckForWalls()
	{
		if ((bool)MapEditorMaster.instance)
		{
			if (GameMaster.instance.GameHasStarted || MapEditorMaster.instance.isTesting || MapEditorMaster.instance.inPlayingMode)
			{
				myCollider.enabled = false;
				MyRend.gameObject.SetActive(value: false);
			}
			else
			{
				myCollider.enabled = true;
				MyRend.gameObject.SetActive(value: true);
			}
		}
		else
		{
			myCollider.enabled = false;
			MyRend.gameObject.SetActive(value: false);
		}
		if (EastCollider == null && !GameMaster.instance.GameHasStarted)
		{
			Collider[] intersecting = Physics.OverlapSphere(base.transform.position + new Vector3(2f, 0f, 0f), 0.1f);
			if (intersecting.Length == 0)
			{
				ElectricPartEast.SetActive(value: false);
			}
			else
			{
				bool GotSolid = false;
				Collider[] array = intersecting;
				foreach (Collider intersect2 in array)
				{
					if ((intersect2.tag == "Solid" || intersect2.tag == "MapBorder") && (intersect2.gameObject.layer == LayerMask.NameToLayer("Wall") || intersect2.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || intersect2.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
					{
						EastCollider = intersect2;
						GotSolid = true;
					}
				}
				if (GotSolid)
				{
					ElectricPartEast.SetActive(value: true);
				}
				else
				{
					ElectricPartEast.SetActive(value: false);
				}
			}
		}
		else if (EastCollider == null && GameMaster.instance.GameHasStarted)
		{
			ElectricPartEast.SetActive(value: false);
		}
		if (WestCollider == null && !GameMaster.instance.GameHasStarted)
		{
			Collider[] intersecting = Physics.OverlapSphere(base.transform.position + new Vector3(-2f, 0f, 0f), 0.1f);
			if (intersecting.Length == 0)
			{
				ElectricPartWest.SetActive(value: false);
			}
			else
			{
				bool GotSolid4 = false;
				Collider[] array2 = intersecting;
				foreach (Collider intersect4 in array2)
				{
					if ((intersect4.tag == "Solid" || intersect4.tag == "MapBorder") && (intersect4.gameObject.layer == LayerMask.NameToLayer("Wall") || intersect4.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || intersect4.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
					{
						WestCollider = intersect4;
						GotSolid4 = true;
					}
				}
				if (GotSolid4)
				{
					ElectricPartWest.SetActive(value: true);
				}
				else
				{
					ElectricPartWest.SetActive(value: false);
				}
			}
		}
		else if (WestCollider == null && GameMaster.instance.GameHasStarted)
		{
			ElectricPartWest.SetActive(value: false);
		}
		if (NorthCollider == null && !GameMaster.instance.GameHasStarted)
		{
			Collider[] intersecting = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 0f, 2f), 0.1f);
			if (intersecting.Length == 0)
			{
				ElectricPartNorth.SetActive(value: false);
			}
			else
			{
				bool GotSolid3 = false;
				Collider[] array3 = intersecting;
				foreach (Collider intersect3 in array3)
				{
					if ((intersect3.tag == "Solid" || intersect3.tag == "MapBorder") && (intersect3.gameObject.layer == LayerMask.NameToLayer("Wall") || intersect3.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || intersect3.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
					{
						NorthCollider = intersect3;
						GotSolid3 = true;
					}
				}
				if (GotSolid3)
				{
					ElectricPartNorth.SetActive(value: true);
				}
				else
				{
					ElectricPartNorth.SetActive(value: false);
				}
			}
		}
		else if (NorthCollider == null && GameMaster.instance.GameHasStarted)
		{
			ElectricPartNorth.SetActive(value: false);
		}
		if (SouthCollider == null && !GameMaster.instance.GameHasStarted)
		{
			Collider[] intersecting = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 0f, -2f), 0.1f);
			if (intersecting.Length == 0)
			{
				ElectricPartSouth.SetActive(value: false);
				return;
			}
			bool GotSolid2 = false;
			Collider[] array4 = intersecting;
			foreach (Collider intersect in array4)
			{
				if ((intersect.tag == "Solid" || intersect.tag == "MapBorder") && (intersect.gameObject.layer == LayerMask.NameToLayer("Wall") || intersect.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || intersect.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
				{
					SouthCollider = intersect;
					GotSolid2 = true;
				}
			}
			if (GotSolid2)
			{
				ElectricPartSouth.SetActive(value: true);
			}
			else
			{
				ElectricPartSouth.SetActive(value: false);
			}
		}
		else if (SouthCollider == null && GameMaster.instance.GameHasStarted)
		{
			ElectricPartSouth.SetActive(value: false);
		}
	}
}
