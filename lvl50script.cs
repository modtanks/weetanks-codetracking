using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvl50script : MonoBehaviour
{
	public List<Transform> myBlocks = new List<Transform>();

	public bool isOverlays = false;

	public float yHeight;

	private void Start()
	{
		myBlocks.Clear();
		foreach (Transform child in base.transform)
		{
			myBlocks.Add(child);
		}
		yHeight = myBlocks[0].position.y;
	}

	private void Update()
	{
		if (GameMaster.instance.GameHasStarted)
		{
			return;
		}
		foreach (Transform block in myBlocks)
		{
			DestroyableBlock DB = block.GetComponent<DestroyableBlock>();
			if (DB.destroyed)
			{
				DB.destroyed = false;
				DB.blockHealth = DB.maxBlockHealth;
				block.transform.position = new Vector3(block.position.x, -20f, block.position.z);
				StartCoroutine("BackUp", block.gameObject);
			}
			else
			{
				DB.blockHealth = DB.maxBlockHealth;
			}
		}
	}

	public IEnumerator BackUp(GameObject Block)
	{
		Vector3 currentPos = Block.transform.position;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 3f;
			Block.transform.position = Vector3.Lerp(currentPos, new Vector3(Block.transform.position.x, yHeight, Block.transform.position.z), Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}
		Block.GetComponent<MeshRenderer>().enabled = true;
		Block.GetComponent<MeshCollider>().enabled = true;
	}
}
