using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMaster : MonoBehaviour
{
	public List<GameObject> Levels = new List<GameObject>();

	public GameObject CanvasPaper;

	public int currentLvl;

	private bool movingPaper;

	public Collider[] mapSolids;

	public bool atTheEnd;

	public AudioClip Completed;

	private Vector3 targetPos;

	private void Awake()
	{
		foreach (GameObject level in Levels)
		{
			level.SetActive(value: false);
		}
	}

	private void Start()
	{
		Levels[0].SetActive(value: true);
		GameMaster.instance.StartGame();
		GameMaster.instance.GameHasStarted = true;
		GameMaster.instance.AmountGoodTanks = 1;
	}

	private IEnumerator BackToMainMenu()
	{
		yield return new WaitForSeconds(9f);
		SceneManager.LoadScene(0);
	}

	private void Update()
	{
		GameMaster.instance.AmountEnemyTanks = GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("Boss").Length;
		if (GameMaster.instance.AmountEnemyTanks < 1 && !movingPaper)
		{
			GameMaster.instance.GameHasStarted = false;
			if (!atTheEnd)
			{
				Levels[currentLvl + 1].SetActive(value: true);
			}
			else
			{
				StartCoroutine(BackToMainMenu());
			}
			GameMaster.instance.DisableGame();
			targetPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - 40f);
			movingPaper = true;
			SFXManager.instance.PlaySFX(Completed, 1f, null);
			Collider[] array = mapSolids;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			StartCoroutine(LerpPosition(targetPos, 8f));
		}
		if (movingPaper && GameMaster.instance.AmountEnemyTanks > 0 && !atTheEnd && Vector3.Distance(Camera.main.transform.position, targetPos) < 0.1f)
		{
			movingPaper = false;
			Levels[currentLvl].SetActive(value: false);
			currentLvl++;
			if (currentLvl == Levels.Count - 1)
			{
				atTheEnd = true;
			}
			GameMaster.instance.GameHasStarted = true;
			GameMaster.instance.StartGame();
			Collider[] array = mapSolids;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}
	}

	private IEnumerator LerpPosition(Vector3 targetPosition, float duration)
	{
		float time = 0f;
		Vector3 startPosition = Camera.main.transform.position;
		while (time < duration)
		{
			Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
			time += Time.deltaTime;
			yield return null;
		}
		Camera.main.transform.position = targetPosition;
	}
}
