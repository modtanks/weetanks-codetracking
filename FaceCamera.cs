using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	public float offsetXrot = 0f;

	private void Update()
	{
		Vector3 relativePos = Vector3.forward;
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		base.transform.rotation = rotation;
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.x + offsetXrot, base.transform.rotation.y, base.transform.rotation.z);
	}
}
