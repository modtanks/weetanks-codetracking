using UnityEngine;

public class LaserScript : MonoBehaviour
{
	public int maxReflectionCount = 2;

	public float maxStepDistance = 200f;

	public bool directionSet = false;

	public int amountBounces = 0;

	public Vector3 thedirection;

	public GameObject WallGonnaBounceInTo;

	private LookAtMyDirection LAMD;

	private EnemyBulletScript EBS;

	[SerializeField]
	public Vector3 initialVelocity;

	public Vector3 lastFrameVelocity;

	private Rigidbody rb;

	public Vector3 lookAtPoint;

	public float distanceToWall;

	public bool canLaser = true;

	private void Start()
	{
		LAMD = GetComponent<LookAtMyDirection>();
		EBS = GetComponent<EnemyBulletScript>();
		distanceToWall = 9999f;
		rb = GetComponent<Rigidbody>();
		InvokeRepeating("CastLaser", 0.03f, 0.03f);
	}

	private void CastLaser()
	{
		if ((bool)LAMD)
		{
			amountBounces = LAMD.BounceAmount;
			maxReflectionCount = LAMD.maxBounces;
		}
		else if ((bool)EBS)
		{
			amountBounces = EBS.BounceAmount;
			maxReflectionCount = EBS.maxBounces;
		}
		initialVelocity = rb.velocity;
		Laser();
	}

	private void Laser()
	{
		DrawReflectionPattern(base.transform.position, initialVelocity, maxReflectionCount - amountBounces, 0);
	}

	private void DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining, int iterations)
	{
		if (reflectionsRemaining < 0)
		{
			return;
		}
		Vector3 startingPosition = position;
		Ray ray = new Ray(position, direction);
		LayerMask LM = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall"));
		if (Physics.Raycast(ray, out var hit, maxStepDistance, LM))
		{
			direction = Vector3.Reflect(direction.normalized, hit.normal);
			position = hit.point;
			if (iterations == 0)
			{
				WallGonnaBounceInTo = hit.transform.gameObject;
			}
		}
		else
		{
			position += direction * maxStepDistance;
		}
		if (iterations == 0)
		{
			thedirection = position - startingPosition;
			if (WallGonnaBounceInTo != null)
			{
				distanceToWall = Vector3.Distance(startingPosition, position);
			}
			lookAtPoint = position;
		}
		if (distanceToWall < 1f)
		{
			if ((bool)LAMD && !LAMD.nearWall)
			{
				LAMD.newVelocity = direction;
				LAMD.nearWall = true;
			}
			else if ((bool)EBS && !EBS.nearWall)
			{
				EBS.newVelocity = direction;
				EBS.nearWall = true;
			}
		}
		else if ((bool)LAMD)
		{
			LAMD.nearWall = false;
		}
		else
		{
			EBS.nearWall = false;
		}
		Debug.DrawLine(startingPosition, position, Color.blue);
		DrawReflectionPattern(position, direction, reflectionsRemaining - 1, 1);
	}
}
