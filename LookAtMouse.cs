using System;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
	public Camera Cam;

	public MousePosition mousescript;

	public MoveTankScript myController;

	public bool isFPS;

	public HealthTanks HealthScript;

	public Vector2 input;

	private float angle;

	private Quaternion targetRotation;

	private Transform cam;

	public Camera ThirdPersonCam;

	public float ThirdPersonCamSpeed = 10f;

	public Vector3 lookPos;

	public Quaternion rotation;

	public Vector3 prevInput;

	private void Start()
	{
		if (Cam != null)
		{
			cam = Cam.transform;
		}
		else
		{
			cam = Camera.main.transform;
		}
		if (mousescript == null)
		{
			mousescript = GameObject.Find("MouseDetectorPosition").GetComponent<MousePosition>();
		}
	}

	private void Update()
	{
		if (GameMaster.instance != null)
		{
			if ((GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted) || myController.isStunned)
			{
				return;
			}
			if (OptionsMainMenu.instance.IsThirdPerson)
			{
				ThirdPersonCam.gameObject.SetActive(value: true);
			}
			if (myController.playerId > 0 || (GameMaster.instance.isPlayingWithController && myController.playerId == 0))
			{
				GetInput();
				if ((double)Math.Abs(input.x) < 0.65 && (double)Mathf.Abs(input.y) < 0.65)
				{
					return;
				}
				CalculateDirection();
				Rotate();
			}
			else if (!GameMaster.instance.isPlayingWithController && myController.playerId == 0)
			{
				if ((bool)mousescript)
				{
					_ = prevInput == mousescript.mouseInput;
				}
				if (!OptionsMainMenu.instance.IsThirdPerson)
				{
					prevInput = mousescript.mouseInput;
					lookPos = mousescript.mousePos - base.transform.position;
					lookPos.y = 0f;
					rotation = Quaternion.LookRotation(lookPos);
					if (GameMaster.instance.inOnlineMode)
					{
						LobbyMaster.instance.MyPlayerInfo.lookDir = lookPos;
					}
					base.transform.rotation = rotation;
				}
				else
				{
					base.transform.Rotate(new Vector3(base.transform.rotation.x, Input.GetAxis("Mouse X"), base.transform.rotation.z) * Time.deltaTime * ThirdPersonCamSpeed);
				}
			}
		}
		if (!HealthScript)
		{
			HealthScript = base.transform.parent.GetComponentInChildren<HealthTanks>();
		}
	}

	private void GetInput()
	{
		if (HealthScript.health > 0)
		{
			input.x = myController.player.GetAxis("Look Horizontal");
			input.y = myController.player.GetAxis("Look Vertically");
		}
	}

	private void CalculateDirection()
	{
		if (isFPS)
		{
			angle = Mathf.Atan2(input.x, input.y) / 30f;
		}
		else
		{
			angle = Mathf.Atan2(input.x, input.y);
		}
		angle = 57.29578f * angle;
		angle += cam.eulerAngles.y;
	}

	private void Rotate()
	{
		targetRotation = Quaternion.Euler(0f, angle, 0f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, targetRotation, Time.deltaTime * 12f);
		_ = GameMaster.instance.inOnlineMode;
	}
}
