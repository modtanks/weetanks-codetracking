using UnityEngine;

public class ThirdPersonCameraScript : MonoBehaviour
{
	public Vector3 BeginPos;

	public Transform bodyTank;

	public Transform HeadTank;

	public float offsetZ = 0f;

	public float offsetY = 0f;

	public float offsetX = 0f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.localPosition = bodyTank.localPosition + new Vector3(offsetX, offsetY, offsetZ);
		base.transform.rotation = HeadTank.rotation;
	}
}
