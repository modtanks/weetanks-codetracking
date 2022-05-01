using UnityEngine;

public class CompanionCustom : MonoBehaviour
{
	public Material GaryBlack;

	public Material GaryBody;

	public Material GaryWheels;

	public GameObject HeadTank;

	public Renderer BodyRend;

	public Renderer Wheels;

	public bool isGary;

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
			EA.transform.Find("Armour").gameObject.SetActive(value: true);
			isGary = true;
			Object.Destroy(HeadTank);
			Material[] materials = BodyRend.materials;
			materials[0] = GaryBody;
			materials[1] = GaryBody;
			materials[2] = GaryBlack;
			BodyRend.materials = materials;
			Wheels.material = GaryWheels;
		}
	}
}
