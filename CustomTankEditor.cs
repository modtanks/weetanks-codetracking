using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class CustomTankEditor : MonoBehaviour
{
	public GameObject ButtonsMenu;

	public GameObject ReplaceMenu;

	public GameObject FilesMenu;

	public GameObject FiringMenu1;

	public GameObject FiringMenu2;

	public GameObject CustomTankImageRockets;

	public ButtonMouseEvents FiringMenu1Button;

	public ButtonMouseEvents FiringMenu2Button;

	public TextMeshProUGUI MessageText;

	public TMP_Dropdown FilesDropdown;

	public GameObject LoadTankStatsButton;

	public Color GreenText;

	public Color NormalText;

	public Color RedText;

	private string theName = "";

	private string thePath = "";

	public List<CustomTankData> CTDs = new List<CustomTankData>();

	private void Start()
	{
		ReplaceMenu.SetActive(value: false);
		FilesMenu.SetActive(value: false);
		ButtonsMenu.SetActive(value: true);
		CustomTankImageRockets.SetActive(value: false);
	}

	public void ShowLoadTankStats()
	{
		LoadTankStatsButton.SetActive(value: true);
	}

	public void EnableFiringMenu1()
	{
		FiringMenu2Button.DeselectButton();
		FiringMenu1.SetActive(value: true);
		FiringMenu2.SetActive(value: false);
	}

	public void EnableFiringMenu2()
	{
		FiringMenu1Button.DeselectButton();
		FiringMenu1.SetActive(value: false);
		FiringMenu2.SetActive(value: true);
	}

	public void OnLoadTank()
	{
		LoadTankStatsButton.SetActive(value: false);
		string directory = "";
		directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		directory += "/My Games/Wee Tanks/custom_tanks/";
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
		CTDs.Clear();
		DirectoryInfo dataPathMap = new DirectoryInfo(directory);
		FileInfo[] mapFiles = dataPathMap.GetFiles("*.customtank");
		FilesDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> NewOptions = new List<TMP_Dropdown.OptionData>();
		if (mapFiles.Length != 0)
		{
			FileInfo[] array = mapFiles;
			foreach (FileInfo mapFile in array)
			{
				string mapname = mapFile.Name.Replace(".customtank", "");
				if (mapname.Length >= 1 && mapname.Length <= 25)
				{
					CustomTankData CTD = LoadData(mapname);
					if (CTD != null)
					{
						LoadTankStatsButton.SetActive(value: true);
						CTDs.Add(CTD);
						TMP_Dropdown.OptionData Option = new TMP_Dropdown.OptionData();
						Option.text = CTD.CustomTankName;
						NewOptions.Add(Option);
					}
				}
			}
		}
		FilesDropdown.AddOptions(NewOptions);
		FilesMenu.SetActive(value: true);
		ButtonsMenu.SetActive(value: false);
	}

	private CustomTankData LoadData(string filename)
	{
		string savePath = Application.persistentDataPath + "/custom_tanks/" + filename + ".customtank";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/custom_tanks/" + filename + ".customtank";
		if (File.Exists(savePath))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(savePath, FileMode.Open);
			stream.Seek(0L, SeekOrigin.Begin);
			CustomTankData data = formatter.Deserialize(stream) as CustomTankData;
			stream.Close();
			return data;
		}
		return null;
	}

	public void OnSaveThisTank()
	{
		string directory = "";
		directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		directory += "/My Games/Wee Tanks/custom_tanks/";
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
		MapEditorMaster MEM = MapEditorMaster.instance;
		theName = MEM.CustomTankDatas[MEM.SelectedCustomTank].CustomTankName;
		if (theName.Length > 1)
		{
			thePath = "";
			thePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
			thePath = thePath + "/My Games/Wee Tanks/custom_tanks/" + theName + ".customtank";
			if (File.Exists(thePath))
			{
				ButtonsMenu.SetActive(value: false);
				MessageText.gameObject.SetActive(value: true);
				MessageText.text = "File \"" + theName + "\" already exists. Replace?";
				MessageText.color = RedText;
				ReplaceMenu.SetActive(value: true);
			}
			else
			{
				ButtonsMenu.SetActive(value: false);
				SaveData(MEM.CustomTankDatas[MEM.SelectedCustomTank], thePath);
			}
		}
		else
		{
			ButtonsMenu.SetActive(value: false);
			MessageText.gameObject.SetActive(value: true);
			MessageText.text = "No tank name entered!";
			MessageText.color = RedText;
			StartCoroutine(ResetMenu());
		}
	}

	private void OnDisable()
	{
		ButtonsMenu.SetActive(value: true);
		ReplaceMenu.SetActive(value: false);
		FilesMenu.SetActive(value: false);
		MessageText.gameObject.SetActive(value: false);
		MessageText.text = "";
	}

	public void ApproveLoad()
	{
		MessageText.text = "Tank data loaded!";
		MessageText.color = GreenText;
		ReplaceMenu.SetActive(value: false);
		FilesMenu.SetActive(value: false);
		MessageText.gameObject.SetActive(value: true);
		string uniqueID = MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank].UniqueTankID;
		MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank] = CTDs[FilesDropdown.value];
		MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank].UniqueTankID = uniqueID;
		MapEditorMaster.instance.CustomMaterial[MapEditorMaster.instance.SelectedCustomTank].color = MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank].CustomTankColor.Color;
		MapEditorMaster.instance.LoadCustomTankDataUI(MapEditorMaster.instance.SelectedCustomTank);
		StartCoroutine(ResetMenu());
	}

	public void ApproveReplace()
	{
		MessageText.text = "";
		ReplaceMenu.SetActive(value: false);
		MessageText.gameObject.SetActive(value: false);
		SaveData(MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank], thePath);
	}

	public void DeclineReplace()
	{
		ButtonsMenu.SetActive(value: true);
		MessageText.gameObject.SetActive(value: false);
		MessageText.text = "";
		ReplaceMenu.SetActive(value: false);
		FilesMenu.SetActive(value: false);
	}

	private IEnumerator ResetMenu()
	{
		yield return new WaitForSeconds(1f);
		ButtonsMenu.SetActive(value: true);
		ReplaceMenu.SetActive(value: false);
		FilesMenu.SetActive(value: false);
		MessageText.gameObject.SetActive(value: false);
		MessageText.text = "";
	}

	private void SaveData(CustomTankData DATA, string savePath)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(savePath, FileMode.Create);
		formatter.Serialize(stream, DATA);
		stream.Close();
		MessageText.gameObject.SetActive(value: true);
		MessageText.color = GreenText;
		MessageText.text = "File \"" + DATA.CustomTankName + "\" saved!";
		StartCoroutine(ResetMenu());
	}

	public void OnDeleteCustomTank()
	{
		MapEditorMaster.instance.CustomTankDatas.Remove(MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank]);
		MapEditorMaster.instance.CustomMaterial.Remove(MapEditorMaster.instance.CustomMaterial[MapEditorMaster.instance.SelectedCustomTank]);
		MapEditorMaster.instance.ShowMenu(4);
	}
}
