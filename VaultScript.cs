using System.Collections;
using UnityEngine;

public class VaultScript : MonoBehaviour
{
	public GameObject[] MarblePiles;

	private void Start()
	{
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(3f);
		GetMarblesState();
	}

	private void GetMarblesState()
	{
		GameObject[] marblePiles = MarblePiles;
		foreach (GameObject Pile in marblePiles)
		{
			Pile.SetActive(value: false);
		}
		int marbles = AccountMaster.instance.PDO.marbles;
		int num = marbles;
		if (num < 2000)
		{
			if (num < 100)
			{
				if (num >= 25)
				{
					if (num < 50)
					{
						MarblePiles[1].SetActive(value: true);
					}
					else
					{
						MarblePiles[2].SetActive(value: true);
					}
				}
				else
				{
					MarblePiles[0].SetActive(value: true);
				}
			}
			else if (num < 500)
			{
				if (num < 200)
				{
					MarblePiles[3].SetActive(value: true);
				}
				else
				{
					MarblePiles[4].SetActive(value: true);
				}
			}
			else if (num < 1000)
			{
				MarblePiles[5].SetActive(value: true);
			}
			else
			{
				MarblePiles[6].SetActive(value: true);
			}
		}
		else if (num < 5000)
		{
			if (num < 3000)
			{
				MarblePiles[7].SetActive(value: true);
			}
			else
			{
				MarblePiles[8].SetActive(value: true);
			}
		}
		else if (num < 7500)
		{
			MarblePiles[9].SetActive(value: true);
		}
	}
}
