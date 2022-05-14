using System.Collections.Generic;
using Rewired;
using TMPro;
using UnityEngine;

public class PlayerInputsMenu : MonoBehaviour
{
	public TMP_Dropdown[] Dropdowns;

	public List<TMP_Dropdown.OptionData> OptionsList = new List<TMP_Dropdown.OptionData>();

	public List<TMP_Dropdown.OptionData> OptionsListPlayerOne = new List<TMP_Dropdown.OptionData>();

	public string[] DropdownSelectedValues;

	public Controller[] Controllers;

	public bool CanPlayWithAI = true;

	public int Scene;

	public bool CallFromController;

	public GameObject SetDifficultyController;

	public MainMenuButtons StartGame;

	public MainMenuButtons Back;

	public GameObject CompanionNote;

	private void Start()
	{
		SetControllers();
		LoadData();
	}

	public void EnableDifficultySetter()
	{
		SetDifficultyController.SetActive(value: true);
		StartGame.Place = 6;
		Back.Place = 7;
	}

	public void DisableDifficultySetter()
	{
		SetDifficultyController.SetActive(value: false);
		StartGame.Place = 5;
		Back.Place = 6;
	}

	public void IncreaseDropdown(int playerID)
	{
	}

	private void OnEnable()
	{
		SetControllers();
		LoadData();
	}

	public void UpdateDropdown(int playerID)
	{
		string text = Dropdowns[playerID].captionText.text;
		Debug.Log("value is now:" + text + CallFromController);
		int num = 0;
		if (text.Contains("AI"))
		{
			num++;
		}
		if (!CallFromController && !text.Contains("Keyboard") && !text.Contains("AI"))
		{
			for (int i = 0; i < Dropdowns.Length; i++)
			{
				if (i != playerID && Dropdowns[i].captionText.text == text)
				{
					Dropdowns[i].SetValueWithoutNotify(0);
				}
			}
		}
		for (int j = 0; j < Dropdowns.Length; j++)
		{
			if (Dropdowns[j].captionText.text.Contains("AI"))
			{
				OptionsMainMenu.instance.MenuCompanion[j] = true;
			}
			else
			{
				OptionsMainMenu.instance.MenuCompanion[j] = false;
			}
		}
		OptionsMainMenu.instance.SaveNewData();
		if (num > 1)
		{
			CompanionNote.SetActive(value: true);
		}
		else
		{
			CompanionNote.SetActive(value: false);
		}
		CallFromController = false;
	}

	public void LoadData()
	{
		if (OptionsMainMenu.instance.MenuCompanion == null)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			int num = 0;
			if (OptionsMainMenu.instance.MenuCompanion[i])
			{
				num++;
				Dropdowns[i].SetValueWithoutNotify(1);
			}
			if (num > 1)
			{
				CompanionNote.SetActive(value: true);
			}
			else
			{
				CompanionNote.SetActive(value: false);
			}
		}
	}

	public void SetControllers()
	{
		for (int i = 0; i < OptionsMainMenu.instance.AIcompanion.Length; i++)
		{
			OptionsMainMenu.instance.AIcompanion[i] = false;
		}
		OptionsList.Clear();
		OptionsListPlayerOne.Clear();
		Controllers = ReInput.controllers.GetControllers(ControllerType.Joystick);
		string[] controllerNames = ReInput.controllers.GetControllerNames(ControllerType.Joystick);
		foreach (string text in controllerNames)
		{
			bool flag = false;
			foreach (TMP_Dropdown.OptionData options in OptionsList)
			{
				if (options.text == text)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
				optionData.text = text;
				OptionsList.Add(optionData);
				OptionsListPlayerOne.Add(optionData);
			}
		}
		if (CanPlayWithAI)
		{
			TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData();
			optionData2.text = "AI companion";
			OptionsList.Add(optionData2);
		}
		TMP_Dropdown.OptionData optionData3 = new TMP_Dropdown.OptionData();
		optionData3.text = "None";
		OptionsList.Insert(0, optionData3);
		for (int k = 0; k < Dropdowns.Length; k++)
		{
			Dropdowns[k].ClearOptions();
			if (k == 0)
			{
				TMP_Dropdown.OptionData optionData4 = new TMP_Dropdown.OptionData();
				optionData4.text = "Mouse & Keyboard";
				OptionsListPlayerOne.Insert(0, optionData4);
				Dropdowns[k].AddOptions(OptionsListPlayerOne);
			}
			else
			{
				Dropdowns[k].AddOptions(OptionsList);
			}
		}
	}
}
