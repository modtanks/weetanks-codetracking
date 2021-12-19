using UnityEngine;

public class DestroySecondPlayer : MonoBehaviour
{
	public bool disabled;

	public GameObject magicBurst;

	public bool joined;

	public int PlayerID = 1;

	private void OnEnable()
	{
		CheckIfPlayer2ModeIsActive();
	}

	private void Start()
	{
		CheckIfPlayer2ModeIsActive();
	}

	private void Awake()
	{
		CheckIfPlayer2ModeIsActive();
	}

	private void DisableMe()
	{
		disabled = true;
		if (MapEditorMaster.instance != null && MapEditorMaster.instance.isTesting)
		{
			GameMaster.instance.AmountGoodTanks--;
		}
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}

	private void CheckIfPlayer2ModeIsActive()
	{
		if (!(GameMaster.instance != null))
		{
			return;
		}
		if ((OptionsMainMenu.instance.AIcompanion > 0 || GameMaster.instance.PlayerModeWithAI[1] == 1) && !GameMaster.instance.isZombieMode && PlayerID == 1)
		{
			if ((bool)GameMaster.instance.CM)
			{
				DisableMe();
			}
		}
		else if ((OptionsMainMenu.instance.AIcompanion > 1 || GameMaster.instance.PlayerModeWithAI[2] == 1) && !GameMaster.instance.isZombieMode && PlayerID == 2)
		{
			if ((bool)GameMaster.instance.CM)
			{
				DisableMe();
			}
		}
		else if ((OptionsMainMenu.instance.AIcompanion > 2 || GameMaster.instance.PlayerModeWithAI[3] == 1) && !GameMaster.instance.isZombieMode && PlayerID == 3)
		{
			if ((bool)GameMaster.instance.CM)
			{
				DisableMe();
			}
		}
		else if (GameMaster.instance.PlayerJoined.Count < 2 && !GameMaster.instance.isZombieMode && PlayerID == 1)
		{
			if ((bool)GameMaster.instance.CM)
			{
				DisableMe();
			}
		}
		else if ((!GameMaster.instance.inMapEditor || MapEditorMaster.instance.isTesting) && GameMaster.instance.PlayerJoined.Count >= PlayerID)
		{
			if (!GameMaster.instance.PlayerJoined[PlayerID] && !disabled)
			{
				DisableMe();
			}
			else if (GameMaster.instance.PlayerJoined[PlayerID])
			{
				joined = true;
			}
		}
	}

	private void Update()
	{
		if (!(GameMaster.instance != null) || GameMaster.instance.isZombieMode || ((bool)GameMaster.instance.CM && GameMaster.instance.PlayerModeWithAI[1] == 1) || ((bool)GameMaster.instance.CM && GameMaster.instance.PlayerModeWithAI[2] == 1) || ((bool)GameMaster.instance.CM && GameMaster.instance.PlayerModeWithAI[3] == 1))
		{
			return;
		}
		if (MapEditorMaster.instance != null)
		{
			if (MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode && !disabled && !joined)
			{
				CheckIfPlayer2ModeIsActive();
			}
			else if (!MapEditorMaster.instance.isTesting && !MapEditorMaster.instance.inPlayingMode && disabled)
			{
				disabled = false;
				foreach (Transform item in base.transform)
				{
					item.gameObject.SetActive(value: true);
				}
			}
		}
		if (GameMaster.instance.GameHasPaused || GameMaster.instance.GameHasStarted || !disabled || (GameMaster.instance.isZombieMode && GameMaster.instance.PlayerModeWithAI[1] == 1))
		{
			return;
		}
		if (!joined && GameMaster.instance.PlayerJoined[PlayerID])
		{
			GameObject obj = Object.Instantiate(magicBurst, base.transform.position, Quaternion.identity);
			ParticleSystem component = obj.GetComponent<ParticleSystem>();
			obj.GetComponent<Play2DClipOnce>().overrideGameStarted = true;
			component.Play();
			obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
			obj.transform.parent = null;
			joined = true;
			disabled = false;
			foreach (Transform item2 in base.transform)
			{
				item2.gameObject.SetActive(value: true);
			}
			GameMaster.instance.FindPlayers();
			GameMaster.instance.StartCoroutine(GameMaster.instance.GetTankTeamData(fast: false));
		}
		if (GameMaster.instance.PlayerModeWithAI[1] == 1 && PlayerID == 1 && disabled)
		{
			disabled = false;
			foreach (Transform item3 in base.transform)
			{
				item3.gameObject.SetActive(value: true);
			}
		}
		else if (GameMaster.instance.PlayerModeWithAI[2] == 1 && PlayerID == 2 && disabled)
		{
			disabled = false;
			foreach (Transform item4 in base.transform)
			{
				item4.gameObject.SetActive(value: true);
			}
		}
		else if (GameMaster.instance.PlayerModeWithAI[3] == 1 && PlayerID == 3 && disabled)
		{
			disabled = false;
			foreach (Transform item5 in base.transform)
			{
				item5.gameObject.SetActive(value: true);
			}
		}
		if (!GameMaster.instance.isZombieMode && GameMaster.instance.CurrentMission != 0)
		{
			MoveTankScript componentInChildren = GetComponentInChildren<MoveTankScript>();
			if ((bool)componentInChildren)
			{
				componentInChildren.enabled = false;
			}
		}
		else if ((bool)ZombieTankSpawner.instance && ZombieTankSpawner.instance.Wave > 0)
		{
			GameMaster.instance.AmountGoodTanks++;
		}
	}
}
