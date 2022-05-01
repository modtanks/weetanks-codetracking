using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvl50script : MonoBehaviour
{
	public List<Transform> myBlocks = new List<Transform>();

	public bool isOverlays;

	public float yHeight;

	private void Start()
	{
		myBlocks.Clear();
		foreach (Transform item in base.transform)
		{
			myBlocks.Add(item);
		}
		yHeight = myBlocks[0].position.y;
	}

	private void Update()
	{
		if (GameMaster.instance.GameHasStarted)
		{
			return;
		}
		foreach (Transform myBlock in myBlocks)
		{
			DestroyableBlock component = myBlock.GetComponent<DestroyableBlock>();
			if (component.destroyed)
			{
				component.destroyed = false;
				component.blockHealth = component.maxBlockHealth;
				myBlock.transform.position = new Vector3(myBlock.position.x, -20f, myBlock.position.z);
				StartCoroutine("BackUp", myBlock.gameObject);
			}
			else
			{
				component.blockHealth = component.maxBlockHealth;
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
