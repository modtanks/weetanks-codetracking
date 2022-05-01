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
			GameObject[] all = UItoDisable3.Concat(UItoDisable1).Concat(UItoDisable2).ToArray();
			GameObject[] array4 = all;
			foreach (GameObject UI in array4)
			{
				UI.SetActive(value: true);
			}
			break;
		}
		case 1:
		{
			GameObject[] all2 = UItoDisable2.Concat(UItoDisable3).ToArray();
			GameObject[] array2 = all2;
			foreach (GameObject UI2 in array2)
			{
				UI2.SetActive(value: true);
			}
			GameObject[] uItoDisable = UItoDisable1;
			foreach (GameObject UI3 in uItoDisable)
			{
				UI3.SetActive(value: false);
			}
			break;
		}
		case 2:
		{
			GameObject[] all3 = UItoDisable1.Concat(UItoDisable2).ToArray();
			GameObject[] uItoDisable2 = UItoDisable3;
			foreach (GameObject UI4 in uItoDisable2)
			{
				UI4.SetActive(value: true);
			}
			GameObject[] array3 = all3;
			foreach (GameObject UI5 in array3)
			{
				UI5.SetActive(value: false);
			}
			break;
		}
		case 3:
		{
			GameObject[] alls = UItoDisable3.Concat(UItoDisable1).Concat(UItoDisable2).ToArray();
			GameObject[] array = alls;
			foreach (GameObject UI6 in array)
			{
				UI6.SetActive(value: false);
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
