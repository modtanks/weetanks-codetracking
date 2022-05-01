using UnityEngine;

public class TemporaryRotation : MonoBehaviour
{
	public bool xAxis;

	public bool zAxis;

	public float RotateSpeed;

	public float RotationTimeLeft;

	private void Update()
	{
		if (RotationTimeLeft > 0f)
		{
			RotationTimeLeft -= Time.deltaTime;
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

	public void RotateFor(float time)
	{
		RotationTimeLeft = time;
	}
}
