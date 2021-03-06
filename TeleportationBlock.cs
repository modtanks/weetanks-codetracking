using System.Collections.Generic;
using UnityEngine;

public class TeleportationBlock : MonoBehaviour
{
	public Color unavailableColor;

	public Color availableColor;

	public bool canTeleportHere;

	public MeshRenderer MR;

	public List<GameObject> inMe = new List<GameObject>();

	public float NoTeleportCounter;

	private void Start()
	{
		MR = GetComponent<MeshRenderer>();
		float num = Random.Range(0.5f, 0.9f);
		InvokeRepeating("CheckPlace", num, num);
	}

	private void Update()
	{
		if (NoTeleportCounter > 0.1f)
		{
			NoTeleportCounter -= Time.deltaTime;
			canTeleportHere = false;
		}
		else if (NoTeleportCounter > 0f && inMe.Count < 1)
		{
			NoTeleportCounter = 0f;
			canTeleportHere = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		OnTrigger(other);
	}

	public void ActivateMe(float sec)
	{
		NoTeleportCounter = sec;
	}

	private void OnTrigger(Collider other)
	{
		if ((other.tag == "Solid" || other.tag == "Other" || other.tag == "Bullet" || other.tag == "Enemy" || other.tag == "Player" || other.tag == "Mine") && !inMe.Contains(other.gameObject))
		{
			inMe.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.tag == "Solid" || other.tag == "Other" || other.tag == "Bullet" || other.tag == "Enemy" || other.tag == "Player" || other.tag == "Mine") && inMe.Contains(other.gameObject))
		{
			inMe.Remove(other.gameObject);
		}
	}

	private void CheckPlace()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject item in inMe)
		{
			if (item == null)
			{
				list.Add(item);
			}
		}
		foreach (GameObject item2 in list)
		{
			inMe.Remove(item2);
		}
		if (!(NoTeleportCounter > 0f))
		{
			if (inMe.Count > 0)
			{
				canTeleportHere = false;
			}
			else
			{
				canTeleportHere = true;
			}
		}
	}
}
