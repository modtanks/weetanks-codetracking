using System;
using System.IO;
using UnityEngine;

public class TankCustoms : MonoBehaviour
{
	public Renderer[] allRends;

	public CustomSkinData MySkinData;

	public bool IsMainPlayer = true;

	[Header("Unlockable Skins")]
	public int activeIndex = -1;

	public int myIndex = -1;

	public Material CustomModMaterial;

	public CustomSkinData ModData;

	public Texture2D texture;

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
		if (num > -1)
		{
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
		CheckForSkin();
	}

	private void CheckForSkin()
	{
		string text = "";
		string text2 = "";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text2 += "/My Games/Wee Tanks/mods/turret_skin.png";
		text += "/My Games/Wee Tanks/mods/tank_skin.png";
		Debug.Log(text);
		texture = new Texture2D(2, 2);
		if (File.Exists(text))
		{
			byte[] data = File.ReadAllBytes(text);
			texture.LoadImage(data);
			if ((bool)texture)
			{
				ModData = new CustomSkinData();
				CustomModMaterial = new Material(Shader.Find("Standard"));
				CustomModMaterial.mainTexture = texture;
				ModData.MainMaterial = CustomModMaterial;
				SetSkin(ModData);
			}
		}
		if (File.Exists(text2))
		{
			byte[] data2 = File.ReadAllBytes(text2);
			texture.LoadImage(data2);
			if ((bool)texture)
			{
				ModData = new CustomSkinData();
				CustomModMaterial = new Material(Shader.Find("Standard"));
				CustomModMaterial.mainTexture = texture;
				ModData.TurretMaterial = CustomModMaterial;
				SetSkin(ModData);
			}
		}
	}

	private void SetSkin(CustomSkinData CSD)
	{
		if (CSD == null)
		{
			return;
		}
		for (int i = 0; i < allRends.Length; i++)
		{
			Material[] materials = allRends[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (CSD.MainMaterial != null)
				{
					materials[j] = CSD.MainMaterial;
				}
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
