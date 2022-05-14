using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public Vector3 target;

	public float speed = 50f;

	public void Seek(Vector3 _target)
	{
		target = _target;
	}

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 vector = target - base.transform.position;
		float num = speed * Time.deltaTime;
		if (vector.magnitude <= num)
		{
			HitTarget();
		}
		else
		{
			base.transform.Translate(vector.normalized * num, Space.World);
		}
	}

	private void HitTarget()
	{
		Debug.Log("HIT!");
	}
}
