using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
	public Transform Slot_1;

	public Transform Slot_2;

	public Transform Slot_3;

	public int TierMachine;

	public float SpinningSpeed = 50f;

	public bool IsSpinning;

	public int SlotsSet;

	public ParticleSystem WinFireworks;

	public List<float> ClosestFloats;

	public void GetShot()
	{
		if (!IsSpinning)
		{
			IsSpinning = true;
			return;
		}
		SlotsSet++;
		SetSlot();
	}

	private void SetSlot()
	{
		if (SlotsSet == 1)
		{
			Debug.Log(Slot_1.eulerAngles.z + " is the z rot");
			Slot_1.localRotation = Quaternion.Euler(0f, 0f, 17.524f + Mathf.Floor(Slot_1.eulerAngles.z / 45f) * 45f);
			Debug.Log(Slot_1.eulerAngles.z + " is the z rot");
		}
		else if (SlotsSet == 2)
		{
			Slot_2.localRotation = Quaternion.Euler(0f, 0f, 17.524f + Mathf.Floor(Slot_2.eulerAngles.z / 45f) * 45f);
		}
		else if (SlotsSet == 3)
		{
			Slot_3.localRotation = Quaternion.Euler(0f, 0f, 17.524f + Mathf.Floor(Slot_3.eulerAngles.z / 45f) * 45f);
		}
	}

	private void Update()
	{
		if (!IsSpinning)
		{
			return;
		}
		if (SlotsSet == 0)
		{
			Slot_1.Rotate(0f, 0f, Time.deltaTime * SpinningSpeed);
			Slot_2.Rotate(0f, 0f, Time.deltaTime * SpinningSpeed);
			Slot_3.Rotate(0f, 0f, Time.deltaTime * SpinningSpeed);
		}
		else if (SlotsSet == 1)
		{
			if (Slot_2.localRotation.z > 359f)
			{
				Slot_2.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			if (Slot_3.localRotation.z > 359f)
			{
				Slot_3.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			Slot_2.Rotate(0f, 0f, Time.deltaTime * SpinningSpeed);
			Slot_3.Rotate(0f, 0f, Time.deltaTime * SpinningSpeed);
		}
		else if (SlotsSet == 2)
		{
			if (Slot_3.localRotation.z > 359f)
			{
				Slot_3.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			Slot_3.Rotate(0f, 0f, Time.deltaTime * SpinningSpeed);
		}
		else if (SlotsSet == 3)
		{
			IsSpinning = false;
			SlotsSet = 0;
		}
	}
}
