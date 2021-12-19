using System.Collections;
using UnityEngine;

public class ObjectPlacing : MonoBehaviour
{
	public GameObject TurretPrefab;

	public GameObject HoldingPrefab;

	public GameObject PlaceParticles;

	private MoveTankScript MTS;

	private SomethingInMe SIM;

	public AudioClip PlaceSound;

	public AudioClip ErrorSound;

	public UpgradeField UF;

	public Renderer GetMaterialHead;

	public Renderer GetMaterialBody;

	public bool InPlacingMode;

	private void Start()
	{
		MTS = GetComponent<MoveTankScript>();
	}

	public void StartPlacing()
	{
		HoldingPrefab = Object.Instantiate(TurretPrefab, base.transform.position, Quaternion.identity);
		HoldingPrefab.GetComponent<BoxCollider>().isTrigger = true;
		SIM = HoldingPrefab.GetComponent<SomethingInMe>();
		SIM.Head = new Material[2];
		SIM.Head[0] = GetMaterialHead.materials[0];
		SIM.Head[1] = GetMaterialHead.materials[0];
		SIM.Body = new Material[2];
		SIM.Body[0] = GetMaterialBody.materials[2];
		SIM.Body[1] = GetMaterialBody.materials[1];
		InPlacingMode = true;
	}

	private void Update()
	{
		Vector3 vector = ((!MTS.DrivingBackwards) ? (base.transform.position + base.transform.forward * 2.5f) : (base.transform.position + -base.transform.forward * 2.5f));
		if ((bool)HoldingPrefab)
		{
			HoldingPrefab.transform.position = new Vector3(Mathf.Round(vector.x), base.transform.position.y, Mathf.Round(vector.z));
			if (MTS.player.GetButtonDown("Use"))
			{
				PlaceTurret();
			}
		}
	}

	private void PlaceTurret()
	{
		if (!SIM.EnteredTrigger)
		{
			GameMaster.instance.Play2DClipAtPoint(PlaceSound, 1f);
			Object.Instantiate(PlaceParticles, HoldingPrefab.transform.position, Quaternion.identity);
			SIM.SetMaterial(Color.white, backToNormal: true);
			HoldingPrefab.GetComponent<BoxCollider>().isTrigger = false;
			HoldingPrefab.GetComponent<Rigidbody>().isKinematic = false;
			WeeTurret component = HoldingPrefab.GetComponent<WeeTurret>();
			component.enabled = true;
			component.myMTS = GetComponent<MoveTankScript>();
			component.PlacedByPlayer = GetComponent<MoveTankScript>().playerId;
			HoldingPrefab = null;
			StartCoroutine(disablePlacingMode());
			if (component.myMTS.isPlayer2)
			{
				GameMaster.instance.TurretsPlaced[1]++;
			}
			else
			{
				GameMaster.instance.TurretsPlaced[0]++;
			}
		}
		else if (SIM.EnteredTrigger)
		{
			GameMaster.instance.Play2DClipAtPoint(ErrorSound, 1f);
		}
	}

	private IEnumerator disablePlacingMode()
	{
		yield return new WaitForSeconds(1f);
		InPlacingMode = false;
	}
}
