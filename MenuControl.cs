using System.Collections;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
	private Vector2 input;

	public bool inOptions = false;

	public bool inLeftMenu = false;

	public int Selection = 0;

	public bool Abut = false;

	public bool doSomething = true;

	public bool Player2 = false;

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
		MainMenuButtons[] myItems = Object.FindObjectsOfType(typeof(MainMenuButtons)) as MainMenuButtons[];
		MainMenuButtons[] array = myItems;
		foreach (MainMenuButtons item in array)
		{
			if (item.Place == Selection)
			{
				item.Selected = true;
			}
			else
			{
				item.Selected = false;
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
		MainMenuButtons[] myItems = Object.FindObjectsOfType(typeof(MainMenuButtons)) as MainMenuButtons[];
		if (Abut)
		{
			MainMenuButtons[] array = myItems;
			foreach (MainMenuButtons item in array)
			{
				if (item.Place == Selection)
				{
					Debug.Log("clicked item in MainMenu!");
					item.Clicked();
					if (item.Place == 9 && inOptions && !inLeftMenu)
					{
						Selection = 3;
						inOptions = false;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (item.Place == 3 && !inOptions && !inLeftMenu)
					{
						Selection = menuAmountMiddle;
						inOptions = true;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (item.Place == -1 && !inOptions && inLeftMenu)
					{
						Selection = 0;
						inLeftMenu = false;
						inOptions = false;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (item.Place == 0 && !inOptions && !inLeftMenu)
					{
						Selection = -menuAmountLeft;
						OptionsMainMenu.instance.StartPlayer2Mode = true;
						inLeftMenu = true;
						yield return new WaitForSeconds(0.5f);
						doSomething = true;
						yield break;
					}
					if (item.Place == 1 && !inOptions && !inLeftMenu)
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
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
