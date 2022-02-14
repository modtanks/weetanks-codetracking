using UnityEngine;

public class TankCustoms : MonoBehaviour
{
	public Renderer[] allRends;

	public CustomSkinData MySkinData;

	public bool IsMainPlayer = true;

	[Header("Unlockable Skins")]
	public int activeIndex = -1;

	public int myIndex = -1;

	private void Awake()
	{
		SetSkin(MySkinData);
		if (IsMainPlayer)
		{
			SetIndex();
		}
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
			MySkinData = OptionsMainMenu.instance.FullBodySkins[myIndex];
			SetSkin(OptionsMainMenu.instance.FullBodySkins[myIndex]);
		}
	}

	private void SetSkin(CustomSkinData CSD)
	{
		for (int i = 0; i < allRends.Length; i++)
		{
			Material[] materials = allRends[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j] = CSD.MainMaterial;
			}
			allRends[i].materials = materials;
		}
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
