using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	public float xOffset = 0f;

	public float yOffset = 0f;

	public float zOffset = 0f;

	private void Start()
	{
	}

	private void Update()
	{
		Camera camera = Camera.main;
		base.transform.LookAt(base.transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
		base.transform.Rotate(0f, 180f, 0f);
		base.transform.Rotate(xOffset, yOffset, zOffset);
	}
}
