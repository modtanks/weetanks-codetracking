using UnityEngine;

public class MoveToPositionOverTime : MonoBehaviour
{
	public Vector3 startPos;

	public Vector3 endPos;

	public float Speed = 3f;

	public bool moveToEnd;

	public bool moveToStart;

	public bool atStart;

	private void Start()
	{
		startPos = base.transform.position;
		endPos = base.transform.position - new Vector3(0f, 200f, 0f);
	}

	private void Update()
	{
		if (moveToEnd && GameMaster.instance.GameHasStarted)
		{
			atStart = false;
			if (MoveTo(endPos))
			{
				moveToEnd = false;
				atStart = false;
			}
		}
		else if (moveToStart)
		{
			if (MoveTo(startPos))
			{
				moveToStart = false;
				atStart = true;
			}
		}
		else if (!GameMaster.instance.GameHasStarted && !atStart && MoveTo(startPos))
		{
			moveToStart = false;
			atStart = true;
		}
	}

	private bool MoveTo(Vector3 POS)
	{
		if (Vector3.Distance(base.transform.position, POS) >= 1f)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, POS, Time.deltaTime * Speed);
			return false;
		}
		return true;
	}
}
