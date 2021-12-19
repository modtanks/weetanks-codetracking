using System.Collections;
using System.Linq;
using Rewired;
using UnityEngine;

public class UIdisabler : MonoBehaviour
{
	public int current = 1;

	public GameObject[] UItoDisable3;

	public GameObject[] UItoDisable2;

	public GameObject[] UItoDisable1;

	public Player player;

	public Controller playerController;

	private void Awake()
	{
		player = ReInput.players.GetPlayer(0);
	}

	private void Start()
	{
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.01f);
		if ((bool)OptionsMainMenu.instance)
		{
			current = OptionsMainMenu.instance.UIsetting;
			SetHUD();
		}
	}

	public void SetHUD()
	{
		if (current > 3)
		{
			current = 0;
		}
		switch (current)
		{
		case 0:
		{
			GameObject[] array = UItoDisable3.Concat(UItoDisable1).Concat(UItoDisable2).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			break;
		}
		case 1:
		{
			GameObject[] array = UItoDisable2.Concat(UItoDisable3).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			array = UItoDisable1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			break;
		}
		case 2:
		{
			GameObject[] array2 = UItoDisable1.Concat(UItoDisable2).ToArray();
			GameObject[] array = UItoDisable3;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			array = array2;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			break;
		}
		case 3:
		{
			GameObject[] array = UItoDisable3.Concat(UItoDisable1).Concat(UItoDisable2).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			break;
		}
		}
	}

	private void Update()
	{
		if (player.GetButtonDown("Toggle HUD"))
		{
			current++;
			OptionsMainMenu.instance.UIsetting = current;
			OptionsMainMenu.instance.SaveNewData();
			SetHUD();
		}
	}
}
