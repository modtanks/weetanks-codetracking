using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
	public int CurrentState;

	public ParticleSystem myPS;

	public List<GameObject> FriendliesInMe = new List<GameObject>();

	public Material LightFog;

	public Material DarkFog;

	private bool LowerScale;

	private void Start()
	{
		myPS.GetComponent<ParticleSystemRenderer>().normalDirection = Random.Range(0, 1);
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
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < FriendliesInMe.Count; i++)
		{
			if (FriendliesInMe[i] == null)
			{
				list.Add(FriendliesInMe[i]);
			}
		}
		foreach (GameObject item in list)
		{
			FriendliesInMe.Remove(item);
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
		else if (!GameMaster.instance.GameHasStarted)
		{
			FriendliesInMe.Clear();
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
