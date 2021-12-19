using UnityEngine;

public class TankCustoms : MonoBehaviour
{
	public int[] activatedSkin;

	public Renderer[] allRends;

	public FiringTank FT;

	public MoveTankScript MTS;

	public Material TankInsideTracks;

	public Material GlobalBodyTanks;

	public Material TankWheels;

	[Header("Unlockable Skins")]
	public int activeIndex = -1;

	public int myIndex = -1;

	private void Awake()
	{
		SetIndex();
	}

	private void SetIndex()
	{
		int num = -1;
		for (int i = 0; i < OptionsMainMenu.instance.FullBodySkins.Length; i++)
		{
			if (OptionsMainMenu.instance.AMselected.Contains(OptionsMainMenu.instance.FullBodySkins[i].AMselectedID))
			{
				num = OptionsMainMenu.instance.FullBodySkins[i].AMselectedID;
			}
		}
		if (num <= -1)
		{
			return;
		}
		myIndex = -1;
		for (int j = 0; j < OptionsMainMenu.instance.FullBodySkins.Length; j++)
		{
			if (OptionsMainMenu.instance.FullBodySkins[j].AMselectedID == num)
			{
				myIndex = j;
			}
		}
		if (myIndex > -1)
		{
			SetSkin(OptionsMainMenu.instance.FullBodySkins[myIndex]);
		}
	}

	private void SetSkin(CustomSkinData CSD)
	{
		Material[] materials = allRends[0].materials;
		materials[0] = CSD.OuterBodyMaterial;
		materials[1] = CSD.OuterBodyMaterial;
		materials[2] = CSD.InnerBodyMaterial;
		allRends[0].materials = materials;
		allRends[1].material = CSD.WheelsMaterial;
		allRends[2].material = CSD.InnerBodyMaterial;
		allRends[3].material = CSD.HeadMaterial;
		allRends[4].material = CSD.BarrelMaterial;
	}

	private void Update()
	{
		if (activeIndex != myIndex)
		{
			myIndex = activeIndex;
			SetIndex();
		}
	}
}
