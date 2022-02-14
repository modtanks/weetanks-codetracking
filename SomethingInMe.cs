using System.Collections.Generic;
using UnityEngine;

public class SomethingInMe : MonoBehaviour
{
	public bool EnteredTrigger;

	public List<Collider> CollidersInMe = new List<Collider>();

	public CustomSkinData TurretSkinData;

	public Renderer[] TurretRenders;

	public Color Unavailable;

	public Color Available;

	public Color OGRend;

	public Collider CollToPrevent;

	private bool placed;

	private void Start()
	{
		InvokeRepeating("ResetColliders", 3f, 3f);
		Renderer[] turretRenders = TurretRenders;
		for (int i = 0; i < turretRenders.Length; i++)
		{
			turretRenders[i].material = TurretSkinData.TurretMaterial;
		}
		OGRend = TurretSkinData.TurretMaterial.color;
		SetMaterial(Available, backToNormal: false);
	}

	private void ResetColliders()
	{
		List<Collider> list = new List<Collider>();
		foreach (Collider item in CollidersInMe)
		{
			if (item == null)
			{
				list.Add(item);
			}
		}
		foreach (Collider item2 in list)
		{
			CollidersInMe.Remove(item2);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!placed && !(other == CollToPrevent) && (other.tag == "Solid" || other.tag == "Untagged") && !CollidersInMe.Contains(other))
		{
			CollidersInMe.Add(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (!placed && (other.tag == "Solid" || other.tag == "Untagged") && CollidersInMe.Contains(other))
		{
			CollidersInMe.Remove(other);
		}
	}

	private void Update()
	{
		if (!placed)
		{
			if (CollidersInMe.Count > 0 && !EnteredTrigger)
			{
				EnteredTrigger = true;
				SetMaterial(Unavailable, backToNormal: false);
			}
			else if (CollidersInMe.Count < 1 && EnteredTrigger)
			{
				EnteredTrigger = false;
				SetMaterial(Available, backToNormal: false);
			}
		}
	}

	public void SetMaterial(Color clr, bool backToNormal)
	{
		if (backToNormal)
		{
			Renderer[] turretRenders = TurretRenders;
			for (int i = 0; i < turretRenders.Length; i++)
			{
				turretRenders[i].material.color = OGRend;
			}
		}
		else
		{
			Renderer[] turretRenders = TurretRenders;
			for (int i = 0; i < turretRenders.Length; i++)
			{
				turretRenders[i].material.color = clr;
			}
		}
	}
}
