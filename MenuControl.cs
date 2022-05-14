using System.Collections;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
	private Vector2 input;

	public bool inOptions;

	public bool inLeftMenu;

	public int Selection;

	public bool Abut;

	public bool doSomething = true;

	public bool Player2;

	public int menuAmountMiddle = 5;

	public int menuAmountLeft = 3;

	public int menuAmountRight = 4;

	public AudioClip MenuClick;

	public AudioClip MenuSelect;

	private void Update()
	{
		input.x = Input.GetAxisRaw("LeftjoystickHorizontal");
		input.y = Input.GetAxisRaw("LeftjoystickVertical") + Input.GetAxisRaw("P1-DPAD-UPDOWN");
		if (Input.GetButton("P1Abutton"))
		{
			Abut = true;
		}
		else
		{
			Abut = false;
		}
		MainMenuButtons[] array = Object.FindObjectsOfType(typeof(MainMenuButtons)) as MainMenuButtons[];
		foreach (MainMenuButtons mainMenuButtons in array)
		{
			if (mainMenuButtons.Place == Selection)
			{
				mainMenuButtons.Selected = true;
			}
			else
			{
				mainMenuButtons.Selected = false;
			}
		}
		if (!doSomething)
		{
			return;
		}
		if (input.y < -0.5f && Selection <= 8)
		{
			if (Selection < menuAmountMiddle && !inOptions && !inLeftMenu)
			{
				doSomething = false;
				StartCoroutine("increaseSelection");
			}
			else if (Selection <= menuAmountMiddle + menuAmountRight && inOptions)
			{
				doSomething = false;
				StartCoroutine("increaseSelection");
			}
			else if (Selection < -1 && !inOptions && inLeftMenu)
			{
				doSomething = false;
				StartCoroutine("increaseSelection");
			}
		}
		else if (input.y > 0.5f && Selection > -5)
		{
			if (Selection > 0 && !inOptions)
			{
				doSomething = false;
				StartCoroutine("decreaseSelection");
			}
			else if (Selection > menuAmountMiddle && inOptions)
			{
				doSomething = false;
				StartCoroutine("decreaseSelection");
			}
			else if (Selection > -menuAmountLeft && inLeftMenu)
			{
				doSomething = false;
				StartCoroutine("decreaseSelection");
			}
		}
		else if (Abut)
		{
			StartCoroutine("Click");
		}
	}

	private IEnumerator increaseSelection()
	{
		Selection++;
		Play2DClipAtPoint(MenuSelect);
		yield return new WaitForSeconds(0.3f);
		doSomething = true;
	}

	public IEnumerator decreaseSelection()
	{
		Selection--;
		Play2DClipAtPoint(MenuSelect);
		yield return new WaitForSeconds(0.3f);
		doSomething = true;
	}

	public IEnumerator Click()
	{
		doSomething = false;
		Play2DClipAtPoint(MenuClick);
		MainMenuButtons[] array = Object.FindObjectsOfType(typeof(MainMenuButtons)) as MainMenuButtons[];
		if (Abut)
		{
			MainMenuButtons[] array2 = array;
			foreach (MainMenuButtons mainMenuButtons in array2)
			{
				if (mainMenuButtons.Place == Selection)
				{
					Debug.Log("clicked item in MainMenu!");
					mainMenuButtons.Clicked();
					if (mainMenuButtons.Place == 9 && inOptions && !inLeftMenu)
					{
						Selection = 3;
						inOptions = false;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (mainMenuButtons.Place == 3 && !inOptions && !inLeftMenu)
					{
						Selection = menuAmountMiddle;
						inOptions = true;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (mainMenuButtons.Place == -1 && !inOptions && inLeftMenu)
					{
						Selection = 0;
						inLeftMenu = false;
						inOptions = false;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (mainMenuButtons.Place == 0 && !inOptions && !inLeftMenu)
					{
						Selection = -menuAmountLeft;
						OptionsMainMenu.instance.StartPlayer2Mode = true;
						inLeftMenu = true;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (mainMenuButtons.Place == 1 && !inOptions && !inLeftMenu)
					{
						Selection = -menuAmountLeft;
						OptionsMainMenu.instance.StartPlayer2Mode = false;
						inLeftMenu = true;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
				}
			}
		}
		yield return new WaitForSeconds(0.25f);
		doSomething = true;
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
