using System.Collections;
using UnityEngine;

public class FollowTank : MonoBehaviour
{
	public Transform bodyTank;

	public Transform thirdPersonCamera;

	public float offsetZ;

	public float offsetY;

	public float offsetX;

	public bool NotY;

	public float withDelay;

	private bool disabling;

	public bool alsoRotate;

	public bool isSpotlight;

	public bool isPlayer2Spotlight;

	public bool isPlayer3Spotlight;

	public bool isPlayer4Spotlight;

	private Light mylight;

	public bool isCamera;

	public void Awake()
	{
		if (bodyTank != null)
		{
			SetPos();
		}
	}

	public void Start()
	{
		if (isSpotlight)
		{
			mylight = GetComponent<Light>();
		}
		if (bodyTank != null)
		{
			SetPos();
		}
		if (isSpotlight)
		{
			InvokeRepeating("CheckPlayers", 0.5f, 0.5f);
		}
	}

	private void SetPos()
	{
		if (!NotY)
		{
			base.transform.position = bodyTank.position;
		}
		else
		{
			base.transform.position = new Vector3(bodyTank.position.x, base.transform.position.y, bodyTank.position.z);
		}
	}

	private void CheckPlayers()
	{
		if (GameMaster.instance.Players.Count <= 0)
		{
			return;
		}
		bool flag = false;
		foreach (GameObject player in GameMaster.instance.Players)
		{
			if (bodyTank == player)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			bodyTank = null;
			SearchMyPlayer();
		}
	}

	private void Update()
	{
		if (bodyTank != null)
		{
			if (isSpotlight && !mylight.enabled)
			{
				mylight.enabled = true;
			}
			if (withDelay > 0f)
			{
				StartCoroutine(changePosition(bodyTank.position));
			}
			else if ((bodyTank.transform.hasChanged && GameMaster.instance.GameHasStarted) || isSpotlight || GameMaster.instance.isZombieMode || GameMaster.instance.inMenuMode || (bool)GameMaster.instance.CM)
			{
				if (!NotY)
				{
					if (isCamera)
					{
						base.transform.position = bodyTank.position + new Vector3(offsetX, offsetY, offsetZ);
					}
					else
					{
						base.transform.position = bodyTank.position + new Vector3(0f, offsetY, 0f);
					}
				}
				else
				{
					base.transform.position = new Vector3(bodyTank.position.x + offsetX, base.transform.position.y, bodyTank.position.z);
				}
				disabling = false;
				if (alsoRotate)
				{
					base.transform.rotation = bodyTank.rotation;
				}
			}
			else if (!GameMaster.instance.GameHasStarted && !disabling && !GameMaster.instance.CM)
			{
				StartCoroutine("slow");
				disabling = true;
			}
		}
		else if (isSpotlight)
		{
			mylight.enabled = false;
			SearchMyPlayer();
		}
	}

	private void SearchMyPlayer()
	{
		if (GameMaster.instance.Players.Count > 0 && !isPlayer2Spotlight && !isPlayer3Spotlight && !isPlayer4Spotlight && GameMaster.instance.Players[0] != null)
		{
			bodyTank = GameMaster.instance.Players[0].transform;
		}
		else if (GameMaster.instance.Players.Count > 1 && isPlayer2Spotlight && GameMaster.instance.Players[1] != null)
		{
			bodyTank = GameMaster.instance.Players[1].transform;
		}
		else if (GameMaster.instance.Players.Count > 2 && isPlayer3Spotlight && GameMaster.instance.Players[2] != null)
		{
			bodyTank = GameMaster.instance.Players[2].transform;
		}
		else if (GameMaster.instance.Players.Count > 3 && isPlayer4Spotlight && GameMaster.instance.Players[3] != null)
		{
			bodyTank = GameMaster.instance.Players[3].transform;
		}
	}

	private void UpdatePosition(Vector3 TankPos)
	{
		if ((bodyTank.transform.hasChanged && GameMaster.instance.GameHasStarted) || isSpotlight || GameMaster.instance.isZombieMode || GameMaster.instance.inMenuMode || (bool)GameMaster.instance.CM)
		{
			if (!NotY)
			{
				base.transform.position = TankPos + new Vector3(0f, offsetY, 0f);
			}
			else
			{
				base.transform.position = new Vector3(TankPos.x + offsetX, base.transform.position.y, TankPos.z);
			}
			disabling = false;
			if (alsoRotate)
			{
				base.transform.rotation = bodyTank.rotation;
			}
		}
		else if (!GameMaster.instance.GameHasStarted && !disabling && !GameMaster.instance.CM)
		{
			StartCoroutine("slow");
			disabling = true;
		}
	}

	private IEnumerator changePosition(Vector3 TankPos)
	{
		yield return new WaitForSeconds(withDelay);
		UpdatePosition(TankPos);
	}

	private IEnumerator slow()
	{
		if (!NotY)
		{
			base.transform.position = bodyTank.position + new Vector3(0f, offsetY, 0f);
		}
		else
		{
			base.transform.position = new Vector3(bodyTank.position.x + offsetX, base.transform.position.y, bodyTank.position.z);
		}
		yield return new WaitForSeconds(0.1f);
		if (disabling)
		{
			StartCoroutine("slow");
		}
	}
}
