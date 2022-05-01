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

	private TankCustoms GetMaterialBody;

	public bool InPlacingMode = false;

	private void Start()
	{
		MTS = GetComponent<MoveTankScript>();
		GetMaterialBody = GetComponent<TankCustoms>();
	}

	public void StartPlacing()
	{
		HoldingPrefab = Object.Instantiate(TurretPrefab, base.transform.position, Quaternion.identity);
		HoldingPrefab.GetComponent<BoxCollider>().isTrigger = true;
		SIM = HoldingPrefab.GetComponent<SomethingInMe>();
		SIM.TurretSkinData = GetMaterialBody.MySkinData;
		InPlacingMode = true;
	}

	private void Update()
	{
		Vector3 placepos = ((!MTS.DrivingBackwards) ? (base.transform.position + base.transform.forward * 2.5f) : (base.transform.position + -base.transform.forward * 2.5f));
		if ((bool)HoldingPrefab)
		{
			HoldingPrefab.transform.position = new Vector3(Mathf.Round(placepos.x), base.transform.position.y, Mathf.Round(placepos.z));
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
			SFXManager.instance.PlaySFX(PlaceSound, 1f, null);
			Object.Instantiate(PlaceParticles, HoldingPrefab.transform.position, Quaternion.identity);
			SIM.SetMaterial(Color.white, backToNormal: true);
			HoldingPrefab.GetComponent<BoxCollider>().isTrigger = false;
			HoldingPrefab.GetComponent<Rigidbody>().isKinematic = false;
			WeeTurret WT = HoldingPrefab.GetComponent<WeeTurret>();
			WT.enabled = true;
			WT.myMTS = GetComponent<MoveTankScript>();
			WT.PlacedByPlayer = GetComponent<MoveTankScript>().playerId;
			HoldingPrefab = null;
			StartCoroutine(disablePlacingMode());
			GameMaster.instance.TurretsPlaced[WT.myMTS.playerId]++;
		}
		else if (SIM.EnteredTrigger)
		{
			SFXManager.instance.PlaySFX(ErrorSound, 1f, null);
		}
	}

	private IEnumerator disablePlacingMode()
	{
		yield return new WaitForSeconds(1f);
		InPlacingMode = false;
	}
}
