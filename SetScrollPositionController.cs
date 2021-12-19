using UnityEngine;
using UnityEngine.UI;

public class SetScrollPositionController : MonoBehaviour
{
	private ScrollRect myScrollRect;

	private void Start()
	{
		myScrollRect = GetComponent<ScrollRect>();
	}

	private void Update()
	{
		if (myScrollRect.verticalNormalizedPosition < 1f && Input.GetAxis("RightjoystickVertical") > 0.2f)
		{
			myScrollRect.verticalNormalizedPosition += Time.deltaTime * Input.GetAxis("RightjoystickVertical");
		}
		else if (myScrollRect.verticalNormalizedPosition > 0f && Input.GetAxis("RightjoystickVertical") < -0.2f)
		{
			myScrollRect.verticalNormalizedPosition -= Time.deltaTime * Mathf.Abs(Input.GetAxis("RightjoystickVertical"));
		}
	}
}
