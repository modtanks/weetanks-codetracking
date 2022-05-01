using UnityEngine;

public class CompanionCustom : MonoBehaviour
{
	public Material GaryBlack;

	public Material GaryBody;

	public Material GaryWheels;

	public GameObject HeadTank;

	public Renderer BodyRend;

	public Renderer Wheels;

	public bool isGary = false;

	public float GarySpeed = 80f;

	public EnemyAI EA;

	private void Update()
	{
		if (!isGary && OptionsMainMenu.instance.AMselected.Contains(31))
		{
			EA.TankSpeed = GarySpeed;
			EA.OriginalTankSpeed = GarySpeed;
			EA.armoured = true;
			EA.HTscript.isGary = true;
			EA.HTscript.health = 3;
			EA.HTscript.maxHealth = 3;
			GameObject armour = EA.transform.Find("Armour").gameObject;
			armour.SetActive(value: true);
			isGary = true;
			Object.Destroy(HeadTank);
			Material[] mats = BodyRend.materials;
			mats[0] = GaryBody;
			mats[1] = GaryBody;
			mats[2] = GaryBlack;
			BodyRend.materials = mats;
			Wheels.material = GaryWheels;
		}
	}
}
