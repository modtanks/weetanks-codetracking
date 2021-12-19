using System.Collections;
using UnityEngine;

public class TrainScript : MonoBehaviour
{
	public float baseSpeed = 15f;

	public float speedOffset = 10f;

	public int DeleteAfter = 7;

	public GameObject Clouds;

	private void Start()
	{
		baseSpeed += Random.Range(0f, speedOffset);
		StartCoroutine(DetachClouds());
	}

	private IEnumerator DetachClouds()
	{
		yield return new WaitForSeconds(DeleteAfter - 1);
		Clouds.transform.parent = null;
		Object.Destroy(Clouds, 5f);
	}

	private void Update()
	{
		base.transform.Translate(Vector3.forward * Time.deltaTime * baseSpeed);
		Object.Destroy(base.gameObject, DeleteAfter);
	}
}
