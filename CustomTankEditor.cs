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
	}

	public void ShowLoadTankStats()
	{
		LoadTankStatsButton.SetActive(value: true);
	}

	public void OnLoadTank()
	{
		LoadTankStatsButton.SetActive(value: false);
		string text = "";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text += "/My Games/Wee Tanks/custom_tanks/";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		CTDs.Clear();
		FileInfo[] files = new DirectoryInfo(text).GetFiles("*.customtank");
		FilesDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		if (files.Length != 0)
		{
			FileInfo[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Name.Replace(".customtank", "");
				if (text2.Length >= 1 && text2.Length <= 25)
				{
					CustomTankData customTankData = LoadData(text2);
					if (customTankData != null)
					{
						LoadTankStatsButton.SetActive(value: true);
						CTDs.Add(customTankData);
						TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
						optionData.text = customTankData.CustomTankName;
						list.Add(optionData);
					}
				}
			}
		}
		FilesDropdown.AddOptions(list);
		FilesMenu.SetActive(value: true);
		ButtonsMenu.SetActive(value: false);
	}

	private CustomTankData LoadData(string filename)
	{
		string text = Application.persistentDataPath + "/custom_tanks/" + filename + ".customtank";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text = text + "/My Games/Wee Tanks/custom_tanks/" + filename + ".customtank";
		if (File.Exists(text))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(text, FileMode.Open);
			fileStream.Seek(0L, SeekOrigin.Begin);
			CustomTankData result = binaryFormatter.Deserialize(fileStream) as CustomTankData;
			fileStream.Close();
			return result;
		}
		return null;
	}

	public void OnSaveThisTank()
	{
		string text = "";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text += "/My Games/Wee Tanks/custom_tanks/";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		MapEditorMaster instance = MapEditorMaster.instance;
		theName = instance.CustomTankDatas[instance.SelectedCustomTank].CustomTankName;
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
				SaveData(instance.CustomTankDatas[instance.SelectedCustomTank], thePath);
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
		string uniqueTankID = MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank].UniqueTankID;
		MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank] = CTDs[FilesDropdown.value];
		MapEditorMaster.instance.CustomTankDatas[MapEditorMaster.instance.SelectedCustomTank].UniqueTankID = uniqueTankID;
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
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(savePath, FileMode.Create);
		binaryFormatter.Serialize(fileStream, DATA);
		fileStream.Close();
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
