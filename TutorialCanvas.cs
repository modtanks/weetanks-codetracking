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

	public bool controllerTutsActive = false;

	public bool inZombieMode = false;

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
		foreach (GameObject text2 in TutorialTexts)
		{
			tutPosition = text2.transform.position;
			tutDownPosition = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
			text2.transform.position = tutDownPosition;
		}
		foreach (GameObject text in TutorialKeyboardTexts)
		{
			tutPosition = text.transform.position;
			tutDownPosition = new Vector3(tutPosition.x, tutPosition.y - downPosition, 0f);
			text.transform.position = tutDownPosition;
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
		float dist = Vector3.Distance(TutorialTexts[currentTut].transform.position, tutDownPosition);
		if (dist >= 0.1f)
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
		foreach (Transform child2 in currentMissionBanner.transform)
		{
			TutorialTexts.Add(child2.gameObject);
			string name = child2.name;
			string val = "";
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsDigit(name[i]))
				{
					val += name[i];
				}
			}
			levelTutorial.Add(int.Parse(val) - 1);
		}
		foreach (Transform child in keyboardAndMouseBanner.transform)
		{
			TutorialKeyboardTexts.Add(child.gameObject);
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
