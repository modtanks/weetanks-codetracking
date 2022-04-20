using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
	public float offsetY = 0f;

	public float offsetZ = 0f;

	public float YPOS = 0f;

	public float ZPOS = 0f;

	private float playerOffsetY = 0f;

	private float playerOffsetZ = 0f;

	public float Ycorrection = -5f;

	public Vector3 startPos;

	public bool isCampaign;

	public float originalZoomScale;

	public float zoomedInScale;

	private Vector3 BetweenPos;

	private Vector3 velocity = Vector3.zero;

	public float DesiredScale = 10f;

	private void Start()
	{
		startPos = base.transform.position;
		originalZoomScale = Camera.main.orthographicSize;
	}

	private void SetPlayerToFollow()
	{
		if ((bool)GameMaster.instance)
		{
			if (GameMaster.instance.Players.Count == 1 || GameMaster.instance.PlayerModeWithAI[1] == 1)
			{
				offsetY = base.transform.position.y - GameMaster.instance.Players[0].transform.position.y;
				offsetZ = base.transform.position.z - GameMaster.instance.Players[0].transform.position.z;
				playerOffsetY = GameMaster.instance.Players[0].transform.position.y;
				playerOffsetZ = GameMaster.instance.Players[0].transform.position.z;
			}
			else if (GameMaster.instance.Players.Count == 2 && GameMaster.instance.PlayerModeWithAI[1] != 0)
			{
				BetweenPos = (GameMaster.instance.Players[0].transform.position + GameMaster.instance.Players[1].transform.position) / 2f;
				offsetY = base.transform.position.y - BetweenPos.y;
				offsetZ = base.transform.position.z - BetweenPos.z;
				playerOffsetY = BetweenPos.y;
				playerOffsetZ = BetweenPos.z;
			}
		}
	}

	private void Update()
	{
		if (GameMaster.instance.GameHasStarted)
		{
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, DesiredScale, Time.deltaTime);
		}
		if (GameMaster.instance.Players.Count == 1)
		{
			DesiredScale = zoomedInScale;
			if (GameMaster.instance.Players[0] != null)
			{
				SetPos(GameMaster.instance.Players[0].transform.position);
			}
		}
		else if (GameMaster.instance.Players.Count == 2)
		{
			BetweenPos = (GameMaster.instance.Players[0].transform.position + GameMaster.instance.Players[1].transform.position) / 2f;
			SetPos(BetweenPos);
			DesiredScale = zoomedInScale * 1.2f;
		}
		else if (GameMaster.instance.Players.Count == 3)
		{
			BetweenPos = (GameMaster.instance.Players[0].transform.position + GameMaster.instance.Players[1].transform.position + GameMaster.instance.Players[2].transform.position) / 3f;
			SetPos(BetweenPos);
			DesiredScale = zoomedInScale * 1.3f;
		}
		else if (GameMaster.instance.Players.Count == 4)
		{
			BetweenPos = (GameMaster.instance.Players[0].transform.position + GameMaster.instance.Players[1].transform.position + GameMaster.instance.Players[2].transform.position + GameMaster.instance.Players[3].transform.position) / 4f;
			SetPos(BetweenPos);
			DesiredScale = zoomedInScale * 1.4f;
		}
		else if ((bool)ZombieTankSpawner.instance)
		{
			if (ZombieTankSpawner.instance.Wave > 0)
			{
				SetPos(new Vector3(0f, 0f, 0f));
			}
		}
		else if (GameMaster.instance.CurrentMission == 99 && GameMaster.instance.HasGotten100Checkpoint && GameMaster.instance.Lives > 0)
		{
			SetPos(GameMaster.instance.playerLocation[0]);
		}
		else
		{
			SetPos(new Vector3(0f, 0f, 0f));
		}
	}

	private void SetPos(Vector3 targetpos)
	{
		Vector3 point = Camera.main.WorldToViewportPoint(targetpos);
		Vector3 delta = targetpos - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
		Vector3 destination = base.transform.position + delta;
		base.transform.position = Vector3.SmoothDamp(base.transform.position, destination, ref velocity, 0.15f);
	}
}
