using UnityEngine;

public class MobileControls : MonoBehaviour
{
	public bool isBooster = false;

	public bool isMiner = false;

	public bool isPressed = false;

	private void Start()
	{
	}

	private void Update()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		MoveTankScript MTS = player.GetComponent<MoveTankScript>();
		FiringTank FT = player.GetComponent<FiringTank>();
		if (isPressed)
		{
			if (isBooster)
			{
				MTS.mobileBoosting = true;
			}
			if (isMiner)
			{
				FT.mobileMine = true;
			}
		}
		else
		{
			if (isBooster)
			{
				MTS.mobileBoosting = false;
			}
			if (isMiner)
			{
				FT.mobileMine = false;
			}
		}
	}

	public void onPointerDownButton()
	{
		isPressed = true;
	}

	public void onPointerUpButton()
	{
		isPressed = false;
	}
}
