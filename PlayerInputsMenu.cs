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

	public int Scene = 0;

	public bool CallFromController = false;

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
		string selectedValue = Dropdowns[playerID].captionText.text;
		Debug.Log("value is now:" + selectedValue + CallFromController);
		int amountCompanions = 0;
		if (selectedValue.Contains("AI"))
		{
			amountCompanions++;
		}
		if (!CallFromController && !selectedValue.Contains("Keyboard") && !selectedValue.Contains("AI"))
		{
			for (int j = 0; j < Dropdowns.Length; j++)
			{
				if (j != playerID && Dropdowns[j].captionText.text == selectedValue)
				{
					Dropdowns[j].SetValueWithoutNotify(0);
				}
			}
		}
		for (int i = 0; i < Dropdowns.Length; i++)
		{
			if (Dropdowns[i].captionText.text.Contains("AI"))
			{
				OptionsMainMenu.instance.MenuCompanion[i] = true;
			}
			else
			{
				OptionsMainMenu.instance.MenuCompanion[i] = false;
			}
		}
		OptionsMainMenu.instance.SaveNewData();
		if (amountCompanions > 1)
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
			int amountCompanions = 0;
			if (OptionsMainMenu.instance.MenuCompanion[i])
			{
				amountCompanions++;
				Dropdowns[i].SetValueWithoutNotify(1);
			}
			if (amountCompanions > 1)
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
		for (int j = 0; j < OptionsMainMenu.instance.AIcompanion.Length; j++)
		{
			OptionsMainMenu.instance.AIcompanion[j] = false;
		}
		OptionsList.Clear();
		OptionsListPlayerOne.Clear();
		Controllers = ReInput.controllers.GetControllers(ControllerType.Joystick);
		string[] controllerNames = ReInput.controllers.GetControllerNames(ControllerType.Joystick);
		string[] array = controllerNames;
		foreach (string name in array)
		{
			bool ListContainsController = false;
			foreach (TMP_Dropdown.OptionData Option in OptionsList)
			{
				if (Option.text == name)
				{
					ListContainsController = true;
				}
			}
			if (!ListContainsController)
			{
				TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
				newOption.text = name;
				OptionsList.Add(newOption);
				OptionsListPlayerOne.Add(newOption);
			}
		}
		if (CanPlayWithAI)
		{
			TMP_Dropdown.OptionData newOption3 = new TMP_Dropdown.OptionData();
			newOption3.text = "AI companion";
			OptionsList.Add(newOption3);
		}
		TMP_Dropdown.OptionData newOption4 = new TMP_Dropdown.OptionData();
		newOption4.text = "None";
		OptionsList.Insert(0, newOption4);
		for (int i = 0; i < Dropdowns.Length; i++)
		{
			Dropdowns[i].ClearOptions();
			if (i == 0)
			{
				TMP_Dropdown.OptionData newOption2 = new TMP_Dropdown.OptionData();
				newOption2.text = "Mouse & Keyboard";
				OptionsListPlayerOne.Insert(0, newOption2);
				Dropdowns[i].AddOptions(OptionsListPlayerOne);
			}
			else
			{
				Dropdowns[i].AddOptions(OptionsList);
			}
		}
	}
}
