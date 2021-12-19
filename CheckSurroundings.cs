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
		if (EastCollider == null && !GameMaster.instance.GameHasStarted)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position + new Vector3(2f, 0f, 0f), 0.1f);
			if (array.Length == 0)
			{
				ElectricPartEast.SetActive(value: false);
			}
			else
			{
				bool flag = false;
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					if ((collider.tag == "Solid" || collider.tag == "MapBorder") && (collider.gameObject.layer == LayerMask.NameToLayer("Wall") || collider.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || collider.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
					{
						EastCollider = collider;
						flag = true;
					}
				}
				if (flag)
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
			Collider[] array = Physics.OverlapSphere(base.transform.position + new Vector3(-2f, 0f, 0f), 0.1f);
			if (array.Length == 0)
			{
				ElectricPartWest.SetActive(value: false);
			}
			else
			{
				bool flag2 = false;
				Collider[] array2 = array;
				foreach (Collider collider2 in array2)
				{
					if (collider2.tag == "Solid" && (collider2.gameObject.layer == LayerMask.NameToLayer("Wall") || collider2.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || collider2.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
					{
						WestCollider = collider2;
						flag2 = true;
					}
				}
				if (flag2)
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
			Collider[] array = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 0f, 2f), 0.1f);
			if (array.Length == 0)
			{
				ElectricPartNorth.SetActive(value: false);
			}
			else
			{
				bool flag3 = false;
				Collider[] array2 = array;
				foreach (Collider collider3 in array2)
				{
					if (collider3.tag == "Solid" && (collider3.gameObject.layer == LayerMask.NameToLayer("Wall") || collider3.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || collider3.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
					{
						NorthCollider = collider3;
						flag3 = true;
					}
				}
				if (flag3)
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
			Collider[] array = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 0f, -2f), 0.1f);
			if (array.Length == 0)
			{
				ElectricPartSouth.SetActive(value: false);
				return;
			}
			bool flag4 = false;
			Collider[] array2 = array;
			foreach (Collider collider4 in array2)
			{
				if (collider4.tag == "Solid" && (collider4.gameObject.layer == LayerMask.NameToLayer("Wall") || collider4.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") || collider4.gameObject.layer == LayerMask.NameToLayer("CorkWall")))
				{
					SouthCollider = collider4;
					flag4 = true;
				}
			}
			if (flag4)
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
