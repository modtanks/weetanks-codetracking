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
		Vector3 dir = target - base.transform.position;
		float distanceThisFrame = speed * Time.deltaTime;
		if (dir.magnitude <= distanceThisFrame)
		{
			HitTarget();
		}
		else
		{
			base.transform.Translate(dir.normalized * distanceThisFrame, Space.World);
		}
	}

	private void HitTarget()
	{
		Debug.Log("HIT!");
	}
}
