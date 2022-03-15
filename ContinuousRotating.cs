using UnityEngine;

public class ContinuousRotating : MonoBehaviour
{
	public float RotateSpeed = 5f;

	public bool isLightHouse;

	private float originalIntensity;

	public bool xAxis;

	public bool zAxis;

	private void Start()
	{
		Light component = GetComponent<Light>();
		if ((bool)component)
		{
			originalIntensity = component.intensity;
		}
	}

	private void Update()
	{
		if (isLightHouse)
		{
			if (RenderSettings.ambientLight != Color.black)
			{
				Light component = GetComponent<Light>();
				if ((bool)component)
				{
					component.intensity = 0f;
				}
			}
			else
			{
				Light component2 = GetComponent<Light>();
				if ((bool)component2)
				{
					component2.intensity = originalIntensity;
				}
			}
		}
		if (xAxis)
		{
			base.transform.Rotate(Time.deltaTime * RotateSpeed, 0f, 0f);
		}
		else if (zAxis)
		{
			base.transform.Rotate(0f, 0f, Time.deltaTime * RotateSpeed);
		}
		else
		{
			base.transform.Rotate(0f, Time.deltaTime * RotateSpeed, 0f);
		}
	}
}
