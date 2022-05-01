using UnityEngine;

public class LookAtObject : MonoBehaviour
{
	public bool FollowPlayer = false;

	public bool LookStraight = false;

	public bool LookStraighttwo = false;

	public int AdditionRotationY = 0;

	private void Start()
	{
	}

	private void Update()
	{
		if (FollowPlayer && GameMaster.instance.Players.Count > 0)
		{
			if ((bool)GameMaster.instance.Players[0])
			{
				Vector3 lookPos = GameMaster.instance.Players[0].transform.position - base.transform.position;
				lookPos.y = 0f;
				Quaternion rotation2 = Quaternion.LookRotation(lookPos);
				rotation2 *= Quaternion.Euler(0f, AdditionRotationY, 0f);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation2, Time.deltaTime);
			}
		}
		else if (LookStraight)
		{
			Quaternion rotation = Quaternion.LookRotation(-Vector3.left);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, Time.deltaTime);
		}
	}
}
