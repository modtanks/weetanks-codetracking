using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	public float xOffset;

	public float yOffset;

	public float zOffset;

	private void Start()
	{
	}

	private void Update()
	{
		Camera main = Camera.main;
		base.transform.LookAt(base.transform.position + main.transform.rotation * Vector3.back, main.transform.rotation * Vector3.up);
		base.transform.Rotate(0f, 180f, 0f);
		base.transform.Rotate(xOffset, yOffset, zOffset);
	}
}
