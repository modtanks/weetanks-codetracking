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
		int i;
		for (i = 0; i < marblePiles.Length; i++)
		{
			marblePiles[i].SetActive(value: false);
		}
		i = AccountMaster.instance.PDO.marbles;
		if (i < 2000)
		{
			if (i < 100)
			{
				if (i >= 25)
				{
					if (i < 50)
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
			else if (i < 500)
			{
				if (i < 200)
				{
					MarblePiles[3].SetActive(value: true);
				}
				else
				{
					MarblePiles[4].SetActive(value: true);
				}
			}
			else if (i < 1000)
			{
				MarblePiles[5].SetActive(value: true);
			}
			else
			{
				MarblePiles[6].SetActive(value: true);
			}
		}
		else if (i < 5000)
		{
			if (i < 3000)
			{
				MarblePiles[7].SetActive(value: true);
			}
			else
			{
				MarblePiles[8].SetActive(value: true);
			}
		}
		else if (i < 7500)
		{
			MarblePiles[9].SetActive(value: true);
		}
	}
}
