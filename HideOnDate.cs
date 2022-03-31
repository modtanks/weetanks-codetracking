using System;
using UnityEngine;

public class HideOnDate : MonoBehaviour
{
	public int[] Months;

	public int[] Days;

	public bool MonthCheck = false;

	public bool DayCheck = false;

	public GameObject[] ToEnable;

	private void Start()
	{
		Debug.Log("Current month = " + DateTime.Now.Month + " and currenty day = " + DateTime.Now.Day);
		if (Months.Length != 0)
		{
			int[] months = Months;
			foreach (int month in months)
			{
				if (DateTime.Now.Month == month)
				{
					MonthCheck = true;
				}
			}
		}
		if (Days.Length != 0)
		{
			int[] days = Days;
			foreach (int day in days)
			{
				if (DateTime.Now.Day == day)
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
			foreach (GameObject enable in toEnable)
			{
				enable.SetActive(value: true);
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
