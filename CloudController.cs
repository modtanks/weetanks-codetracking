using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
	public int CurrentState = 0;

	public ParticleSystem myPS;

	public List<GameObject> FriendliesInMe = new List<GameObject>();

	public Material LightFog;

	public Material DarkFog;

	private bool LowerScale = false;

	private void Start()
	{
		ParticleSystemRenderer PSR = myPS.GetComponent<ParticleSystemRenderer>();
		PSR.normalDirection = Random.Range(0, 1);
		InvokeRepeating("CheckList", Random.Range(2f, 3f), Random.Range(2f, 3f));
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !FriendliesInMe.Contains(other.gameObject))
		{
			FriendliesInMe.Add(other.gameObject);
			CheckList();
		}
	}

	public void CheckList()
	{
		List<GameObject> ToRemove = new List<GameObject>();
		for (int i = 0; i < FriendliesInMe.Count; i++)
		{
			if (FriendliesInMe[i] == null)
			{
				ToRemove.Add(FriendliesInMe[i]);
			}
		}
		foreach (GameObject Remove in ToRemove)
		{
			FriendliesInMe.Remove(Remove);
		}
		if (FriendliesInMe.Count > 0 && CurrentState != 2)
		{
			CurrentState = 2;
			myPS.Stop();
			LowerScale = true;
		}
		else if (FriendliesInMe.Count < 1 && CurrentState == 2)
		{
			CurrentState = 1;
			myPS.Play();
			LowerScale = false;
		}
	}

	private void Update()
	{
		if (LowerScale && base.transform.GetChild(0).localScale.x > 0.01f)
		{
			base.transform.GetChild(0).localScale = new Vector3(base.transform.GetChild(0).localScale.x - Time.deltaTime * 1.5f, base.transform.GetChild(0).localScale.y - Time.deltaTime * 1.5f, base.transform.GetChild(0).localScale.z - Time.deltaTime * 1.5f);
		}
		else if (!LowerScale && base.transform.GetChild(0).localScale.x < 1f)
		{
			base.transform.GetChild(0).localScale = new Vector3(base.transform.GetChild(0).localScale.x + Time.deltaTime * 1.5f, base.transform.GetChild(0).localScale.y + Time.deltaTime * 1.5f, base.transform.GetChild(0).localScale.z + Time.deltaTime * 1.5f);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" && FriendliesInMe.Contains(other.gameObject))
		{
			FriendliesInMe.Remove(other.gameObject);
			CheckList();
		}
	}
}
