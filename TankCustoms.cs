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

	public Material CustomModTurretMaterial;

	public CustomSkinData ModData;

	public Texture2D texture;

	public Texture2D texture_turret;

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
		int AMselNumber = -1;
		for (int j = 0; j < OptionsMainMenu.instance.FullBodySkins.Length; j++)
		{
			if (OptionsMainMenu.instance.AMselected.Contains(OptionsMainMenu.instance.FullBodySkins[j].AMselectedID))
			{
				AMselNumber = OptionsMainMenu.instance.FullBodySkins[j].AMselectedID;
			}
		}
		if (AMselNumber > -1)
		{
			myIndex = -1;
			for (int i = 0; i < OptionsMainMenu.instance.FullBodySkins.Length; i++)
			{
				if (OptionsMainMenu.instance.FullBodySkins[i].AMselectedID == AMselNumber)
				{
					myIndex = i;
				}
			}
			if (myIndex > -1)
			{
				MySkinData = OptionsMainMenu.instance.FullBodySkins[myIndex];
				SetSkin(OptionsMainMenu.instance.FullBodySkins[myIndex]);
			}
		}
		if (myIndex == -1)
		{
			CheckForSkin();
		}
	}

	private void CheckForSkin()
	{
		string savePath = "";
		string savePathTurret = "";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePathTurret = savePath + "/My Games/Wee Tanks/mods/turret_skin.png";
		savePath += "/My Games/Wee Tanks/mods/tank_skin.png";
		texture = new Texture2D(2, 2);
		if (File.Exists(savePath))
		{
			byte[] fileData2 = File.ReadAllBytes(savePath);
			texture.LoadImage(fileData2);
			if ((bool)texture)
			{
				ModData = new CustomSkinData();
				CustomModMaterial = new Material(Shader.Find("Standard"));
				CustomModMaterial.mainTexture = texture;
				ModData.MainMaterial = CustomModMaterial;
				SetSkin(ModData);
				activeIndex = myIndex;
				MySkinData = ModData;
			}
		}
		texture_turret = new Texture2D(2, 2);
		if (!File.Exists(savePathTurret))
		{
			return;
		}
		byte[] fileData = File.ReadAllBytes(savePathTurret);
		texture_turret.LoadImage(fileData);
		if ((bool)texture_turret)
		{
			if (!ModData)
			{
				ModData = new CustomSkinData();
			}
			CustomModTurretMaterial = new Material(Shader.Find("Standard"));
			CustomModTurretMaterial.mainTexture = texture_turret;
			ModData.TurretMaterial = CustomModTurretMaterial;
			activeIndex = myIndex;
			MySkinData = ModData;
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
			Material[] mats = allRends[i].materials;
			for (int j = 0; j < mats.Length; j++)
			{
				if (CSD.MainMaterial != null)
				{
					mats[j] = CSD.MainMaterial;
				}
			}
			allRends[i].materials = mats;
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
