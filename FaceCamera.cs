using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	public float offsetXrot;

	private void Update()
	{
		Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		base.transform.rotation = rotation;
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.x + offsetXrot, base.transform.rotation.y, base.transform.rotation.z);
	}
}
