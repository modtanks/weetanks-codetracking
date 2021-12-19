using System.Collections.Generic;
using UnityEngine;

public class SomethingInMe : MonoBehaviour
{
	public bool EnteredTrigger;

	public List<Collider> CollidersInMe = new List<Collider>();

	public Material[] Head;

	public Material[] Body;

	public Renderer Rend1;

	public Renderer Rend2;

	public Color Unavailable;

	public Color Available;

	public Color[] OGRend1;

	public Color[] OGRend2;

	public Collider CollToPrevent;

	private bool placed;

	private void Start()
	{
		InvokeRepeating("ResetColliders", 3f, 3f);
		Rend1.materials = Head;
		Rend2.materials = Body;
		OGRend1 = new Color[Rend1.materials.Length];
		for (int i = 0; i < Rend1.materials.Length; i++)
		{
			OGRend1[i] = Rend1.materials[i].color;
		}
		OGRend2 = new Color[Rend2.materials.Length];
		for (int j = 0; j < Rend2.materials.Length; j++)
		{
			OGRend2[j] = Rend2.materials[j].color;
		}
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
		Material[] array = new Material[Rend1.materials.Length];
		for (int i = 0; i < Rend1.materials.Length; i++)
		{
			if (backToNormal)
			{
				placed = true;
				CollidersInMe.Clear();
				array[i] = Rend1.materials[i];
				array[i].SetColor("_Color", OGRend1[i]);
			}
			else
			{
				array[i] = Rend1.materials[i];
				array[i].SetColor("_Color", clr);
			}
		}
		Rend1.materials = array;
		Material[] array2 = new Material[Rend2.materials.Length];
		for (int j = 0; j < Rend2.materials.Length; j++)
		{
			if (backToNormal)
			{
				array2[j] = Rend2.materials[j];
				array2[j].SetColor("_Color", OGRend2[j]);
			}
			else
			{
				array2[j] = Rend2.materials[j];
				array2[j].SetColor("_Color", clr);
			}
		}
		Rend2.materials = array2;
	}
}
