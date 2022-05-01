using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvl30script : MonoBehaviour
{
	public List<GameObject> Blocks = new List<GameObject>();

	public GameObject Boss30;

	private HealthTanks Boss30HT;

	private EnemyAI Boss30EA;

	private int updated;

	private void Start()
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.GetChild(i).tag == "Solid")
			{
				Blocks.Add(base.transform.GetChild(i).gameObject);
			}
		}
		if (Boss30 != null)
		{
			Boss30HT = Boss30.GetComponent<HealthTanks>();
			Boss30EA = Boss30.GetComponent<EnemyAI>();
		}
	}

	private void Update()
	{
		if (GameMaster.instance.GameHasStarted)
		{
			if (Boss30 != null && Boss30HT != null && GameMaster.instance.AmountGoodTanks > 0)
			{
				if (Boss30HT.health < Boss30EA.lvl30Boss2modeLives && updated == 0)
				{
					int num = Mathf.CeilToInt((float)Blocks.Count / 1.5f);
					for (int i = 0; i < num; i++)
					{
						int index = Random.Range(0, Blocks.Count);
						StartCoroutine("MoveBlockDown", Blocks[index]);
						Blocks.RemoveAt(index);
					}
					updated = 1;
				}
				else if (Boss30HT.health < Boss30EA.lvl30Boss3modeLives && updated == 1)
				{
					for (int j = 0; j < Blocks.Count; j++)
					{
						StartCoroutine("MoveBlockDown", Blocks[j]);
					}
					updated = 2;
				}
			}
			else if (GameMaster.instance.AmountGoodTanks < 1 && updated != 0)
			{
				ResetAll();
			}
		}
		else if (updated != 0)
		{
			ResetAll();
		}
	}

	private void ResetAll()
	{
		updated = 0;
		Blocks = new List<GameObject>();
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.GetChild(i).tag == "Solid" && !Blocks.Contains(base.transform.GetChild(i).gameObject))
			{
				Blocks.Add(base.transform.GetChild(i).gameObject);
			}
		}
		for (int j = 0; j < Blocks.Count; j++)
		{
			StartCoroutine("ResetDown", Blocks[j]);
		}
	}

	public IEnumerator MoveBlockDown(GameObject Block)
	{
		Vector3 currentPos = Block.transform.position;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			Block.transform.position = Vector3.Lerp(currentPos, new Vector3(Block.transform.position.x, -0.97f, Block.transform.position.z), t);
			yield return null;
		}
		Block.GetComponent<BoxCollider>().enabled = false;
	}

	public IEnumerator ResetDown(GameObject Block)
	{
		Vector3 currentPos = Block.transform.position;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 3f;
			Block.transform.position = Vector3.Lerp(currentPos, new Vector3(Block.transform.position.x, 1f, Block.transform.position.z), t);
			yield return null;
		}
		Block.GetComponent<BoxCollider>().enabled = true;
	}
}
