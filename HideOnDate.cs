using System;
using UnityEngine;

public class HideOnDate : MonoBehaviour
{
	public int[] Months;

	public int[] Days;

	public bool MonthCheck;

	public bool DayCheck;

	public GameObject[] ToEnable;

	private void Start()
	{
		Debug.Log("Current month = " + DateTime.Now.Month + " and currenty day = " + DateTime.Now.Day);
		if (Months.Length != 0)
		{
			int[] months = Months;
			foreach (int num in months)
			{
				if (DateTime.Now.Month == num)
				{
					MonthCheck = true;
				}
			}
		}
		if (Days.Length != 0)
		{
			int[] months = Days;
			foreach (int num2 in months)
			{
				if (DateTime.Now.Day == num2)
				{
					DayCheck = true;
				}
			}
		}
		if (MonthCheck && DayCheck)
		{
			base.gameObject.SetActive(value: true);
			OptionsMainMenu.instance.SnowMode = true;
			GameObject[] toEnable = ToEnable;
			for (int i = 0; i < toEnable.Length; i++)
			{
				toEnable[i].SetActive(value: true);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
