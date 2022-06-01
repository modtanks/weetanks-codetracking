using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKillsUI : MonoBehaviour
{
	public TextMeshProUGUI[] Pkills;

	public GameObject[] P1Boosts;

	public GameObject[] P2Boosts;

	public GameObject[] P3Boosts;

	public GameObject[] P4Boosts;

	public Texture[] SpeedBoostTextures;

	public Texture[] BuildBoostTextures;

	public Texture[] ShieldBoostTextures;

	public RawImage[] Bullets;

	public RawImage[] BoostAmounts;

	public RawImage[] BulletsP2;

	public RawImage[] BulletsP3;

	public RawImage[] BulletsP4;

	public Transform[] StatsTanks;

	public Texture rocketBoostTexture;

	public Texture TripmineBoostTexture;

	public TextMeshProUGUI[] PlayerkillsTitle;

	public GameObject[] PlayerBox;

	public float BoostPosYup;

	public float BoostPosYdown;

	private string[] PkillsTitle_original = new string[4];

	public List<MoveTankScript> MTSplayers = new List<MoveTankScript>();

	public Animator KillsAnimator;

	private RectTransform myRect;

	public Vector3 oldpos;

	public Vector3 destination;

	public float speed = 2f;

	public float startTime;

	private float journeyLength;

	public UIdisabler UIdis;

	private void OnEnable()
	{
		if (!(GameMaster.instance != null))
		{
			return;
		}
		for (int i = 0; i < GameMaster.instance.PlayerJoined.Count; i++)
		{
			if (GameMaster.instance.PlayerJoined[i])
			{
				PlayerBox[i].SetActive(value: true);
			}
			else
			{
				PlayerBox[i].SetActive(value: false);
			}
		}
	}

	private void Start()
	{
		KillsAnimator = GetComponent<Animator>();
		InvokeRepeating("SlowUpdate", 0.25f, 0.25f);
		for (int i = 0; i < 4; i++)
		{
			PkillsTitle_original[i] = PlayerkillsTitle[i].text;
		}
		for (int j = 0; j < P1Boosts.Length; j++)
		{
			P1Boosts[j].transform.localPosition = new Vector3(P1Boosts[j].transform.localPosition.x, BoostPosYdown, P1Boosts[j].transform.localPosition.z);
			RawImage component = P1Boosts[j].GetComponent<RawImage>();
			SetBoostText(j, 0, component);
		}
		for (int k = 0; k < P2Boosts.Length; k++)
		{
			P2Boosts[k].transform.localPosition = new Vector3(P2Boosts[k].transform.localPosition.x, BoostPosYdown, P2Boosts[k].transform.localPosition.z);
			RawImage component2 = P2Boosts[k].GetComponent<RawImage>();
			SetBoostText(k, 0, component2);
		}
		for (int l = 0; l < P3Boosts.Length; l++)
		{
			P3Boosts[l].transform.localPosition = new Vector3(P3Boosts[l].transform.localPosition.x, BoostPosYdown, P3Boosts[l].transform.localPosition.z);
			RawImage component3 = P3Boosts[l].GetComponent<RawImage>();
			SetBoostText(l, 0, component3);
		}
		for (int m = 0; m < P4Boosts.Length; m++)
		{
			P4Boosts[m].transform.localPosition = new Vector3(P4Boosts[m].transform.localPosition.x, BoostPosYdown, P4Boosts[m].transform.localPosition.z);
			RawImage component4 = P4Boosts[m].GetComponent<RawImage>();
			SetBoostText(m, 0, component4);
		}
		Transform[] statsTanks;
		if ((bool)GameMaster.instance && GameMaster.instance.isZombieMode)
		{
			statsTanks = StatsTanks;
			for (int n = 0; n < statsTanks.Length; n++)
			{
				statsTanks[n].localPosition += new Vector3(0f, 50f, 0f);
			}
		}
		statsTanks = StatsTanks;
		for (int n = 0; n < statsTanks.Length; n++)
		{
			statsTanks[n].gameObject.SetActive(value: false);
		}
	}

	public void ResetAll(int player)
	{
		switch (player)
		{
		case 1:
		{
			for (int l = 0; l < P1Boosts.Length; l++)
			{
				P1Boosts[l].transform.localPosition = new Vector3(P1Boosts[l].transform.localPosition.x, BoostPosYdown, P1Boosts[l].transform.localPosition.z);
				RawImage component4 = P1Boosts[l].GetComponent<RawImage>();
				SetBoostText(l, 0, component4);
			}
			break;
		}
		case 2:
		{
			for (int j = 0; j < P2Boosts.Length; j++)
			{
				P2Boosts[j].transform.localPosition = new Vector3(P2Boosts[j].transform.localPosition.x, BoostPosYdown, P2Boosts[j].transform.localPosition.z);
				RawImage component2 = P2Boosts[j].GetComponent<RawImage>();
				SetBoostText(j, 0, component2);
			}
			break;
		}
		case 3:
		{
			for (int k = 0; k < P3Boosts.Length; k++)
			{
				P3Boosts[k].transform.localPosition = new Vector3(P3Boosts[k].transform.localPosition.x, BoostPosYdown, P3Boosts[k].transform.localPosition.z);
				RawImage component3 = P3Boosts[k].GetComponent<RawImage>();
				SetBoostText(k, 0, component3);
			}
			break;
		}
		case 4:
		{
			for (int i = 0; i < P4Boosts.Length; i++)
			{
				P4Boosts[i].transform.localPosition = new Vector3(P4Boosts[i].transform.localPosition.x, BoostPosYdown, P4Boosts[i].transform.localPosition.z);
				RawImage component = P4Boosts[i].GetComponent<RawImage>();
				SetBoostText(i, 0, component);
			}
			break;
		}
		}
	}

	private void SetBoostText(int typeBoost, int level, RawImage img)
	{
		if (typeBoost == 0)
		{
			img.texture = BuildBoostTextures[level];
		}
		if (typeBoost == 1)
		{
			img.texture = SpeedBoostTextures[level];
		}
		if (typeBoost == 2)
		{
			img.texture = ShieldBoostTextures[level];
		}
		if (typeBoost == 3)
		{
			img.texture = rocketBoostTexture;
		}
		if (typeBoost == 4)
		{
			img.texture = TripmineBoostTexture;
		}
	}

	public void NewUpgrade(int player, int type, int lvl)
	{
		if (type != 5)
		{
			Debug.Log("MOVING IT UP BABY??" + player);
			if (player == 0)
			{
				Debug.Log("MOVING IT UP BABY");
				startTime = Time.time;
				destination = new Vector3(P1Boosts[type].transform.localPosition.x, BoostPosYup, P1Boosts[type].transform.localPosition.z);
				journeyLength = Vector3.Distance(P1Boosts[type].transform.localPosition, destination);
				StartCoroutine(MoveUpgradeUp(player, type, destination, journeyLength, startTime));
				RawImage component = P1Boosts[type].GetComponent<RawImage>();
				SetBoostText(type, lvl, component);
			}
			if (player == 1)
			{
				startTime = Time.time;
				destination = new Vector3(P2Boosts[type].transform.localPosition.x, BoostPosYup, P2Boosts[type].transform.localPosition.z);
				journeyLength = Vector3.Distance(P2Boosts[type].transform.localPosition, destination);
				StartCoroutine(MoveUpgradeUp(player, type, destination, journeyLength, startTime));
				RawImage component2 = P2Boosts[type].GetComponent<RawImage>();
				SetBoostText(type, lvl, component2);
			}
			if (player == 2)
			{
				startTime = Time.time;
				destination = new Vector3(P3Boosts[type].transform.localPosition.x, BoostPosYup, P3Boosts[type].transform.localPosition.z);
				journeyLength = Vector3.Distance(P3Boosts[type].transform.localPosition, destination);
				StartCoroutine(MoveUpgradeUp(player, type, destination, journeyLength, startTime));
				RawImage component3 = P3Boosts[type].GetComponent<RawImage>();
				SetBoostText(type, lvl, component3);
			}
			if (player == 3)
			{
				startTime = Time.time;
				destination = new Vector3(P4Boosts[type].transform.localPosition.x, BoostPosYup, P4Boosts[type].transform.localPosition.z);
				journeyLength = Vector3.Distance(P4Boosts[type].transform.localPosition, destination);
				StartCoroutine(MoveUpgradeUp(player, type, destination, journeyLength, startTime));
				RawImage component4 = P4Boosts[type].GetComponent<RawImage>();
				SetBoostText(type, lvl, component4);
			}
		}
	}

	public IEnumerator StartPlayerKillsAnimation(int player)
	{
		switch (player)
		{
		case 1:
			KillsAnimator.SetBool("Kill", value: true);
			yield return new WaitForSeconds(0.1f);
			KillsAnimator.SetBool("Kill", value: false);
			break;
		case 2:
			KillsAnimator.Play("Player2Kill", -1, 0f);
			break;
		case 3:
			KillsAnimator.Play("Player3Kill", -1, 0f);
			break;
		case 4:
			KillsAnimator.Play("Player4Kill", -1, 0f);
			break;
		}
	}

	public IEnumerator StartPlayerDeniedAnimation(int player)
	{
		switch (player)
		{
		case 1:
			KillsAnimator.SetBool("DeniedP1", value: true);
			yield return new WaitForSeconds(0.1f);
			KillsAnimator.SetBool("DeniedP1", value: false);
			break;
		case 2:
			KillsAnimator.Play("Player2Denied", -1, 0f);
			break;
		case 3:
			KillsAnimator.Play("Player3Denied", -1, 0f);
			break;
		case 4:
			KillsAnimator.Play("Player4Denied", -1, 0f);
			break;
		}
	}

	private IEnumerator MoveUpgradeUp(int player, int type, Vector3 Dest, float Length, float myTime)
	{
		switch (player)
		{
		case 0:
			if (Vector3.Distance(P1Boosts[type].transform.localPosition, Dest) > 0.01f)
			{
				float t3 = (Time.time - myTime) * speed / Length;
				P1Boosts[type].transform.localPosition = Vector3.Lerp(P1Boosts[type].transform.localPosition, Dest, t3);
				yield return null;
				break;
			}
			Debug.Log("BREAK!");
			P1Boosts[type].transform.localPosition = Dest;
			yield break;
		case 1:
			if (Vector3.Distance(P2Boosts[type].transform.localPosition, Dest) > 0.01f)
			{
				float t2 = (Time.time - myTime) * speed / Length;
				P2Boosts[type].transform.localPosition = Vector3.Lerp(P2Boosts[type].transform.localPosition, Dest, t2);
				yield return null;
				break;
			}
			P2Boosts[type].transform.localPosition = Dest;
			yield break;
		case 2:
			if (Vector3.Distance(P3Boosts[type].transform.localPosition, Dest) > 0.01f)
			{
				float t4 = (Time.time - myTime) * speed / Length;
				P3Boosts[type].transform.localPosition = Vector3.Lerp(P3Boosts[type].transform.localPosition, Dest, t4);
				yield return null;
				break;
			}
			P3Boosts[type].transform.localPosition = Dest;
			yield break;
		case 3:
			if (Vector3.Distance(P4Boosts[type].transform.localPosition, Dest) > 0.01f)
			{
				float t = (Time.time - myTime) * speed / Length;
				P4Boosts[type].transform.localPosition = Vector3.Lerp(P4Boosts[type].transform.localPosition, Dest, t);
				yield return null;
				break;
			}
			P4Boosts[type].transform.localPosition = Dest;
			yield break;
		}
		StartCoroutine(MoveUpgradeUp(player, type, Dest, Length, myTime));
	}

	private void Update()
	{
		for (int i = 0; i < GameMaster.instance.PlayerJoined.Count; i++)
		{
			if (GameMaster.instance.PlayerJoined[i] || OptionsMainMenu.instance.AIcompanion[i])
			{
				PlayerkillsTitle[i].text = PkillsTitle_original[i];
				Pkills[i].text = "x " + GameMaster.instance.Playerkills[i];
				PlayerBox[i].SetActive(value: true);
			}
			if (OptionsMainMenu.instance.AIcompanion[i])
			{
				StatsTanks[i].gameObject.SetActive(value: false);
			}
			else if (GameMaster.instance.PlayerJoined[i])
			{
				StatsTanks[i].gameObject.SetActive(value: true);
			}
		}
		Pkills[0].text = "x " + GameMaster.instance.Playerkills[0];
	}

	private void SlowUpdate()
	{
		if (!(GameMaster.instance != null))
		{
			return;
		}
		if (GameMaster.instance.PlayerJoined[0])
		{
			PlayerBox[0].SetActive(value: true);
		}
		else if (!GameMaster.instance.PlayerJoined[0])
		{
			PlayerBox[0].SetActive(value: false);
		}
		if (GameMaster.instance.PlayerJoined[1] && GameMaster.instance.PlayerModeWithAI[1] != 1)
		{
			PlayerBox[1].SetActive(value: true);
		}
		else if (GameMaster.instance.PlayerModeWithAI[1] == 1)
		{
			PlayerBox[1].SetActive(value: true);
		}
		if (GameMaster.instance.PlayerJoined[2])
		{
			PlayerBox[2].SetActive(value: true);
		}
		if (GameMaster.instance.PlayerJoined[3])
		{
			PlayerBox[3].SetActive(value: true);
		}
		if (!GameMaster.instance.GameHasStarted || GameMaster.instance.Players.Count <= 0)
		{
			return;
		}
		foreach (GameObject player in GameMaster.instance.Players)
		{
			MoveTankScript component = player.GetComponent<MoveTankScript>();
			if (component != null)
			{
				MTSplayers[component.playerId] = component;
			}
		}
	}
}
