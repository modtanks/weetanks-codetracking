using UnityEngine;

public class OnlinePlayer : MonoBehaviour
{
	public int playerID = 0;

	public GameObject MyPlayerHolder;

	public bool isOnlinePlayer = false;

	public GameObject TurningBase;

	public float TurnSpeed = 2.5f;

	private void Start()
	{
		if (LobbyMaster.instance.MyPlayerID != playerID && !isOnlinePlayer)
		{
			MyPlayerHolder.SetActive(value: true);
			Object.Destroy(base.gameObject);
		}
		else if (!LobbyMaster.instance.isDoingInfo)
		{
			LobbyMaster.instance.StartCoroutine(LobbyMaster.instance.DoPlayerInfo());
		}
	}

	private void Update()
	{
		Rotate();
	}

	private void Rotate()
	{
		Quaternion rotation = Quaternion.LookRotation(LobbyMaster.instance.OtherPlayerInfo.lookDir);
		TurningBase.transform.rotation = Quaternion.Lerp(TurningBase.transform.rotation, rotation, Time.deltaTime * TurnSpeed);
	}
}
