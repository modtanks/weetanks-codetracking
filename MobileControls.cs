using UnityEngine;

public class MobileControls : MonoBehaviour
{
	public bool isBooster;

	public bool isMiner;

	public bool isPressed;

	private void Start()
	{
	}

	private void Update()
	{
		GameObject obj = GameObject.FindGameObjectWithTag("Player");
		MoveTankScript component = obj.GetComponent<MoveTankScript>();
		FiringTank component2 = obj.GetComponent<FiringTank>();
		if (isPressed)
		{
			if (isBooster)
			{
				component.mobileBoosting = true;
			}
			if (isMiner)
			{
				component2.mobileMine = true;
			}
		}
		else
		{
			if (isBooster)
			{
				component.mobileBoosting = false;
			}
			if (isMiner)
			{
				component2.mobileMine = false;
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
