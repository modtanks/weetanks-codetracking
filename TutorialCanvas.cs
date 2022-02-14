using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
	public List<GameObject> TutorialTexts = new List<GameObject>();

	public List<GameObject> TutorialKeyboardTexts = new List<GameObject>();

	public List<int> levelTutorial = new List<int>();

	public GameObject currentMissionBanner;

	public GameObject keyboardAndMouseBanner;

	public AudioClip ShowTutorialSound;

	public Vector3 tutPosition;

	public Vector3 tutDownPosition;

	public bool moveTut;

	public bool moveDown;

	public int currentTut;

	public float downPosition = -395f;

	public bool controllerTutsActive;

	public bool inZombieMode;

	private void Awake()
	{
		SetTuts();
	}

	private void Start()
	{
		if (GameMaster.instance.isZombieMode)
		{
			inZombieMode = true;
		}
		foreach (GameObject tutorialText in TutorialTexts)
		{
			tutPosition = tutorialText.transform.position;
			tutDownPosition = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
			tutorialText.transform.position = tutDownPosition;
		}
		foreach (GameObject tutorialKeyboardText in TutorialKeyboardTexts)
		{
			tutPosition = tutorialKeyboardText.transform.position;
			tutDownPosition = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
			tutorialKeyboardText.transform.position = tutDownPosition;
		}
		moveDown = false;
		TutorialTexts[currentTut].SetActive(value: false);
		TutorialKeyboardTexts[currentTut].SetActive(value: false);
	}

	public void CheckTut()
	{
		if (!(GameMaster.instance != null))
		{
			return;
		}
		for (int i = 0; i < levelTutorial.Count; i++)
		{
			if (GameMaster.instance.CurrentMission == levelTutorial[i] && !inZombieMode)
			{
				StartCoroutine("disableTut", i);
			}
			else if (inZombieMode && ZombieTankSpawner.instance.Wave == levelTutorial[i] + 1)
			{
				StartCoroutine("disableTut", i);
			}
		}
	}

	private void Update()
	{
		if (moveDown)
		{
			if (GameMaster.instance.CurrentMission == 0)
			{
				if (GameMaster.instance.mission1HasShooted || !GameMaster.instance.GameHasStarted)
				{
					MoveTutDown();
				}
			}
			else if (GameMaster.instance.CurrentMission == 1)
			{
				if (GameMaster.instance.mission2HasBoosted || !GameMaster.instance.GameHasStarted)
				{
					MoveTutDown();
				}
			}
			else if (GameMaster.instance.CurrentMission == 2)
			{
				if (GameMaster.instance.mission3HasMined || !GameMaster.instance.GameHasStarted)
				{
					MoveTutDown();
				}
			}
			else
			{
				MoveTutDown();
			}
		}
		else if (moveTut)
		{
			TutorialTexts[currentTut].transform.position = Vector3.Lerp(TutorialTexts[currentTut].transform.position, tutPosition, Time.deltaTime * 3f);
			TutorialKeyboardTexts[currentTut].transform.position = Vector3.Lerp(TutorialKeyboardTexts[currentTut].transform.position, tutPosition, Time.deltaTime * 3f);
		}
		if (GameMaster.instance.isPlayingWithController && !controllerTutsActive)
		{
			currentMissionBanner.SetActive(value: true);
			keyboardAndMouseBanner.SetActive(value: false);
			controllerTutsActive = true;
		}
		else if (!GameMaster.instance.isPlayingWithController && controllerTutsActive)
		{
			currentMissionBanner.SetActive(value: false);
			keyboardAndMouseBanner.SetActive(value: true);
			controllerTutsActive = false;
		}
	}

	private void MoveTutDown()
	{
		if (Vector3.Distance(TutorialTexts[currentTut].transform.position, tutDownPosition) >= 0.1f)
		{
			TutorialTexts[currentTut].transform.position = Vector3.Lerp(TutorialTexts[currentTut].transform.position, tutDownPosition, Time.deltaTime * 3f);
			TutorialKeyboardTexts[currentTut].transform.position = Vector3.Lerp(TutorialKeyboardTexts[currentTut].transform.position, tutDownPosition, Time.deltaTime * 3f);
		}
		else
		{
			moveDown = false;
			TutorialTexts[currentTut].SetActive(value: false);
			TutorialKeyboardTexts[currentTut].SetActive(value: false);
		}
	}

	private void SetTuts()
	{
		foreach (Transform item in currentMissionBanner.transform)
		{
			TutorialTexts.Add(item.gameObject);
			string text = item.name;
			string text2 = "";
			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsDigit(text[i]))
				{
					text2 += text[i];
				}
			}
			levelTutorial.Add(int.Parse(text2) - 1);
		}
		foreach (Transform item2 in keyboardAndMouseBanner.transform)
		{
			TutorialKeyboardTexts.Add(item2.gameObject);
		}
		currentMissionBanner.SetActive(value: false);
		keyboardAndMouseBanner.SetActive(value: true);
		controllerTutsActive = false;
	}

	private IEnumerator disableTut(int select)
	{
		yield return new WaitForSeconds(0.1f);
		currentTut = select;
		tutPosition = TutorialTexts[select].transform.position;
		tutDownPosition = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
		TutorialTexts[select].transform.position = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
		TutorialKeyboardTexts[select].transform.position = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
		TutorialTexts[select].SetActive(value: true);
		TutorialKeyboardTexts[select].SetActive(value: true);
		moveTut = true;
		SFXManager.instance.PlaySFX(ShowTutorialSound, 1f, null);
		yield return new WaitForSeconds(7f);
		moveDown = true;
		moveTut = false;
	}
}
