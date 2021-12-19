using UnityEngine;

public class LookAtObject : MonoBehaviour
{
	public bool FollowPlayer;

	public bool LookStraight;

	public bool LookStraighttwo;

	public int AdditionRotationY;

	private void Start()
	{
	}

	private void Update()
	{
		if (FollowPlayer && GameMaster.instance.Players.Count > 0)
		{
			if ((bool)GameMaster.instance.Players[0])
			{
				Vector3 forward = GameMaster.instance.Players[0].transform.position - base.transform.position;
				forward.y = 0f;
				Quaternion b = Quaternion.LookRotation(forward);
				b *= Quaternion.Euler(0f, AdditionRotationY, 0f);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime);
			}
		}
		else if (LookStraight)
		{
			Quaternion b2 = Quaternion.LookRotation(-Vector3.left);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b2, Time.deltaTime);
		}
	}
}
